using HRLeaveManagement.Application.Models.Email;

namespace HRLeaveManagement.Application.Contracts.Email
{
    public interface IEmailSender
    {
        Task<bool> SendEmail(EmailMessage email);
    }
}
