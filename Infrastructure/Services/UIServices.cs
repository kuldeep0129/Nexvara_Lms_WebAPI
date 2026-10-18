using AutoMapper;
using Core.Dtos.Common;
using Core.Dtos.UserInterFace;
using Core.Dtos.UserInterFace.GetUserInterface;
using Core.Interface.Repository;
using Core.Interface.Services;
using Domain.common;
using Domain.Entities;
using Domain.Entities.UserInterface;
using Domain.Enums;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Infrastructure.Services
{
    public class UIServices : IUIServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailServices _emailService;
        private readonly IMapper _mapper;
        private readonly IUIRepository _uIRepository;
        private readonly IFileUploadServices _uploadServices;
        public UIServices(IUnitOfWork unitOfWork, IUIRepository uIRepository, IMapper mapper, IFileUploadServices uploadServices, IEmailServices emailService)
        {
            _unitOfWork = unitOfWork;
            _uIRepository = uIRepository;
            _mapper = mapper;
            _uploadServices = uploadServices;
            _emailService = emailService;
        }


        public async Task<ResponseDto<object>> CreateAdminssionApplicationAsync(CreateAdmissionApplicationDto admission)
        {
            var response = new ResponseDto<object>();
            if (admission == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            var IsExistProgram = await _uIRepository.IsCheckExistsOrPublishAsync(admission.ProgramId);
            if (!IsExistProgram)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.ProgramNotExists;
                return response;
            }
            var application = _mapper.Map<AdmissionApplication>(admission);
            application.ApplicationNumber = $"ADM-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid():N}";
            application.Status = EntityStatus.Submitted.ToString();
            application.SubmittedAt = DateTime.Now;
            application.IsActive = true;
            application.CreateAt = DateTime.Now;
            await _uIRepository.CreateAdminssionApplicationAsync(application);
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.Created;
            response.Success = true;
            response.Message = SystemMessage.SaveSuccess;
            return response;
        }

        public async Task<ResponseDto<object>> CreateAnnouncementAsync(CreateAnnouncementDto announcement)
        {

            var response = new ResponseDto<object>();
            if (announcement == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            if (announcement.ExpireAt < announcement.PublishAt)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidStartDate;
                return response;
            }

            var announcements = _mapper.Map<Announcement>(announcement);
            announcements.IsPinned = true;
            announcements.IsActive = true;
            announcements.CreateAt = DateTime.UtcNow;
            await _uIRepository.CreateAnnouncementAsync(announcements);
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.Created;
            response.Success = true;
            response.Message = SystemMessage.SaveSuccess;

            return response;

        }

        public async Task<ResponseDto<object>> CreateCertificateAsync(CreateCertificateDto certificate)
        {

            var response = new ResponseDto<object>();

            if (certificate == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
          //  var certificates = _mapper.Map<Certificate>(certificate);
            //certificates.CertificateNumber = $"NXV-CERT-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid():N}";
            //certificates.VerificationToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            //certificates.Status = EntityStatus.Issued.ToString();
            //certificates.IsActive = true;
            //certificates.CreateAt = DateTime.UtcNow;

            //await _uIRepository.CreateCertificateAsync(certificates);
            //await _unitOfWork.SaveAsync();

            response.ResponseCode = (int)APIResponseCodes.Created;
            response.Success = true;
            response.Message = SystemMessage.SaveSuccess;

            return response;

        }

        public async Task<ResponseDto<object>> CreateCourseAsync(CreateCourseDto course)
        {
            var response = new ResponseDto<object>();
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (course == null)
                {
                    response.ResponseCode = (int)APIResponseCodes.BadRequest;
                    response.Success = false;
                    response.Message = SystemMessage.RequestBodyNull;
                    return response;
                }
                var slugExists = await _uIRepository.IsCourseSlugExistsAsync(course.Slug.Trim());
                if (slugExists)
                {
                    response.ResponseCode = (int)APIResponseCodes.BadRequest;
                    response.Success = false;
                    response.Message = SystemMessage.CourseAlreadyExists;
                    return response;
                }
                var courses = _mapper.Map<Course>(course);
                course.Slug = course.Slug.Trim().ToLowerInvariant();
                courses.IsPublished = false;
                courses.SortOrder = 0;
                if (course.ImageUrl is { Length: > 0 })
                {
                    var imagePath = await _uploadServices.UploadAsync("courses", course.ImageUrl);
                    courses.ImageUrl = imagePath;
                }
                courses.IsActive = true;
                courses.CreateAt = DateTime.Now;

                await _uIRepository.CreateCourseAsync(courses);
                await _unitOfWork.SaveAsync();
                var setting = _mapper.Map<CourseSettings>(course.setting);
                setting.CourseId = courses.Id;
                setting.IsActive = true;
                setting.CreateAt = DateTime.Now;
                await _uIRepository.CreateCourseSettingAsync(setting);
                var project = _mapper.Map<CourseLastProject>(course.lastProject);
                project.CourseId = courses.Id;
                project.IsActive = true;
                project.CreateAt = DateTime.Now;
                await _uIRepository.CreateCourseLastProjectAsync(project);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
                response.ResponseCode = (int)APIResponseCodes.Created;
                response.Success = true;
                response.Message = SystemMessage.SaveSuccess;
                return response;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                response.ResponseCode = (int)APIResponseCodes.InternalServerError;
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }


        }

        public async Task<ResponseDto<object>> CreateEnquiryAsync(CreateEnquiryDto enquiry)
        {
            var response = new ResponseDto<object>();

            if (enquiry == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            var enquirys = _mapper.Map<Enquiry>(enquiry);
            enquirys.Status = EntityStatus.New.ToString();
            //enquirys.AssignedToUserId = null;
            enquirys.IsActive = true;
            enquirys.CreateAt = DateTime.UtcNow;
            await _uIRepository.CreateEnquiryAsync(enquirys);
            await _unitOfWork.SaveAsync();
            try
            {
                await _emailService.SendEnquiryNotificationAsync(enquiry);
            }
            catch (Exception ex)
            {

            }
            response.ResponseCode = (int)APIResponseCodes.Created;
            response.Success = true;
            response.Message = SystemMessage.SaveSuccess;

            return response;
        }

        public async Task<ResponseDto<object>> CreateOrganization(CreateOrganizationDto dto, string userId)
        {
            var response = new ResponseDto<object>();
            if (dto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            if (userId == null)
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }
            var org = _mapper.Map<Organization>(dto);
            org.AspNetUserId = userId;
            org.IsActive = true;
            org.CreateAt = DateTime.UtcNow;
            if (dto.Sortlogo is { Length: > 0 })
            {
                var imagePath = await _uploadServices.UploadAsync("Organization", dto.Sortlogo);
                org.SortLog = imagePath;
            }
            if (dto.Fulllogo is { Length: > 0 })
            {
                var imagePath = await _uploadServices.UploadAsync("Organization", dto.Fulllogo);
                org.FullLogo = imagePath;
            }
            await _uIRepository.CreateOrganizationAsync(org);
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.Created;
            response.Success = true;
            response.Message = SystemMessage.SaveSuccess;

            return response;
        }

        public async Task<ResponseDto<object>> CreateProgramAsync(CreateProgramDto programdto)
        {
            var response = new ResponseDto<object>();
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (programdto == null)
                {
                    response.ResponseCode = (int)APIResponseCodes.BadRequest;
                    response.Success = false;
                    response.Message = SystemMessage.RequestBodyNull;
                    return response;
                }
                var normalizedSlug = programdto.Slug.Trim().ToLowerInvariant();

                var slugExists = await _uIRepository.IsProgramSlugExistsAsync(normalizedSlug);

                if (slugExists)
                {
                    response.ResponseCode = (int)APIResponseCodes.BadRequest;
                    response.Success = false;
                    response.Message = SystemMessage.ProgramAlreadyExists;
                    return response;
                }
                var program = _mapper.Map<Domain.Entities.UserInterface.Program>(programdto);

                program.Name = programdto.Name.Trim();
                program.Slug = normalizedSlug;
                program.IsPublished = false;
                program.SortOrder = 0;
                program.IsActive = true;
                program.CreateAt = DateTime.Now;
                await _uIRepository.CreateProgramAsync(program);
                await _unitOfWork.SaveAsync();
                var programsetting = _mapper.Map<ProgramSetting>(programdto.setting);
                programsetting.ProgramId = program.Id;
                programsetting.IsActive = true;
                programsetting.CreateAt = DateTime.Now;
                await _uIRepository.CreateProgramSettings(programsetting);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
                response.ResponseCode = (int)APIResponseCodes.Created;
                response.Success = true;
                response.Message = SystemMessage.SaveSuccess;
                return response;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                response.ResponseCode = (int)APIResponseCodes.InternalServerError;
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }
        }

        public async Task<ResponseDto<object>> CreateProgramTypeAsync(CreateProgramTypeDto dto)
        {
            var response = new ResponseDto<object>();

            if (dto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            var programType = _mapper.Map<ProgramType>(dto);
            programType.IsActive = true;
            programType.CreateAt = DateTime.Now;
            await _uIRepository.CreateProgramType(programType);
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.Created;
            response.Success = true;
            response.Message = SystemMessage.SaveSuccess;
            return response;
        }

        public async Task<ResponseDto<object>> CreateResourcesAsync(CreateResourceDto resourcedto)
        {
            var response = new ResponseDto<object>();

            if (resourcedto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }

            var normalizedSlug = resourcedto.Slug.Trim().ToLowerInvariant();
            var slugExists = await _uIRepository.IsResourceSlugExistsAsync(normalizedSlug);
            if (slugExists)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.ResourcesAlreadyExists;
                return response;
            }
            var resource = _mapper.Map<Resource>(resourcedto);
            resource.Title = resourcedto.Title.Trim();
            resource.Slug = normalizedSlug;
            resource.ResourceType = resourcedto.ResourceType.Trim();
            resource.PublishedAt = null;
            resource.IsPublished = false;
            resource.IsActive = true;
            resource.CreateAt = DateTime.UtcNow;

            await _uIRepository.CreateResourcesAsync(resource);
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.Created;
            response.Success = true;
            response.Message = SystemMessage.SaveSuccess;
            return response;
        }

        public async Task<ResponseDto<object>> CreateSiteContentAsync(CreateSiteContentDto siteContentdto)
        {
            var response = new ResponseDto<object>();
            if (siteContentdto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            var normalizedSectionKey = siteContentdto.SectionKey.Trim();
            var sectionExists = await _uIRepository.IsSiteContentSectionExistsAsync(normalizedSectionKey);
            if (sectionExists)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.ContentAlreadyExists;
                return response;
            }
            // Map DTO to entity
            var siteContent = _mapper.Map<SiteContent>(siteContentdto);
            siteContent.SectionKey = normalizedSectionKey;
            if (siteContentdto.ImageUrl is { Length: > 0 })
            {
                var imagePath = await _uploadServices.UploadAsync("SiteContent", siteContentdto.ImageUrl);
                siteContent.ImageUrl = imagePath;
            }

            // Server-controlled properties
            siteContent.IsPublished = false;
            siteContent.SortOrder = 0;
            siteContent.IsActive = true;
            siteContent.CreateAt = DateTime.UtcNow;
            await _uIRepository.CreateSiteContentAsync(siteContent);
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.Created;
            response.Success = true;
            response.Message = SystemMessage.SaveSuccess;
            return response;
        }

        public async Task<ResponseDto<object>> CreateStudentAchievementAsync(CreateStudentAchievementDto studentAchievementdto)
        {
            var response = new ResponseDto<object>();

            if (studentAchievementdto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }

            // Map DTO to entity
            var achievement = _mapper.Map<StudentAchievement>(studentAchievementdto);

            // Normalize user input
            achievement.StudentDisplayName = studentAchievementdto.StudentDisplayName.Trim();

            achievement.AchievementTitle = studentAchievementdto.AchievementTitle.Trim();
            if (studentAchievementdto.ImageUrl is { Length: > 0 })
            {
                var imagePath = await _uploadServices.UploadAsync("StudentAchievement", studentAchievementdto.ImageUrl);
                achievement.ImageUrl = imagePath;
            }

            // Server-controlled properties
            achievement.IsApproved = false;
            achievement.IsPublished = false;
            achievement.IsActive = true;
            achievement.CreateAt = DateTime.UtcNow;

            await _uIRepository.CreateStudentAchievementAsync(achievement);
            await _unitOfWork.SaveAsync();

            response.ResponseCode = (int)APIResponseCodes.Created;
            response.Success = true;
            response.Message = SystemMessage.SaveSuccess;

            return response;
        }

        public async Task<ResponseDto<object>> CreateStudentProjectAsync(CreateStudentProjectDto studentProjectdto)
        {
            var response = new ResponseDto<object>();

            if (studentProjectdto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }

            var project = _mapper.Map<StudentProject>(studentProjectdto);

            project.Title = studentProjectdto.Title.Trim();
            if (studentProjectdto.ThumbnailUrl is { Length: > 0 })
            {
                var imagePath = await _uploadServices.UploadAsync("StudentThumbnail", studentProjectdto.ThumbnailUrl);
                project.ThumbnailUrl = imagePath;
            }
            project.IsApproved = false;
            project.IsPublished = false;
            project.IsActive = true;

            project.CreateAt = DateTime.UtcNow;

            await _uIRepository.CreateStudentProjectAsync(project);
            await _unitOfWork.SaveAsync();

            response.ResponseCode = (int)APIResponseCodes.Created;
            response.Success = true;
            response.Message = SystemMessage.SaveSuccess;

            return response;
        }

        public async Task<ResponseDto<object>> CreateTestimonalsAsync(CreateTestimonialDto testimonialdto, string userId)
        {
            var response = new ResponseDto<object>();
            if(userId == null)
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }
            if (testimonialdto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }

            var testimonial = _mapper.Map<Testimonial>(testimonialdto);
            testimonial.UserId = userId;
            testimonial.ReviewText = testimonialdto.ReviewText.Trim();
            testimonial.IsPublished = false;
            testimonial.IsActive = true;
            testimonial.CreateAt = DateTime.UtcNow;


            await _uIRepository.CreateTestimonalsAsync(testimonial);
            await _unitOfWork.SaveAsync();

            response.ResponseCode = (int)APIResponseCodes.Created;
            response.Success = true;
            response.Message = SystemMessage.SaveSuccess;

            return response;
        }

        public async Task<ResponseDto<object>> GetActiveAnnouncementsAsync()
        {
            var response = new ResponseDto<object>();
            var announcements = await _uIRepository.GetActiveAnnouncementsAsync();
            var result = _mapper.Map<AnnouncementResponseDto>(announcements);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Message = SystemMessage.ActiveannouncementsRetrieve;
            response.Data = result;
            return response;
        }

        public async Task<ResponseDto<object>> GetCoursesAsync(bool featured)
        {
            var response = new ResponseDto<object>();
            var courses = await _uIRepository.GetCoursesAsync(featured);
            var courseDtos = _mapper.Map<List<CourseResponseDto>>(courses);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.CourseRetrieve;
            response.Data = courseDtos;
            return response;
        }

        public async Task<ResponseDto<object>> GetOrganizationAsync()
        {
            var response = new ResponseDto<object>();

            var organizaiton = await _uIRepository.GetOrganizationAsync();
          
            var orgMap = _mapper.Map<GetOranizationDto>(organizaiton);
            orgMap.Name = organizaiton.applicationUser.FullName;
            orgMap.MapLocation = organizaiton.MapLocation;
            orgMap.Email = organizaiton.applicationUser.Email;
            orgMap.Phone = organizaiton.applicationUser.PhoneNumber;
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.TestimonialsRetrieve;
            response.Data = orgMap;
            return response;
        }

        public async Task<ResponseDto<object>> GetProgramsAsync()
        {
            var response = new ResponseDto<object>();

            var programs = await _uIRepository.GetProgramsAsync();

            var programDtos = _mapper.Map<List<ProgramResponseDto>>(programs);

            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.ProgramRetrieve;
            response.Data = programDtos;

            return response;
        }

        public async Task<ResponseDto<object>> GetSiteContentBySectionKeyAsync(string sectionKey)
        {
            var response = new ResponseDto<object>();

            if (string.IsNullOrWhiteSpace(sectionKey))
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.SectionKeyRequired;
                return response;
            }

            var normalizedKey = sectionKey.Trim().ToLowerInvariant();

            var siteContent = await _uIRepository.GetSiteContentBySectionKeyAsync(normalizedKey);

            if (siteContent == null)
            {
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                response.Success = false;
                response.Message = SystemMessage.NotFoundSiteContent;
                return response;
            }

            var contentDto = _mapper.Map<SiteContentResponseDto>(siteContent);

            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.SiteContentRetrive;
            response.Data = contentDto;

            return response;
        }

        public async Task<ResponseDto<object>> GetStudentAchievementsAsync(bool featured)
        {
            var response = new ResponseDto<object>();
            var achievements = await _uIRepository.GetStudentAchievementsAsync(featured);
            var achievementDtos = _mapper.Map<List<StudentAchievementResponseDto>>(achievements);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.StudentachievementsRetrieve;
            response.Data = achievementDtos;
            return response;
        }

        public async Task<ResponseDto<object>> GetStudentProjectsAsync(bool featured)
        {
            var response = new ResponseDto<object>();
            var projects = await _uIRepository.GetStudentProjectsAsync(featured);
            var projectDtos = _mapper.Map<List<StudentProjectResponseDto>>(projects);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.StudentProjectRetrieve;
            response.Data = projectDtos;
            return response;
        }

        public async Task<ResponseDto<object>> GetTestimonialsAsync(bool featured)
        {
            var response = new ResponseDto<object>();
            var testimonials = await _uIRepository.GetTestimonialsAsync(featured);
            var testimonialDtos = _mapper.Map<List<TestimonialResponseDto>>(testimonials);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.TestimonialsRetrieve;
            response.Data = testimonialDtos;
            return response;
        }
    }
}
