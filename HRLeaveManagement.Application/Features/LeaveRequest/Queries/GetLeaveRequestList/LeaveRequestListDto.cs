using HRLeaveManagement.Application.Features.LeaveType.Queries.GetAllLeaveTyeps;
using HRLeaveManagement.Application.Models.Identity;

namespace HRLeaveManagement.Application.Features.LeaveRequest.Queries.GetLeaveRequestList
{
    public class LeaveRequestListDto
    {
        public int Id { get; set; }
        public Employee Employee { get; set; }
        public string RequestingEmployeeId { get; set; } 
        public LeaveTypeDto LeaveType { get; set; }
        public DateTime DateRequested { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool? Approved { get; set; }
        public bool Cancelled { get; set; }
    }
}
