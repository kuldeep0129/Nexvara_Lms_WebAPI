using Core.Dtos.Account;
using Core.Dtos.Common;
using Core.Dtos.Profile;
using Core.Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Nexvara.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountServices _accountServices;
        public AccountController(IAccountServices accountServices)
        {
            _accountServices = accountServices;
        }


        [HttpPost("Email-Confirmation")]
        public async Task<IActionResult> EmailConfirmation([FromQuery] string userId , string token)
        {
            var result = await _accountServices.ConfirmEmailAsync(userId,token);
            return StatusCode((int)result.ResponseCode, result);
        }

        [HttpPost("Login-User")]
        public async Task<IActionResult> LoginUser([FromBody] LoginDto loginDto)
        {
            var result = await _accountServices.LoginAsync(loginDto);
            return StatusCode((int)result.ResponseCode, result);
        }
        [HttpPost("refresh-Token")]
        public async Task<IActionResult> Refresh([FromBody] string refershToken)
        {
            var result = await _accountServices.RefreshAsync(refershToken);
            return StatusCode((int)result.ResponseCode, result);
        }

        [HttpPost("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout(LogoutDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var result = await _accountServices.LogoutAsync(dto, userId, ipAddress);

            return StatusCode((int)result.ResponseCode, result);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegistrationDto dto)
        {
            var result = await _accountServices.RegisterAsync(dto);
            return StatusCode((int)result.ResponseCode, result);
        }

        //student
        [HttpPost("Update-Student-Profile")]
        [Authorize]
        public async Task<IActionResult> UpdateStudentProfile([FromForm] CreateStudentProfile dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var result = await _accountServices.StudentProfileAsync(dto, userId);
            return StatusCode((int)result.ResponseCode, result);
        }

        [HttpPut("Update-Status-Student-Profile")]
        [Authorize]
        public async Task<IActionResult> UpdateStatusStudentProfile([FromQuery] string Id)
        {
            var userId = "";
            if (Id != null)
            {
                userId = Id.Trim();
            }
            else
            {
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var result = await _accountServices.UpdateStatusStudentProfileAsync(userId);
            return StatusCode((int)result.ResponseCode, result);
        }


        [HttpGet("Get-ById-StudentProfile")]
        [Authorize]
        public async Task<IActionResult> GetByIdStudentProfile([FromQuery] string Id)
        {
            var userId = "";
            if (Id != null)
            {
                userId = Id.Trim();
            }
            else
            {
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var result = await _accountServices.GetByIdStudentProfileAsync(userId);
            return StatusCode((int)result.ResponseCode, result);
        }

        [HttpGet("Get-List-StudentProfile")]
        [Authorize]
        public async Task<IActionResult> GetListtudentProfile([FromQuery] PaginationRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var result = await _accountServices.GetListStudentProfileAsync(dto);
            return StatusCode((int)result.ResponseCode, result);
        }


        //Teacher 
        [HttpPost("Update-Teacher-Profile")]
        [Authorize]
        public async Task<IActionResult> UpdateTeacherProfile([FromForm] CreateTeacherProfile dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var result = await _accountServices.TeacherProfileAsync(dto, userId);
            return StatusCode((int)result.ResponseCode, result);
        }

        [HttpPut("Update-Status-Teacher-Profile")]
        [Authorize]
        public async Task<IActionResult> UpdateStatusTeacherProfile([FromQuery] string Id)
        {
            var userId = "";
            if (Id != null)
            {
                userId = Id.Trim();
            }
            else
            {
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var result = await _accountServices.UpdateStatusTeacherProfileAsync(userId);
            return StatusCode((int)result.ResponseCode, result);
        }


        [HttpGet("Get-ById-TeacherProfile")]
        [Authorize]
        public async Task<IActionResult> GetByIdTeacherProfile([FromQuery] string Id)
        {
            var userId = "";
            if (Id != null)
            {
                userId = Id.Trim();
            }
            else
            {
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var result = await _accountServices.GetByIdTeacherProfileAsync(userId);
            return StatusCode((int)result.ResponseCode, result);
        }

        [HttpGet("Get-List-TeacherProfile")]
        [Authorize]
        public async Task<IActionResult> GetListTeacherProfile([FromQuery] PaginationRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var result = await _accountServices.GetListTeacherProfileAsync(dto);
            return StatusCode((int)result.ResponseCode, result);
        }



        [HttpGet("Get-Common-Profile")]
        [Authorize]
        public async Task<IActionResult> GetCommonProfile([FromQuery] string Id)
        {
            var   userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var result = await _accountServices.GetCommonProfileDto(userId);
            return StatusCode((int)result.ResponseCode, result);
        }
    }
}
