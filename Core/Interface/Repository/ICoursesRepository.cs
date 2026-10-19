using Core.Dtos.Common;
using Core.Dtos.Courses;
using Domain.Entities;
using Domain.Entities.Student;
using Domain.Entities.Teacher;
using Domain.Entities.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface.Repository
{
    public interface ICoursesRepository
    {
        //Course
        Task CreateCourseAsync(Course course);
        Task<Course?> GetByIdCoursesUpdateAsync(int Id);
        Task<GetCoursesDto?> GetByIdCoursesAsync(int Id);
        Task<PageResultDto<GetCoursesDto>?> GetListCourseAsync(int pageNumber, int pageSize);
        Task<Course?> UpdateCourseAsync(int Id);

        //Course Setting
        Task CreateCourseSettingAsync(CourseSettings course);
        Task<CourseSettings?> GetByIdCoursesSettingAsync(int Id);

        //Course Module
        Task CreateCourseModuleAsync(CourseModule course);
        Task<CourseModule?> GetByIdCoursesModuleUpdateAsync(int Id);
        Task<GetCourseModuleDto?> GetByIdCoursesModuleAsync(int Id);
        Task<PageResultDto<GetCourseModuleDto>?> GetListCourseModuleAsync(int pageNumber, int pageSize);
        Task<CourseModule?> UpdateCourseModuleAsync(int Id);

        //Batch 
        Task CreateBatchAsync(Batch entity);
        Task<Batch?> GetByIdBatchUpdateAsync(int Id);
        Task<GetCourseBatchDto?> GetByIdBatchAsync(int Id);
        Task<PageResultDto<GetCourseBatchDto>?> GetListBatchAsync(int pageNumber, int pageSize);
        Task<Batch?> UpdateBatchAsync(int Id);

       
    }
}
