using Core.Dtos.Batch;
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
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherServices _teacherServices;
        private readonly UserManager<ApplicationUser> _userManger;
        public TeacherController(ITeacherServices teacherServices, UserManager<ApplicationUser> userManger)
        {
            _teacherServices = teacherServices;
            _userManger = userManger;
        }

        #region  Create
        [HttpPost("Create-BatchTeacher")]
        [Authorize]
        public async Task<IActionResult> CreateBatchTeacher([FromBody] CreateTeacherBatchDto dto)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _teacherServices.CreateBatchTeacherAsync(dto, userId);
            return StatusCode(res.ResponseCode, res);

        }

        [HttpPost("Create-AssignBatch/{batchId:int}/teachers")]
        public async Task<IActionResult> CreateAssignTeacher(int batchId, [FromBody] CreateBatchTeacherDto dto)
        {
            var userId = _userManger.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();
            var response = await _teacherServices.AssignTeacherAsync(batchId, dto, userId);
            return StatusCode(response.ResponseCode, response);
        }
        #endregion


        #region  Update
        [HttpPut("Update-BatchTeacher")]
        [Authorize]
        public async Task<IActionResult> UpdateBatchTeacher([FromBody] UpdateTeacherBatchDto dto)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _teacherServices.UpdateBatchTeacherAsync(dto, userId);
            return StatusCode(res.ResponseCode, res);

        }


        [HttpPut("Update-BatchTeacher/{batchId:int}/teachers/{teacherId}")]
        public async Task<IActionResult> UpdateTeacherAssignment(int batchId, string teacherId, [FromBody] UpdateBatchTeacherDto dto)
        {
            var userId = _userManger.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();
            var response = await _teacherServices.UpdateTeacherAssignmentAsync(batchId, teacherId, dto, userId);
            return StatusCode(response.ResponseCode, response);
        }

        [HttpPatch("Update-Batch/{batchId:int}/teachers/{teacherId}/primary")]
        public async Task<IActionResult> UpdateTeacherPrimary(int batchId, string teacherId, [FromBody] UpdateBatchTeacherAssignStatusDto dto)
        {
            var userId = _userManger.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();
            var response = await _teacherServices.UpdateTeacherAssigmentPrimaryAsync(batchId, teacherId, dto, userId);
            return StatusCode(response.ResponseCode, response);
        }
        #endregion


        #region  Update Status
        [HttpPut("update-status-BatchTeacher")]
        [Authorize]
        public async Task<IActionResult> UpdateStatusBatchTeacher([FromQuery] int Id)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _teacherServices.UpdateStatusBatchTeacherAsync(Id, userId);
            return StatusCode(res.ResponseCode, res);

        }

        [HttpPatch("Update-BatchAssign/{batchId:int}/teachers/{teacherId}/status")]
        public async Task<IActionResult> UpdateTeacherStatus(int batchId, string teacherId)
        {
            var userId = _userManger.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();
            var response = await _teacherServices.UpdateTeacherAssigmentStatusAsync(batchId, teacherId, userId);
            return StatusCode(response.ResponseCode, response);
        }
        #endregion



        #region  GetById
        [HttpGet("Get-BatchTeacher")]
        [Authorize]
        public async Task<IActionResult> GetBatchTeacher([FromQuery] int Id)
        {
            var response = await _teacherServices.GetByIdBatchTeacherAsync(Id);
            return StatusCode(response.ResponseCode, response);
        }


        #endregion


        #region  GetList
        [HttpGet("Get-List-BatchTeacher")]
        [Authorize]
        public async Task<IActionResult> GetListBatchTeacher([FromQuery] PaginationRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var result = await _teacherServices.GetListBatchTeacherAsync(dto);
            return StatusCode((int)result.ResponseCode, result);
        }

        [HttpGet("Get-List-BatchTeacher-Assign-Admin")]
        [Authorize]
        public async Task<IActionResult> GetListAdminTeacherAssignAsync([FromQuery] AdminTeacherRequestVm dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var result = await _teacherServices.GetListAdminTeacherAssignAsync(dto);
            return StatusCode((int)result.ResponseCode, result);
        }



        [HttpGet("Get-List-Teacher-IdName")]
        [Authorize]
        public async Task<IActionResult> GetListTeacherIdName()
        {
            var result = await _teacherServices.GetListTeacherIdNameAsync();
            return StatusCode((int)result.ResponseCode, result);
        }



        [HttpGet("Get-List-Batch/{batchId:int}/teachers")]
        public async Task<IActionResult> GetBatchTeachers(int batchId)
        {
            var userId = _userManger.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();
            var response = await _teacherServices.GetBatchTeachersAsync(batchId);
            return StatusCode(response.ResponseCode, response);
        }
        #endregion

    }
}
