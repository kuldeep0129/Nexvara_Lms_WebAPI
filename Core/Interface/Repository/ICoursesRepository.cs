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
        Task<PageResultDto<GetCoursesDto>> GetListCourseFeatureAsync(int pageNumber,int pageSize);
        Task<PageResultDto<GetCoursesDto>> GetListCoursePublishAsync(int pageNumber,int pageSize);
        Task<PageResultDto<GetCourseSyllabusDto>> GetListCourseSyllabusAsync(int pageNumber,int pageSize, int courseId);

        //Course Setting
        Task CreateCourseSettingAsync(CourseSettings course);
        Task<CourseSettings?> GetByIdCoursesSettingAsync(int Id);

        //Course Module
        Task CreateCourseModuleAsync(CourseModule course);
        Task<CourseModule?> GetByIdCoursesModuleUpdateAsync(int Id);
        Task<GetCourseModuleDto?> GetByIdCoursesModuleAsync(int Id);
        Task<PageResultDto<GetCourseModuleDto>?> GetListCourseModuleAsync(int pageNumber, int pageSize);
        Task<CourseModule?> UpdateCourseModuleAsync(int Id);
        Task<GetStudyMaterialdashboardDto> GetStudyMaterialAsync(string useId);


        //Batch 
        Task CreateBatchAsync(Batch entity);
        Task<Batch?> GetByIdBatchUpdateAsync(int Id);
        Task<GetCourseBatchDto?> GetByIdBatchAsync(int Id);
        Task<PageResultDto<GetCourseBatchDto>?> GetListBatchAsync(int pageNumber, int pageSize);
        Task<Batch?> UpdateBatchAsync(int Id);

        //Course Syllabus 
        Task CreateCourseSyllabusAsync(CourseSyllabus syllabus);
        Task<CourseSyllabus> GetByIdCourseSyllabusAsync(int Id);
        Task<bool> IsDisplayOrderExistsCourseSyllabusAsync(int Id, int displayOrder);

        //Course Material 
        Task CreateCourseMaterialAsync(CourseMaterials entity);
        Task<CourseMaterials> GetByIdCourseMaterialAsync(int Id);


        //For Admin and Teacher Manage Course Batch
        Task<bool> CourseExistsAsync(int courseId);
        Task<GetCourseCurriculumGroupDto?> GetCourseCurriculumAsync( int courseId);

        Task<GetCourseSyllabusDto> GetCourseSyllabusAsync(int id);
    }
}
