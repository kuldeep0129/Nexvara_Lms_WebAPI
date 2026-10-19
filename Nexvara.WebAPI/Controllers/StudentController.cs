using Core.Dtos.Common;
using Core.Dtos.Courses;
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
    public class StudentController : ControllerBase
    {

        private readonly IStudentServices _studentServices;
        private readonly UserManager<ApplicationUser> _userManger;
        public StudentController(IStudentServices studentServices, UserManager<ApplicationUser> userManger)
        {
            _studentServices = studentServices;
            _userManger = userManger;
        }

        #region  Create
        [HttpPost("Create-BatchStudent")]
        [Authorize]
        public async Task<IActionResult> CreateBatchStudent([FromBody] CreateBatchStudentDto dto)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _studentServices.CreateBatchStudentAsync(dto, userId);
            return StatusCode(res.ResponseCode, res);

        }


        #endregion


        #region  Update
        [HttpPut("Update-BatchStudent")]
        [Authorize]
        public async Task<IActionResult> UpdateBatchStudent([FromBody] UpdateBatchStudentDto dto)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _studentServices.UpdateBatchStudentAsync(dto, userId);
            return StatusCode(res.ResponseCode, res);

        }

        #endregion


        #region  Update Status
        [HttpPut("update-status-BatchStudent")]
        [Authorize]
        public async Task<IActionResult> UpdateStatusBatchStudent([FromQuery] int Id)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _studentServices.UpdateStatusBatchStudentAsync(Id, userId);
            return StatusCode(res.ResponseCode, res);

        }

        #endregion



        #region  GetById
        [HttpGet("Get-BatchStudent")]
        [Authorize]
        public async Task<IActionResult> GetBatchStudent([FromQuery] int Id)
        {
            var response = await _studentServices.GetByIdBatchStudentAsync(Id);
            return StatusCode(response.ResponseCode, response);
        }


        #endregion


        #region  GetList
        [HttpGet("Get-List-BatchStudent")]
        [Authorize]
        public async Task<IActionResult> GetListBatchStudent([FromQuery] PaginationRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var result = await _studentServices.GetListBatchStudentAsync(dto);
            return StatusCode((int)result.ResponseCode, result);
        }

        #endregion

    }
}
