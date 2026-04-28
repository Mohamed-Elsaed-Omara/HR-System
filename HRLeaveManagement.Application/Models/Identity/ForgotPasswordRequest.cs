using System.ComponentModel.DataAnnotations;

namespace HRLeaveManagement.Application.Models.Identity
{
    public class ForgotPasswordRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
