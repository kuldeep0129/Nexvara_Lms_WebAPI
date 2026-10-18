using Core.Dtos.Common;
using Core.Dtos.UserInterFace;
using Domain.Entities;
using Domain.Entities.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface.Services
{
    public interface IUIServices
    {
        #region Create

        Task<ResponseDto<object>> CreateAdminssionApplicationAsync(CreateAdmissionApplicationDto admission);
        Task<ResponseDto<object>> CreateAnnouncementAsync(CreateAnnouncementDto announcement);
        Task<ResponseDto<object>> CreateProgramTypeAsync(CreateProgramTypeDto dto);
        Task<ResponseDto<object>> CreateCertificateAsync(CreateCertificateDto certificate);
        Task<ResponseDto<object>> CreateCourseAsync(CreateCourseDto course);
        Task<ResponseDto<object>> CreateEnquiryAsync(CreateEnquiryDto enquiry);
        Task<ResponseDto<object>> CreateProgramAsync(CreateProgramDto programdto);
        Task<ResponseDto<object>> CreateResourcesAsync(CreateResourceDto resourcedto);
        Task<ResponseDto<object>> CreateSiteContentAsync(CreateSiteContentDto siteContentdto);
        Task<ResponseDto<object>> CreateStudentAchievementAsync(CreateStudentAchievementDto studentAchievementdto);
        Task<ResponseDto<object>> CreateStudentProjectAsync(CreateStudentProjectDto studentProjectdto);
        Task<ResponseDto<object>> CreateTestimonalsAsync(CreateTestimonialDto testimonialdto,string userId);
        Task<ResponseDto<object>> CreateOrganization(CreateOrganizationDto dto,string userId);

        #endregion

        #region HeroSection
        Task<ResponseDto<object>> GetSiteContentBySectionKeyAsync( string sectionKey);
        Task<ResponseDto<object>> GetProgramsAsync();
        Task<ResponseDto<object>> GetCoursesAsync(bool featured);
        Task<ResponseDto<object>> GetStudentAchievementsAsync(bool featured);
        Task<ResponseDto<object>> GetStudentProjectsAsync(bool featured);
        Task<ResponseDto<object>> GetTestimonialsAsync(bool featured);
        Task<ResponseDto<object>> GetActiveAnnouncementsAsync();
        Task<ResponseDto<object>> GetOrganizationAsync();
        #endregion
    }
}
