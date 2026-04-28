using HRLeaveManagement.Application.Contracts.Identity;
using HRLeaveManagement.Application.Models.Identity;
using HRLeaveManagement.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HRLeaveManagementClean.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("PerIpPolicy")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) 
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<ActionResult<AuthResponse>> Login(AuthRequest request)
        => Ok(await _authService.Login(request));
        

        [HttpPost("register")]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<ActionResult<RegistrationResponse>> Register(RegistrationRequest request)
        => Ok(await _authService.Register(request));

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst("uid")?.Value;
            return Ok(await _authService.Logout(userId));
        }

        [HttpPost("forgot-password")]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<IActionResult> ForgotPaswword(ForgotPasswordRequest request)
            => Ok(await _authService.ForgotPassword(request));

        [HttpPost("reset-password")]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
            => Ok(await _authService.ResetPassword(request));

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailRequest request)
            => Ok(await _authService.ConfirmEmail(request));

        [HttpPost("refresh-token")]
        [EnableRateLimiting("GeneralPolicy")]

        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
        => Ok(await _authService.RefreshToken(request));

        [HttpPost("revoke-token")]
        [Authorize]
        public async Task<IActionResult> RevokeToken([FromBody] string refreshToken)
            => Ok(await _authService.RevokeToken(refreshToken));
    }
}
