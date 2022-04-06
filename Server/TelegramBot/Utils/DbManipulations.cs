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
                OrganizationId = staff.OrganizationId,
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
                Patronymic = " ",
                UserTypeId = 2,
                PhoneNum = value.Phone,
                Password = "1",
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
                .Where(x => x.UserTypeId == 1 && x.Surname == snp[0] && x.Name == snp[1] && x.Patronymic == snp[2])
                .Select(x => new viStaffOrganizationId { StaffId = x.Id, OrganizationId = x.OrganizationId})
                .FirstAsync();
            return stuff;
        }

        public static async Task<List<IdValue>> GetStaffByCategory(MyDbContext db, string category)
        {
            return await db.tbUsers
                         .AsNoTracking()
                         .Include(i => i.Category)
                         .Where(x => x.UserTypeId == 1 && x.Category.NameUz == category)
                         .Select(x => new IdValue { Id = x.Id, Name = $"{x.Surname} {x.Name} {x.Patronymic}" })
                         .ToListAsync();
        }
        public static async Task<bool> CheckStaffByCategory(MyDbContext db, string category, string value)
        {
            var list = await db.tbUsers
                         .AsNoTracking()
                         .Include(i => i.Category)
                         .Where(x => x.UserTypeId == 1 && x.Category.NameUz == category)
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
        public static async Task<List<string>> CheckServices(MyDbContext db)
        {
            return await db.spGlobalCategories
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
        public static async Task<int> GetGlobalCategoryIdByName(MyDbContext db, string name)
        {
            var global_category = await db.spGlobalCategories.AsNoTracking().FirstAsync(x => x.NameUz == name);
            return global_category.Id;
        }
        public static async Task<List<IdValue>> GetCategoriesByType(MyDbContext db, string value)
        {
            int globalCategory = await GetGlobalCategoryIdByName(db, value);
            return await db.spCategories.AsNoTracking()
                .Where(x => x.GlobalCategoryId == globalCategory)
                .Select(x => new IdValue { Id = x.Id, Name = x.NameUz })
                .ToListAsync();
        }
        public static async Task<List<IdValue>> GetAllGlobalCats(MyDbContext db)
        {
            return await db.spGlobalCategories.AsNoTracking().Select(x => new IdValue { Id = x.Id, Name = x.NameUz}).ToListAsync();
        }
        public static async Task<long> GetGroupId(MyDbContext db)
        {
            var chat = await db.tbOrganizations
                .AsNoTracking()
                .FirstAsync(x => x.TypeId == 1);
            return chat.Id;
        }
    }
}
