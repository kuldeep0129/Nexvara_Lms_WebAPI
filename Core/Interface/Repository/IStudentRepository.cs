using Core.Dtos.Certificate;
using Core.Dtos.Common;
using Core.Dtos.Courses;
using Core.Dtos.Dashboard;
using Core.Dtos.Payments;
using Core.Dtos.Student;
using Core.Dtos.Student.Attendance;
using Core.Dtos.Student.Batch;
using Core.Dtos.Student.Sechdule;
using Domain.Entities;
using Domain.Entities.Common;
using Domain.Entities.Student;
using Domain.Entities.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface.Repository
{
    public interface IStudentRepository
    {
        //Batch Student
        Task CreateBatchStudentAsync(BatchStudent entity);
        Task<BatchStudent?> GetByIdBatchStudentUpdateAsync(int Id);
        Task<GetBatchStudentDto?> GetByIdBatchStudentAsync(int Id);
        Task<PageResultDto<GetBatchStudentDto>?> GetListBatchStudentAsync(int pageNumber, int pageSize);
        Task<BatchStudent?> UpdateBatchStudentAsync(int Id);

        //New Batch Code 
        Task<ExploreCourseResponseDto<ExploreBatchDto>> GetExploreCoursesAsync(ExploreCourseRequestDto request, string userId);

        //Enrollment
        Task CreateStudentEnrollmentAsync(Enrollment entity);
        Task<bool> IsCheckAlreadyStudentEnrollAsync(string userId, int courseId, int batchId);
        Task<bool> IsCheckBatchFreeOrPaidAsync(int courseId,int batchId);
        Task CreatePaymentAsync(Payments entity);
        Task<Batch?> GetBatchForEnrollmentAsync(int batchId);

        //Schedule 
        Task<List<GetStudentSechduleDto>> GetStudentSechduleAsync(DateTime? dateTime, string studentId);

        //My Batch
        Task<int> GetStudentBatchCountAsync( string studentId,string? status);

        Task<List<GetStudentEnrolledBatchDto>> GetStudentEnrolledBatchesAsync(string studentId, StudentEnrolledBatchFilterDto filter);
        Task<List<GetContinueLearningDto>> GetContinueLearningAsync(string studentId);

        // Student Attendance
        Task<PageResultDto<GetStudentClassAttendanceDto>>   GetStudentClassAttendanceAsync( string studentId,  int pageNumber,  int pageSize);

        // Certificates
        Task<List<StudentCertificateDto>> GetMyCertificatesAsync(string studentId);
        Task<bool> IsCertificateIssuedAsync(string studentId, int courseId);
        Task<bool> IsStudentEligibleForCertificateAsync(string studentId, int courseId);
        Task<Certificates?> GetCertificateByVerificationCodeAsync(string verificationCode);
        Task CreateStudentCertificateAsync(Certificates certificate);

        //Payment Repoisitory
        Task<List<StudentPaymentDto>> GetStudentPaymentsAsync(string studentId);

        // Student Dashboard
        Task<GetStudentDashboardDto> GetStudentDashboardAsync(string studentId);

        // Student Assessment

        Task<List<StudentAssessmentListDto>> GetMyAssessmentsAsync(string studentId);
        Task<StudentAssessmentDetailsDto?> GetAssessmentDetailsAsync( int assessmentId, string studentId);
        Task<Assessments?> GetAssessmentByIdAsync(int assessmentId);
        Task<AssessmentAttempts?> GetInProgressAssessmentAttemptAsync( int assessmentId, string studentId);
        Task<AssessmentAttempts?> GetAssessmentAttemptForSubmitAsync( int attemptId, string studentId);
        Task CreateAssessmentAttemptAsync(AssessmentAttempts attempt);
        Task<List<AssessmentQuestions>> GetAssessmentQuestionsAsync( int assessmentId);
        Task CreateAssessmentAnswersAsync( List<AssessmentAnswers> answers);
        Task<List<StudentAssessmentResultDto>> GetMyAssessmentResultsAsync(string studentId);
        Task<List<QuestionOptions>> GetQuestionOptionsAsync(int questionId);


    }
}
