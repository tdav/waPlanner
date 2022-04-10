using System.Threading.Tasks;
using waPlanner.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using waPlanner.ModelViews;
using waPlanner.Database.Models;
using waPlanner.TelegramBot;
using System;

namespace waPlanner.Services
{
    public interface IStaffService
    {
        Task<List<viStaff>> GetStaffByOrganizationId(int organization_id);
        Task AddStaffAsync(viStaff user);
        Task<List<IdValue>> GetStuffList(int organization_id);
        Task UpdateStaffStatus(int staff_id, byte status);
        Task UpdateStaff(int staff_id, viStaff staff);
        Task<viStaff> GetStaffById(int staff_id);
    }

    public class StaffService: IStaffService
    {
        private readonly MyDbContext db;
        public StaffService(MyDbContext db)
        {
            this.db = db;
        }

        public async Task<List<viStaff>> GetStaffByOrganizationId(int organization_id)
        {
            return await db.tbUsers
                .AsNoTracking()
                .Include(s => s.Organization)
                .Include(s => s.Category)
                .Include(s => s.UserType)
                .Where(s => s.UserTypeId == 1 && s.OrganizationId == organization_id)
                .Select(x=> new viStaff
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
                    UserTypeId = (int)UserTypes.STAFF,
                    UserType = x.UserType.NameUz,
                    Photo = x.Photo
                }
                ).ToListAsync();
        }

        public async Task AddStaffAsync(viStaff staff)
        {
            var newStaff = new tbUser
            {
                Surname = staff.Surname,
                Name = staff.Name,
                Patronymic = staff.Patronymic,
                PhoneNum = staff.PhoneNum,
                BirthDay = staff.BirthDay,
                Photo = staff.Photo,
                UserTypeId = (int)UserTypes.STAFF,
                CategoryId = staff.CategoryId,
                OrganizationId = staff.OrganizationId,
                TelegramId = staff.TelegramId,                
                Experience = staff.Experience,
                Availability = staff.Availability,
                CreateDate = DateTime.Now,
                CreateUser = 1,
                Password = "123456",
                Status = 1
            };
            await db.tbUsers.AddAsync(newStaff);
            await db.SaveChangesAsync();
        }

        public async Task<List<IdValue>> GetStuffList(int organization_id)
        {
            return await db.tbUsers
                           .AsNoTracking()
                           .Where(s => s.UserTypeId == (int)UserTypes.STAFF && s.OrganizationId == organization_id)
                           .Select(x => new IdValue
                           {
                               Id = x.Id,
                               Name = $"{x.Surname} {x.Name} {x.Patronymic}"
                           }
                           ).ToListAsync();
        }

        public async Task UpdateStaffStatus(int staff_id, byte status)
        {
            var sh = await db.tbUsers.FindAsync(staff_id);
            sh.Status = status;
            sh.UpdateUser = 1;
            sh.UpdateDate = DateTime.Now;
            await db.SaveChangesAsync();
        }

        public async Task UpdateStaff(int staff_id, viStaff staff)
        {
            var updateStaff = await db.tbUsers.FindAsync(staff_id);

            if (staff.BirthDay.HasValue)
                updateStaff.BirthDay = staff.BirthDay.Value;

            if (staff.CategoryId.HasValue)
                updateStaff.CategoryId = staff.CategoryId.Value;

            if(staff.OrganizationId.HasValue)
                updateStaff.OrganizationId = staff.OrganizationId.Value;

            if (staff.Experience.HasValue)
                updateStaff.Experience = staff.Experience.Value;

            if(staff.Status.HasValue)
                updateStaff.Status = staff.Status.Value;

            if(staff.Surname is not null)
                updateStaff.Surname = staff.Surname;

            if(staff.Name is not null)
                updateStaff.Name = staff.Name;

            if(staff.Patronymic is not null)
                updateStaff.Patronymic = staff.Patronymic;

            if(staff.Password is not null)
                updateStaff.Password = staff.Password;

            if(staff.PhoneNum is not null)
                updateStaff.PhoneNum = staff.PhoneNum;

            updateStaff.UserTypeId = (int)UserTypes.STAFF;
            updateStaff.UpdateDate = DateTime.Now;
            updateStaff.UpdateUser = 1;
            await db.SaveChangesAsync();
        }
        public async Task<viStaff> GetStaffById(int staff_id)
        {
            return await db.tbUsers
                .AsNoTracking()
                .Include(s => s.Organization)
                .Include(s => s.Category)
                .Include(s => s.UserType)
                .Where(s => s.UserTypeId == 1 && s.Id == staff_id)
                .Select(x => new viStaff
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
                    UserTypeId = (int)UserTypes.STAFF,
                    UserType = x.UserType.NameUz,
                    Photo = x.Photo
                }
                ).FirstOrDefaultAsync();
        }
    }
}
