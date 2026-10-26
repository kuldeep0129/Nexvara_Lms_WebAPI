using Core.Dtos.Common;
using Core.Dtos.Courses;
using Core.Dtos.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface.Services
{
    public interface ICoursesServices
    {
        Task<ResponseDto<object>> CreateCoursesAsync(CreateCourses course, string userId);
        Task<ResponseDto<object>> UpdateCoursesAsync(UpdateCourses course, string userId);
        Task<ResponseDto<object>> UpdateStatusCoursesAsync(int Id,string userId);
        Task<ResponseDto<object>> GetByIdCoursesAsync(int Id);
        Task<ResponseDto<object>> GetListCoursesAsync(PaginationRequestDto dto);
        Task<ResponseDto<object>> GetListCoursesFeaturesAsync(PaginationRequestDto dto);
        Task<ResponseDto<object>> GetListCoursesPublishAsync(PaginationRequestDto dto);

        //CourseModule
        Task<ResponseDto<object>> CreateCoursesModuleAsync(CreateCourseModuleDto course, string userId);
        Task<ResponseDto<object>> UpdateCoursesModuleAsync(UpdateCourseModule course, string userId);
        Task<ResponseDto<object>> UpdateStatusCoursesModuleAsync(int Id, string userId);
        Task<ResponseDto<object>> GetByIdCoursesModuleAsync(int Id);
        Task<ResponseDto<object>> GetListCoursesModuleAsync(PaginationRequestDto dto);

        //Batch
        Task<ResponseDto<object>> CreateBatchAsync(CreateBatchDto dto, string userId);
        Task<ResponseDto<object>> UpdateBatchAsync(UpdateCourseBatchDto dto, string userId);
        Task<ResponseDto<object>> UpdateStatusBatchAsync(int Id, string userId);
        Task<ResponseDto<object>> GetByIdBatchAsync(int Id);
        Task<ResponseDto<object>> GetListBatchAsync(PaginationRequestDto dto);
    }
}
