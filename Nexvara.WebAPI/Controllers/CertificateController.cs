using Core.Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Nexvara.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificateController : ControllerBase
    {
        private readonly IStudentServices _studentServices;
        public CertificateController(IStudentServices studentServices) => _studentServices = studentServices;

        [HttpGet("Verify/{verificationCode}")]
        [AllowAnonymous]
        public async Task<IActionResult> Verify(string verificationCode)
        {
            var result = await _studentServices.VerifyCertificateAsync(verificationCode);
            return StatusCode(result.ResponseCode, result);
        }
    }
}
