using Core.Dtos.UserInterFace.GetUserInterface;
using Core.Interface.Repository;
using Domain.Entities;
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
    public class UIRepository : IUIRepository
    {
        private readonly ApplicationDbContext _context;
        public UIRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateAdminssionApplicationAsync(AdmissionApplication admission)
        {
            await _context.AdmissionApplications.AddAsync(admission);
        }

        public async Task CreateAnnouncementAsync(Announcement announcement)
        {
            await _context.Announcements.AddAsync(announcement);
        }

        public async Task CreateApplicationLogsAsync(ApplicationLogs applicationLogs)
        {
            await _context.ApplicationLogs.AddAsync(applicationLogs);
        }

     

        public async Task CreateCourseAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
        }

        public async Task CreateEnquiryAsync(Enquiry enquiry)
        {
            await _context.Enquiries.AddAsync(enquiry);
        }

        public async Task CreateProgramAsync(Program program)
        {
            await _context.Programs.AddAsync(program);
        }

        public async Task CreateProgramSettings(ProgramSetting setting)
        {
           await _context.ProgramSettings.AddAsync(setting);
        }

        public async Task CreateProgramType(ProgramType type)
        {
           await _context.ProgramTypes.AddAsync(type);
        }

        public async Task CreateRefershTokenAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
        }

        public async Task CreateResourcesAsync(Resource resource)
        {
            await _context.Resources.AddAsync(resource);
        }

        public async Task CreateSiteContentAsync(SiteContent siteContent)
        {
            await _context.SiteContents.AddAsync(siteContent);
        }

        public async Task CreateStudentAchievementAsync(StudentAchievement studentAchievement)
        {
            await _context.StudentAchievements.AddAsync(studentAchievement);
        }

        public async Task CreateStudentProjectAsync(StudentProject studentProject)
        {
            await _context.StudentProjects.AddAsync(studentProject);
        }

        public async Task CreateTestimonalsAsync(Testimonial testimonial)
        {
            await _context.Testimonials.AddAsync(testimonial);
        }

        public async Task<Announcement> GetActiveAnnouncementsAsync()
        {
            var currentTime = DateTime.UtcNow;

            return await _context.Announcements
        .AsNoTracking()
        .Where(x =>
            x.IsActive &&
            x.IsPinned &&
            x.PublishAt <= currentTime &&
            (x.ExpireAt == null || x.ExpireAt >= currentTime)
        )
        .OrderByDescending(x => x.PublishAt)
        .FirstOrDefaultAsync();
        }

        public async Task<List<CourseResponseDto>> GetCoursesAsync(bool featured)
        {
            var query =
                   from c in _context.Courses.AsNoTracking()

                   join cs in _context.CourseSettings.AsNoTracking()
                       on c.Id equals cs.CourseId into settings

                   from cs in settings.DefaultIfEmpty()

                   join lp in _context.CourseLastProjects.AsNoTracking()
                       on c.Id equals lp.CourseId into projects

                   from lp in projects.DefaultIfEmpty()

                   where c.IsActive && c.IsPublished

                   select new
                   {
                       Course = c,
                       Setting = cs,
                       LastProject = lp
                   };

            if (featured)
            {
                query = query.Where(x => x.Course.IsFeatured);
            }

            var result = await query
                .OrderBy(x => x.Course.SortOrder)
                .ThenBy(x => x.Course.Title)
                .Select(x => new CourseResponseDto
                {
                    Id = x.Course.Id,
                    Title = x.Course.Title,
                    Slug = x.Course.Slug,
                    Summary = x.Course.Summary,
                    Description = x.Course.Description,

                    Tags = x.Course.Technologies,
                    Point = x.Course.Points,
                    Duration = x.Course.Duration,
                    Level = x.Course.Level,
                    ImageUrl = x.Course.ImageUrl,
                    Demand = x.Course.Demand,

                    Icon = x.Setting != null
                        ? x.Setting.Icon
                        : null,

                    color = x.Setting != null
                        ? x.Setting.ColorCode
                        : null,

                    SoftColor = x.Setting != null
                        ? x.Setting.SoftColorCode
                        : null,

                    LastProjectIcon = x.LastProject != null
                        ? x.LastProject.Icon ?? string.Empty
                        : string.Empty,

                    LastProjectName = x.LastProject != null
                        ? x.LastProject.Name
                        : string.Empty,

                    LastProjectDescriptions = x.LastProject != null
                        ? x.LastProject.Description ?? string.Empty
                        : string.Empty
                })
                .ToListAsync();

            return result;
        }

        public async Task<List<ProgramResponseDto>> GetProgramsAsync()
        {
            var rows = await ( from p in _context.Programs.AsNoTracking()
         join s in _context.ProgramSettings.AsNoTracking() on p.Id equals s.ProgramId into settings
         from s in settings.DefaultIfEmpty() where p.IsActive && p.IsPublished orderby p.SortOrder, p.Name
         select new
         {
             Id = p.Slug,
             Title = p.Name,

             Icon = s != null ? s.Icon : null,
             ColorCode = s != null ? s.ColorCode : null,
             SoftColorCode = s != null ? s.SoftColorCode : null,

             p.Summary,
             p.Description,
             p.Durationvalue,
             p.DurationUnit,
             p.Mode,
             p.Eligibility,
             p.WithoutFee,
             p.Fee,
             p.Point
         }).ToListAsync();
            return rows.Select(x => new ProgramResponseDto
            {
                Id = x.Id,
                Title = x.Title,
                Icon = x.Icon,
                C = x.ColorCode,
                CS = x.SoftColorCode,
                Summary = x.Summary,
                Description = x.Description,
                Duration = x.Durationvalue.HasValue  ? $"{x.Durationvalue.Value} {x.DurationUnit}"  : null,
                Mode = x.Mode,
                Eligibility = x.Eligibility,
                Fee = x.Fee.HasValue && x.Fee.Value > 0  ? $"From ₹{x.Fee.Value:N0}*"   : !string.IsNullOrWhiteSpace(x.WithoutFee) ? x.WithoutFee.Trim()    : null,
                Points = x.Point
            }).ToList();
        }

     

        public async Task<SiteContent?> GetSiteContentBySectionKeyAsync(string sectionKey)
        {
            return await _context.SiteContents.AsNoTracking().FirstOrDefaultAsync(x =>x.SectionKey == sectionKey && x.IsActive && x.IsPublished);
        }

        public async Task<List<StudentAchievement>> GetStudentAchievementsAsync(bool featured)
        {
            var query = _context.StudentAchievements.AsNoTracking().Where(x => x.IsActive &&  x.IsApproved && x.IsPublished);
            if (featured)
            {
                query = query.Where(x => x.IsFeatured);
            }
            return await query.OrderBy(x => x.CreateAt).ThenBy(x => x.AchievementTitle).ToListAsync();
        }

        public async Task<List<StudentProject>> GetStudentProjectsAsync(bool featured)
        {
            var query = _context.StudentProjects.AsNoTracking().Where(x => x.IsActive &&   x.IsApproved && x.IsPublished);
            if (featured)
            {
                query = query.Where(x => x.IsFeatured);
            }
            return await query.OrderBy(x => x.CreateAt).ThenBy(x => x.Title).ToListAsync();
        }

        public async Task<List<Testimonial>> GetTestimonialsAsync(bool featured)
        {
            var query = _context.Testimonials.Include(x=> x.applicationUser).AsNoTracking().Where(x => x.IsActive  && x.IsPublished);
            return await query.OrderByDescending(x => x.CreateAt).ToListAsync();
        }

        public async Task<bool> IsCheckExistsOrPublishAsync(int programId)
        {
            return await _context.Programs.AnyAsync(x => x.Id == programId && x.IsPublished);
        }

        public async Task<bool> IsCourseSlugExistsAsync(string slug)
        {
            return await _context.Courses.AnyAsync(x => x.Slug == slug);
        }

        public async Task<bool> IsProgramSlugExistsAsync(string slug)
        {

            return await _context.Programs.AnyAsync(p => p.Slug == slug);
        }

        public async Task<bool> IsResourceSlugExistsAsync(string slug)
        {
            return await _context.Resources.AnyAsync(r => r.Slug == slug);
        }

        public async Task<bool> IsSiteContentSectionExistsAsync(string sectionKey)
        {
            return await _context.SiteContents.AnyAsync(x => x.SectionKey == sectionKey && x.IsActive);
        }

        private static List<string> DeserializePoints(string? pointJson)
        {
            if (string.IsNullOrWhiteSpace(pointJson))
                return new List<string>();

            try
            {
                return System.Text.Json.JsonSerializer
                    .Deserialize<List<string>>(pointJson)
                    ?? new List<string>();
            }
            catch (System.Text.Json.JsonException)
            {
                return new List<string>();
            }
        }

        public async Task CreateCourseSettingAsync(CourseSettings settings)
        {
           await _context.CourseSettings.AddAsync(settings);
        }

        public async Task CreateCourseLastProjectAsync(CourseLastProject project)
        {
           await _context.CourseLastProjects.AddAsync(project);
        }

        public async Task CreateOrganizationAsync(Organization org)
        {
           await _context.Organizations.AddAsync(org);
        }

        public async Task<Organization?> GetOrganizationAsync()
        {
          return  await _context.Organizations.Include(x => x.applicationUser).FirstOrDefaultAsync();
        }
    }
}
