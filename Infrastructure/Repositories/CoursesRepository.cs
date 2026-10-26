using CloudinaryDotNet;
using Core.Dtos.Common;
using Core.Dtos.Courses;
using Core.Interface.Repository;
using Domain.Entities;
using Domain.Entities.Student;
using Domain.Entities.UserInterface;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
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

        public async Task CreateBatchAsync(Batch entity)
        {
            await _context.Batches.AddAsync(entity);
        }

        public async Task CreateCourseAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
        }

        public async Task CreateCourseModuleAsync(CourseModule course)
        {
            await _context.CourseModules.AddAsync(course);
        }

        public async Task CreateCourseSettingAsync(CourseSettings course)
        {
           await _context.CourseSettings.AddAsync(course);
        }

        public async Task<GetCourseBatchDto?> GetByIdBatchAsync(int Id)
        {
            return await _context.Batches.Include(x=>x.course).AsNoTracking().Select(x=> new GetCourseBatchDto
            {
                Id = x.Id,
                CourseName = x.course.Title,
                CourseId = x.CourseId,
                Name = x.Name,
                BatchCode = x.BatchCode,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Capacity = x.Capacity,
                IsActive = x.IsActive
            }).FirstOrDefaultAsync(x=> x.Id == Id && x.IsActive);
        }

        public async Task<Batch?> GetByIdBatchUpdateAsync(int Id)
        {
            return await _context.Batches.FirstOrDefaultAsync(x=> x.Id == Id && x.IsActive);
        }

        public async Task<GetCoursesDto?> GetByIdCoursesAsync(int Id)
        {
            return await _context.Courses.AsNoTracking().Where(x=> x.Id  == Id && x.IsActive).Select(x => new GetCoursesDto
            {
                Id = x.Id,
                Title = x.Title,
                Slug = x.Slug,
                Summary = x.Summary,
                Points = x.Points,
                Technologies = x.Technologies,
                Description = x.Description,
                Duration = x.Duration,
                Demand = x.Demand,
                Star = x.Star,
                Level = x.Level,
                ImageUrl = x.ImageUrl,
                IsPublished = x.IsPublished,
                SortOrder = x.SortOrder,
                IsFeatured = x.IsFeatured,
                IsActive = x.IsActive,

                Icon = _context.CourseSettings
                .Where(s => s.CourseId == x.Id)
                .Select(s => s.Icon)
                .FirstOrDefault(),

                ColorCode = _context.CourseSettings
                .Where(s => s.CourseId == x.Id)
                .Select(s => s.ColorCode)
                .FirstOrDefault(),

                SoftColorCode = _context.CourseSettings
                .Where(s => s.CourseId == x.Id)
                .Select(s => s.SoftColorCode)
                .FirstOrDefault()
            }).FirstOrDefaultAsync();
        }

        public async Task<GetCourseModuleDto?> GetByIdCoursesModuleAsync(int Id)
        {
            return await _context.CourseModules.AsNoTracking().Include(x => x.course).Where(x => x.Id == Id && x.IsActive)
                .Select(x => new GetCourseModuleDto
                {
                    Id = x.Id,
                    CourseId = x.CourseId,
                    CourseName = x.course.Title,
                    Name = x.Name,
                    Description = x.Description,
                    Sortorder = x.Sortorder,
                    IsActive = x.IsActive,
                }).FirstOrDefaultAsync();
        }

        public async Task<CourseModule?> GetByIdCoursesModuleUpdateAsync(int Id)
        {
            return await _context.CourseModules.FirstOrDefaultAsync(x=> x.Id == Id && x.IsActive);
        }

        public async Task<CourseSettings?> GetByIdCoursesSettingAsync(int Id)
        {
            return await _context.CourseSettings.Include(x=> x.Course).FirstOrDefaultAsync(x => x.Id == Id && x.IsActive);
        }

        public async Task<Course?> GetByIdCoursesUpdateAsync(int Id)
        {
            return await _context.Courses.FirstOrDefaultAsync(x=> x.Id == Id && x.IsActive);
        }

        public async Task<PageResultDto<GetCourseBatchDto>?> GetListBatchAsync(int pageNumber, int pageSize)
        {
            var query = _context.Batches.AsNoTracking().Include(x=> x.course) .Select(x => new GetCourseBatchDto
            {
                Id = x.Id,
                CourseName = x.course.Title,
                CourseId = x.CourseId,
                Name = x.Name,
                BatchCode = x.BatchCode,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Capacity = x.Capacity,
                IsActive = x.IsActive
            });

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PageResultDto<GetCourseBatchDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PageResultDto<GetCoursesDto>?> GetListCourseAsync(int pageNumber, int pageSize)
        {
            var query = _context.Courses.AsNoTracking().Select(x => new GetCoursesDto
       {
           Id = x.Id,
           Title = x.Title,
           Slug = x.Slug,
           Summary = x.Summary,
           Points = x.Points,
           Technologies = x.Technologies,
           Description = x.Description,
           Duration = x.Duration,
           Demand = x.Demand,
           Star = x.Star,
           Level = x.Level,
           ImageUrl = x.ImageUrl,
           IsPublished = x.IsPublished,
           SortOrder = x.SortOrder,
           IsFeatured = x.IsFeatured,
           IsActive = x.IsActive,

           Icon = _context.CourseSettings
               .Where(s => s.CourseId == x.Id)
               .Select(s => s.Icon)
               .FirstOrDefault(),

           ColorCode = _context.CourseSettings
               .Where(s => s.CourseId == x.Id)
               .Select(s => s.ColorCode)
               .FirstOrDefault(),

           SoftColorCode = _context.CourseSettings
               .Where(s => s.CourseId == x.Id)
               .Select(s => s.SoftColorCode)
               .FirstOrDefault()
       });

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.SortOrder)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PageResultDto<GetCoursesDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PageResultDto<GetCoursesDto>> GetListCourseFeatureAsync(int pageNumber, int pageSize)
        {
            var query = _context.Courses.AsNoTracking().Select(x => new GetCoursesDto
            {
                Id = x.Id,
                Title = x.Title,
                Slug = x.Slug,
                Summary = x.Summary,
                Points = x.Points,
                Technologies = x.Technologies,
                Description = x.Description,
                Duration = x.Duration,
                Demand = x.Demand,
                Star = x.Star,
                Level = x.Level,
                ImageUrl = x.ImageUrl,
                IsPublished = x.IsPublished,
                SortOrder = x.SortOrder,
                IsFeatured = x.IsFeatured,
                IsActive = x.IsActive,

                Icon = _context.CourseSettings
             .Where(s => s.CourseId == x.Id)
             .Select(s => s.Icon)
             .FirstOrDefault(),

                ColorCode = _context.CourseSettings
             .Where(s => s.CourseId == x.Id)
             .Select(s => s.ColorCode)
             .FirstOrDefault(),

                SoftColorCode = _context.CourseSettings
             .Where(s => s.CourseId == x.Id)
             .Select(s => s.SoftColorCode)
             .FirstOrDefault()
            }).Where(x=> x.IsFeatured == true);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.SortOrder)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PageResultDto<GetCoursesDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PageResultDto<GetCourseModuleDto>?> GetListCourseModuleAsync(int pageNumber, int pageSize)
        {
           var query =  _context.CourseModules.AsNoTracking().Include(x => x.course)
                .Select(x => new GetCourseModuleDto
                {
                    Id = x.Id,
                    CourseId = x.CourseId,
                    CourseName = x.course.Title,
                    Name = x.Name,
                    Description = x.Description,
                    Sortorder = x.Sortorder,
                    IsActive = x.IsActive,
                });
            var totalCount = await query.CountAsync();
            var items = await query
               .OrderBy(x => x.Sortorder)
               .Skip((pageNumber - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync();

            return new PageResultDto<GetCourseModuleDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PageResultDto<GetCoursesDto>> GetListCoursePublishAsync(int pageNumber, int pageSize)
        {
            var query = _context.Courses.AsNoTracking().Select(x => new GetCoursesDto
            {
                Id = x.Id,
                Title = x.Title,
                Slug = x.Slug,
                Summary = x.Summary,
                Points = x.Points,
                Technologies = x.Technologies,
                Description = x.Description,
                Duration = x.Duration,
                Demand = x.Demand,
                Star = x.Star,
                Level = x.Level,
                ImageUrl = x.ImageUrl,
                IsPublished = x.IsPublished,
                SortOrder = x.SortOrder,
                IsFeatured = x.IsFeatured,
                IsActive = x.IsActive,

                Icon = _context.CourseSettings
              .Where(s => s.CourseId == x.Id)
              .Select(s => s.Icon)
              .FirstOrDefault(),

                ColorCode = _context.CourseSettings
              .Where(s => s.CourseId == x.Id)
              .Select(s => s.ColorCode)
              .FirstOrDefault(),

                SoftColorCode = _context.CourseSettings
              .Where(s => s.CourseId == x.Id)
              .Select(s => s.SoftColorCode)
              .FirstOrDefault()
            }).Where(x=> x.IsPublished == true);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.SortOrder)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PageResultDto<GetCoursesDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<Batch?> UpdateBatchAsync(int Id)
        {
            return await _context.Batches.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<Course?> UpdateCourseAsync(int Id)
        {
            return await _context.Courses.FirstOrDefaultAsync(x => x.Id == Id );

        }

        public async Task<CourseModule?> UpdateCourseModuleAsync(int Id)
        {
            return await _context.CourseModules.FirstOrDefaultAsync(x => x.Id == Id);
        }
    }
}
