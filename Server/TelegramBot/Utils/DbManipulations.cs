﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;
using waPlanner.ModelViews;
using System;

namespace waPlanner.TelegramBot.Utils
{
    public static class DbManipulations
    {
        public static async Task<List<IdValue>> SendFavorites(MyDbContext db, long chat_id)
        {
            int user_id = await GetUserId(chat_id, db);
            if (user_id != 0)
            {
                return await db.tbFavorites
                .AsNoTracking()
                .Where(f => f.UserId == user_id)
                .Select(f => new IdValue
                {
                    Id = f.StaffId,
                    Name = $"{f.Staff.Surname} {f.Staff.Name} {f.Staff.Patronymic}"
                })
                .ToListAsync();
            }
            return null;
        }

        public static async Task<List<IdValue>> SendSpecializations(MyDbContext db)
        {
            return await db.spSpecializations
                .AsNoTracking()
                .Where(x => x.Status == 1)
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
                .Where(x => x.SpecializationId == spec_id && x.Status == 1)
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
                .Where(x => x.OrganizationId == organization_id && x.Status == 1)
                .Select(x => new IdValue { Id = x.Id, Name = x.NameUz })
                .ToListAsync();
        }
        public static async Task RegistrateUserPlanner(long chat_id, TelegramBotValuesModel value, MyDbContext db)
        {
            viStaffInfo staff = await GetStaffInfoByName(db, value.Staff);
            int userId = await GetUserId(chat_id, db);
            string[] userSelectedTime = value.Time.Split(":");
            DateTime plannerDate = value.Calendar
                .AddHours(int.Parse(userSelectedTime[0]))
                .AddMinutes(int.Parse(userSelectedTime[1]));

            var planner = new tbScheduler
            {
                UserId = userId,
                StaffId = staff.StaffId.Value,
                AppointmentDateTime = plannerDate,
                CategoryId = staff.CategoryId.Value,
                OrganizationId = staff.OrganizationId.Value,
                CreateDate = DateTime.Now,
                Status = 1,
                CreateUser = 1
            };

            await db.tbSchedulers.AddAsync(planner);
            await db.SaveChangesAsync();
        }

        public static async Task<int> GetUserId(long chat_id, MyDbContext db)
        {
            var user_id = await db.tbUsers.AsNoTracking().FirstOrDefaultAsync(x => x.TelegramId == chat_id);
            if(user_id is not null)
                return user_id.Id;
            return 0;
        }

        public static async Task FinishProcessAsync(long chat_id, TelegramBotValuesModel value, MyDbContext db)
        {
            var telegramUser = new tbUser
            {
                TelegramId = chat_id,
                Surname = "TelegramUser",
                Name = value.UserName,
                Patronymic = "",
                PhoneNum = value.Phone,
                CreateDate = DateTime.Now,
                Status = 1
            };  
            await db.tbUsers.AddAsync(telegramUser);
            await db.SaveChangesAsync();
        }

        public static async Task<bool> CheckUser(long chat_id, MyDbContext db)
        {
            var result = await db.tbUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TelegramId == chat_id);

            if (result != null) return true;
            return false;
        }

        public static async Task<viStaffInfo> GetStaffInfoByName(MyDbContext db, string name)
        {            
            var snp = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var stuff = await db.tbStaffs
                .AsNoTracking()
                .Where(x => x.Surname == snp[0] && x.Name == snp[1] && x.Patronymic == snp[2])
                .Select(x => new viStaffInfo
                {
                    Staff = $"{x.Surname} {x.Name} {x.Patronymic}",
                    StaffId = x.Id,
                    Organization = x.Organization.Name,
                    OrganizationId = x.OrganizationId,
                    Category = x.Category.NameUz,
                    CategoryId = x.CategoryId,
                    Specialization = x.Organization.Specialization.NameUz
                })
                .FirstAsync();
            return stuff;
        }

        public static async Task<List<IdValue>> GetStaffByCategory(MyDbContext db, string category)
        {
            return await db.tbStaffs
                         .AsNoTracking()
                         .Include(i => i.Category)
                         .Where(x => x.RoleId == (int)UserRoles.STAFF && x.Category.NameUz == category && x.Status == 1)
                         .Select(x => new IdValue { Id = x.Id, Name = $"{x.Surname} {x.Name} {x.Patronymic}" })
                         .ToListAsync();
        }

        public static async Task<bool> CheckStaffByCategory(MyDbContext db, string category, string value)
        {
            var list = await db.tbStaffs
                         .AsNoTracking()
                         .Include(i => i.Category)
                         .Where(x => x.RoleId == (int)UserRoles.STAFF && x.Category.NameUz == category)
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
            var staff = await GetStaffInfoByName(db, value.Staff);
            return await db.tbSchedulers
                .AsNoTracking()
                .Where(x => x.StaffId == staff.StaffId && x.AppointmentDateTime.Date == value.Calendar.Date)
                .Select(x => x.AppointmentDateTime)
                .ToListAsync();
            
        }

        public static async Task<int> CheckFreeDay(MyDbContext db, string staff_name, DateTime date)
        {
            var staff = await GetStaffInfoByName(db, staff_name);
            return await db.tbSchedulers
                .AsNoTracking()
                .Where(x => x.StaffId == staff.StaffId && x.AppointmentDateTime.Date == date.Date)
                .CountAsync();
        }

        public static async Task<int[]> CheckStaffAvailability(MyDbContext db, string staff_name)
        {
            var staff = await GetStaffInfoByName(db, staff_name);
            return await db.tbStaffs
                .AsNoTracking()
                .Where(x => x.Id == staff.StaffId && x.Status == 1)
                .Select(x => x.Availability)
                .FirstOrDefaultAsync();
        }

        public static async Task AddToFavorites(MyDbContext db, TelegramBotValuesModel value, long chat_id)
        {
            var staff = await GetStaffInfoByName(db, value.Staff);
            var user = await GetUserId(chat_id, db);
            var addFavorite = new tbFavorites();
            addFavorite.StaffId = staff.StaffId.Value;
            addFavorite.UserId = user;
            addFavorite.TelegramId = chat_id;
            addFavorite.OrganizationId = staff.OrganizationId.Value;
            addFavorite.Status = 1;
            addFavorite.CreateDate = DateTime.Now;

            await db.tbFavorites.AddAsync(addFavorite);
            await db.SaveChangesAsync();
            
        }

        public static async Task<bool> CheckFavorites(MyDbContext db, string value, long chat_id)
        {
            var staff = await GetStaffInfoByName(db, value);
            var user = await GetUserId(chat_id, db);
            var result = await db.tbFavorites
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.StaffId == staff.StaffId && x.UserId == user);

            if (result is null)
                return false;
            return true;
        }

        public static async Task UpdateUserName(MyDbContext db, long chat_id, string name)
        {
            var user_id = await GetUserId(chat_id, db);
            var user = new tbUser() { Id = user_id, Name = name };
            db.tbUsers.Attach(user);
            db.Entry(user).Property(x => x.Name).IsModified = true;
            await db.SaveChangesAsync();
        }
        public static async Task UpdateUserPhone(MyDbContext db, long chat_id, string phone)
        {
            var user_id = await GetUserId(chat_id, db);
            var user = new tbUser() { Id = user_id, PhoneNum = phone };
            db.tbUsers.Attach(user);
            db.Entry(user).Property(x => x.PhoneNum).IsModified = true;
            await db.SaveChangesAsync();
        }

        public

        public static async Task<long> GetGroupId(MyDbContext db, string org_name)
        {
            var chat = await db.spOrganizations
                .AsNoTracking()
                .FirstAsync(x => x.Name == org_name);
            return chat.ChatId;
        }
    }
}
