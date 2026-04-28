namespace HRLeaveManagement.Identity.Models
{
    public class UserRefreshToken
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }           // الـ refresh token نفسه
        public string JwtId { get; set; }           // Jti بتاع الـ JWT المرتبط بيه
        public bool IsRevoked { get; set; } = false;
        public bool IsUsed { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }

        public ApplicationUser User { get; set; }
    }
}
