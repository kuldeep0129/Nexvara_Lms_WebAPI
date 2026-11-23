using Core.Dtos.Certificate;
using Core.Dtos.Common;
using Core.Dtos.Courses;
using Core.Dtos.Student;
using Core.Dtos.Student.Batch;
using Core.Dtos.Student.Sechdule;
using Domain.Entities.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface.Services
{
    public interface IStudentServices
    {
        //BatchStudent
        Task<ResponseDto<object>> CreateBatchStudentAsync(CreateBatchStudentDto dto, string userId);
        Task<ResponseDto<object>> UpdateBatchStudentAsync(UpdateBatchStudentDto dto, string userId);
        Task<ResponseDto<object>> UpdateStatusBatchStudentAsync(int Id, string userId);
        Task<ResponseDto<object>> GetByIdBatchStudentAsync(int Id);
        Task<ResponseDto<object>> GetListBatchStudentAsync(PaginationRequestDto dto);

        //New Batch Code
        Task<ResponseDto<object>> GetExploreCoursesAsync(ExploreCourseRequestDto dto);

        //Enrollment
        Task<ResponseDto<object>> CreateStudentEnrollmentAsync(CreateStudentEnrollmentDto dto, string userId);

        //Schedule 
        Task<ResponseDto<object>> GetStudentSechduleAsync(DateTime? dateTime, string studentId);
        Task<ResponseDto<object>> GetEnrolledBatchesAsync(  string studentId, StudentEnrolledBatchFilterDto filter);
        Task<ResponseDto<object>> GetContinueLearningAsync(  string studentId);

        //For Student Attandnce
        Task<ResponseDto<object>> GetMyClassAttendanceAsync( string studentId,  PaginationRequestDto dto);

        // Certificates
        Task<ResponseDto<object>> GetMyCertificatesAsync(string studentId);
        Task<ResponseDto<object>> IssueCertificateAsync(IssueCertificateDto dto, string issuedBy);
        Task<ResponseDto<object>> VerifyCertificateAsync(string verificationCode);

        //Payment
        Task<ResponseDto<object>> GetStudentPaymentsAsync(string studentId);

        // Student Dashboard
        Task<ResponseDto<object>> GetStudentDashboardAsync(string studentId);


        // Student Assessment
        Task<ResponseDto<object>> GetMyAssessmentsAsync(string studentId);
        Task<ResponseDto<object>> GetAssessmentDetailsAsync(  int assessmentId, string studentId);
        Task<ResponseDto<object>> StartAssessmentAsync(int assessmentId,  string studentId);
        Task<ResponseDto<object>> SubmitAssessmentAsync( int attemptId, SubmitAssessmentDto dto, string studentId);
        Task<ResponseDto<object>> GetMyAssessmentResultsAsync(string studentId);
        Task<List<QuestionOptions>> GetQuestionOptionsAsync(int questionId);


    }
}
