using Core.Dtos.Common;
using Core.Dtos.Courses;
using Core.Dtos.LiveClasses;
using Core.Interface.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Nexvara.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LiveClassController : ControllerBase
    {
        private readonly ILiveClassServices _liveServices;
        private readonly UserManager<ApplicationUser> _userManger;
        public LiveClassController(ILiveClassServices liveServices, UserManager<ApplicationUser> userManger)
        {
            _liveServices = liveServices;
            _userManger = userManger;
        }

        #region  Create
        [HttpPost("Create-LiveClass")]
        [Authorize]
        public async Task<IActionResult> CreateLiveClass([FromForm] CreateLiveClassDto dto)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _liveServices.CreateLiveClassAsync(dto, userId);
            return StatusCode(res.ResponseCode, res);

        }


        [HttpPost("Create-ClassAttendance")]
        [Authorize]
        public async Task<IActionResult> CreateClassAttendance([FromBody] CreateClassAttendanceDto dto)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _liveServices.CreateClassAttendanceAsync(dto, userId);
            return StatusCode(res.ResponseCode, res);

        }

        [HttpPost("Create-CourseVideo")]
        [Authorize]
        public async Task<IActionResult> CreateCourseVideo([FromForm] CreateCourseVideoDto dto)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _liveServices.CreateCourseVideoAsync(dto, userId);
            return StatusCode(res.ResponseCode, res);

        }
        [HttpPost("Create-VideoProgress")]
        [Authorize]
        public async Task<IActionResult> CreateVideoProgress([FromForm] CreateVideoProgressDto dto)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _liveServices.CreateVideoProgressAsync(dto, userId);
            return StatusCode(res.ResponseCode, res);

        }


        #endregion


        #region  Update
        [HttpPut("Update-LiveClass")]
        [Authorize]
        public async Task<IActionResult> UpdateLiveClass([FromForm] UpdateLiveClassesDto dto)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _liveServices.UpdateLiveClassAsync(dto, userId);
            return StatusCode(res.ResponseCode, res);

        }


        [HttpPut("Update-ClassAttendance")]
        [Authorize]
        public async Task<IActionResult> UpdateClassAttendance([FromBody] UpdateClassAttendance dto)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _liveServices.UpdateClassAttendanceAsync(dto, userId);
            return StatusCode(res.ResponseCode, res);

        }

        [HttpPut("Update-CourseVideo")]
        [Authorize]
        public async Task<IActionResult> UpdateCourseVideo([FromForm] UpdateCourseVideosDto dto)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _liveServices.UpdateCourseVideoAsync(dto, userId);
            return StatusCode(res.ResponseCode, res);

        }
        [HttpPut("Update-VideoProgress")]
        [Authorize]
        public async Task<IActionResult> UpdateVideoProgress([FromForm] UpdateVideoProgressDto dto)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _liveServices.UpdateVideoProgressAsync(dto, userId);
            return StatusCode(res.ResponseCode, res);

        }

        #endregion


        #region  Update Status
        [HttpPut("update-status-LiveClass")]
        [Authorize]
        public async Task<IActionResult> UpdateStatusLiveClass([FromQuery] int Id)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _liveServices.UpdateStatusLiveClassAsync(Id, userId);
            return StatusCode(res.ResponseCode, res);

        }

        [HttpPut("update-status-ClassAttendance")]
        [Authorize]
        public async Task<IActionResult> UpdateStatusClassAttendance([FromQuery] int Id)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _liveServices.UpdateStatusClassAttendanceAsync(Id, userId);
            return StatusCode(res.ResponseCode, res);

        }

        [HttpPut("update-status-CourseVideo")]
        [Authorize]
        public async Task<IActionResult> UpdateStatusCourseVideo([FromQuery] int Id)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _liveServices.UpdateStatusCourseVideoAsync(Id, userId);
            return StatusCode(res.ResponseCode, res);

        }
        [HttpPut("update-status-VideoProgress")]
        [Authorize]
        public async Task<IActionResult> UpdateStatusVideoProgress([FromQuery] int Id)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _liveServices.UpdateStatusVideoProgressAsync(Id, userId);
            return StatusCode(res.ResponseCode, res);

        }

        #endregion



        #region  GetById
        [HttpGet("Get-LiveClass")]
        [Authorize]
        public async Task<IActionResult> GetLiveClass([FromQuery] int Id)
        {
            var response = await _liveServices.GetByIdLiveClassAsync(Id);
            return StatusCode(response.ResponseCode, response);
        }
        [HttpGet("Get-ClassAttendance")]
        [Authorize]
        public async Task<IActionResult> GetClassAttendance([FromQuery] int Id)
        {
            var response = await _liveServices.GetByIdClassAttendanceAsync(Id);
            return StatusCode(response.ResponseCode, response);
        }
        [HttpGet("Get-CourseVideo")]
        [Authorize]
        public async Task<IActionResult> GetCourseVideo([FromQuery] int Id)
        {
            var response = await _liveServices.GetByIdCourseVideoAsync(Id);
            return StatusCode(response.ResponseCode, response);
        }
        [HttpGet("Get-VideoProgress")]
        [Authorize]
        public async Task<IActionResult> GetVideoProgress([FromQuery] int Id)
        {
            var response = await _liveServices.GetByIdVideoProgressAsync(Id);
            return StatusCode(response.ResponseCode, response);
        }


        #endregion


        #region  GetList
        [HttpGet("Get-List-ClassAttendance")]
        [Authorize]
        public async Task<IActionResult> GetListClassAttendance([FromQuery] PaginationRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var result = await _liveServices.GetListClassAttendanceAsync(dto);
            return StatusCode((int)result.ResponseCode, result);
        }

        [HttpGet("Get-List-CourseVideo")]
        [Authorize]
        public async Task<IActionResult> GetListCourseVideo([FromQuery] PaginationRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var result = await _liveServices.GetListCourseVideoAsync(dto);
            return StatusCode((int)result.ResponseCode, result);
        }

        [HttpGet("Get-List-LiveClass")]
        [Authorize]
        public async Task<IActionResult> GetLiveClass([FromQuery] PaginationRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var result = await _liveServices.GetListLiveClassAsync(dto);
            return StatusCode((int)result.ResponseCode, result);
        }
        [HttpGet("Get-List-VideoProgress")]
        [Authorize]
        public async Task<IActionResult> GetListVideoProgress([FromQuery] PaginationRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var result = await _liveServices.GetListVideoProgressAsync(dto);
            return StatusCode((int)result.ResponseCode, result);
        }

        #endregion
    }
}
