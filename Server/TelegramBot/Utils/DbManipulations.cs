using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;
using waPlanner.ModelViews;
using System;

namespace waPlanner.TelegramBot.Utils
{
    public class DbManipulations
    {
        public static async Task<List<IdValue>> SendSpecializations(MyDbContext db)
        {
            return await db.spSpecializations
                .AsNoTracking()
                .Select(x => new IdValue { Id = x.Id, Name = x.NameUz})
                .ToListAsync();
        }

        public static async Task<int> GetSpecializationIdByName(MyDbContext db, string spec_name)
        {
            var spec_id = await db.spSpecializations
                .AsNoTracking()
                .Where(x => x.NameUz == spec_name)
                .FirstAsync();
            return spec_id.Id;
        }

        public static async Task<List<IdValue>> SendOrganizations(MyDbContext db, string spec_name)
        {
            int spec_id = await GetSpecializationIdByName(db, spec_name);
            return await db.spOrganizations
                .AsNoTracking()
                .Where(x => x.SpecializationId == spec_id)
                .Select(x => new IdValue { Id = x.Id, Name = x.Name })
                .ToListAsync();
        }

        public static async Task<int> GetOrganizationId(MyDbContext db, string organization_name)
        {
            var organization = await db.spOrganizations
                .AsNoTracking()
                .FirstAsync(x => x.Name == organization_name);
            return organization.Id;
        }

        public static async Task<List<IdValue>> SendCategoriesByOrgName(MyDbContext db, string organization_name)
        {
            int organization_id = await GetOrganizationId(db, organization_name);
            return await db.spCategories
                .AsNoTracking()
                .Where(x => x.OrganizationId == organization_id)
                .Select(x => new IdValue { Id = x.Id, Name = x.NameUz })
                .ToListAsync();
        }
        public static async Task RegistrateUserPlanner(long chat_id, TelegramBotValuesModel value, MyDbContext db)
        {
            viStaffOrganizationId staff = await GetStaffIdByNameAsync(db, value.Stuff);
            int categoryId = await GetCategoryIdByName(db, value.Category);
            int userId = await GetUserId(chat_id, db);
            string[] userSelectedTime = value.Time.Split(":");
            DateTime plannerDate = value.Calendar
                .AddHours(int.Parse(userSelectedTime[0]))
                .AddMinutes(int.Parse(userSelectedTime[1]));

            var planner = new tbScheduler
            {
                UserId = userId,
                DoctorId = staff.StaffId,
                AppointmentDateTime = plannerDate,
                CategoryId = categoryId,
                OrganizationId = staff.OrganizationId.Value,
                CreateDate = DateTime.Now,
                Status = 1,
                CreateUser = 1
            };

            await db.tbSchedulers.AddAsync(planner);
            await db.SaveChangesAsync();
        }

        private static async Task<int> GetUserId(long chat_id, MyDbContext db)
        {
            var user_id = await db.tbUsers.AsNoTracking().FirstAsync(x => x.TelegramId == chat_id);
            return user_id.Id;
        }

        public static async Task FinishProcessAsync(long chat_id, TelegramBotValuesModel value, MyDbContext db)
        {
            var telegramUser = new tbUser
            {
                TelegramId = chat_id,
                Surname = "TelegramUser",
                Name = value.UserName,
                Patronymic = "",
                UserTypeId = (int)UserTypes.TELEGRAM_USER,
                PhoneNum = value.Phone,
                Password = "123456",
                CreateDate = DateTime.Now,
                Status=1
            };  
            await db.tbUsers.AddAsync(telegramUser);
            await db.SaveChangesAsync();
        }

        public static async Task<bool> CheckUser(long chat_id, MyDbContext db)
        {
            var result = await db.tbUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TelegramId == chat_id);

            if (result != null)                return true;
            return false;
        }
        public static async Task<int> GetCategoryIdByName(MyDbContext db, string name)
        {
            var category = await db.spCategories
                .AsNoTracking()
                .FirstAsync(x => x.NameUz == name);
            return category.Id;
        }

        public static async Task<viStaffOrganizationId> GetStaffIdByNameAsync(MyDbContext db, string name)
        {
            var snp = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var stuff = await db.tbUsers
                .AsNoTracking()
                .Where(x => x.UserTypeId == (int)UserTypes.STAFF && x.Surname == snp[0] && x.Name == snp[1] && x.Patronymic == snp[2])
                .Select(x => new viStaffOrganizationId { StaffId = x.Id, OrganizationId = x.OrganizationId})
                .FirstAsync();
            return stuff;
        }

        public static async Task<List<IdValue>> GetStaffByCategory(MyDbContext db, string category)
        {
            return await db.tbUsers
                         .AsNoTracking()
                         .Include(i => i.Category)
                         .Where(x => x.UserTypeId == (int)UserTypes.STAFF && x.Category.NameUz == category)
                         .Select(x => new IdValue { Id = x.Id, Name = $"{x.Surname} {x.Name} {x.Patronymic}" })
                         .ToListAsync();
        }

        public static async Task<bool> CheckStaffByCategory(MyDbContext db, string category, string value)
        {
            var list = await db.tbUsers
                         .AsNoTracking()
                         .Include(i => i.Category)
                         .Where(x => x.UserTypeId == (int)UserTypes.STAFF && x.Category.NameUz == category)
                         .Select(x => new IdValue { Id = x.Id, Name = $"{x.Surname} {x.Name} {x.Patronymic}" })
                         .ToListAsync();
            return list.Any(x => x.Name == value);
        }
        public static async Task<List<string>> CheckCategory(MyDbContext db)
        {
            return await db.spCategories
                .AsNoTracking()
                .Select(x => x.NameUz).ToListAsync();
        }

        public static async Task<List<DateTime>> GetStaffBusyTime(MyDbContext db, TelegramBotValuesModel value)
        {
            var staff = await GetStaffIdByNameAsync(db, value.Stuff);
            return await db.tbSchedulers
                .AsNoTracking()
                .Where(x => x.DoctorId == staff.StaffId && x.AppointmentDateTime.Date == value.Calendar.Date)
                .Select(x => x.AppointmentDateTime)
                .ToListAsync();
            
        }

        //public static async Task<long> GetGroupId(MyDbContext db)
        //{
        //    var chat = await db.spOrganizations
        //        .AsNoTracking()
        //        .FirstAsync(x => x.TypeId == 1);
        //    return chat.Id;
        //}
    }
}
