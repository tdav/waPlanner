using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using waPlanner.ModelViews;

namespace waPlanner.Services
{
    public interface IStaffService
    {
        Task<List<viUser>> GetStaffById(int organization_id);
    }
    public class StaffService: IStaffService
    {
        private readonly MyDbContext db;
        public StaffService(MyDbContext db)
        {
            this.db = db;
        }
        public async Task<List<viUser>> GetStaffById(int organization_id)
        {
            return await db.tbUsers
                .Include(s => s.Organization)
                .Include(s => s.Category)
                .Include(s => s.UserType)
                .Where(s => s.UserTypeId == 1 && s.OrganizationId == organization_id)
                .Select(x=>new viUser 
                {
                    Id = x.Id,
                    Name = x.Name,
                    Surname = x.Surname,
                    BirthDay = x.BirthDay,
                    PhoneNum = x.PhoneNum,
                    Patronymic = x.Patronymic,
                    TelegramId = x.TelegramId,
                    Online = x.Online,
                    Availability = x.Availability,
                    Experience = x.Experience,
                    OrganizationId = x.OrganizationId,
                    Organization = x.Organization.Name,
                    CategoryId = x.CategoryId,
                    Category = x.Category.NameUz,
                    UserTypeId=x.UserTypeId,
                    UserType=x.UserType.NameUz,

                }
                ).ToListAsync();
            
        }
    }
}
