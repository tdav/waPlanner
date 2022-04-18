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

namespace waPlanner.Services
{
    public interface IStaffService
    {
        Task<viStaff[]> GetStaffByOrganizationId();
        Task AddStaffAsync(viStaff user);
        Task<List<IdValue>> GetStuffList(int category_id);
        Task SetStatusAsync(viStaff staff, int status);
        Task UpdateStaff(viStaff staff);
        Task<viStaff> GetStaffById(int staff_id);
        Task<viStaff[]> SearchStaffAsync(string name);
        Task SetActivity(viStaff staff, bool online);
        //Task<viStaffAvailability> GetStaffAvailabilityAsync(int staff_id);
        ValueTask<Answer< TokenModel>> TokenAsync(LoginModel value);
        ValueTask<AnswerBasic> ChangePaswwordAsync(ChangePasswordModel value);
    }

    public class StaffService: IStaffService
    {
        private readonly MyDbContext db;
        private readonly IConfiguration conf;
        private readonly IHttpContextAccessorExtensions accessor;

        public StaffService(MyDbContext db, IConfiguration conf, IHttpContextAccessorExtensions accessor)
        {
            this.accessor = accessor;
            this.db = db;
            this.conf = conf;
        }



        public async Task<viStaff[]> GetStaffByOrganizationId()
        {
            int org_id = accessor.GetOrgId();
            return await db.tbStaffs
                .AsNoTracking()
                .Include(s => s.Organization)
                .Include(s => s.Category)
                .Where(s => s.OrganizationId == org_id && s.RoleId == (int)UserRoles.STAFF)
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
                    RoleId = x.RoleId,
                    Photo = x.PhotoUrl,
                    Gender = x.Gender
                })
                .Take(20)
                .ToArrayAsync();
        }

        public async Task AddStaffAsync(viStaff staff)
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
            newStaff.PhotoUrl = staff.Photo;
            newStaff.OrganizationId = org_id;
            newStaff.RoleId = staff_role;
            newStaff.TelegramId = staff.TelegramId;
            newStaff.Experience = staff.Experience;
            newStaff.Availability = staff.Availability;
            newStaff.CreateDate = DateTime.Now;
            newStaff.Gender = staff.Gender;
            newStaff.CreateUser = user_id;
            newStaff.Password = CHash.EncryptMD5(staff.Password);
            newStaff.Status = 1;
            newStaff.Online = true;

            await db.tbStaffs.AddAsync(newStaff);
            await db.SaveChangesAsync();
        }

        public async Task<List<IdValue>> GetStuffList(int category_id)
        {
            int org_id = accessor.GetOrgId();
            return await db.tbStaffs
                           .AsNoTracking()
                           .Where(s => s.OrganizationId == org_id && s.CategoryId == category_id && s.RoleId == (int)UserRoles.STAFF && s.Status == 1)
                           .Select(x => new IdValue
                           {
                               Id = x.Id,
                               Name = $"{x.Surname} {x.Name} {x.Patronymic}"
                           }
                           ).ToListAsync();
        }

        public async Task SetStatusAsync(viStaff staff, int status)
        {
            int user_id = accessor.GetId();
            var st = await db.tbStaffs.FindAsync(staff.Id);
            st.Status = status;
            st.UpdateUser = user_id;
            st.UpdateDate = DateTime.Now;
            await db.SaveChangesAsync();
        }

        public async Task SetActivity(viStaff staff, bool activity)
        {
            int user_id = accessor.GetId();
            var st = await db.tbStaffs.FindAsync(staff.Id);
            st.Online = activity;
            st.UpdateDate = DateTime.Now;
            st.UpdateUser = user_id;
            await db.SaveChangesAsync();
        }

        public async Task UpdateStaff(viStaff staff)
        {
            var updateStaff = await db.tbStaffs.FindAsync(staff.Id);
            int org_id = accessor.GetOrgId();
            int user_id = accessor.GetId();
            int role_id = accessor.GetRoleId();
            int staff_role = (int)UserRoles.STAFF;

            //if (staff.BirthDay.HasValue)
                updateStaff.BirthDay = staff.BirthDay.Value;

            //if (staff.CategoryId.HasValue)
                updateStaff.CategoryId = staff.CategoryId.Value;

            //if (staff.Experience.HasValue)
                updateStaff.Experience = staff.Experience.Value;

            //if(staff.Status.HasValue)
                updateStaff.Status = staff.Status.Value;

            //if(staff.Surname is not null)
                updateStaff.Surname = staff.Surname;

            //if(staff.Name is not null)
                updateStaff.Name = staff.Name;

            //if(staff.Patronymic is not null)
                updateStaff.Patronymic = staff.Patronymic;

            //if(staff.Password is not null)
                updateStaff.Password = staff.Password;

            //if(staff.PhoneNum is not null)
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

            updateStaff.OrganizationId = org_id;
            updateStaff.RoleId = staff_role;
            updateStaff.UpdateDate = DateTime.Now;
            updateStaff.UpdateUser = user_id;
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
                    RoleId = x.RoleId,
                    Photo = x.PhotoUrl,
                    Gender = x.Gender
                }
                ).FirstOrDefaultAsync();
        }

        public async Task<viStaff[]> SearchStaffAsync(string name)
        {
            int org_id = accessor.GetOrgId();
            return await (from s in db.tbStaffs
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
                            Online = x.Online,
                            Availability = x.Availability,
                            Experience = x.Experience,
                            OrganizationId = x.OrganizationId,
                            Organization = x.Organization.Name,
                            CategoryId = x.CategoryId,
                            Category = x.Category.NameUz,
                            RoleId = x.RoleId,
                            Photo = x.PhotoUrl,
                            Gender = x.Gender
                        })
                        .ToArrayAsync();
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
            return new AnswerBasic(true, "");
        }
    }
}
