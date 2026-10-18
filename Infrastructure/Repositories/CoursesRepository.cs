using Core.Dtos.Common;
using Core.Interface.Repository;
using Domain.Entities.UserInterface;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class CoursesRepository : ICoursesRepository
    {
        private readonly ApplicationDbContext _context;
        public CoursesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateCourseAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
        }

        public Task<Course?> GetByIdCoursesAsync(string Id)
        {
            throw new NotImplementedException();
        }

        public Task<PageResultDto<Course>?> GetListCourseAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<Course?> UpdateCourseAsync(string Id)
        {
            throw new NotImplementedException();
        }
    }
}
