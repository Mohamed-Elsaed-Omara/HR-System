using FluentValidation;
using HRLeaveManagement.Application.Contracts.Persistence;
using HRLeaveManagement.Application.Features.LeaveRequest.Shared;

namespace HRLeaveManagement.Application.Features.LeaveRequest.Commands.CreateLeaveRequest
{
    public class CreateLeaveRequestCommandValidator : AbstractValidator<CreateLeaveRequestCommand>
    {

        public CreateLeaveRequestCommandValidator(ILeaveTypeRepository leaveTypeRepository) 
        {
            Include(new BaseLeaveRequestValidator(leaveTypeRepository));
        }
    }
}
