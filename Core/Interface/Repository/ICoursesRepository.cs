using Core.Dtos.Common;
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
        Task CreateCourseAsync(Course course);
        Task<Course?> GetByIdCoursesAsync(string Id);
        Task<PageResultDto<Course>?> GetListCourseAsync(int pageNumber, int pageSize);
        Task<Course?> UpdateCourseAsync(string Id);
    }
}
