using AutoMapper;
using Core.Dtos.Profile;
using Core.Dtos.UserInterFace;
using Core.Dtos.UserInterFace.GetUserInterface;
using Domain.Entities;
using Domain.Entities.Common;
using Domain.Entities.Student;
using Domain.Entities.Teacher;
using Domain.Entities.UserInterface;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Mapping
{
    public class MapppingData : Profile
    {
        public MapppingData()
        {
            #region Row Data 
            CreateMap<CreateAdmissionApplicationDto, AdmissionApplication>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ApplicationNumber, opt => opt.Ignore())
                .ForMember(dest => dest.SubmittedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Program, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreateAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreateBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateBy, opt => opt.Ignore());

            CreateMap<CreateAnnouncementDto, Announcement>()
          .ForMember(dest => dest.Id,
              opt => opt.Ignore())
          .ForMember(dest => dest.IsActive,
              opt => opt.Ignore())
          .ForMember(dest => dest.CreateBy,
              opt => opt.Ignore())
          .ForMember(dest => dest.CreateAt,
              opt => opt.Ignore())
          .ForMember(dest => dest.UpdateBy,
              opt => opt.Ignore())
          .ForMember(dest => dest.UpdateAt,
              opt => opt.Ignore());


          

            CreateMap<CreateCertificateDto, Certificates>()
           .ForMember(dest => dest.Id,
               opt => opt.Ignore())
           .ForMember(dest => dest.CertificateNumber,
               opt => opt.Ignore())       
           .ForMember(dest => dest.IsActive,
               opt => opt.Ignore())
           .ForMember(dest => dest.CreateBy,
               opt => opt.Ignore())
           .ForMember(dest => dest.CreateAt,
               opt => opt.Ignore())
           .ForMember(dest => dest.UpdateBy,
               opt => opt.Ignore())
           .ForMember(dest => dest.UpdateAt,
               opt => opt.Ignore());

            CreateMap<CreateCourseDto, Course>()
           .ForMember(dest => dest.Id,
               opt => opt.Ignore())
           .ForMember(dest => dest.ImageUrl,
               opt => opt.Ignore())
           .ForMember(dest => dest.IsPublished,
               opt => opt.Ignore())
           .ForMember(dest => dest.SortOrder,
               opt => opt.Ignore())
           .ForMember(dest => dest.IsActive,
               opt => opt.Ignore())
           .ForMember(dest => dest.CreateBy,
               opt => opt.Ignore())
           .ForMember(dest => dest.CreateAt,
               opt => opt.Ignore())
           .ForMember(dest => dest.UpdateBy,
               opt => opt.Ignore())
           .ForMember(dest => dest.UpdateAt,
               opt => opt.Ignore());


            CreateMap<CreateCourseSettings, CourseSettings>()
             .ForMember(dest => dest.Id, opt => opt.Ignore())
             .ForMember(dest => dest.CourseId, opt => opt.Ignore())
             .ForMember(dest => dest.Course, opt => opt.Ignore())
             .ForMember(dest => dest.IsActive, opt => opt.Ignore())
             .ForMember(dest => dest.CreateAt, opt => opt.Ignore())
             .ForMember(dest => dest.CreateBy, opt => opt.Ignore())
             .ForMember(dest => dest.UpdateAt, opt => opt.Ignore())
             .ForMember(dest => dest.UpdateBy, opt => opt.Ignore());

            CreateMap<CreateCourseLastProject, CourseLastProject>()
             .ForMember(dest => dest.Id, opt => opt.Ignore())
             .ForMember(dest => dest.CourseId, opt => opt.Ignore())
             .ForMember(dest => dest.Course, opt => opt.Ignore())
             .ForMember(dest => dest.IsActive, opt => opt.Ignore())
             .ForMember(dest => dest.CreateAt, opt => opt.Ignore())
             .ForMember(dest => dest.CreateBy, opt => opt.Ignore())
             .ForMember(dest => dest.UpdateAt, opt => opt.Ignore())
             .ForMember(dest => dest.UpdateBy, opt => opt.Ignore());



            CreateMap<CreateEnquiryDto, Enquiry>()
           .ForMember(dest => dest.Id,
               opt => opt.Ignore())
           .ForMember(dest => dest.Status,
               opt => opt.Ignore())
           .ForMember(dest => dest.IsActive,
               opt => opt.Ignore())
           .ForMember(dest => dest.CreateBy,
               opt => opt.Ignore())
           .ForMember(dest => dest.CreateAt,
               opt => opt.Ignore())
           .ForMember(dest => dest.UpdateBy,
               opt => opt.Ignore())
           .ForMember(dest => dest.UpdateAt,
               opt => opt.Ignore());


            CreateMap<CreateProgramDto, Domain.Entities.UserInterface.Program>()
            .ForMember(dest => dest.Id,
                opt => opt.Ignore())
            .ForMember(dest => dest.IsPublished,
                opt => opt.Ignore())
            .ForMember(dest => dest.SortOrder,
                opt => opt.Ignore())
            .ForMember(dest => dest.IsActive,
                opt => opt.Ignore())
            .ForMember(dest => dest.CreateBy,
                opt => opt.Ignore())
            .ForMember(dest => dest.CreateAt,
                opt => opt.Ignore())
            .ForMember(dest => dest.UpdateBy,
                opt => opt.Ignore())
            .ForMember(dest => dest.UpdateAt,
                opt => opt.Ignore());

            CreateMap<CreateProgramSettingDto, Domain.Entities.UserInterface.ProgramSetting>()
    .ForMember(dest => dest.Id,
        opt => opt.Ignore())
    .ForMember(dest => dest.ProgramId,
        opt => opt.Ignore())
    .ForMember(dest => dest.program,
        opt => opt.Ignore())
    .ForMember(dest => dest.IsActive,
        opt => opt.Ignore())
    .ForMember(dest => dest.CreateBy,
        opt => opt.Ignore())
    .ForMember(dest => dest.CreateAt,
        opt => opt.Ignore())
    .ForMember(dest => dest.UpdateBy,
        opt => opt.Ignore())
    .ForMember(dest => dest.UpdateAt,
        opt => opt.Ignore());


            CreateMap<CreateResourceDto, Resource>()
        .ForMember(dest => dest.Id,
            opt => opt.Ignore())
        .ForMember(dest => dest.PublishedAt,
            opt => opt.Ignore())
        .ForMember(dest => dest.IsPublished,
            opt => opt.Ignore())
        .ForMember(dest => dest.IsActive,
            opt => opt.Ignore())
        .ForMember(dest => dest.CreateBy,
            opt => opt.Ignore())
        .ForMember(dest => dest.CreateAt,
            opt => opt.Ignore())
        .ForMember(dest => dest.UpdateBy,
            opt => opt.Ignore())
        .ForMember(dest => dest.UpdateAt,
            opt => opt.Ignore());


            CreateMap<CreateSiteContentDto, SiteContent>()
           .ForMember(dest => dest.Id,
               opt => opt.Ignore())
           .ForMember(dest => dest.ImageUrl,
               opt => opt.Ignore())
           .ForMember(dest => dest.IsPublished,
               opt => opt.Ignore())
           .ForMember(dest => dest.SortOrder,
               opt => opt.Ignore())
           .ForMember(dest => dest.IsActive,
               opt => opt.Ignore())
           .ForMember(dest => dest.CreateBy,
               opt => opt.Ignore())
           .ForMember(dest => dest.CreateAt,
               opt => opt.Ignore())
           .ForMember(dest => dest.UpdateBy,
               opt => opt.Ignore())
           .ForMember(dest => dest.UpdateAt,
               opt => opt.Ignore());

            CreateMap<CreateStudentAchievementDto, StudentAchievement>()
           .ForMember(dest => dest.Id,
               opt => opt.Ignore())
           .ForMember(dest => dest.ImageUrl,
               opt => opt.Ignore())
           .ForMember(dest => dest.IsApproved,
               opt => opt.Ignore())
           .ForMember(dest => dest.IsPublished,
               opt => opt.Ignore())
           .ForMember(dest => dest.IsActive,
               opt => opt.Ignore())
           .ForMember(dest => dest.CreateBy,
               opt => opt.Ignore())
           .ForMember(dest => dest.CreateAt,
               opt => opt.Ignore())
           .ForMember(dest => dest.UpdateBy,
               opt => opt.Ignore())
           .ForMember(dest => dest.UpdateAt,
               opt => opt.Ignore());


            CreateMap<CreateStudentProjectDto, StudentProject>()
         .ForMember(dest => dest.Id,
             opt => opt.Ignore())
         .ForMember(dest => dest.ThumbnailUrl,
             opt => opt.Ignore())
         .ForMember(dest => dest.IsApproved,
             opt => opt.Ignore())
         .ForMember(dest => dest.IsPublished,
             opt => opt.Ignore())
         .ForMember(dest => dest.IsActive,
             opt => opt.Ignore())
         .ForMember(dest => dest.CreateBy,
             opt => opt.Ignore())
         .ForMember(dest => dest.CreateAt,
             opt => opt.Ignore())
         .ForMember(dest => dest.UpdateBy,
             opt => opt.Ignore())
         .ForMember(dest => dest.UpdateAt,
             opt => opt.Ignore());

            CreateMap<CreateStudentProjectDto, StudentProject>()
           .ForMember(dest => dest.Id,
               opt => opt.Ignore())
           .ForMember(dest => dest.IsApproved,
               opt => opt.Ignore())
           .ForMember(dest => dest.IsPublished,
               opt => opt.Ignore())
           .ForMember(dest => dest.IsActive,
               opt => opt.Ignore())
           .ForMember(dest => dest.CreateBy,
               opt => opt.Ignore())
           .ForMember(dest => dest.CreateAt,
               opt => opt.Ignore())
           .ForMember(dest => dest.UpdateBy,
               opt => opt.Ignore())
           .ForMember(dest => dest.UpdateAt,
               opt => opt.Ignore());

            CreateMap<CreateProgramTypeDto, ProgramType>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreateAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreateBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateBy, opt => opt.Ignore());

            CreateMap<CreateOrganizationDto, Organization>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SortLog, opt => opt.Ignore())
                .ForMember(dest => dest.FullLogo, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreateAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreateBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateAt, opt => opt.Ignore())
                .ForMember(dest => dest.AspNetUserId, opt => opt.Ignore())
                .ForMember(dest => dest.applicationUser, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateBy, opt => opt.Ignore());


            CreateMap<CreateAnouncementDto, Announcement>()
             .ForMember(dest => dest.AnouncementType, opt => opt.MapFrom(src => src.AnouncementType.ToString()))
             .ForMember(dest => dest.Id, opt => opt.Ignore())
             .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
             .ForMember(dest => dest.CreateBy, opt => opt.Ignore())
             .ForMember(dest => dest.CreateAt, opt => opt.Ignore())
             .ForMember(dest => dest.UpdateBy, opt => opt.Ignore())
             .ForMember(dest => dest.UpdateAt, opt => opt.Ignore());

            CreateMap<CreateTestimonialDto, Testimonial>()
    .ForMember(dest => dest.Id, opt => opt.Ignore())
    .ForMember(dest => dest.UserId, opt => opt.Ignore())
    .ForMember(dest => dest.applicationUser, opt => opt.Ignore())
    .ForMember(dest => dest.IsPublished, opt => opt.Ignore())
     .ForMember(dest => dest.CreateBy, opt => opt.Ignore())
             .ForMember(dest => dest.CreateAt, opt => opt.Ignore())
             .ForMember(dest => dest.UpdateBy, opt => opt.Ignore())
             .ForMember(dest => dest.UpdateAt, opt => opt.Ignore());


            //Get Data Mapping

            CreateMap<SiteContent, SiteContentResponseDto>();
            CreateMap<Program, ProgramResponseDto>();
            CreateMap<Course, CourseResponseDto>();
            CreateMap<StudentAchievement, StudentAchievementResponseDto>();
            CreateMap<StudentProject, StudentProjectResponseDto>();
            CreateMap<Testimonial, TestimonialResponseDto>()
                .ForMember(
                    dest => dest.StudentDisplayName,
                    opt => opt.MapFrom(src =>
                        src.applicationUser != null
                            ? src.applicationUser.FullName
                            : string.Empty)
                ); CreateMap<Organization, GetOranizationDto>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.applicationUser != null ? src.applicationUser.FullName : string.Empty))
                .ForMember(dest => dest.LinkedIn, opt => opt.MapFrom(src => src.LinkedInURL))
                .ForMember(dest => dest.Facebook, opt => opt.MapFrom(src => src.FacebookURL))
                .ForMember(dest => dest.YouTube, opt => opt.MapFrom(src => src.YouTubeURL))
                .ForMember(dest => dest.Twitter, opt => opt.MapFrom(src => src.TwitterURL));
            CreateMap<Announcement, AnnouncementResponseDto>()
                .ForMember(
                    dest => dest.AnouncementType,
                    opt => opt.MapFrom(src => src.AnouncementType ?? string.Empty)
                );
            #endregion

            #region student Profile 
            CreateMap<CreateStudentProfile, StudentProfile>()
              .ForMember(dest => dest.Id, opt => opt.Ignore())
              .ForMember(dest => dest.UserId,  opt => opt.Ignore())
              .ForMember(dest => dest.user,  opt => opt.Ignore())
              .ForMember(dest => dest.ProfileImageUrl, opt => opt.Ignore())
              .ForMember(dest => dest.ProfileImagePublicId,  opt => opt.Ignore())
              .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.HasValue  ? src.Gender.Value.ToString() : null))
              .ForMember(dest => dest.CollegeName, opt => opt.MapFrom(src => src.collegename))
              .ForMember(dest => dest.PassingYear,  opt => opt.MapFrom(src => src.Passingyear))
              .ForMember(dest => dest.CreateBy, opt => opt.Ignore())
              .ForMember(dest => dest.CreateAt, opt => opt.Ignore())
              .ForMember(dest => dest.UpdateBy,  opt => opt.Ignore())
              .ForMember(dest => dest.UpdateAt,  opt => opt.Ignore());


            CreateMap<StudentProfile, StudentProfileResponseDto>()
             .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.user != null ? src.user.FullName : null))
             .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.user != null ? src.user.Email : null))
             .ForMember(dest => dest.MobileNumber, opt => opt.MapFrom(src => src.user != null ? src.user.PhoneNumber : null));

            #endregion

            #region Teacher Profile
            CreateMap<CreateTeacherProfile, TeacherProfiles>()
               .ForMember(dest => dest.Id,
                   opt => opt.Ignore())            
               .ForMember(dest => dest.ExperienceYears,
                   opt => opt.MapFrom(src => src.Experienceyear))
               .ForMember(dest => dest.Bio,
                   opt => opt.MapFrom(src => src.bio))
               .ForMember(dest => dest.Specialization,
                   opt => opt.MapFrom(src => src.specialization))
               .ForMember(dest => dest.UserId,
                   opt => opt.Ignore())
               .ForMember(dest => dest.user,
                   opt => opt.Ignore());

            CreateMap<TeacherProfiles, TeacherProfileResponseDto>()
              .ForMember(dest => dest.UserName,
                  opt => opt.MapFrom(src => src.user != null
                      ? src.user.FullName : null))
              .ForMember(dest => dest.Email,
                  opt => opt.MapFrom(src => src.user != null
                      ? src.user.Email : null))
              .ForMember(dest => dest.MobileNo,
                  opt => opt.MapFrom(src => src.user != null
                      ? src.user.PhoneNumber : null));

            #endregion
        }
    }
}
