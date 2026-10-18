using Core.Dtos.UserInterFace.GetUserInterface;
using Domain.Entities;
using Domain.Entities.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface.Repository
{
    public interface IUIRepository
    {
        #region create 
        Task CreateAdminssionApplicationAsync(AdmissionApplication admission);
        Task CreateProgramType(ProgramType type);
        Task CreateProgramSettings(ProgramSetting setting);
        Task CreateAnnouncementAsync(Announcement announcement);
        Task CreateCourseAsync(Course course);
        Task CreateEnquiryAsync(Enquiry enquiry);
        Task CreateProgramAsync(Program program);
        Task CreateResourcesAsync(Resource resource);
        Task CreateSiteContentAsync(SiteContent siteContent);
        Task CreateStudentAchievementAsync(StudentAchievement studentAchievement);
        Task CreateStudentProjectAsync(StudentProject studentProject);
        Task CreateTestimonalsAsync(Testimonial testimonial);
        Task CreateApplicationLogsAsync(ApplicationLogs applicationLogs);
        Task CreateRefershTokenAsync(RefreshToken refreshToken);
        Task CreateCourseSettingAsync(CourseSettings settings);
        Task CreateCourseLastProjectAsync(CourseLastProject project);
        Task CreateOrganizationAsync(Organization org);

        #endregion

        #region Get 
        Task<bool> IsCheckExistsOrPublishAsync(int programId);
        Task<bool> IsCourseSlugExistsAsync(string slug);
        Task<bool> IsProgramSlugExistsAsync(string slug);
        Task<bool> IsResourceSlugExistsAsync(string slug);
        Task<bool> IsSiteContentSectionExistsAsync(string sectionKey);
        #endregion

        #region  Hero section
        Task<SiteContent?> GetSiteContentBySectionKeyAsync(string sectionKey);
        Task<List<ProgramResponseDto>> GetProgramsAsync();
        Task<List<CourseResponseDto>> GetCoursesAsync(bool featured);
        Task<List<StudentAchievement>> GetStudentAchievementsAsync(  bool featured);
        Task<List<StudentProject>> GetStudentProjectsAsync(bool featured);
        Task<List<Testimonial>> GetTestimonialsAsync(bool featured);
        Task<Announcement> GetActiveAnnouncementsAsync();
        Task<Organization?> GetOrganizationAsync();
        #endregion
    }
}
