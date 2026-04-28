using HRLeaveManagement.Application.Contracts.Email;
using HRLeaveManagement.Application.Contracts.Identity;
using HRLeaveManagement.Application.Contracts.Logging;
using HRLeaveManagement.Application.Exceptions;
using HRLeaveManagement.Application.Models.Email;
using HRLeaveManagement.Application.Models.Identity;
using HRLeaveManagement.Domain;
using HRLeaveManagement.Identity.DbContext;
using HRLeaveManagement.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HRLeaveManagement.Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IAppLogger<AuthService> _appLogger;
        private readonly HrLeaveManagementIdentityDbContext _context;
        private readonly JwtSettings _jwtSettings;
        public AuthService(UserManager<ApplicationUser> userManager,
                            SignInManager<ApplicationUser> signInManager,
                            IOptions<JwtSettings> jwtSettings,
                            IEmailSender emailSender,
                            IAppLogger<AuthService> appLogger,
                            HrLeaveManagementIdentityDbContext context) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _appLogger = appLogger;
            _context = context;
            _jwtSettings = jwtSettings.Value;
        }
        // ── Login ──────────────────────────────────────────
        public async Task<AuthResponse> Login(AuthRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) {
                throw new NotFoundException($"User with {request.Email} not found",request.Email);
            }
            if (!user.EmailConfirmed)
            {
                throw new BadRequestException("Email is not confirmed yet.");
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password,false);
            
            if (result.Succeeded == false) 
                throw new BadRequestException($"Credentials for '{request.Email} aren't vaild'.");

            JwtSecurityToken jwtSecurityToken = await GenerateToken(user);

            var jwtId = jwtSecurityToken.Id;  

            var refreshToken = await CreateRefreshTokenAsync(user.Id, jwtId);

            return new AuthResponse
            {
                Id = user.Id,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                RefreshToken = refreshToken.Token,
                Email = request.Email,
                UserName = user.UserName
            };

        }
        // ── RefreshToken ──────────────────────────────────────────
        public async Task<AuthResponse> RefreshToken(RefreshTokenRequest request)
        {
            var principal = GetPrincipalFromExpiredToken(request.AccessToken);
            var jwtId = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var userId = principal.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;

            if (jwtId == null || userId == null)
                throw new BadRequestException("Invalid access token.");

            var storedToken = await _context.UserRefreshTokens
                .FirstOrDefaultAsync(t => t.Token == request.RefreshToken);

            if (storedToken == null)
                throw new BadRequestException("Refresh token not found.");

            if (storedToken.IsRevoked)
                throw new BadRequestException("Refresh token has been revoked.");

            if (storedToken.IsUsed)
                throw new BadRequestException("Refresh token has already been used.");

            if (storedToken.ExpiresAt < DateTime.UtcNow)
                throw new BadRequestException("Refresh token has expired.");

            if (storedToken.JwtId != jwtId)
                throw new BadRequestException("Refresh token does not match the access token.");

            if (storedToken.UserId != userId)
                throw new BadRequestException("Refresh token does not belong to this user.");

            storedToken.IsUsed = true;
            storedToken.IsRevoked = true;
            _context.UserRefreshTokens.Update(storedToken);
            await _context.SaveChangesAsync();

            // 5. new Token
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new NotFoundException("User not found.", userId);

            var newJwtToken = await GenerateToken(user);
            var newRefreshToken = await CreateRefreshTokenAsync(user.Id, newJwtToken.Id);

            return new AuthResponse
            {
                Id = user.Id,
                Token = new JwtSecurityTokenHandler().WriteToken(newJwtToken),
                RefreshToken = newRefreshToken.Token,
                Email = user.Email,
                UserName = user.UserName
            };
        }
        // ── Revoke Token  ─────────────────
        public async Task<BaseCommandResponse> RevokeToken(string refreshToken)
        {
            var storedToken = await _context.UserRefreshTokens
                .FirstOrDefaultAsync(t => t.Token == refreshToken);

            if (storedToken == null)
                throw new BadRequestException("Refresh token not found.");

            if (storedToken.IsRevoked)
                return new BaseCommandResponse { Success = true, Message = "Token already revoked." };

            storedToken.IsRevoked = true;
            _context.UserRefreshTokens.Update(storedToken);
            await _context.SaveChangesAsync();

            return new BaseCommandResponse { Success = true, Message = "Token revoked successfully." };
        }

        // ── Private Helpers ───────────────────────────────────

        private async Task<UserRefreshToken> CreateRefreshTokenAsync(string userId, string jwtId)
        {
            var refreshToken = new UserRefreshToken
            {
                UserId = userId,
                JwtId = jwtId,
                Token = GenerateSecureToken(),
                IsRevoked = false,
                IsUsed = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays)
            };

            await _context.UserRefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }

        private static string GenerateSecureToken()
        {
            // 64 byte = 86 char base64 — cryptographically secure
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                                               Encoding.UTF8.GetBytes(_jwtSettings.Key)),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = false, 
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(
                token,
                tokenValidationParameters,
                out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                throw new BadRequestException("Invalid token algorithm.");
            }

            return principal;
        }
        // ── GenerateToken ──────────────────────────────────────────
        private async Task<JwtSecurityToken> GenerateToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = roles.Select(q => new Claim(ClaimTypes.Role, q)).ToList();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id),

            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(_jwtSettings.DurationInMinutes),
                    signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }
        // ── Register ──────────────────────────────────────────
        public async Task<RegistrationResponse> Register(RegistrationRequest request)
        {
            var user = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                EmailConfirmed = false 
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join("\n", result.Errors.Select(e => $"• {e.Description}"));
                throw new BadRequestException(errors);
            }

            await _userManager.AddToRoleAsync(user, "Employee");

            // إبعت confirmation email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var confirmUrl = $"https://localhost:7142/api/auth/confirm-email?userId={user.Id}&token={encodedToken}";

            try
            {
                var email = new EmailMessage
                {
                    To = user.Email,
                    Body = $"<p>Please confirm your email:</p><a href='{confirmUrl}'>Confirm Email</a>"
,
                    Subject = "Confirm Your Email"
                };

                await _emailSender.SendEmail(email);
            }
            catch (Exception ex)
            {
                _appLogger.LogWarning(ex.Message);
            }

            return new RegistrationResponse { UserId = user.Id };
        }
        // ── Logout ──────────────────────────────────────────
        public async Task<BaseCommandResponse> Logout(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found", userId);

            await _userManager.UpdateSecurityStampAsync(user);

            return new BaseCommandResponse { Success = true, Message = "Logged out successfully." };
        }
        // ── Forgot Password ─────────────────────────────────
        public async Task<BaseCommandResponse> ForgotPassword(ForgotPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                return new BaseCommandResponse
                {
                    Success = true,
                    Message = "If this email is registered, you will receive a password reset link."
                };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var resetUrl = $"https://localhost:7142/reset-password?email={user.Email}&token={encodedToken}";
            try
            {
                var email = new EmailMessage
                {
                    To = user.Email,
                    Body = $"<p>Click the link to reset your password:</p><a href='{resetUrl}'>Reset Password</a>",
                    Subject = "Reset Your Password"
                };

                await _emailSender.SendEmail(email);
            }
            catch (Exception ex)
            {
                _appLogger.LogWarning(ex.Message);
            }


            return new BaseCommandResponse
            {
                Success = true,
                Message = "If this email is registered, you will receive a password reset link."
            };
        }
        // ── Reset Password ──────────────────────────────────
        public async Task<BaseCommandResponse> ResetPassword(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new NotFoundException("User not found", request.Email);

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join("\n", result.Errors.Select(e => $"• {e.Description}"));
                throw new BadRequestException(errors);
            }

            return new BaseCommandResponse { Success = true, Message = "Password reset successfully." };
        }

        // ── Confirm Email ────────────────────────────────────
        public async Task<BaseCommandResponse> ConfirmEmail(ConfirmEmailRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                throw new NotFoundException("User not found", request.UserId);

            if (await _userManager.IsEmailConfirmedAsync(user))
                return new BaseCommandResponse { Success = true, Message = "Email already confirmed." };

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
            {
                var errors = string.Join("\n", result.Errors.Select(e => $"• {e.Description}"));
                throw new BadRequestException(errors);
            }

            return new BaseCommandResponse { Success = true, Message = "Email confirmed successfully." };
        }

    }
}
