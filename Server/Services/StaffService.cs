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

namespace waPlanner.Services
{
    public interface IStaffService
    {
        Task<Answer<viStaff[]>> GetStaffsByOrganizationId();
        Task<Answer<viStaff>> AddStaffAsync(viStaff user);
        Task<Answer<List<IdValue>>> GetStuffList(int category_id);
        Task<AnswerBasic> SetStatusAsync(int staff_id, int status);
        Task<Answer<viStaff>> UpdateStaff(viStaff staff);
        Task<Answer<viStaff>> GetStaffById(int staff_id);
        Task<Answer<viStaff[]>> SearchStaffAsync(string name);
        Task<AnswerBasic> SetActivity(int staff_id, bool online);
        //Task<viStaffAvailability> GetStaffAvailabilityAsync(int staff_id);
        ValueTask<Answer< TokenModel>> TokenAsync(LoginModel value);
        ValueTask<AnswerBasic> ChangePaswwordAsync(ChangePasswordModel value);
    }

    public class StaffService: IStaffService
    {
        private readonly MyDbContext db;
        private readonly IConfiguration conf;
        private readonly IHttpContextAccessorExtensions accessor;
        private readonly ILogger<StaffService> logger;

        public StaffService(MyDbContext db, IConfiguration conf, IHttpContextAccessorExtensions accessor, ILogger<StaffService> logger)
        {
            this.accessor = accessor;
            this.db = db;
            this.conf = conf;
            this.logger = logger;
        }

        public async Task<Answer<viStaff[]>> GetStaffsByOrganizationId()
        {
            try
            {
                int org_id = accessor.GetOrgId();
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

                int user_id = accessor.GetId();
                int role_id = accessor.GetRoleId();
                int staff_role = (int)UserRoles.STAFF;
                int org_id = accessor.GetOrgId();

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

                await db.tbStaffs.AddAsync(newStaff);
                await db.SaveChangesAsync();

                return new Answer<viStaff>(true, "", null);
            }
            catch (Exception e)
            {
                logger.LogError($"StaffService.AddStaffAsync Error:{e.Message} Model: {staff}");
                return new Answer<viStaff>(false, "Ошибка программы", null);
            }
           
        }

        public async Task<Answer<List<IdValue>>> GetStuffList(int category_id)
        {
            try
            {
                int org_id = accessor.GetOrgId();
                var staffList = await db.tbStaffs
                               .AsNoTracking()
                               .Where(s => s.OrganizationId == org_id && s.CategoryId == category_id && s.RoleId == (int)UserRoles.STAFF && s.Status == 1)
                               .Select(x => new IdValue
                               {
                                   Id = x.Id,
                                   Name = $"{x.Surname} {x.Name} {x.Patronymic}"
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

        public async Task<AnswerBasic> SetStatusAsync(int staff_id, int status)
        {
            try
            {
                int user_id = accessor.GetId();
                var st = await db.tbStaffs.FindAsync(staff_id);
                st.Status = status;
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
                int user_id = accessor.GetId();
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
                int org_id = accessor.GetOrgId();
                int user_id = accessor.GetId();
                int role_id = accessor.GetRoleId();
                int staff_role = (int)UserRoles.STAFF;

                if (staff.BirthDay.HasValue)
                    updateStaff.BirthDay = staff.BirthDay.Value;

                //if (staff.CategoryId.HasValue)
                updateStaff.CategoryId = staff.CategoryId.Value;

                //if (staff.Experience.HasValue)
                updateStaff.Experience = staff.Experience.Value;

                if (staff.Status.HasValue)
                    updateStaff.Status = staff.Status.Value;

                //if (staff.Surname is not null)
                updateStaff.Surname = staff.Surname;

                //if (staff.Name is not null)
                updateStaff.Name = staff.Name;

                //if (staff.Patronymic is not null)
                updateStaff.Patronymic = staff.Patronymic;

                if (staff.Password is not null)
                    updateStaff.Password = CHash.EncryptMD5(staff.Password);

                //if (staff.PhoneNum is not null)
                updateStaff.PhoneNum = staff.PhoneNum;

                //if (staff.Gender is not null)
                updateStaff.Gender = staff.Gender;

                //if (staff.Online.HasValue)
                updateStaff.Online = staff.Online;

                if (role_id == (int)UserRoles.SUPER_ADMIN)
                {
                    staff_role = staff.RoleId;
                    org_id = staff.OrganizationId.Value;
                }

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
                logger.LogError($"StaffService.UpdateStaff Error:{e.Message} Model: {staff}");
                return new Answer<viStaff>(false, "Ошибка программы", null);
            }
            
        }
        public async Task<Answer<viStaff>> GetStaffById(int staff_id)
        {
            try
            {
                int org_id = accessor.GetOrgId();
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

        public async Task<Answer<viStaff[]>> SearchStaffAsync(string name)
        {
            try
            {
                int org_id = accessor.GetOrgId();
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
            //TODO
            return new AnswerBasic(true, "");
        }
    }
}
