using Core.Dtos.Common;
using Core.Dtos.Courses;
using Core.Dtos.LiveClasses;
using Core.Interface.Services;
using Domain.Entities;
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

        [HttpPost("StartSession")]
        [Authorize]
        public async Task<IActionResult> CreateVideoProgress([FromBody] StartVideoSessionDto dto)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }

            var sessionId = await _liveServices.StartSessionAsync(
                userId, dto.VideoId);

            return Ok(new
            {
                success = true,
                message = "Video session started.",
                data = new { sessionId }
            });

        }

        [HttpPost("SaveProgress")]
        [Authorize]
        public async Task<IActionResult> CreateVideoProgressAsync([FromBody] SaveVideoProgressDto dto)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _liveServices.SaveProgressAsync(
              userId, dto);

            if (!result)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Unable to save video progress."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Progress saved successfully."
            });

        }

        [HttpPost("EndSession")]
        public async Task<IActionResult> EndSession(Guid sessionId, [FromQuery] string reason = "UserClosed")
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var result = await _liveServices.EndSessionAsync(
                userId, sessionId, reason);

            if (!result)
                return NotFound(new
                {
                    success = false,
                    message = "Active session not found."
                });

            return Ok(new
            {
                success = true,
                message = "Session closed."
            });
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
        public async Task<IActionResult> GetListCourseVideo([FromQuery] PaginationRequestDto dto, int batchId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var result = await _liveServices.GetListCourseVideoAsync(dto, batchId);
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

        [HttpGet("Get-Student-CourseVideo")]
        [Authorize]
        public async Task<IActionResult> GetStudentCourseVideosAsync([FromQuery] int batchId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var result = await _liveServices.GetStudentCourseVideosAsync(batchId, userId);
            return StatusCode((int)result.ResponseCode, result);
        }

        [HttpGet("{videoId:int}")]
        public async Task<IActionResult> GetProgress(int videoId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _liveServices.GetProgressAsync(
                userId, videoId);

            return Ok(new
            {
                success = true,
                data = result
            });
        }

        // Get student's last watched video
        [HttpGet("LastWatched")]
        public async Task<IActionResult> GetLastWatched()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _liveServices.GetLastWatchedAsync(
                userId);

            return Ok(new
            {
                success = true,
                data = result
            });
        }


        [HttpGet("Get-Student-Upcoming-LiveClasses")]
        [Authorize]
        public async Task<IActionResult> GetStudentUpcomingLiveClasses([FromQuery] PaginationRequestDto dto)
        {
            var studentId = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            if (string.IsNullOrWhiteSpace(studentId))
            {
                return Unauthorized();
            }

            var result = await _liveServices.GetStudentUpcomingLiveClassesAsync(dto, studentId);

            return StatusCode((int)result.ResponseCode, result);
        }

        #endregion
    }
}
