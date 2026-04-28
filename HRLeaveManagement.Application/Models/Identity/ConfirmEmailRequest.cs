using System.ComponentModel.DataAnnotations;

namespace HRLeaveManagement.Application.Models.Identity
{
    public class ConfirmEmailRequest
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
