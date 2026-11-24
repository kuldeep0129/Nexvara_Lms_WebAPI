using AutoMapper;
using Core.Dtos.Certificate;
using Core.Dtos.Common;
using Core.Dtos.Courses;
using Core.Dtos.Payments;
using Core.Dtos.Student;
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

        public async Task<ResponseDto<object>> GetAssessmentDetailsAsync(int assessmentId, string studentId)
        {
            var response = new ResponseDto<object>();

            if (string.IsNullOrWhiteSpace(studentId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }

            if (assessmentId <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = "Invalid assessment ID.";
                return response;
            }

            var data = await _reposiotry.GetAssessmentDetailsAsync(
                assessmentId, studentId);

            if (data == null)
            {
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                response.Success = false;
                response.Message = "Assessment not found or you are not enrolled in this batch.";
                return response;
            }

            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = "Assessment details fetched successfully.";
            response.Data = data;

            return response;
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

        public async Task<ResponseDto<object>> GetExploreCoursesAsync(ExploreCourseRequestDto dto,string userId)
        {
            var response = new ResponseDto<object>();
            if (dto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            var res = await _reposiotry.GetExploreCoursesAsync(dto,userId);
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

        public async Task<ResponseDto<object>> GetMyAssessmentResultsAsync(string studentId)
        {
            var response = new ResponseDto<object>();

            if (string.IsNullOrWhiteSpace(studentId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }

            var data = await _reposiotry.GetMyAssessmentResultsAsync(studentId);

            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = "Assessment results fetched successfully.";
            response.Data = data;

            return response;
        }

        public async Task<ResponseDto<object>> GetMyAssessmentsAsync(string studentId)
        {
            var response = new ResponseDto<object>();

            if (string.IsNullOrWhiteSpace(studentId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }

            var data = await _reposiotry.GetMyAssessmentsAsync(studentId);

            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = "Assessments fetched successfully.";
            response.Data = data;

            return response;
        }

        public async Task<ResponseDto<object>> GetMyCertificatesAsync(string studentId)
        {

            var response = new ResponseDto<object>();
            if (string.IsNullOrWhiteSpace(studentId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = "Student authentication is required.";
                return response;
            }

            response.Data = await _reposiotry.GetMyCertificatesAsync(studentId);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = "Certificates fetched successfully.";
            return response;
        }

        public async Task<ResponseDto<object>> GetMyClassAttendanceAsync(string studentId, PaginationRequestDto dto)
        {
            var response = new ResponseDto<object>();

            if (string.IsNullOrWhiteSpace(studentId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }

            if (dto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }

            dto.PageNumber = Math.Max(1, dto.PageNumber);
            dto.PageSize = Math.Clamp(dto.PageSize, 1, 50);

            var data = await _reposiotry.GetStudentClassAttendanceAsync(  studentId,  dto.PageNumber,  dto.PageSize);

            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordFetch;
            response.Data = data;

            return response;

        }

        public Task<List<QuestionOptions>> GetQuestionOptionsAsync(int questionId)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDto<object>> GetStudentDashboardAsync(string studentId)
        {
            var response = new ResponseDto<object>();

            // =====================================================
            // 1. Validate Student
            // =====================================================

            if (string.IsNullOrWhiteSpace(studentId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }

            // =====================================================
            // 2. Get Dashboard Data
            // =====================================================

            var data = await _reposiotry.GetStudentDashboardAsync(studentId);

            // =====================================================
            // 3. Prepare Response
            // =====================================================

            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = "Student dashboard fetched successfully.";
            response.Data = data;

            return response;
        }

        public async Task<ResponseDto<object>> GetStudentPaymentsAsync(string studentId)
        {

            var response = new ResponseDto<object>();
            if (string.IsNullOrWhiteSpace(studentId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }
            var res = await _reposiotry.GetStudentPaymentsAsync(studentId);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordFetch;
            response.Data = res;
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

        public async Task<ResponseDto<object>> IssueCertificateAsync(IssueCertificateDto dto, string issuedBy)
        {
            var response = new ResponseDto<object>();
            if (dto == null || string.IsNullOrWhiteSpace(dto.StudentId) || dto.CourseId <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = "Valid StudentId and CourseId are required.";
                return response;
            }
            if (!await _reposiotry.IsStudentEligibleForCertificateAsync(dto.StudentId, dto.CourseId))
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = "Student must complete the course and have a successful/free enrollment before certificate issuance.";
                return response;
            }
            if (await _reposiotry.IsCertificateIssuedAsync(dto.StudentId, dto.CourseId))
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = "A certificate has already been issued for this student and course.";
                return response;
            }

            var now = DateTime.UtcNow;
            var certificate = new Certificates
            {
                StudentId = dto.StudentId.Trim(),
                CourseId = dto.CourseId,
                CertificateNumber = $"NXV-{now:yyyyMMdd}-{Guid.NewGuid():N}".ToUpperInvariant(),
                VerificationCode = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(24)),
                IssueDate = now,
                CertificateUrl = string.IsNullOrWhiteSpace(dto.CertificateUrl) ? null : dto.CertificateUrl.Trim(),
                IsActive = true,
                CreateAt = now,
                CreateBy = issuedBy
            };

            await _reposiotry.CreateStudentCertificateAsync(certificate);
            await _unitOfWork.SaveAsync();

            response.Data = new
            {
                certificate.Id,
                certificate.CertificateNumber,
                certificate.VerificationCode,
                certificate.StudentId,
                certificate.CourseId,
                certificate.IssueDate,
                certificate.CertificateUrl
            };
            response.ResponseCode = (int)APIResponseCodes.Created;
            response.Success = true;
            response.Message = "Certificate issued successfully.";
            return response;
        }

        public async Task<ResponseDto<object>> StartAssessmentAsync(int assessmentId, string studentId)
        {
            var response = new ResponseDto<object>();

            if (string.IsNullOrWhiteSpace(studentId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }

            var assessment = await _reposiotry.GetAssessmentDetailsAsync(
                assessmentId, studentId);

            if (assessment == null)
            {
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                response.Success = false;
                response.Message = "Assessment not found.";
                return response;
            }

            var now = DateTime.Now;

            if (assessment.StartAt > now)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = "Assessment has not started yet.";
                return response;
            }

            if (assessment.EndAt.HasValue && assessment.EndAt.Value <= now)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = "Assessment time has expired.";
                return response;
            }

            var existingAttempt =
                await _reposiotry.GetInProgressAssessmentAttemptAsync(
                    assessmentId, studentId);

            if (existingAttempt != null)
            {
                var existingExpiry = existingAttempt.StartedAt!.Value.AddMinutes(
                    (double)assessment.DurationInMinutes);

                if (now > existingExpiry ||
                    (assessment.EndAt.HasValue && now > assessment.EndAt.Value))
                {
                    response.ResponseCode = (int)APIResponseCodes.BadRequest;
                    response.Success = false;
                    response.Message = "Your assessment attempt has expired.";
                    return response;
                }

                response.ResponseCode = (int)APIResponseCodes.OK;
                response.Success = true;
                response.Message = "Existing assessment attempt resumed.";

                response.Data = new StartAssessmentDto
                {
                    AttemptId = existingAttempt.Id,
                    AssessmentId = assessmentId,
                    StartedAt = existingAttempt.StartedAt.Value,
                    ExpiresAt = assessment.EndAt.HasValue
                        ? (existingExpiry < assessment.EndAt.Value
                            ? existingExpiry
                            : assessment.EndAt.Value)
                        : existingExpiry
                };

                return response;
            }

            var attempt = new AssessmentAttempts
            {
                AssessmentId = assessmentId,
                StudentId = studentId,
                StartedAt = now,
                SubmittedAt = default,
                ObtainedMarks = 0,
                Percentage = 0,
                IsPassed = false,
                IsActive = true,
                CreateBy = studentId,
                CreateAt = now
            };

            await _reposiotry.CreateAssessmentAttemptAsync(attempt);
            await _unitOfWork.SaveAsync();

            var expiresAt = now.AddMinutes(
                (double)assessment.DurationInMinutes);

            if (assessment.EndAt.HasValue &&
                assessment.EndAt.Value < expiresAt)
            {
                expiresAt = assessment.EndAt.Value;
            }

            response.ResponseCode = (int)APIResponseCodes.Created;
            response.Success = true;
            response.Message = "Assessment started successfully.";

            response.Data = new StartAssessmentDto
            {
                AttemptId = attempt.Id,
                AssessmentId = assessmentId,
                StartedAt = now,
                ExpiresAt = expiresAt
            };

            return response;
        }

        public async Task<ResponseDto<object>> SubmitAssessmentAsync(int attemptId, SubmitAssessmentDto dto, string studentId)
        {
            var response = new ResponseDto<object>();

            if (string.IsNullOrWhiteSpace(studentId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }

            if (dto == null || attemptId <= 0 || dto.Answers == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = "Invalid assessment submission.";
                return response;
            }

            var attempt = await _reposiotry.GetAssessmentAttemptForSubmitAsync(
                attemptId, studentId);

            if (attempt == null)
            {
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                response.Success = false;
                response.Message = "Assessment attempt not found.";
                return response;
            }

           

            if (!attempt.AssessmentId.HasValue ||
                !attempt.StartedAt.HasValue)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = "Invalid assessment attempt.";
                return response;
            }

            var assessment = await _reposiotry.GetAssessmentByIdAsync(
                attempt.AssessmentId.Value);

            if (assessment == null)
            {
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                response.Success = false;
                response.Message = "Assessment not found.";
                return response;
            }

            var now = DateTime.Now;

            var expiryTime = attempt.StartedAt.Value.AddMinutes(
                (double)(assessment.DurationInMinutes ?? 0));

            if (assessment.EndAt.HasValue &&
                assessment.EndAt.Value < expiryTime)
            {
                expiryTime = assessment.EndAt.Value;
            }

            if (now > expiryTime)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = "Assessment submission time has expired.";
                return response;
            }

            if (dto.Answers
                .GroupBy(x => x.QuestionId)
                .Any(x => x.Count() > 1))
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = "Duplicate question answers are not allowed.";
                return response;
            }

            var questions = await _reposiotry.GetAssessmentQuestionsAsync(
                attempt.AssessmentId.Value);

            var questionDictionary = questions.ToDictionary(x => x.Id);

            if (dto.Answers.Any(x =>
                !questionDictionary.ContainsKey(x.QuestionId)))
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = "Invalid question submitted.";
                return response;
            }

            decimal obtainedMarks = 0;

            var answerEntities = new List<AssessmentAnswers>();

            foreach (var answer in dto.Answers)
            {
                var question = questionDictionary[answer.QuestionId];

                if (answer.SelectedOptionId.HasValue &&
                    !string.IsNullOrWhiteSpace(answer.AnswerText))
                {
                    response.ResponseCode = (int)APIResponseCodes.BadRequest;
                    response.Success = false;
                    response.Message = "Submit either an option or text answer for a question.";
                    return response;
                }

                decimal? marksObtained = null;

                if (answer.SelectedOptionId.HasValue)
                {
                    var options = await _reposiotry.GetQuestionOptionsAsync(
                        question.Id);

                    var selectedOption = options.FirstOrDefault(x =>
                        x.Id == answer.SelectedOptionId.Value);

                    if (selectedOption == null)
                    {
                        response.ResponseCode = (int)APIResponseCodes.BadRequest;
                        response.Success = false;
                        response.Message = "Invalid question option selected.";
                        return response;
                    }

                    marksObtained = selectedOption.IsCorrect
                        ? question.Marks
                        : 0;

                    obtainedMarks += marksObtained.Value;
                }
                else if (string.IsNullOrWhiteSpace(answer.AnswerText))
                {
                    marksObtained = 0;
                }

                answerEntities.Add(new AssessmentAnswers
                {
                    AttemptId = attempt.Id,
                    QuestionId = question.Id,
                    SelectedOptionId = answer.SelectedOptionId,
                    AnswerText = answer.AnswerText?.Trim(),
                    MarksObtained = marksObtained,
                    IsActive = true,
                    CreateAt = now,
                    CreateBy = studentId
                });
            }

            var totalMarks = assessment.TotalMarks ?? 0;

            var percentage = totalMarks > 0
                ? Math.Round((obtainedMarks / totalMarks) * 100, 2)
                : 0;

            attempt.ObtainedMarks = obtainedMarks;
            attempt.Percentage = percentage;
            attempt.IsPassed = obtainedMarks >= (assessment.PassingMarks ?? 0);
            attempt.SubmittedAt = now;
            attempt.UpdateAt = now;
            attempt.UpdateBy = studentId;

            await _reposiotry.CreateAssessmentAnswersAsync(answerEntities);

            await _unitOfWork.SaveAsync();

            var result = new StudentAssessmentResultDto
            {
                AttemptId = attempt.Id,
                AssessmentId = assessment.Id,
                AssessmentTitle = assessment.Title,
                TotalMarks = totalMarks,
                ObtainedMarks = obtainedMarks,
                Percentage = percentage,
                PassingMarks = assessment.PassingMarks ?? 0,
                IsPassed = attempt.IsPassed,
                StartedAt = attempt.StartedAt,
                SubmittedAt = attempt.SubmittedAt
            };

            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = "Assessment submitted successfully.";
            response.Data = result;

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

        public async Task<ResponseDto<object>> VerifyCertificateAsync(string verificationCode)
        {

            var response = new ResponseDto<object>();
            if (string.IsNullOrWhiteSpace(verificationCode))
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = "Verification code is required.";
                return response;
            }

            var certificate = await _reposiotry.GetCertificateByVerificationCodeAsync(verificationCode.Trim());
            if (certificate == null)
            {
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                response.Success = false;
                response.Message = "Certificate not found.";
                return response;
            }

            response.Data = new CertificateVerificationDto
            {
                IsValid = !certificate.IsActive,
                CertificateNumber = certificate.CertificateNumber,
                StudentName = certificate.user?.FullName ?? string.Empty,
                CourseName = certificate.course?.Title ?? string.Empty,
                IssuedAt = certificate.IssueDate ?? default
            };
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = certificate.IsActive ? "This certificate has been revoked." : "Certificate is valid.";
            return response;
        }
    }
}
