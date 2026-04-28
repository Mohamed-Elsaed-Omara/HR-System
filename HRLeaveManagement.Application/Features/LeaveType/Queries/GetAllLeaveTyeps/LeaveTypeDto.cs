namespace HRLeaveManagement.Application.Features.LeaveType.Queries.GetAllLeaveTyeps
{
    public class LeaveTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DefaultDays { get; set; }
    }
}
