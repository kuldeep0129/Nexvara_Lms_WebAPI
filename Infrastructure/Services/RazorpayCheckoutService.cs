using Core.Dtos.Common;
using Core.Dtos.Payments;
using Core.Interface.Repository;
using Core.Interface.Services;
using Domain.common;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class RazorpayCheckoutService : IPaymentCheckoutService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public RazorpayCheckoutService(IConfiguration configuration, IPaymentRepository paymentRepository, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _paymentRepository = paymentRepository;
        }


        public async Task<PaymentCheckoutResult> CreateCheckoutAsync(string orderReference, decimal amount, string studentId, string courseName)
        {
            if (string.IsNullOrWhiteSpace(orderReference))
            {
                return new PaymentCheckoutResult
                {
                    Success = false,
                    ErrorMessage = "Order reference is required."
                };
            }

            if (string.IsNullOrWhiteSpace(studentId))
            {
                return new PaymentCheckoutResult
                {
                    Success = false,
                    ErrorMessage = "Student ID is required."
                };
            }

            if (string.IsNullOrWhiteSpace(courseName))
            {
                return new PaymentCheckoutResult
                {
                    Success = false,
                    ErrorMessage = "Course name is required."
                };
            }

            if (amount <= 0)
            {
                return new PaymentCheckoutResult
                {
                    Success = false,
                    ErrorMessage = "Payment amount must be greater than zero."
                };
            }

            var keyId = _configuration["Razorpay:KeyId"];
            var keySecret = _configuration["Razorpay:KeySecret"];

            if (string.IsNullOrWhiteSpace(keyId) ||
                string.IsNullOrWhiteSpace(keySecret))
            {
                return new PaymentCheckoutResult
                {
                    Success = false,
                    ErrorMessage = "Razorpay credentials are not configured."
                };
            }

            var student = await _userManager.FindByIdAsync(studentId);

            if (student == null)
            {
                return new PaymentCheckoutResult
                {
                    Success = false,
                    ErrorMessage = "Student account was not found."
                };
            }

            try
            {
                var client = new RazorpayClient(keyId, keySecret);

                // Razorpay amount is in paise.
                var amountInPaise = (int)Math.Round(amount * 100);

                var options = new Dictionary<string, object>
                {
                    { "amount", amountInPaise },
                    { "currency", "INR" },
                    { "receipt", orderReference },
                    { "payment_capture", 1 },
                    {
                        "notes",
                        new Dictionary<string, string>
                        {
                            { "student_id", studentId },
                            { "course_name", courseName }
                        }
                    }
                };

                Order order = client.Order.Create(options);

                return new PaymentCheckoutResult
                {
                    Success = true,
                    OrderId = order["id"]?.ToString(),
                    KeyId = keyId,
                    Amount = amount,
                    Currency = "INR"
                };
            }
            catch (Exception ex)
            {
                return new PaymentCheckoutResult
                {
                    Success = false,
                    ErrorMessage = $"Unable to create Razorpay order: {ex.Message}"
                };
            }

        }

        public async Task<ResponseDto<object>> VerifyPaymentAsync(VerifyPaymentDto dto)
        {
            var response = new ResponseDto<object>();

            // ---------- 1. Basic validation ----------
            if (dto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }

            if (dto.EnrollmentId <= 0 || dto.PaymentId <= 0 ||
                string.IsNullOrWhiteSpace(dto.RazorpayOrderId) ||
                string.IsNullOrWhiteSpace(dto.RazorpayPaymentId) ||
                string.IsNullOrWhiteSpace(dto.RazorpaySignature))
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = "Invalid payment verification request.";
                return response;
            }

            // ---------- 2. Load payment + enrollment ----------
            var payment = await _paymentRepository.GetPaymentByIdAsync(dto.PaymentId);
            if (payment == null)
            {
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                response.Success = false;
                response.Message = "Payment record not found.";
                return response;
            }

            if (payment.EnrollmentId != dto.EnrollmentId)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = "Payment does not belong to the provided enrollment.";
                return response;
            }

            // Idempotency — already paid
            if (payment.PaymentStatus == PaymentStatus.Success.ToString())
            {
                response.ResponseCode = (int)APIResponseCodes.OK;
                response.Success = true;
                response.Message = "Payment already verified.";
                response.Data = new
                {
                    EnrollmentId = payment.EnrollmentId,
                    PaymentId = payment.Id,
                    PaymentStatus = payment.PaymentStatus
                };
                return response;
            }

            var enrollment = await _paymentRepository.GetEnrollmentByIdAsync(dto.EnrollmentId);
            if (enrollment == null)
            {
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                response.Success = false;
                response.Message = "Enrollment record not found.";
                return response;
            }

            // ---------- 3. Verify Razorpay signature ----------
            var keySecret = _configuration["Razorpay:KeySecret"];
            if (string.IsNullOrWhiteSpace(keySecret))
            {
                response.ResponseCode = (int)APIResponseCodes.InternalServerError;
                response.Success = false;
                response.Message = "Payment gateway credentials are not configured.";
                return response;
            }

            var payload = $"{dto.RazorpayOrderId}|{dto.RazorpayPaymentId}";

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(keySecret));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var generatedSignature = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            if (!CryptographicOperations.FixedTimeEquals( Encoding.UTF8.GetBytes(generatedSignature), Encoding.UTF8.GetBytes(dto.RazorpaySignature.ToLowerInvariant())))
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = "Payment signature verification failed.";
                return response;
            }

            // ---------- 4. Update payment + enrollment ----------
            try
            {
                payment.PaymentStatus = PaymentStatus.Success.ToString();
                payment.PaidAt = DateTime.Now;
                payment.ProviderEventId = dto.RazorpayPaymentId;
                payment.TransactionId = dto.RazorpayOrderId; // order reference stays
                payment.UpdateAt = DateTime.Now;
                payment.UpdateBy = payment.StudentId;

                await _paymentRepository.UpdatePaymentAsync(payment);

                enrollment.IsActive = true;
                enrollment.PaymentStatus = PaymentStatus.Success.ToString();
                enrollment.UpdateAt = DateTime.Now;
                enrollment.UpdateBy = enrollment.StudentId;

                await _paymentRepository.UpdateEnrollmentAsync(enrollment);

                await _paymentRepository.SaveAsync();

                response.ResponseCode = (int)APIResponseCodes.OK;
                response.Success = true;
                response.Message = "Payment verified successfully.";
                response.Data = new
                {
                    EnrollmentId = enrollment.Id,
                    PaymentId = payment.Id,
                    PaymentStatus = payment.PaymentStatus,
                    PaidAt = payment.PaidAt,
                    IsActive = enrollment.IsActive
                };
                return response;
            }
            catch (Exception)
            {
                response.ResponseCode = (int)APIResponseCodes.InternalServerError;
                response.Success = false;
                response.Message = "Unable to complete payment verification. Please contact support.";
                return response;
            }
        }
    }
}
