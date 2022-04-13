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
        Task AddStaffAsync(viStaff user, int organization_id);
        Task<List<IdValue>> GetStuffList(int organization_id);
        Task UpdateStaffStatus(viStaff staff, byte status);
        Task UpdateStaff(viStaff staff);
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
            return await db.tbStaffs
                .AsNoTracking()
                .Include(s => s.Organization)
                .Include(s => s.Category)
                .Where(s => s.OrganizationId == organization_id)
                .Select(x=> new viStaff
                {
                    Id = x.Id,
                    Name = x.Name,
                    Surname = x.Surname,
                    BirthDay = x.BirthDay,
                    PhoneNum = x.PhoneNum ?? "Нет данных",
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
                    Photo = x.Photo ?? "Нет данных",
                    Gender = x.Gender ?? "Нет данных"
                }
                ).ToListAsync();
        }

        public async Task AddStaffAsync(viStaff staff, int organization_id)
        {
            var newStaff = new tbStaff();

            newStaff.Surname = staff.Surname;
            newStaff.Name = staff.Name;
            newStaff.Patronymic = staff.Patronymic;
            newStaff.PhoneNum = staff.PhoneNum;
            newStaff.BirthDay = staff.BirthDay;
            newStaff.Photo = staff.Photo;
            newStaff.UserTypeId = (int)UserTypes.STAFF;
            newStaff.CategoryId = staff.CategoryId;

            if (staff.OrganizationId != 0)
                newStaff.OrganizationId = organization_id;
            else
                newStaff.OrganizationId = 1;
            newStaff.TelegramId = staff.TelegramId;
            newStaff.Experience = staff.Experience;
            newStaff.Availability = staff.Availability;
            newStaff.CreateDate = DateTime.Now;
            newStaff.Gender = staff.Gender;
            newStaff.CreateUser = 1;
            newStaff.Password = "123456";
            newStaff.Status = 1;

            await db.tbStaffs.AddAsync(newStaff);
            await db.SaveChangesAsync();
        }

        public async Task<List<IdValue>> GetStuffList(int organization_id)
        {
            return await db.tbStaffs
                           .AsNoTracking()
                           .Where(s => s.OrganizationId == organization_id)
                           .Select(x => new IdValue
                           {
                               Id = x.Id,
                               Name = $"{x.Surname} {x.Name} {x.Patronymic}"
                           }
                           ).ToListAsync();
        }

        public async Task UpdateStaffStatus(viStaff staff, byte status)
        {
            var sh = await db.tbUsers.FindAsync(staff.Id);
            sh.Status = status;
            sh.UpdateUser = 1;
            sh.UpdateDate = DateTime.Now;
            await db.SaveChangesAsync();
        }

        public async Task UpdateStaff(viStaff staff)
        {
            var updateStaff = await db.tbStaffs.FindAsync(staff.Id);

            if (staff.BirthDay.HasValue)
                updateStaff.BirthDay = staff.BirthDay.Value;

            if (staff.CategoryId.HasValue)
                updateStaff.CategoryId = staff.CategoryId.Value;

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

            if (staff.Gender is not null)
                updateStaff.Gender = staff.Gender;

            updateStaff.UserTypeId = (int)UserTypes.STAFF;
            updateStaff.UpdateDate = DateTime.Now;
            updateStaff.UpdateUser = 1;
            await db.SaveChangesAsync();
        }
        public async Task<viStaff> GetStaffById(int staff_id)
        {
            return await db.tbStaffs
                .AsNoTracking()
                .Include(s => s.Organization)
                .Include(s => s.Category)
                .Where(s => s.Id == staff_id)
                .Select(x => new viStaff
                {
                    Id = x.Id,
                    Name = x.Name,
                    Surname = x.Surname,
                    BirthDay = x.BirthDay,
                    PhoneNum = x.PhoneNum ?? "Нет данных",
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
                    Photo = x.Photo,
                    Gender = x.Gender ?? "Нет данных"
                }
                ).FirstOrDefaultAsync();
        }
    }
}
