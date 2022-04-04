using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;


namespace waPlanner.Services
{
    public interface IStaffService
    {
        Task<tbUser> GetStaffById(int id);
    }
    public class StaffService
    {
        private readonly MyDbContext db;
        public StaffService(MyDbContext db)
        {
            this.db = db;
        }
        public async Task<tbUser> GetStaffById(int id)
        {
            return await db.tbUsers.AsNoTracking().FirstAsync(x => x.UserTypeId == 1);
        }
    }
}
