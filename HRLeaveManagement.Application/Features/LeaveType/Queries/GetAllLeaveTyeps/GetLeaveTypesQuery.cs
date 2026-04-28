using MediatR;

namespace HRLeaveManagement.Application.Features.LeaveType.Queries.GetAllLeaveTyeps
{
    public record GetLeaveTypesQuery : IRequest<List<LeaveTypeDto>>;
}
