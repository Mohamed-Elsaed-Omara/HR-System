using System.ComponentModel.DataAnnotations;

namespace HRLeaveManagement.Application.Models.Identity
{
    public class RefreshTokenRequest
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
