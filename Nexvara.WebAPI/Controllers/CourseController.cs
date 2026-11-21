using Core.Dtos.Common;
using Core.Dtos.Courses;
using Core.Interface.Services;
using Domain.Entities;
using Domain.Entities.Student;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Nexvara.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {

        private readonly ICoursesServices _courseServices;
        private readonly UserManager<ApplicationUser> _userManger;
        public CourseController(ICoursesServices courseServices, UserManager<ApplicationUser> userManger)
        {
            _courseServices = courseServices;
            _userManger = userManger;
        }

        #region create 
        [HttpPost("Create-Course")]
        [Authorize]
        public async Task<IActionResult> CreateCourse([FromForm] CreateCourses course)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _courseServices.CreateCoursesAsync(course, userId);
            return StatusCode(res.ResponseCode, res);

        }

        [HttpPost("Create-CourseModule")]
        [Authorize]
        public async Task<IActionResult> CreateCourseModule([FromBody] CreateCourseModuleDto course)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _courseServices.CreateCoursesModuleAsync(course, userId);
            return StatusCode(res.ResponseCode, res);

        }


        [HttpPost("Create-Batch")]
        [Authorize]
        public async Task<IActionResult> CreateBatch([FromBody] CreateBatchDto dto)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _courseServices.CreateBatchAsync(dto, userId);
            return StatusCode(res.ResponseCode, res);

        }



        [HttpPost("Create-CourseSyllabus")]
        [Authorize]
        public async Task<IActionResult> CreateCourseSyllabus([FromBody] CreateCourseSyllabusDto dto)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _courseServices.CreateCourseSyllabusAsync(dto, userId);
            return StatusCode(res.ResponseCode, res);

        }



        [HttpPost("Create-CourseMaterial")]
        [Authorize]
        public async Task<IActionResult> CreateCourseMaterial([FromForm] CreateCourseMaterialsDto dto)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _courseServices.CreateCourseMaterialAsync(dto, userId);
            return StatusCode(res.ResponseCode, res);

        }


        #endregion

        #region Update
        [HttpPut("Update-Course")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateCourse([FromForm] UpdateCourses course)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _courseServices.UpdateCoursesAsync(course, userId);
            return StatusCode(res.ResponseCode, res);

        }

        [HttpPut("Update-CourseModule")]
        [Authorize]
        public async Task<IActionResult> UpdateCourseModule([FromBody] UpdateCourseModule course)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _courseServices.UpdateCoursesModuleAsync(course, userId);
            return StatusCode(res.ResponseCode, res);

        }

        [HttpPut("Update-Batch")]
        [Authorize]
        public async Task<IActionResult> UpdateBatch([FromBody] UpdateCourseBatchDto dto)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _courseServices.UpdateBatchAsync(dto, userId);
            return StatusCode(res.ResponseCode, res);

        }



        [HttpPut("Update-CourseSyllabus")]
        [Authorize]
        public async Task<IActionResult> UpdateCourseSyllabus([FromBody] UpdateCourseSyllabusDto dto)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _courseServices.UpdateCourseSyllabusAsync(dto, userId);
            return StatusCode(res.ResponseCode, res);

        }



        [HttpPut("Update-CourseMaterial")]
        [Authorize]
        public async Task<IActionResult> UpdateCourseMaterial([FromForm] UpdateCourseMaterialDto dto)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _courseServices.UpdateCourseMaterialAsync(dto, userId);
            return StatusCode(res.ResponseCode, res);

        }
        #endregion

        #region UpdateStatus
        [HttpPut("update-status-Course")]
        [Authorize]
        public async Task<IActionResult> UpdateStatusCourse([FromQuery] int Id)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _courseServices.UpdateStatusCoursesAsync(Id, userId);
            return StatusCode(res.ResponseCode, res);

        }

        [HttpPut("update-status-CourseModule")]
        [Authorize]
        public async Task<IActionResult> UpdateStatusCourseModule([FromQuery] int Id)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _courseServices.UpdateStatusCoursesModuleAsync(Id, userId);
            return StatusCode(res.ResponseCode, res);

        }

        [HttpPut("update-status-Batch")]
        [Authorize]
        public async Task<IActionResult> UpdateStatusBatch([FromQuery] int Id)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _courseServices.UpdateStatusBatchAsync(Id, userId);
            return StatusCode(res.ResponseCode, res);

        }


        [HttpPut("update-status-CourseSyllabus")]
        [Authorize]
        public async Task<IActionResult> UpdateStatusCourseSyllabus([FromQuery] int Id)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _courseServices.UpdateCourseSyllabusStatusAsync(Id, userId);
            return StatusCode(res.ResponseCode, res);

        }

        [HttpPut("update-status-CourseMaterial")]
        [Authorize]
        public async Task<IActionResult> UpdateStatusCourseMaterial([FromQuery] int Id)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _courseServices.UpdateCourseMaterialStatusAsync(Id, userId);
            return StatusCode(res.ResponseCode, res);

        }


        #endregion

        #region GetById
        [HttpGet("Get-Course")]
        [Authorize]
        public async Task<IActionResult> GetCourse([FromQuery] int Id)
        {
            var response = await _courseServices.GetByIdCoursesAsync(Id);
            return StatusCode(response.ResponseCode, response);
        }

        [HttpGet("Get-CourseModule")]
        [Authorize]
        public async Task<IActionResult> GetCourseModule([FromQuery] int Id)
        {
            var response = await _courseServices.GetByIdCoursesModuleAsync(Id);
            return StatusCode(response.ResponseCode, response);
        }

        [HttpGet("Get-Batch")]
        [Authorize]
        public async Task<IActionResult> GetBatch([FromQuery] int Id)
        {
            var response = await _courseServices.GetByIdBatchAsync(Id);
            return StatusCode(response.ResponseCode, response);
        }


        [HttpGet("{courseId:int}/curriculum")]
        public async Task<IActionResult> GetCourseCurriculum(int courseId)
        {
            var response = await _courseServices.GetCourseCurriculumAsync(courseId);

            return StatusCode(response.ResponseCode, response);
        }

        [HttpGet("CouseSyllabus")]
        public async Task<IActionResult> GetCourseCouseSyllabus(int Id)
        {
            var response = await _courseServices.GetByIdCourseSyllabusAsync(Id);

            return StatusCode(response.ResponseCode, response);
        }
        [HttpGet("CouseMaterial")]
        public async Task<IActionResult> GetCourseCouseMaterial(int Id)
        {
            var response = await _courseServices.GetByIdCourseMaterialAsync(Id);

            return StatusCode(response.ResponseCode, response);
        }

        #endregion

        #region GetList
        [HttpGet("Get-List-Course")]
        [Authorize]
        public async Task<IActionResult> GetListCoure([FromQuery] PaginationRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var result = await _courseServices.GetListCoursesAsync(dto);
            return StatusCode((int)result.ResponseCode, result);
        }

        [HttpGet("Get-List-CourseModule")]
        [Authorize]
        public async Task<IActionResult> GetListCoureModule([FromQuery] PaginationRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var result = await _courseServices.GetListCoursesModuleAsync(dto);
            return StatusCode((int)result.ResponseCode, result);
        }

        [HttpGet("Get-List-Batch")]
        [Authorize]
        public async Task<IActionResult> GetListBatch([FromQuery] PaginationRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var result = await _courseServices.GetListBatchAsync(dto);
            return StatusCode((int)result.ResponseCode, result);
        }

        [HttpGet("Get-List-Draft-Course")]
        [Authorize]
        public async Task<IActionResult> GetListDraftCourse([FromQuery] PaginationRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var result = await _courseServices.GetListCoursesFeaturesAsync(dto);
            return StatusCode((int)result.ResponseCode, result);
        }

        [HttpGet("Get-List-Publish-Course")]
        [Authorize]
        public async Task<IActionResult> GetListPublistCourse([FromQuery] PaginationRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var result = await _courseServices.GetListCoursesPublishAsync(dto);
            return StatusCode((int)result.ResponseCode, result);
        }
        

        [HttpGet("Get-List-Course-Syllabus")]
        [Authorize]
        public async Task<IActionResult> GetListCourseSyllabusAsync([FromQuery] PaginationRequestDto dto, int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var result = await _courseServices.GetListCourseSyllabusAsync(dto,courseId);
            return StatusCode((int)result.ResponseCode, result);
        }

        [HttpGet("Get-List-Study-Material")]
        [Authorize]
        public async Task<IActionResult> GetListStudyMaterial()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var result = await _courseServices.GetStudyMaterialAsync(userId);
            return StatusCode((int)result.ResponseCode, result);
        }




        #endregion

        #region Publish
        [HttpPut("update-Publish-CourseMaterial")]
        [Authorize]
        public async Task<IActionResult> UpdatePublishCourseMaterial([FromQuery] int Id)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _courseServices.UpdateCourseMaterialPublishAsync(Id, userId);
            return StatusCode(res.ResponseCode, res);

        }
        #endregion


    }
}
