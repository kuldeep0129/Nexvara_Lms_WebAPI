using Core.Dtos.Common;
using Core.Dtos.Courses;
using Core.Dtos.LiveClasses;
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
    public interface ILiveClassRepository
    {
        //LiveClass
        Task CreateLiveClassAsync(LiveClasses entity);
        Task<LiveClasses?> GetByIdLiveClassUpdateAsync(int Id);
        Task<GetLiveClassDto?> GetByIdLiveClassAsync(int Id);
        Task<PageResultDto<GetLiveClassDto>?> GetListLiveClassAsync(int pageNumber, int pageSize);
        Task<LiveClasses?> UpdateLiveClassAsync(int Id);

        //ClassAttendance 
        Task CreateClassAttendanceAsync(ClassAttendance entity);
        Task<ClassAttendance?> GetByIdClassAttendanceUpdateAsync(int Id);
        Task<GetClassAttendanc?> GetByIdClassAttendanceAsync(int Id);
        Task<PageResultDto<GetClassAttendanc>?> GetListClassAttendanceAsync(int pageNumber, int pageSize);
        Task<ClassAttendance?> UpdateClassAttendanceAsync(int Id);


        //CourseVideos 
        Task CreateCourseVideosAsync(CourseVideos entity);
        Task<CourseVideos?> GetByIdCourseVideosUpdateAsync(int Id);
        Task<GetCourseVideoDto?> GetByIdCourseVideosAsync(int Id);
        Task<PageResultDto<GetCourseVideoDto>?> GetListCourseVideosAsync(int pageNumber, int pageSize);
        Task<CourseVideos?> UpdateCourseVideosAsync(int Id);

        //VideoProgress 
        Task CreateVideoProgressAsync(VideoProgress entity);
        Task<VideoProgress?> GetByIdVideoProgressUpdateAsync(int Id);
        Task<GetVideoProgressDto?> GetByIdVideoProgressAsync(int Id);
        Task<PageResultDto<GetVideoProgressDto>?> GetListVideoProgressAsync(int pageNumber, int pageSize);
        Task<VideoProgress?> UpdateVideoProgressAsync(int Id);
    }
}
