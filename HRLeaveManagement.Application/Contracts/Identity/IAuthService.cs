using HRLeaveManagement.Application.Models.Identity;

namespace HRLeaveManagement.Application.Contracts.Identity
{
    public interface IAuthService
    {
        Task<AuthResponse> Login(AuthRequest request);
        Task<RegistrationResponse> Register(RegistrationRequest request);
        Task<AuthResponse> RefreshToken(RefreshTokenRequest request);
        Task<BaseCommandResponse> RevokeToken(string refreshToken);
        Task<BaseCommandResponse> Logout(string userId);
        Task<BaseCommandResponse> ForgotPassword(ForgotPasswordRequest request);
        Task<BaseCommandResponse> ResetPassword(ResetPasswordRequest request);
        Task<BaseCommandResponse> ConfirmEmail(ConfirmEmailRequest request);
    }
}
