using System.Threading.Tasks;
using waPlanner.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using waPlanner.ModelViews;
using waPlanner.Database.Models;
using waPlanner.TelegramBot;
using System;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using waPlanner.Utils;
using waPlanner.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using waPlanner.Interfaces;

namespace waPlanner.Services
{
    public interface IStaffService
    {
        Task<Answer<viStaff[]>> GetStaffsByOrganizationId();
        Task<Answer<viStaff>> AddStaffAsync(viStaff user);
        Task<Answer<List<IdValue>>> GetStuffList(int category_id);
        Task<AnswerBasic> SetStatusAsync(viSetStatus status);
        Task<Answer<viStaff>> UpdateStaff(viStaff staff);
        Task<Answer<viStaff>> GetStaffById(int staff_id);
        ValueTask<Answer<viStaff[]>> SearchStaffAsync(string name);
        ValueTask<Answer<string>> SetPhotoAsync(viSetPhoto staff);
        Task<AnswerBasic> SetActivity(int staff_id, bool online);
        //Task<viStaffAvailability> GetStaffAvailabilityAsync(int staff_id);
        ValueTask<Answer<TokenModel>> TokenAsync(LoginModel value);
        ValueTask<AnswerBasic> ChangePaswwordAsync(ChangePasswordModel value);
        ValueTask<Answer<IdValue>> OnForgotPassword(string PhoneNum);
    }

    public class StaffService : IStaffService, IAutoRegistrationScopedLifetimeService
    {
        private readonly MyDbContext db;
        private readonly IConfiguration conf;
        private readonly ILogger<StaffService> logger;
        private readonly IServiceProvider provider;

        public StaffService(MyDbContext db, IConfiguration conf, ILogger<StaffService> logger, IServiceProvider provider)
        {
            this.db = db;
            this.conf = conf;
            this.logger = logger;
            this.provider = provider;
        }

        private IHttpContextAccessorExtensions GetAccessor()
        {
            using (var scope = provider.CreateScope())
            {
                var accessor = scope.ServiceProvider.GetService<IHttpContextAccessorExtensions>();
                return accessor;
            }
        }

        public async Task<Answer<viStaff[]>> GetStaffsByOrganizationId()
        {
            try
            {
                int org_id = GetAccessor().GetOrgId();
                var staffs = await db.tbStaffs
                    .AsNoTracking()
                    .Include(s => s.Organization)
                    .Include(s => s.Category)
                    .Where(s => s.OrganizationId == org_id && s.RoleId == (int)UserRoles.STAFF && s.Status == 1)
                    .Select(x => new viStaff
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Surname = x.Surname,
                        BirthDay = x.BirthDay,
                        PhoneNum = x.PhoneNum,
                        Patronymic = x.Patronymic,
                        TelegramId = x.TelegramId,
                        Online = x.Online.HasValue,
                        Availability = x.Availability,
                        Experience = x.Experience,
                        OrganizationId = x.OrganizationId,
                        Organization = x.Organization.Name,
                        CategoryId = x.CategoryId,
                        Category = x.Category.NameUz,
                        RoleId = x.RoleId,
                        PhotoUrl = x.PhotoUrl,
                        Gender = x.Gender
                    })
                    .Take(20)
                    .ToArrayAsync();
                return new Answer<viStaff[]>(true, "", staffs);
            }
            catch (Exception e)
            {
                logger.LogError($"StaffService.GetStaffByOrganizationId Error:{e.Message}");
                return new Answer<viStaff[]>(false, "Ошибка программы", null);
            }

        }

        public async Task<Answer<viStaff>> AddStaffAsync(viStaff staff)
        {
            try
            {
                var newStaff = new tbStaff();

                int user_id = GetAccessor().GetId();
                int role_id = GetAccessor().GetRoleId();
                int staff_role = (int)UserRoles.STAFF;
                int org_id = GetAccessor().GetOrgId();

                if (role_id == (int)UserRoles.SUPER_ADMIN)
                {
                    staff_role = staff.RoleId;
                    org_id = staff.OrganizationId.Value;
                }

                if (staff.CategoryId.HasValue)
                    newStaff.CategoryId = staff.CategoryId;

                newStaff.Surname = staff.Surname;
                newStaff.Name = staff.Name;
                newStaff.Patronymic = staff.Patronymic;
                newStaff.PhoneNum = staff.PhoneNum;
                newStaff.BirthDay = staff.BirthDay;
                newStaff.PhotoUrl = staff.PhotoUrl;
                newStaff.OrganizationId = org_id;
                newStaff.RoleId = staff_role;
                newStaff.TelegramId = staff.TelegramId;
                newStaff.Experience = staff.Experience;
                newStaff.Availability = new int[] { 1, 1, 1, 1, 1, 1, 1 };
                newStaff.CreateDate = DateTime.Now;
                newStaff.Gender = staff.Gender;
                newStaff.CreateUser = user_id;
                newStaff.Password = CHash.EncryptMD5(staff.Password);
                newStaff.Status = 1;
                newStaff.Online = true;
                newStaff.PeriodTime = 30;

                await db.tbStaffs.AddAsync(newStaff);
                await db.SaveChangesAsync();

                return new Answer<viStaff>(true, "", null);
            }
            catch (Exception e)
            {
                logger.LogError($"StaffService.AddStaffAsync Error:{e.Message} Model: {staff.ToJson()}");
                return new Answer<viStaff>(false, "Ошибка программы", null);
            }

        }

        public async Task<Answer<List<IdValue>>> GetStuffList(int category_id)
        {
            try
            {
                int org_id = GetAccessor().GetOrgId();
                var staffList = await db.tbStaffs
                               .AsNoTracking()
                               .Where(s => s.OrganizationId == org_id && s.CategoryId == category_id && s.RoleId == (int)UserRoles.STAFF && s.Status == 1)
                               .Select(x => new IdValue
                               {
                                   Id = x.Id,
                                   Value = $"{x.Surname} {x.Name} {x.Patronymic}"
                               }
                               ).ToListAsync();
                return new Answer<List<IdValue>>(true, "", staffList);
            }
            catch (Exception e)
            {
                logger.LogError($"StaffService.GetStuffList Error:{e.Message}");
                return new Answer<List<IdValue>>(false, "Ошибка программы", null);
            }


        }

        public async Task<AnswerBasic> SetStatusAsync(viSetStatus status)
        {
            try
            {
                int user_id = GetAccessor().GetId();
                var st = await db.tbStaffs.FindAsync(status.Id);
                st.Status = status.Status;
                st.UpdateUser = user_id;
                st.UpdateDate = DateTime.Now;
                await db.SaveChangesAsync();

                return new AnswerBasic(true, "");
            }
            catch (Exception e)
            {
                logger.LogError($"StaffService.SetStatusAsync Error:{e.Message}");
                return new AnswerBasic(false, "Ошибка программы");
            }

        }

        public async Task<AnswerBasic> SetActivity(int staff_id, bool activity)
        {
            try
            {
                int user_id = GetAccessor().GetId();
                var st = await db.tbStaffs.FindAsync(staff_id);
                st.Online = activity;
                st.UpdateDate = DateTime.Now;
                st.UpdateUser = user_id;
                await db.SaveChangesAsync();

                return new AnswerBasic(true, "");
            }
            catch (Exception e)
            {
                logger.LogError($"StaffService.SetActivity Error:{e.Message}");
                return new AnswerBasic(false, "Ошибка программы");
            }

        }

        public async Task<Answer<viStaff>> UpdateStaff(viStaff staff)
        {
            try
            {
                var updateStaff = await db.tbStaffs.FindAsync(staff.Id);
                int org_id = GetAccessor().GetOrgId();
                int user_id = GetAccessor().GetId();
                int role_id = GetAccessor().GetRoleId();
                int staff_role = (int)UserRoles.STAFF;

                if (staff.BirthDay.HasValue)
                    updateStaff.BirthDay = staff.BirthDay.Value;

                updateStaff.CategoryId = staff.CategoryId.Value;
                updateStaff.Experience = staff.Experience.Value;

                if (staff.Status.HasValue)
                    updateStaff.Status = staff.Status.Value;

                updateStaff.Surname = staff.Surname;
                updateStaff.Name = staff.Name;
                updateStaff.Patronymic = staff.Patronymic;

                if (staff.Password is not null)
                    updateStaff.Password = CHash.EncryptMD5(staff.Password);

                updateStaff.PhoneNum = staff.PhoneNum;
                updateStaff.Gender = staff.Gender;
                updateStaff.Online = staff.Online;

                if (role_id == (int)UserRoles.SUPER_ADMIN)
                {
                    staff_role = staff.RoleId;
                    org_id = staff.OrganizationId.Value;
                }

                updateStaff.PeriodTime = staff.PeriodTime;
                updateStaff.PhotoUrl = staff.PhotoUrl;
                updateStaff.Availability = staff.Availability;
                updateStaff.OrganizationId = org_id;
                updateStaff.RoleId = staff_role;
                updateStaff.UpdateDate = DateTime.Now;
                updateStaff.UpdateUser = user_id;
                await db.SaveChangesAsync();

                return new Answer<viStaff>(true, "", null);
            }
            catch (Exception e)
            {
                logger.LogError($"StaffService.UpdateStaff Error:{e.Message} Model: {staff.ToJson()}");
                return new Answer<viStaff>(false, "Ошибка программы", null);
            }

        }
        public async Task<Answer<viStaff>> GetStaffById(int staff_id)
        {
            try
            {
                int org_id = GetAccessor().GetOrgId();
                var staff = await db.tbStaffs
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
                        PhoneNum = x.PhoneNum,
                        Patronymic = x.Patronymic,
                        TelegramId = x.TelegramId,
                        Online = x.Online.Value,
                        Availability = x.Availability,
                        Experience = x.Experience,
                        OrganizationId = org_id,
                        Organization = x.Organization.Name,
                        CategoryId = x.CategoryId,
                        Category = x.Category.NameUz,
                        RoleId = x.RoleId,
                        PhotoUrl = x.PhotoUrl,
                        Gender = x.Gender
                    })
                    .FirstOrDefaultAsync();

                return new Answer<viStaff>(true, "", staff);
            }
            catch (Exception e)
            {
                logger.LogError($"StaffService.GetStaffById Error:{e.Message}");
                return new Answer<viStaff>(false, "Ошибка программы", null);
            }

        }

        public async ValueTask<Answer<viStaff[]>> SearchStaffAsync(string name)
        {
            try
            {
                int org_id = GetAccessor().GetOrgId();
                var search = await (from s in db.tbStaffs
                                    where EF.Functions.ILike(s.Surname, $"%{name}%")
                                    || EF.Functions.ILike(s.Name, $"%{name}%")
                                    || EF.Functions.ILike(s.Patronymic, $"%{name}%")
                                    select s)
                            .AsNoTracking()
                            .Where(x => x.Status == 1 && x.RoleId == (int)UserRoles.STAFF && x.OrganizationId == org_id)
                            .Select(x => new viStaff
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Surname = x.Surname,
                                BirthDay = x.BirthDay,
                                PhoneNum = x.PhoneNum,
                                Patronymic = x.Patronymic,
                                TelegramId = x.TelegramId,
                                Online = x.Online.Value,
                                Availability = x.Availability,
                                Experience = x.Experience,
                                OrganizationId = x.OrganizationId,
                                Organization = x.Organization.Name,
                                CategoryId = x.CategoryId,
                                Category = x.Category.NameUz,
                                RoleId = x.RoleId,
                                PhotoUrl = x.PhotoUrl,
                                Gender = x.Gender
                            })
                            .ToArrayAsync();

                return new Answer<viStaff[]>(false, "", search);
            }
            catch (Exception e)
            {
                logger.LogError($"StaffService.SearchStaffAsync Error:{e.Message}");
                return new Answer<viStaff[]>(false, "Ошибка программы", null);
            }
        }

        public async ValueTask<Answer<string>> SetPhotoAsync(viSetPhoto staff)
        {
            try
            {
                var updateStaff = await db.tbStaffs.FindAsync(staff.StaffId);
                int user_id = GetAccessor().GetId();

                updateStaff.PhotoUrl = staff.PhotoUrl;
                updateStaff.UpdateDate = DateTime.Now;
                updateStaff.UpdateUser = user_id;

                await db.SaveChangesAsync();

                return new Answer<string>(true, "", updateStaff.PhotoUrl);
            }
            catch (Exception e)
            {
                logger.LogError($"StaffService.SearchStaffAsync Error:{e.Message} Model: {staff.ToJson()}");
                return new Answer<string>(false, "Ошибка программы", null);
            }
        }

        //public async Task<viStaffAvailability> GetStaffAvailabilityAsync(int staff_id)
        //{
        //    int org_id = accessor.GetOrgId();
        //    return await db.tbStaffs
        //        .AsNoTracking()
        //        .Where(x => x.OrganizationId == org_id &&
        //        x.Id == staff_id
        //        && x.Status == 1
        //        && x.RoleId == (int)UserRoles.STAFF)
        //        .Select(x => new viStaffAvailability
        //        {
        //            StaffId = x.Id,
        //            Availability = x.Availability
        //        })
        //        .FirstOrDefaultAsync();
        //}

        public async ValueTask<Answer<TokenModel>> TokenAsync(LoginModel value)
        {
            var hash_pasw = CHash.EncryptMD5(value.password);
            var user = await db.tbStaffs.FirstOrDefaultAsync(x => x.PhoneNum == value.phoneNum && x.Password == hash_pasw);
            if (user != null)
            {

                return new Answer<TokenModel>(true, "", GenerateToken(user));
            }
            else
                return new Answer<TokenModel>(false, "Маълумот топилмадаи", null);
        }

        public TokenModel GenerateToken(tbStaff user)
        {
            var claims = new ClaimsIdentity(new Claim[]
                           {
                                       new Claim(ClaimTypes.Sid, user.Id.ToString()),
                                       new Claim(ClaimTypes.Name, $"{user.Surname} {user.Name} {user.Patronymic}"),
                                       new Claim(ClaimTypes.MobilePhone, user.PhoneNum),
                                       new Claim("OrganizationId", user.OrganizationId.ToString()),
                                       new Claim("RoleId", user.RoleId.ToString()),
                           });

            var str = conf["SystemVars:PrivateKeyString"];
            var key = Encoding.ASCII.GetBytes(str);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = "waPlanner",
                Audience = "waPlanner",
                Subject = claims,
                Expires = DateTime.Now.AddYears(1),
                NotBefore = DateTime.Now.AddMinutes(-10),
                IssuedAt = DateTime.Now.AddMinutes(-10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var modelToken = new TokenModel()
            {
                id = user.Id,
                token = tokenHandler.WriteToken(token),
                userName = $"{user.Surname} {user.Name} {user.Patronymic}",
                roleId = user.RoleId,
            };
            if (user.OrganizationId.HasValue)
                modelToken.orgId = user.OrganizationId.Value;
            return modelToken;
        }

        public async ValueTask<AnswerBasic> ChangePaswwordAsync(ChangePasswordModel value)
        {
            try
            {
                int user_id = GetAccessor().GetId();
                var staff = await db.tbStaffs.FindAsync(user_id);

                if (staff.Password == CHash.EncryptMD5(value.oldPassword) && staff.Id == user_id)
                {
                    staff.Password = CHash.EncryptMD5(value.newPassword);
                    db.Update(staff);
                    await db.SaveChangesAsync();
                    return new AnswerBasic(true, "");
                }
                return new AnswerBasic(false, "Старый пароль не верен, повторите еще раз!");
            }
            catch (Exception e)
            {
                logger.LogError($"StaffService.ChangePaswwordAsync Error:{e.Message}");
                return new AnswerBasic(false, "Ошибка программы");
            }

        }

        public async ValueTask<Answer<IdValue>> OnForgotPassword(string PhoneNum)
        {
            using (var scope = provider.CreateScope())
            {
                var telegram = scope.ServiceProvider.GetService<ITelegramGroupCreatorService>();
                try
                {
                    var value = await telegram.SendRandomPassword(PhoneNum);

                    if (value.Data is null)
                        return new Answer<IdValue>(false, "Такой номер не зарегистрирован!", null);

                    var staff = await db.tbStaffs.FindAsync(value.Data.Id);     
                    staff.Password = CHash.EncryptMD5(value.Data.Value);
                    await db.SaveChangesAsync();
                    return new Answer<IdValue>(true, "", null);
                }

                catch (Exception e)
                {
                    logger.LogError($"StaffService.OnForgotPassword Error:{e.Message}");
                    return await telegram.SendRandomPassword(null);
                }
            }
        }
    }
}
