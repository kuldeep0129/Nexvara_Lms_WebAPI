using Core.Dtos.UserInterFace;
using Core.Interface.Services;
using Domain.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Nexvara.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebstiteUIController : ControllerBase
    {
        private readonly IUIServices _uIServices;
        private readonly UserManager<ApplicationUser> _userManger;
        public WebstiteUIController(IUIServices uIServices , UserManager<ApplicationUser> userManger)
        {
            _uIServices = uIServices;
            _userManger = userManger;
        }

        #region Create


        [HttpPost("CreateAdminssionApplication")]
        public async Task<IActionResult> CreateAdminssionApplication([FromBody] CreateAdmissionApplicationDto dto)
        {
           var res = await _uIServices.CreateAdminssionApplicationAsync(dto);
            return StatusCode((res.ResponseCode), res);
            
        }

        [HttpPost("CreateOrganization")]
        public async Task<IActionResult> CrateOrganization([FromForm] CreateOrganizationDto dto)
        {
            var userId = _userManger.GetUserId(User);
            if(userId == null)
            {
                return Unauthorized();
            }
                var res = await _uIServices.CreateOrganization(dto,userId);
                return StatusCode((res.ResponseCode), res);                   
        }

        [HttpPost("CreateProgramType")]
        public async Task<IActionResult> CreateProgramType([FromBody] CreateProgramTypeDto dto)
        {
           var res = await _uIServices.CreateProgramTypeAsync(dto);
            return StatusCode((res.ResponseCode), res);
            
        }

        [HttpPost("CreateAnnouncement")]
        public async Task<IActionResult> CreateAnnouncement([FromBody] CreateAnnouncementDto dto)
        {
           var res = await _uIServices.CreateAnnouncementAsync(dto);
            return StatusCode((res.ResponseCode), res);
            
        }

        [HttpPost("CreateCertificate")]
        public async Task<IActionResult> CreateCertificate([FromBody] CreateCertificateDto dto)
        {
            var res = await _uIServices.CreateCertificateAsync(dto);
            return StatusCode((res.ResponseCode), res);

        }

        [HttpPost("CreateCourse")]
        public async Task<IActionResult> CreateCourse([FromForm] CreateCourseDto dto)
        {
            var res = await _uIServices.CreateCourseAsync(dto);
            return StatusCode((res.ResponseCode), res);

        }

        [HttpPost("CreateEnquiry")]
        public async Task<IActionResult> CreateEnquiry([FromBody] CreateEnquiryDto dto)
        {
            var res = await _uIServices.CreateEnquiryAsync(dto);
            return StatusCode((res.ResponseCode), res);

        }

        [HttpPost("CreateProgram")]
        public async Task<IActionResult> CreateProgram([FromBody] CreateProgramDto dto)
        {
            var res = await _uIServices.CreateProgramAsync(dto);
            return StatusCode((res.ResponseCode), res);

        }


        [HttpPost("CreateResources")]
        public async Task<IActionResult> CreateResources([FromBody] CreateResourceDto dto)
        {
            var res = await _uIServices.CreateResourcesAsync(dto);
            return StatusCode((res.ResponseCode), res);

        }


        [HttpPost("CreateSiteContent")]
        public async Task<IActionResult> CreateSiteContent([FromForm] CreateSiteContentDto dto)
        {
            var res = await _uIServices.CreateSiteContentAsync(dto);
            return StatusCode((res.ResponseCode), res);

        }

        [HttpPost("CreateStudentAchievement")]
        public async Task<IActionResult> CreateStudentAchievement([FromForm] CreateStudentAchievementDto dto)
        {
            var res = await _uIServices.CreateStudentAchievementAsync(dto);
            return StatusCode((res.ResponseCode), res);

        }


        [HttpPost("CreateStudentProject")]
        public async Task<IActionResult> CreateStudentProject([FromForm] CreateStudentProjectDto dto)
        {
            var res = await _uIServices.CreateStudentProjectAsync(dto);
            return StatusCode((res.ResponseCode), res);

        }

        [HttpPost("CreateTestimonial")]
        public async Task<IActionResult> CreateTestimonial([FromBody] CreateTestimonialDto dto)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _uIServices.CreateTestimonalsAsync(dto,userId);
            return StatusCode((res.ResponseCode), res);

        }

        #endregion

        #region Get Hero Section

        [HttpGet("site-content")]
        public async Task<IActionResult> GetSiteContentBySectionKey( [FromQuery] string sectionKey)
        {
            var response = await _uIServices.GetSiteContentBySectionKeyAsync(sectionKey);
            return StatusCode(response.ResponseCode, response);
        }


        [HttpGet("programs")]
        public async Task<IActionResult> GetPrograms()
        {
            var response = await _uIServices.GetProgramsAsync();
            return StatusCode(response.ResponseCode, response);
        }


        [HttpGet("courses")]
        public async Task<IActionResult> GetCourses( [FromQuery] bool featured = false)
        {
            var response = await _uIServices.GetCoursesAsync(featured);
            return StatusCode(response.ResponseCode, response);
        }


        [HttpGet("student-achievements")]
        public async Task<IActionResult> GetStudentAchievements( [FromQuery] bool featured = false)
        {
            var response = await _uIServices.GetStudentAchievementsAsync(featured);
            return StatusCode(response.ResponseCode, response);
        }


        [HttpGet("student-projects")]
        public async Task<IActionResult> GetStudentProjects( [FromQuery] bool featured = false)
        {
            var response =  await _uIServices.GetStudentProjectsAsync(featured);
            return StatusCode(response.ResponseCode, response);
        }


        [HttpGet("testimonials")]
        public async Task<IActionResult> GetTestimonials( [FromQuery] bool featured = false)
        {
            var response = await _uIServices.GetTestimonialsAsync(featured);
            return StatusCode(response.ResponseCode, response);
        }

        [HttpGet("active-Anouncement")]
        public async Task<IActionResult> GetActiveAnnouncements()
        {
            var response =   await _uIServices.GetActiveAnnouncementsAsync();
            return StatusCode(response.ResponseCode, response);
        }
        [HttpGet("Get-Organization")]
        public async Task<IActionResult> GetOrganization()
        {
            var response =   await _uIServices.GetOrganizationAsync();
            return StatusCode(response.ResponseCode, response);
        }

        #endregion
    }
}
