using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;
using waPlanner.ModelViews;

namespace waPlanner.TelegramBot.Utils
{
    public interface IDbManipulations
    {
        Task<List<IdValue>> SendFavorites(long chatId);
        Task<List<IdValue>> SendSpecializations(string lg);
        Task<int> GetSpecializationIdByName(string spec_name);
        Task<List<IdValue>> SendOrganizations(string spec_name);
        Task<int> GetOrganizationId(string organization_name);
        Task<List<IdValue>> SendCategoriesByOrgName(string organization_name, string lg);
        Task RegistrateUserPlanner(long chat_id, TelegramBotValuesModel value);
        Task<viSetup> GetUserLang(long chat_id);
        Task<int> GetUserId(long chat_id);
        Task FinishProcessAsync(long chat_id, TelegramBotValuesModel value);
        Task<bool> CheckUser(long chat_id);
        Task<viStaffInfo> GetStaffInfoByName(string name);
        Task<List<IdValue>> GetStaffByCategory(string category);
        Task<bool> CheckStaffByCategory(string category, string value);
        Task<List<string>> CheckCategory(string lg);
        Task<List<DateTime>> GetStaffBusyTime(TelegramBotValuesModel value);
        Task<int[]> CheckStaffAvailability(string staff_name);
        Task AddToFavorites(TelegramBotValuesModel value, long chat_id);
        Task<bool> CheckFavorites(string value, long chat_id);
        Task UpdateUserName(long chat_id, string name);
        Task UpdateUserPhone(long chat_id, string phone);
        Task UpdateUserLang(long chat_id, string lg);
        Task<long> GetOrganizationGroupId(string org_name);
        Task<viTelegramUser> GetUserInfo(long chat_id);
        Task<viOrgTimes> GetOrganizationBreak(string org_name);
        Task<viOrgTimes> GetOrgWorkTime(string org_name);
        Task<viTelegramCommonStatistic> GetCommonStatistic();
        Task<List<viTelegramOrgsStatistic>> GetOrgsStatistics();
        Task<bool> CheckGovermentOrg(string spec);
        Task<bool> CheckUserPINFL(string pinfl);
        Task<bool> CheckUserPassportSeria(string seria);
        Task<string> GetOrgMessage(string organization, string lg);
    }

    public class DbManipulations : IDbManipulations
    {
        private readonly MyDbContext db;
        private readonly IMemoryCache cache;

        public DbManipulations(MyDbContext db, IMemoryCache cache)
        {
            this.db = db;
            this.cache = cache;
        }

        public async Task<List<IdValue>> SendFavorites(long chat_id)
        {
            int user_id = await GetUserId(chat_id);
            if (user_id != 0)
            {
                return await db.tbFavorites
                .Include(x => x.Organization)
                .AsNoTracking()
                .Where(f => f.UserId == user_id)
                .Select(f => new IdValue
                {
                    Id = f.StaffId,
                    Name = $"({f.Organization.Name}) {f.Staff.Surname} {f.Staff.Name} {f.Staff.Patronymic}"
                })
                .ToListAsync();
            }
            return null;
        }

        public async Task<List<IdValue>> SendSpecializations(string lg)
        {
            switch (lg)
            {
                case "ru":
                    {
                        return await db.spSpecializations
                            .AsNoTracking()
                            .Where(x => x.Status == 1)
                            .Select(x => new IdValue { Id = x.Id, Name = x.NameRu })
                            .ToListAsync();
                    }
                case "uz":
                    {

                        return await db.spSpecializations
                            .AsNoTracking()
                            .Where(x => x.Status == 1)
                            .Select(x => new IdValue { Id = x.Id, Name = x.NameUz })
                            .ToListAsync();
                    }
                default:
                    return await db.spSpecializations
                            .AsNoTracking()
                            .Where(x => x.Status == 1)
                            .Select(x => new IdValue { Id = x.Id, Name = x.NameLt })
                            .ToListAsync();
            }
        }

        public async Task<int> GetSpecializationIdByName(string spec_name)
        {
            var spec_id = await db.spSpecializations
                .AsNoTracking()
                .Where(x => x.NameUz == spec_name || x.NameRu == spec_name || x.NameLt == spec_name)
                .FirstAsync();
            return spec_id.Id;
        }

        public async Task<List<IdValue>> SendOrganizations(string spec_name)
        {
            int spec_id = await GetSpecializationIdByName(spec_name);
            return await db.spOrganizations
                .AsNoTracking()
                .Where(x => x.SpecializationId == spec_id && x.Status == 1)
                .Select(x => new IdValue { Id = x.Id, Name = x.Name })
                .ToListAsync();
        }

        public async Task<int> GetOrganizationId(string organization_name)
        {
            var organization = await db.spOrganizations
                .AsNoTracking()
                .FirstAsync(x => x.Name == organization_name);
            return organization.Id;
        }

        public async Task<List<IdValue>> SendCategoriesByOrgName(string organization_name, string lg)
        {
            int organization_id = await GetOrganizationId(organization_name);
            switch (lg)
            {
                case "ru":
                    {
                        return await db.spCategories
                            .AsNoTracking()
                            .Where(x => x.OrganizationId == organization_id && x.Status == 1)
                            .Select(x => new IdValue { Id = x.Id, Name = x.NameRu })
                            .ToListAsync();
                    }
                case "uz":
                    {
                        return await db.spCategories
                                .AsNoTracking()
                                .Where(x => x.OrganizationId == organization_id && x.Status == 1)
                                .Select(x => new IdValue { Id = x.Id, Name = x.NameUz })
                                .ToListAsync();
                    }
                default:
                    {
                        return await db.spCategories
                                .AsNoTracking()
                                .Where(x => x.OrganizationId == organization_id && x.Status == 1)
                                .Select(x => new IdValue { Id = x.Id, Name = x.NameLt })
                                .ToListAsync();
                    }
            }
        }
        public async Task RegistrateUserPlanner(long chat_id, TelegramBotValuesModel value)
        {
            viStaffInfo staff = await GetStaffInfoByName(value.Staff);
            int userId = await GetUserId(chat_id);
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

        public async Task RegisterUserSettings(long chat_id, string lg)
        {
            int user_id = await GetUserId(chat_id);
            var setting = new viSetup { Lang = lg, Theme = "white" };

            var setup = new spSetup
            {
                UserId = user_id,
                Text = Newtonsoft.Json.JsonConvert.SerializeObject(setting),
                CreateDate = DateTime.Now,
                CreateUser = user_id,
                Status = 1
            };
            await db.spSetups.AddAsync(setup);
            await db.SaveChangesAsync();
        }

        public async Task<viSetup> GetUserLang(long chat_id)
        {
            int user_id = await GetUserId(chat_id);
            if (user_id != 0)
            {
                var setup = await db.spSetups
                   .AsNoTracking()
                   .FirstOrDefaultAsync(x => x.UserId == user_id);
                viSetup a = JsonSerializer.Deserialize<viSetup>(setup.Text);
                return a;
            }
            return null;
        }

        public async Task<int> GetUserId(long chat_id)
        {
            if (cache.TryGetValue($"GETUSERID-{chat_id}", out int value))
            {
                return value;
            }
            else
            {
                var user = await db.tbUsers.AsNoTracking().FirstOrDefaultAsync(x => x.TelegramId == chat_id);
                if (user is not null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions();
                    cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));

                    cache.Set($"GETUSERID-{chat_id}", user.Id, cacheEntryOptions);
                    return user.Id;
                }
                else
                    return 0;
            }
        }

        public async Task FinishProcessAsync(long chat_id, TelegramBotValuesModel value)
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
            await RegisterUserSettings(chat_id, value.Lang);
        }

        public async Task<bool> CheckUser(long chat_id)
        {
            var result = await db.tbUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TelegramId == chat_id);

            if (result != null) return true;
            return false;
        }

        public async Task<viStaffInfo> GetStaffInfoByName(string name)
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

        public async Task<List<IdValue>> GetStaffByCategory(string category)
        {
            return await db.tbStaffs
                         .AsNoTracking()
                         .Include(i => i.Category)
                         .Where(x => x.RoleId == (int)UserRoles.STAFF && x.Status == 1 && x.Online == true &&
                          (x.Category.NameUz == category || x.Category.NameRu == category || x.Category.NameLt == category))
                         .Select(x => new IdValue { Id = x.Id, Name = $"{x.Surname} {x.Name} {x.Patronymic}" })
                         .ToListAsync();
        }

        public async Task<bool> CheckStaffByCategory(string category, string value)
        {
            var list = await db.tbStaffs
                         .AsNoTracking()
                         .Include(i => i.Category)
                         .Where(x => x.RoleId == (int)UserRoles.STAFF &&
                         (x.Category.NameUz == category || x.Category.NameRu == category || x.Category.NameLt == category))
                         .Select(x => new IdValue { Id = x.Id, Name = $"{x.Surname} {x.Name} {x.Patronymic}" })
                         .ToListAsync();
            return list.Any(x => x.Name == value);
        }
        public async Task<List<string>> CheckCategory(string lg)
        {
            switch (lg)
            {
                case "ru":
                    {
                        return await db.spCategories
                            .AsNoTracking()
                            .Select(x => x.NameRu)
                            .ToListAsync();
                    }
                case "uz":
                    {
                        return await db.spCategories
                            .AsNoTracking()
                            .Select(x => x.NameUz)
                            .ToListAsync();
                    }
                default:
                    {
                        return await db.spCategories
                            .AsNoTracking()
                            .Select(x => x.NameLt)
                            .ToListAsync();
                    }
            }
        }

        public async Task<List<DateTime>> GetStaffBusyTime(TelegramBotValuesModel value)
        {
            var staff = await GetStaffInfoByName(value.Staff);
            return await db.tbSchedulers
                .AsNoTracking()
                .Where(x => x.StaffId == staff.StaffId && x.AppointmentDateTime.Date == value.Calendar.Date)
                .Select(x => x.AppointmentDateTime)
                .ToListAsync();
        }

        public async Task<int[]> CheckStaffAvailability(string staff_name)
        {
            var staff = await GetStaffInfoByName(staff_name);
            return await db.tbStaffs
                .AsNoTracking()
                .Where(x => x.Id == staff.StaffId && x.Status == 1)
                .Select(x => x.Availability)
                .FirstOrDefaultAsync();
        }

        public async Task AddToFavorites(TelegramBotValuesModel value, long chat_id)
        {
            var staff = await GetStaffInfoByName(value.Staff);
            var user = await GetUserId(chat_id);
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

        public async Task<bool> CheckFavorites(string value, long chat_id)
        {
            var staff = await GetStaffInfoByName(value);
            var user = await GetUserId(chat_id);
            var result = await db.tbFavorites
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.StaffId == staff.StaffId && x.UserId == user);

            if (result is null)
                return false;
            return true;
        }

        public async Task UpdateUserName(long chat_id, string name)
        {
            var user_id = await GetUserId(chat_id);
            var user = new tbUser() { Id = user_id, Name = name };
            db.tbUsers.Attach(user);
            db.Entry(user).Property(x => x.Name).IsModified = true;
            await db.SaveChangesAsync();
        }
        public async Task UpdateUserPhone(long chat_id, string phone)
        {
            var user_id = await GetUserId(chat_id);
            var user = new tbUser() { Id = user_id, PhoneNum = phone };
            db.tbUsers.Attach(user);
            db.Entry(user).Property(x => x.PhoneNum).IsModified = true;
            await db.SaveChangesAsync();
        }

        public async Task UpdateUserLang(long chat_id, string lg)
        {
            var user_id = await GetUserId(chat_id);
            var setup = await db.spSetups
                .AsNoTracking()
                .Where(x => x.UserId == user_id)
                .FirstAsync();
            var sets = Newtonsoft.Json.JsonConvert.DeserializeObject<viSetup>(setup.Text);

            var setting = new viSetup { Lang = lg, Theme = sets.Theme };
            setup.Text = Newtonsoft.Json.JsonConvert.SerializeObject(setting);
            db.spSetups.Update(setup);
            await db.SaveChangesAsync();
        }

        public async Task<viTelegramUser> GetUserInfo(long chat_id)
        {
            return await db.tbUsers
                .AsNoTracking()
                .Where(x => x.TelegramId == chat_id)
                .Select(x => new viTelegramUser
                {
                    Name = x.Name,
                    Surname = x.Surname,
                    Phone = x.PhoneNum,
                    CreateDate = x.CreateDate
                })
                .FirstAsync();
        }

        public async Task<long> GetOrganizationGroupId(string org_name)
        {
            var chat = await db.spOrganizations
                .AsNoTracking()
                .FirstAsync(x => x.Name == org_name);
            return chat.ChatId;
        }

        public async Task<viOrgTimes> GetOrganizationBreak(string org_name)
        {
            return await db.spOrganizations
                .AsNoTracking()
                .Where(x => x.Name == org_name)
                .Select(x => new viOrgTimes
                {
                    Start = x.BreakTimeStart,
                    End = x.BreakTimeEnd
                })
                .FirstOrDefaultAsync();
        }

        public async Task<viOrgTimes> GetOrgWorkTime(string org_name)
        {
            var time = await db.spOrganizations
                .AsNoTracking()
                .Where(x => x.Name == org_name)
                .Select(x => new viOrgTimes
                {
                    Start = x.WorkStart,
                    End = x.WorkEnd
                })
                .FirstOrDefaultAsync();

            if (time.End.HasValue)
                return time;
            time.End = new DateTime(1970, 1, 1, 0, 0, 0);
            return time;
        }

        public async Task<viTelegramCommonStatistic> GetCommonStatistic()
        {
            int totalUsersCount = await db.tbUsers.AsNoTracking().CountAsync();
            int totalBooks = await db.tbSchedulers.AsNoTracking().CountAsync();
            return await db.tbSchedulers
                .AsNoTracking()
                .Include(x => x.Organization)
                .GroupBy(x => new { x.Organization.Name })
                .Select(x => new viTelegramCommonStatistic
                {
                    TotalUsersCount = totalUsersCount,
                    TotalBooks = totalBooks,
                })
                .FirstAsync();
        }

        public async Task<List<viTelegramOrgsStatistic>> GetOrgsStatistics()
        {
            return await db.tbSchedulers
                .AsNoTracking()
                .Include(x => x.Organization)
                .GroupBy(x => new { x.Organization.Name })
                .Select(x => new viTelegramOrgsStatistic
                {
                    Name = x.Key.Name,
                    Count = x.Count()
                })
                .ToListAsync();
        }

        public async Task<bool> CheckGovermentOrg(string spec)
        {
            int spec_id = await GetSpecializationIdByName(spec);
            var check = await db.spSpecializations
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == spec_id && (x.NameUz == "Davlat xizmatlari" || x.NameRu == "Госуслуги" || x.NameLt == "Госуслуги"));

            if (check is not null)
                return true;
            return false;
        }

        public async Task<bool> CheckUserPINFL(string pinfl)
        {
            var check = await db.tbUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.PINFL == pinfl);

            if (check is not null)
                return true;
            return false;
        }

        public async Task<bool> CheckUserPassportSeria(string seria)
        {
            var check = await db.tbUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.PassportSeria == seria);
            if (check is not null)
                return true;
            return false;
        }

        public async Task<string> GetOrgMessage(string organization, string lg)
        {
            var get_org = await db.spOrganizations
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.Name == organization);
            switch (lg)
            {
                case "ru":
                    {
                        return get_org.MessageRu;
                    }
                case "uz":
                    {
                        return get_org.MessageUz;
                    }
                default:
                    return get_org.MessageLt;
            }
        }
    }
}
