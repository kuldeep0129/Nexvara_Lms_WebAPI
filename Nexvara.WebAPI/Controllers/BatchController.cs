using Core.Dtos.Batch;
using Core.Dtos.Common;
using Core.Dtos.Courses;
using Core.Interface.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Nexvara.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BatchController : ControllerBase
    {
        private readonly IBatchServices _batchServices;
        private readonly UserManager<ApplicationUser> _userManager;

        public BatchController(  IBatchServices batchServices, UserManager<ApplicationUser> userManager)
        {
            _batchServices = batchServices;
            _userManager = userManager;
        }


        #region Create

        [HttpPost("Create-Batch")]
        public async Task<IActionResult> CreateBatch( [FromBody] CreateBatchDto dto)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var response =  await _batchServices.CreateBatchAsync(   dto,  userId);
            return StatusCode( response.ResponseCode, response);
        }

        [HttpPost("Create-Batch/{batchId:int}/schedules")]
        public async Task<IActionResult> CreateSchedule( int batchId, [FromBody] CreateBatchScheduleDto dto)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var response = await _batchServices.CreateScheduleAsync(batchId, dto, userId);

            return StatusCode(response.ResponseCode,  response);
        }
        #endregion

        #region Get List

        [HttpGet("Get-List-Batch")]
        public async Task<IActionResult> GetBatchList( [FromQuery] PaginationRequestDto pagination,[FromQuery] BatchFilterDto filter)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var response =await _batchServices.GetBatchListAsync( pagination,filter);
            return StatusCode(  response.ResponseCode,  response);
        }

        [HttpGet("Get-List-batch-schedules")]
        public async Task<IActionResult> GetBatchScheduleList( [FromQuery] PaginationRequestDto pagination, [FromQuery] BatchScheduleFilterDto filter)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();
            var response = await _batchServices.GetBatchScheduleListAsync( pagination, filter);
            return StatusCode(  response.ResponseCode, response);
        }


        #endregion


        #region Get By Id

        [HttpGet("Get-ById-Batch/{id:int}")]
        public async Task<IActionResult> GetBatchById(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var response =  await _batchServices.GetBatchByIdAsync(id);
            return StatusCode( response.ResponseCode, response);
        }

        [HttpGet("Get-ById-BatchSchedule/{id:int}")]
        public async Task<IActionResult> GetBatchSchduleById(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var response =await _batchServices.GetByIdScheduleAsync(id);

            return StatusCode( response.ResponseCode,response);
        }

        [HttpGet("Get-ByID-BatchList/{batchId:int}/schedules")]
        public async Task<IActionResult> GetBatchSchedules(int batchId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();
            var response = await _batchServices.GetListBatchSchedulesAsync(batchId);
            return StatusCode( response.ResponseCode, response);
        }

        #endregion

        #region Update

        [HttpPut("Update-Batch/{id:int}")]
        public async Task<IActionResult> UpdateBatch( int id,[FromBody] UpdateBatchDto dto)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();
            var response = await _batchServices.UpdateBatchAsync(id, dto, userId);
            return StatusCode(response.ResponseCode, response);
        }

        [HttpPatch("Update-BatchStatus/{id:int}/status")]
        public async Task<IActionResult> UpdateBatchStatus( int id,  [FromBody] UpdateBatchStatusDto dto)
        {
            var userId = _userManager.GetUserId(User);
           if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();
            var response = await _batchServices.UpdateBatchStatusAsync(id, dto, userId);

            return StatusCode(response.ResponseCode, response);
        }

        [HttpPatch("Update-BatchStatus/{id:int}/publish")]
        public async Task<IActionResult> UpdateBatchPublish(  int id,[FromBody] UpdateBatchPublishDto dto)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();
            var response = await _batchServices.UpdateBatchPublishAsync(id, dto, userId);
            return StatusCode(response.ResponseCode, response);
        }

        [HttpPatch("Update-batches/{batchId}/schedules/{scheduleId}")]
        public async Task<IActionResult> UpdateBatchSchedule(int batchId, int scheduleId, [FromBody] UpdateBatchScheduleDto dto)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();
            var response = await _batchServices.UpdateScheduleAsync(batchId,scheduleId, dto, userId);
            return StatusCode(response.ResponseCode, response);
        }

        #endregion

        #region Update Status
        [HttpDelete("Delete-Batch/{id:int}")]
        public async Task<IActionResult> ArchiveBatch(int id)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var response = await _batchServices.ArchiveBatchAsync(id, userId);
            return StatusCode(response.ResponseCode, response);
        }

        [HttpPatch("Update-Status-BatchSchedule")]
        public async Task<IActionResult> UpdateStatusBatchSchedule(int batchId,int scheduleId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();
            var response = await _batchServices.UpdateBatchSchedulesStatusAsync(scheduleId,batchId, userId);
            return StatusCode(response.ResponseCode, response);
        }


        #endregion


    }
}
