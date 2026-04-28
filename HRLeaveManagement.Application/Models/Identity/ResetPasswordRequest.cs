using System.ComponentModel.DataAnnotations;

namespace HRLeaveManagement.Application.Models.Identity
{
    public class ResetPasswordRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required, MinLength(6)]
        public string NewPassword { get; set; }
    }
}
