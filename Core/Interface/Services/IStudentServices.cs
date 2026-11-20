using Core.Dtos.Common;
using Core.Dtos.Courses;
using Core.Dtos.Student.Batch;
using Core.Dtos.Student.Sechdule;
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


    }
}
