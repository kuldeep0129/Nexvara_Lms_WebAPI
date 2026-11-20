using AutoMapper;
using Core.Dtos.Common;
using Core.Dtos.Courses;
using Core.Dtos.Payments;
using Core.Dtos.Student.Batch;
using Core.Interface.Repository;
using Core.Interface.Services;
using Domain.common;
using Domain.Entities.Common;
using Domain.Entities.Student;
using Domain.Entities.Teacher;
using Domain.Enums;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class StudentServices : IStudentServices
    {
        private readonly IStudentRepository _reposiotry;
        private readonly IFileUploadServices _uploadServices;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentCheckoutService _paymentCheckoutServices;
        public StudentServices(IMapper mapper, IUnitOfWork unitOfWork, IPaymentCheckoutService paymentCheckoutServices, IStudentRepository reposiotry, IFileUploadServices fileservices)
        {
            _reposiotry = reposiotry;
            _uploadServices = fileservices;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _paymentCheckoutServices = paymentCheckoutServices;
        }


        public async Task<ResponseDto<object>> CreateBatchStudentAsync(CreateBatchStudentDto dto, string userId)
        {
            var response = new ResponseDto<object>();
            if (dto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }

            var data = new BatchStudent
            {
                BatchId = dto.BatchId,
                StudentId = dto.StudentId,
                EnrolledAt = dto.EnrolledAt,
                CompleteAt = dto.CompleteAt,
                IsActive = true,
                CreateAt = DateTime.Now,
                CreateBy = userId
            };

            await _reposiotry.CreateBatchStudentAsync(data);
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordCreate;
            return response;
        }

        public async Task<ResponseDto<object>> CreateStudentEnrollmentAsync(CreateStudentEnrollmentDto dto, string userId)
        {
            var response = new ResponseDto<object>();
            if (dto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }

            var batch = await _reposiotry.GetBatchForEnrollmentAsync(dto.BatchId);
            if (batch == null || !batch.IsPublished || !batch.CourseId.HasValue || batch.CourseId.Value != dto.CourseId)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidOrUnavailableBatch;
                return response;
            }
            var res = await _reposiotry.IsCheckAlreadyStudentEnrollAsync(userId, dto.CourseId, dto.BatchId);
            if (res)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.AlreadyEnrolled;
                return response;
            }
            var data = new Enrollment
            {
                BatchId = dto.BatchId,
                CourseId = dto.CourseId,
                StudentId = userId,
                EnrollmentDate = DateTime.Now,
                PaymentStatus = batch.course.AccessType == CourseAccessType.Free.ToString() ? CourseAccessType.Free.ToString() : PaymentStatus.Pending.ToString(),
                CompletionPercentage = 0,
                IsActive = true,
                CreateAt = DateTime.Now,
                CreateBy = userId
            };
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _reposiotry.CreateStudentEnrollmentAsync(data);
                if (batch.course.AccessType == CourseAccessType.Free.ToString())
                {
                    await _unitOfWork.SaveAsync();
                    await _unitOfWork.CommitTransactionAsync();
                    response.ResponseCode = (int)APIResponseCodes.OK;
                    response.Success = true;
                    response.Data = new
                    {
                        EnrollmentId = data.Id,
                        BatchId = data.BatchId,
                        CourseId = data.CourseId,
                        PaymentStatus = data.PaymentStatus,
                        IsActive = data.IsActive
                    };
                    response.Message = SystemMessage.RecordCreate;
                    return response;
                }
                await _unitOfWork.SaveAsync();
                var orderReference = $"NXV-{Guid.NewGuid():N}";

                var payment = new Payments
                {
                    StudentId = userId,
                    EnrollmentId = data.Id,
                    Amount = batch.course.Price,
                    PaymentGateway = "RazerPay",
                    PaymentStatus = PaymentStatus.Pending.ToString(),
                    TransactionId = orderReference,
                    PaidAt = null,
                    ProviderEventId = null,
                    CreateAt = DateTime.Now,
                    CreateBy = userId
                };
                await _reposiotry.CreatePaymentAsync(payment);
                await _unitOfWork.SaveAsync();
                // Call the actual ToroPay adapter only after its request
                // contract and credentials have been configured.

                var checkout = await _paymentCheckoutServices.CreateCheckoutAsync(orderReference, batch.course.Price ?? 0, userId, batch.Name ?? "Nexvara LMS Course");

                if (!checkout.Success || string.IsNullOrWhiteSpace(checkout.OrderId))
                {

                    payment.PaymentStatus = PaymentStatus.Failed.ToString();
                    await _unitOfWork.SaveAsync();

                    response.ResponseCode = (int)APIResponseCodes.BadRequest;
                    response.Success = false;
                    response.Message = SystemMessage.UnablePaymentCheckout;
                    return response;
                }
                if (!string.IsNullOrWhiteSpace(checkout.OrderId))
                {
                    payment.TransactionId = checkout.OrderId;
                    await _unitOfWork.SaveAsync();
                }
                await _unitOfWork.CommitTransactionAsync();
                response.ResponseCode = (int)APIResponseCodes.OK;
                response.Success = true;
                response.Message = SystemMessage.EnrollmentCreatedPaymentPending;
                response.Data = new
                {
                    EnrollmentId = data.Id,
                    BatchId = data.BatchId,
                    CourseId = data.CourseId,
                    PaymentId = payment.Id,
                    RazorpayOrderId = checkout.OrderId,
                    RazorpayKeyId = checkout.KeyId,
                    Amount = checkout.Amount,
                    Currency = checkout.Currency,
                    PaymentStatus = payment.PaymentStatus,
                    IsActive = data.IsActive
                };
                return response;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<ResponseDto<object>> GetByIdBatchStudentAsync(int Id)
        {
            var response = new ResponseDto<object>();
            if (Id <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }
            var res = await _reposiotry.GetByIdBatchStudentAsync(Id);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Data = res;
            response.Message = SystemMessage.RecordFetch;
            return response;
        }

        public async Task<ResponseDto<object>> GetContinueLearningAsync(string studentId)
        {
            var response = new ResponseDto<object>();
            // 1. Validate input
            if (string.IsNullOrWhiteSpace(studentId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = "Student not found.";
                return response;
            }
            var data = await _reposiotry.GetContinueLearningAsync(studentId);

            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordFetch;
            response.Data = data;

            return response;
        }

        public async Task<ResponseDto<object>> GetEnrolledBatchesAsync(string studentId, StudentEnrolledBatchFilterDto filter)
        {
            var response = new ResponseDto<object>();
            // 1. Validate input
            if (string.IsNullOrWhiteSpace(studentId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = "Student not found.";
                return response;
            }

            // 2. Pagination validation
            filter.PageNumber = Math.Max(1, filter.PageNumber);
            filter.PageSize = Math.Clamp(filter.PageSize, 1, 50);

            // 3. Get counts
            var totalBatches = await _reposiotry.GetStudentBatchCountAsync(studentId, null);

            var activeBatches = await _reposiotry.GetStudentBatchCountAsync(studentId, "Active");

            var completedBatches = await _reposiotry.GetStudentBatchCountAsync(studentId, "Completed");

            var upcomingBatches = await _reposiotry.GetStudentBatchCountAsync(studentId, "Upcoming");

            // 4. Get filtered count
            var filteredCount = await _reposiotry.GetStudentBatchCountAsync(studentId, filter.Status);

            // Search count is handled separately below in the
            // improved repository version described after this code.

            // 5. Get paginated batches
            var batches = await _reposiotry.GetStudentEnrolledBatchesAsync(studentId, filter);

            // 6. Prepare response
            var result = new GetStudentEnrolledBatchListDto
            {
                TotalBatches = totalBatches,
                ActiveBatches = activeBatches,
                CompletedBatches = completedBatches,
                UpcomingBatches = upcomingBatches,

                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,

                TotalPages = (int)Math.Ceiling(
                    filteredCount / (double)filter.PageSize),

                Batches = batches
            };

            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = "Enrolled batches fetched successfully.";
            response.Data = result;

            return response;

        }

        public async Task<ResponseDto<object>> GetExploreCoursesAsync(ExploreCourseRequestDto dto)
        {
            var response = new ResponseDto<object>();
            if (dto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            var res = await _reposiotry.GetExploreCoursesAsync(dto);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Data = res;
            response.Message = SystemMessage.RecordFetch;
            return response;
        }

        public async Task<ResponseDto<object>> GetListBatchStudentAsync(PaginationRequestDto dto)
        {
            var response = new ResponseDto<object>();
            if (dto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            var res = await _reposiotry.GetListBatchStudentAsync(dto.PageNumber, dto.PageSize);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Data = res;
            response.Message = SystemMessage.RecordFetch;
            return response;
        }

        public async Task<ResponseDto<object>> GetStudentSechduleAsync(DateTime? dateTime, string studentId)
        {
            var response = new ResponseDto<object>();
            if (string.IsNullOrWhiteSpace(studentId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }
            var res = await _reposiotry.GetStudentSechduleAsync(dateTime, studentId);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordFetch;
            response.Data = res;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateBatchStudentAsync(UpdateBatchStudentDto dto, string userId)
        {
            var response = new ResponseDto<object>();
            if (dto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }
            var res = await _reposiotry.GetByIdBatchStudentUpdateAsync(dto.Id);
            res.BatchId = dto.BatchId;
            res.StudentId = dto.StudentId;
            res.EnrolledAt = dto.EnrolledAt;
            res.CompleteAt = dto.CompleteAt;
            res.UpdateAt = DateTime.Now;
            res.UpdateBy = userId;
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordUpdate;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateStatusBatchStudentAsync(int Id, string userId)
        {
            var response = new ResponseDto<object>();
            if (Id <= 0)
            {

                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }
            var res = await _reposiotry.UpdateBatchStudentAsync(Id);
            res.IsActive = !res.IsActive;
            res.UpdateAt = DateTime.Now;
            res.UpdateBy = userId;
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordUpdate;
            return response;
        }
    }
}
