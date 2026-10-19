using Core.Dtos.Common;
using Core.Dtos.LiveClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface.Services
{
    public interface ILiveClassServices
    {
        //LiveClass
        Task<ResponseDto<object>> CreateLiveClassAsync(CreateLiveClassDto dto, string userId);
        Task<ResponseDto<object>> UpdateLiveClassAsync(UpdateLiveClassesDto dto, string userId);
        Task<ResponseDto<object>> UpdateStatusLiveClassAsync(int Id, string userId);
        Task<ResponseDto<object>> GetByIdLiveClassAsync(int Id);
        Task<ResponseDto<object>> GetListLiveClassAsync(PaginationRequestDto dto);


        //ClassAttendance
        Task<ResponseDto<object>> CreateClassAttendanceAsync(CreateClassAttendanceDto dto, string userId);
        Task<ResponseDto<object>> UpdateClassAttendanceAsync(UpdateClassAttendance dto, string userId);
        Task<ResponseDto<object>> UpdateStatusClassAttendanceAsync(int Id, string userId);
        Task<ResponseDto<object>> GetByIdClassAttendanceAsync(int Id);
        Task<ResponseDto<object>> GetListClassAttendanceAsync(PaginationRequestDto dto);


        //CourseVideo
        Task<ResponseDto<object>> CreateCourseVideoAsync(CreateCourseVideoDto dto, string userId);
        Task<ResponseDto<object>> UpdateCourseVideoAsync(UpdateCourseVideosDto dto, string userId);
        Task<ResponseDto<object>> UpdateStatusCourseVideoAsync(int Id, string userId);
        Task<ResponseDto<object>> GetByIdCourseVideoAsync(int Id);
        Task<ResponseDto<object>> GetListCourseVideoAsync(PaginationRequestDto dto);



        //VideoProgress
        Task<ResponseDto<object>> CreateVideoProgressAsync(CreateVideoProgressDto dto, string userId);
        Task<ResponseDto<object>> UpdateVideoProgressAsync(UpdateVideoProgressDto dto, string userId);
        Task<ResponseDto<object>> UpdateStatusVideoProgressAsync(int Id, string userId);
        Task<ResponseDto<object>> GetByIdVideoProgressAsync(int Id);
        Task<ResponseDto<object>> GetListVideoProgressAsync(PaginationRequestDto dto);
    }
}
