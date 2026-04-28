using HRLeaveManagement.Application.Contracts.Persistence;
using HRLeaveManagement.Domain;
using HRLeaveManagement.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace HRLeaveManagement.Persistence.Repositories
{
    public class LeaveTypeRepository : GenericRepository<LeaveType> , ILeaveTypeRepository
    {
        public LeaveTypeRepository(HrDatabaseContext context):base(context)
        {

        }
        public async Task<bool> IsLeaveTypeUnique(string name)
        {
            return await _context.leaveTypes.AnyAsync(q => q.Name == name);
        }

    }
}
