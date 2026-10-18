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
        Task<ResponseDto<object>> UpdateStatusCoursesAsync(string userId);
        Task<ResponseDto<object>> GetByIdCoursesAsync(string userId);
        Task<ResponseDto<object>> GetListCoursesAsync(PaginationRequestDto dto);
    }
}
