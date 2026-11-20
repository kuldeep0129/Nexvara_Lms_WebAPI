using Core.Dtos.Common;
using Core.Dtos.Courses;
using Core.Dtos.Payments;
using Core.Dtos.Student.Batch;
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
        private readonly IPaymentCheckoutService _paymentService;
        public StudentController(IStudentServices studentServices, UserManager<ApplicationUser> userManger, IPaymentCheckoutService paymentService)
        {
            _studentServices = studentServices;
            _userManger = userManger;
            _paymentService = paymentService;
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

        [HttpPost("Create-StudentEnrollment")]
        [Authorize]
        public async Task<IActionResult> CreateStudentEnrollment([FromBody] CreateStudentEnrollmentDto dto)
        {
            var userId = _userManger.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var res = await _studentServices.CreateStudentEnrollmentAsync(dto, userId);
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


        [HttpGet("Get-List-Explore-Course")]
        [Authorize]
        public async Task<IActionResult> GetListExpoloreCourse([FromQuery] ExploreCourseRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var result = await _studentServices.GetExploreCoursesAsync(dto);
            return StatusCode((int)result.ResponseCode, result);
        }


        [HttpGet("Get-List-Student-Schedule")]
        [Authorize]
        public async Task<IActionResult> GetListStudentSchedule([FromQuery] DateTime dateTime)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var result = await _studentServices.GetStudentSechduleAsync(dateTime,userId);
            return StatusCode((int)result.ResponseCode, result);
        }


        [HttpGet("Get-List-Enrolled-Batches")]
        [Authorize]
        public async Task<IActionResult> GetListEnrolledBatches(  [FromQuery] StudentEnrolledBatchFilterDto filter)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(studentId))
                return Unauthorized();

            var result = await _studentServices.GetEnrolledBatchesAsync(studentId, filter);

            return StatusCode((int)result.ResponseCode, result);
        }
        [HttpGet("Get-Continue-Learning")]
        public async Task<IActionResult> GetContinueLearning()
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(studentId))
                return Unauthorized();
            var result = await _studentServices.GetContinueLearningAsync(studentId);
            return Ok(result);
        }


        #endregion


        #region Payment Verfication

        [HttpPost("Verify")]
        [Authorize]
        public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentDto dto)
        {
            var res = await _paymentService.VerifyPaymentAsync(dto);
            return StatusCode(res.ResponseCode, res);
        }


        #endregion
    }
}
