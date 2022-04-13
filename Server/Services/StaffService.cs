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
        Task<int> AddStaffAsync(viStaff user);
        Task<List<IdValue>> GetStuffList(int category_id);
        Task SetStatusAsync(viStaff staff, int status);
        Task UpdateStaff(viStaff staff);
        Task<viStaff> GetStaffById(int staff_id);
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
                .Where(s => s.OrganizationId == org_id)
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
                }
                ).ToArrayAsync();
        }

        public async Task<int> AddStaffAsync(viStaff staff)
        {
            int org_id = accessor.GetOrgId();
            int user_id = accessor.GetId();
            int role_id = accessor.GetRoleId();
            var newStaff = new tbStaff();
            int staff_role = (int)UserRoles.STAFF;

            if (role_id == (int)UserRoles.SUPER_ADMIN)
            {
                staff_role = staff.RoleId;
                org_id = staff.OrganizationId.Value;
            }
            newStaff.Surname = staff.Surname;
            newStaff.Name = staff.Name;
            newStaff.Patronymic = staff.Patronymic;
            newStaff.PhoneNum = staff.PhoneNum;
            newStaff.BirthDay = staff.BirthDay;
            newStaff.PhotoUrl = staff.Photo;
            newStaff.OrganizationId = org_id;
            newStaff.CategoryId = staff.CategoryId;
            newStaff.RoleId = staff_role;
            
            newStaff.TelegramId = staff.TelegramId;
            newStaff.Experience = staff.Experience;
            newStaff.Availability = staff.Availability;
            newStaff.CreateDate = DateTime.Now;
            newStaff.Gender = staff.Gender;
            newStaff.CreateUser = user_id;
            newStaff.Password = CHash.EncryptMD5(staff.Password);
            newStaff.Status = 1;

            await db.tbStaffs.AddAsync(newStaff);
            await db.SaveChangesAsync();

            return newStaff.Id;
        }

        public async Task<List<IdValue>> GetStuffList(int category_id)
        {
            int org_id = accessor.GetOrgId();
            return await db.tbStaffs
                           .AsNoTracking()
                           .Where(s => s.OrganizationId == org_id && s.CategoryId == category_id)
                           .Select(x => new IdValue
                           {
                               Id = x.Id,
                               Name = $"{x.Surname} {x.Name} {x.Patronymic}"
                           }
                           ).ToListAsync();
        }

        public async Task SetStatusAsync(viStaff staff, int status)
        {
            var sh = await db.tbStaffs.FindAsync(staff.Id);
            sh.Status = status;
            sh.UpdateUser = 1;
            sh.UpdateDate = DateTime.Now;
            await db.SaveChangesAsync();
        }

        public async Task UpdateStaff(viStaff staff)
        {
            var updateStaff = await db.tbStaffs.FindAsync(staff.Id);
            int org_id = accessor.GetOrgId();

            updateStaff.OrganizationId = org_id;

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

            updateStaff.RoleId = (int)UserRoles.STAFF;
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
                orgId = user.OrganizationId.Value
            };

            return modelToken;   
    }

        public async ValueTask<AnswerBasic> ChangePaswwordAsync(ChangePasswordModel value)
        {
            return new AnswerBasic(true, "");
        }
    }
}
