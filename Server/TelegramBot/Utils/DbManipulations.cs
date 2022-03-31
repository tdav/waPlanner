using Arch.EntityFrameworkCore;
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
            int stuffId = GetStuffIdByNameAsync(db, value.Stuff);
            int categoryId = GetCategoryIdByName(db, value.Category);
            int userId = GetUserId(chat_id, db);
            string[] userSelectedTime = value.Time.Split(":");
            DateTime plannerDate = value.Calendar
                .AddHours(int.Parse(userSelectedTime[0]))
                .AddMinutes(int.Parse(userSelectedTime[1]));

            var planner = new tbScheduler
            {
                UserId = userId,
                DoctorId = stuffId,
                AppointmentDateTime = plannerDate,
                CategoryId = categoryId,
            };

            await db.tbSchedulers.AddAsync(planner);
            await db.SaveChangesAsync();
        }
        private static int GetUserId(long chat_id, MyDbContext db)
        {
            return db.tbUsers.AsNoTracking().First(x => x.TelegramId == chat_id).Id;
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
                CreateDate = DateTime.Now
            };
            await db.tbUsers.AddAsync(telegramUser);
            await db.SaveChangesAsync();
        }
        public static bool CheckUser(long chat_id, MyDbContext db)
        {
            var result = db.tbUsers
                .AsNoTracking()
                .FirstOrDefault(x => x.TelegramId == chat_id);
            if (result != null)
                return true;
            return false;
        }
        public static int GetCategoryIdByName(MyDbContext db, string name)
        {
            // todo async qarap ko'rish kk
            var category = db.spCategories
                .AsNoTracking()
                .First(x => x.NameUz == name);
            
            return category.Id;
        }
        public static int GetStuffIdByNameAsync(MyDbContext db, string name)
        {
            var snp = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var stuff = db.tbUsers
                .AsNoTracking()
                .First(x => x.UserTypeId == 1 && x.Surname == snp[0] && x.Name == snp[1] && x.Patronymic == snp[2]);
            return stuff.Id;
        }
        public static List<IdValue> GetStuffByCategory(MyDbContext db, string category)
        {
            return db.tbUsers
                         .AsNoTracking()
                         .Include(i => i.Category)
                         .Where(x => x.UserTypeId == 1 && x.Category.NameUz == category)
                         .Select(x => new IdValue { Id = x.Id, Name = $"{x.Surname} {x.Name} {x.Patronymic}" })
                         .ToList();
        }
        public static bool CheckStuffByCategory(MyDbContext db, string category, string value)
        {
            var list = db.tbUsers
                         .AsNoTracking()
                         .Include(i => i.Category)
                         .Where(x => x.UserTypeId == 1 && x.Category.NameUz == category)
                         .Select(x => new IdValue { Id = x.Id, Name = $"{x.Surname} {x.Name} {x.Patronymic}" })
                         .ToList();
            return list.Any(x => x.Name == value);
        }
        public static List<IdValue> GetAllCategories(MyDbContext db)
        {
            return db.spCategories.AsNoTracking().Select(x => new IdValue { Id = x.Id, Name = x.NameUz }).ToList();
        }
    }
}
