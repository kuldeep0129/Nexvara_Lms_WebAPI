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

        public async Task<bool> CourseExistsAsync(int courseId)
        {
            return await _context.Courses.AsNoTracking().AnyAsync(x => x.Id == courseId);
        }

        public async Task CreateBatchAsync(Batch entity)
        {
            await _context.Batches.AddAsync(entity);
        }

        public async Task CreateCourseAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
        }

        public async Task CreateCourseMaterialAsync(CourseMaterials entity)
        {
            await _context.CourseMaterials.AddAsync(entity);
        }

        public async Task CreateCourseModuleAsync(CourseModule course)
        {
            await _context.CourseModules.AddAsync(course);
        }

        public async Task CreateCourseSettingAsync(CourseSettings course)
        {
            await _context.CourseSettings.AddAsync(course);
        }

        public async Task CreateCourseSyllabusAsync(CourseSyllabus syllabus)
        {
            await _context.CourseSyllabus.AddAsync(syllabus);
        }

        public async Task<GetCourseBatchDto?> GetByIdBatchAsync(int Id)
        {
            return await _context.Batches.Include(x => x.course).AsNoTracking().Select(x => new GetCourseBatchDto
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
            }).FirstOrDefaultAsync(x => x.Id == Id && x.IsActive);
        }

        public async Task<Batch?> GetByIdBatchUpdateAsync(int Id)
        {
            return await _context.Batches.FirstOrDefaultAsync(x => x.Id == Id && x.IsActive);
        }

        public async Task<CourseMaterials> GetByIdCourseMaterialAsync(int Id)
        {
            return await _context.CourseMaterials.Include(x => x.course).Include(x => x.module).Include(x => x.batch).Include(x => x.user).FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<GetCoursesDto?> GetByIdCoursesAsync(int Id)
        {
            return await _context.Courses.AsNoTracking().Where(x => x.Id == Id && x.IsActive).Select(x => new GetCoursesDto
            {
                Id = x.Id,
                Title = x.Title,
                Slug = x.Slug,
                Summary = x.Summary,
                Points = x.Points,
                Technologies = x.Technologies,
                Mode = x.Mode,
                Description = x.Description,
                Duration = x.Duration,
                Demand = x.Demand,
                Star = x.Star,
                CourseType = x.AccessType,
                Price = x.Price,
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
            return await _context.CourseModules.FirstOrDefaultAsync(x => x.Id == Id && x.IsActive);
        }

        public async Task<CourseSettings?> GetByIdCoursesSettingAsync(int Id)
        {
            return await _context.CourseSettings.Include(x => x.Course).FirstOrDefaultAsync(x => x.Id == Id && x.IsActive);
        }

        public async Task<Course?> GetByIdCoursesUpdateAsync(int Id)
        {
            return await _context.Courses.FirstOrDefaultAsync(x => x.Id == Id && x.IsActive);
        }

        public async Task<CourseSyllabus> GetByIdCourseSyllabusAsync(int Id)
        {
            return await _context.CourseSyllabus.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<GetCourseCurriculumGroupDto?> GetCourseCurriculumAsync(int courseId)
        {
            // --------------------------------------------------
            // 1. COURSE
            // --------------------------------------------------
            var course = await _context.Courses.AsNoTracking().Where(x => x.Id == courseId).Select(x => new GetCourseCurriculumGroupDto
            {
                Id = x.Id,
                CourseName = x.Title,
                CourseShortDescription = x.Summary,
                CourseDescription = x.Description,
                CousrseImage = x.ImageUrl,
                CoursePoint = x.Points,
                Technologies = x.Technologies,
                CourseLevel = x.Level,
                CourseDuration = x.Duration,
                Rating = x.Star,
                IsPublished = x.IsPublished,
                IsActive = x.IsActive
            }).FirstOrDefaultAsync();

            if (course == null)
            {
                return null;
            }

            // --------------------------------------------------
            // 2. SYLLABUS
            // --------------------------------------------------


            course.Syllabus = await _context.CourseSyllabus
                .AsNoTracking()
                .Where(x => x.CourseId == courseId)
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new GetCourseSyllabusCurriculumDto
                {
                    SyllabusId = x.Id,
                    SyllabusTitle = x.Title,
                    SyllabusDescription = x.Description,
                    DisplayOrder = x.DisplayOrder ?? 0,
                    IsActive = x.IsActive
                })
                .ToListAsync();


            //// --------------------------------------------------
            //// 3. MODULES
            //// --------------------------------------------------

            //var modules = await _context.CourseModules
            //    .AsNoTracking()
            //    .Where(x =>
            //        x.CourseId == courseId &&
            //        x.IsActive)
            //    .OrderBy(x => x.Sortorder)
            //    .Select(x => new GetCourseModuleCurriculumDto
            //    {
            //        ModuleId = x.Id,
            //        ModuleTitle = x.Name,
            //        ModuleDescription = x.Description,
            //        SortOrder = x.Sortorder,
            //        IsActive = x.IsActive
            //    })
            //    .ToListAsync();


            //// --------------------------------------------------
            //// 4. VIDEOS
            //// --------------------------------------------------

            //var videos = await _context.CourseVideos
            //    .AsNoTracking()
            //    .Where(x =>
            //        x.CourseId == courseId &&
            //        x.IsActive)
            //    .OrderBy(x => x.DisplayOrder)
            //    .Select(x => new
            //    {
            //        ModuleId = x.ModuleId,

            //        Video = new GetCourseVideoCurriculumDto
            //        {
            //            VideoId = x.Id,
            //            VideoTitle = x.Title,
            //            VideoDescription = x.Description,
            //            VideoUrl = x.VideoUrl,
            //            VideoUrlPublicId = x.VideoUrlPublicId,

            //            DurationInSeconds =
            //                x.DurationInSeconds ?? 0,

            //            DisplayOrder =
            //                x.DisplayOrder ?? 0,

            //            IsPublished = x.IsPublished,
            //            IsActive = x.IsActive
            //        }
            //    })
            //    .ToListAsync();


            //// --------------------------------------------------
            //// 5. MATERIALS
            //// --------------------------------------------------

            //var materials = await _context.CourseMaterials
            //    .AsNoTracking()
            //    .Where(x =>
            //        x.CourseId == courseId &&
            //        x.IsActive)
            //    .OrderBy(x => x.Id)
            //    .Select(x => new
            //    {
            //        ModuleId = x.ModuleId,

            //        Material = new GetCourseMaterialCurriculumDto
            //        {
            //            MaterialId = x.Id,
            //            MaterialTitle = x.Title,

            //            FileUrl = x.FileUrl,
            //            FileType = x.FileType,
            //            FileSize = x.FileSize ?? 0,

            //            UploadedById = x.UploadedBy,

            //            IsPublished = x.IsPublished,
            //            IsActive = x.IsActive
            //        }
            //    })
            //    .ToListAsync();


            //// --------------------------------------------------
            //// 6. GROUP VIDEOS + MATERIALS INTO MODULES
            //// --------------------------------------------------

            //foreach (var module in modules)
            //{
            //    module.Videos = videos
            //        .Where(x => x.ModuleId == module.ModuleId)
            //        .Select(x => x.Video)
            //        .ToList();

            //    module.Materials = materials
            //        .Where(x => x.ModuleId == module.ModuleId)
            //        .Select(x => x.Material)
            //        .ToList();
            //}

            //course.Modules = modules;


            //// --------------------------------------------------
            //// 7. BATCHES
            //// --------------------------------------------------

            //course.Batches = await _context.Batches
            //    .AsNoTracking()
            //    .Where(x =>
            //        x.CourseId == courseId &&
            //        x.IsActive)
            //    .OrderByDescending(x => x.StartDate)
            //    .Select(x => new GetBatchCurriculumDto
            //    {
            //        BatchId = x.Id,
            //        BatchName = x.Name,
            //        BatchDescription = x.Description,

            //        StartDate = x.StartDate,
            //        EndDate = x.EndDate,

            //        Capacity = x.Capacity,

            //        IsPublished = x.IsPublished,
            //        IsActive = x.IsActive
            //    })
            //    .ToListAsync();


            return course;

        }

        public async Task<GetCourseSyllabusDto> GetCourseSyllabusAsync(int id)
        {
            return await _context.CourseSyllabus.Where(x => x.Id == id).Select(x => new GetCourseSyllabusDto
            {
                Id = x.Id,
                Title = x.Title,
                CouseId = x.CourseId ?? 0,
                Description = x.Description,
                DisplayOrder = x.DisplayOrder ?? 0
            }).FirstOrDefaultAsync();
        }

        public async Task<PageResultDto<GetCourseBatchDto>?> GetListBatchAsync(int pageNumber, int pageSize)
        {
            var query = _context.Batches.AsNoTracking().Include(x => x.course).Select(x => new GetCourseBatchDto
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
                Mode = x.Mode,
                Demand = x.Demand,
                Star = x.Star,
                Level = x.Level,
                ImageUrl = x.ImageUrl,
                IsPublished = x.IsPublished,
                Price =x.Price,
                CourseType = x.AccessType,
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
            }).Where(x => x.IsFeatured == true);

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
            var query = _context.CourseModules.AsNoTracking().Include(x => x.course)
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
            }).Where(x => x.IsPublished == true);

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

        public async Task<PageResultDto<GetCourseSyllabusDto>> GetListCourseSyllabusAsync(int pageNumber, int pageSize,int courseId)
        {
            var query = _context.CourseSyllabus.AsNoTracking().Where(x=> x.IsActive && x.CourseId ==courseId).Select(x => new GetCourseSyllabusDto
            {
                Id = x.Id,
                CouseId = x.CourseId ?? 0,
                Title = x.Title,
                Description = x.Description,
                DisplayOrder = x.DisplayOrder ?? 0
            });

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.DisplayOrder)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PageResultDto<GetCourseSyllabusDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<bool> IsDisplayOrderExistsCourseSyllabusAsync(int Id, int displayOrder)
        {
            return await _context.CourseSyllabus.AnyAsync(x => x.CourseId == Id && x.DisplayOrder == displayOrder);
        }

        public async Task<Batch?> UpdateBatchAsync(int Id)
        {
            return await _context.Batches.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<Course?> UpdateCourseAsync(int Id)
        {
            return await _context.Courses.FirstOrDefaultAsync(x => x.Id == Id);

        }

        public async Task<CourseModule?> UpdateCourseModuleAsync(int Id)
        {
            return await _context.CourseModules.FirstOrDefaultAsync(x => x.Id == Id);
        }
    }
}
