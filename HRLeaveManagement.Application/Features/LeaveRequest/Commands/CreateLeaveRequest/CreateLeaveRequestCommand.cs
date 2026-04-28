using HRLeaveManagement.Application.Features.LeaveRequest.Shared;
using MediatR;

namespace HRLeaveManagement.Application.Features.LeaveRequest.Commands.CreateLeaveRequest
{
    public class CreateLeaveRequestCommand : BaseLeaveRequest , IRequest<Unit>
    {
        public string RequestComments { get; set; } = string.Empty;
    }
}
