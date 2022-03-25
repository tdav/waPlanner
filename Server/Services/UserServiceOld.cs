//using Arch.EntityFrameworkCore.UnitOfWork;
//using AsbtCore.UtilsV2;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using Microsoft.IdentityModel.Tokens;
//using Service.Extensions;
//using System;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;
//using waPlanner.Database.Models;
//using waPlanner.ModelViews;

//namespace waPlanner.Services
//{
//    public interface IUserService
//    {
//        ValueTask<Answer<tbUser>> Get(int id);
//        ValueTask<Answer<tbUser[]>> GetAll();
//        ValueTask<AnswerBasic> ChangePassword(ChangePasswordModel value);
//        ValueTask<AnswerBasic> ChangeStatus(int user_id, int status);
//        ValueTask<Answer<int>> CreateUser(tbUser u);
//        ValueTask<Answer<TokenModel>> TokenAsync(LoginModel value);
//        TokenModel GenerateToken(tbUser user);
//    }


//    public class UserService : IUserService, IAsyncDisposable
//    {
//        private readonly IUnitOfWork uow;
//        private readonly ILogger<UserService> logger;
//        private readonly IHttpContextAccessorExtensions accessor;
//        private readonly Vars vars;

//        public UserService(IUnitOfWork uow, ILogger<UserService> logger, IOptions<Vars> options, IHttpContextAccessorExtensions accessor)
//        {
//            this.uow = uow;
//            this.logger = logger;
//            this.accessor = accessor;
//            vars = options.Value;
//        }

//        public async ValueTask DisposeAsync()
//        {
//            try
//            {
//                await uow.GetDbConnection().CloseAsync();
//            }
//            catch (Exception ee)
//            {
//                logger.LogError("DisposeAsync Error:{@error}", ee.GetAllMessages());
//            }
//            uow.Dispose();
//        }

//        public async ValueTask<Answer<tbUser>> Get(int id)
//        {
//            //if (User.GetId() != id) return NotFound("Фойдаланувчи маълумотларини кўриш тақиқланган");


//            var rep = uow.GetRepository<tbUser>();
//            var user = await rep.GetFirstOrDefaultAsync(predicate: x => x.Id == id);
//            if (user != null)
//            {
//                user.Password = "";
//                return new Answer<tbUser>(true, "", user);
//            }
//            else
//                return new Answer<tbUser>(false, "Маълумот топилмади", null);
//        }

//        public async ValueTask<Answer<tbUser[]>> GetAll()
//        {
//            var rep = uow.GetRepository<tbUser>();
//            var list = await rep.GetAllAsync();
//            if (list != null && list.Count > 0)
//            {
//                foreach (var it in list) it.Password = "";
//                return new Answer<tbUser[]>(true, "", list.ToArray());
//            }
//            else
//                return new Answer<tbUser[]>(false, "Маълумот топилмади", null);
//        }

//        public async ValueTask<AnswerBasic> ChangePassword(ChangePasswordModel value)
//        {
//            long UserId = accessor.GetId();

//            var rep = uow.GetRepository<tbUser>();
//            var user = await rep.GetFirstOrDefaultAsync(predicate: x => x.Id == UserId && x.Password == value.oldPassword, disableTracking: false);
//            if (user != null)
//            {
//                user.Password = CHash.EncryptMD5(value.newPassword);
//                await uow.SaveChangesAsync();
//                return new AnswerBasic(true, "");
//            }
//            else
//                return new AnswerBasic(false, "Маълумот топилмади");
//        }

//        public async ValueTask<AnswerBasic> ChangeStatus(int user_id, int status)
//        {
//            long UserId = accessor.GetId();

//            var rep = uow.GetRepository<tbUser>();
//            var user = await rep.GetFirstOrDefaultAsync(predicate: x => x.Id == user_id, disableTracking: false);
//            if (user != null)
//            {
//                user.Status = status;
//                await uow.SaveChangesAsync();
//                return new AnswerBasic(true, "");
//            }
//            else
//                return new AnswerBasic(false, "Маълумот топилмади");
//        }

//        public async ValueTask<Answer<int>> CreateUser(tbUser u)
//        {
//            var rep = uow.GetRepository<tbUser>();

//            var etx = await rep.ExistsAsync(x => x.PhoneNum == u.PhoneNum);
//            if (etx == true)
//                return new Answer<int>(false, "Бундай тел. ракамли фойдаланувчи бор", 0);

//            u.Password = CHash.EncryptMD5(u.Password);
//            var user = await rep.InsertAsync(u);
//            await uow.SaveChangesAsync();
//            return new Answer<int>(true, "", user.Entity.Id);
//        }

//        public async ValueTask<Answer<TokenModel>> TokenAsync(LoginModel value)
//        {
//            long UserId = accessor.GetId();

//            var rep = uow.GetRepository<tbUser>();
//            var hash_pasw = CHash.EncryptMD5(value.password);
//            var user = await rep.GetFirstOrDefaultAsync(predicate: x => x.PhoneNum == value.phoneNum && x.Password == hash_pasw);
//            if (user != null)
//            {
//                var repSes = uow.GetRepository<tbSesion>();

//                var us = new tbSesion()
//                {
//                    CreateDate = DateTime.Now,
//                    UserId = user.Id,
//                    IpAdress = accessor.GetUserIp(),
//                    UserType = user.UserTypeId
//                };
//                await repSes.InsertAsync(us);
//                await uow.SaveChangesAsync();

//                return new Answer<TokenModel>(true, "", GenerateToken(user));
//            }
//            else
//                return new Answer<TokenModel>(false, "Маълумот топилмадаи", null);
//        }

//        public TokenModel GenerateToken(tbUser user)
//        {
//            var roles = string.Join(',', user.Roles);
//            var claims = new ClaimsIdentity(new Claim[]
//                           {
//                               new Claim(ClaimTypes.Name, $"{user.Surname} {user.Name} {user.Patronymic}"),
//                               new Claim(ClaimTypes.Sid, user.Id.ToStr()),
//                               new Claim(ClaimTypes.Role, roles),
//                               new Claim(ClaimTypes.MobilePhone, user.PhoneNum)
//                           });

//            var str = vars.PrivateKeyString;
//            var key = Encoding.ASCII.GetBytes(str);

//            var tokenDescriptor = new SecurityTokenDescriptor
//            {
//                Issuer = "waMarketplace",
//                Audience = "waMarketplace",
//                Subject = claims,
//                Expires = DateTime.Now.AddYears(1),
//                NotBefore = DateTime.Now.AddMinutes(-10),
//                IssuedAt = DateTime.Now.AddMinutes(-10),
//                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
//            };

//            var tokenHandler = new JwtSecurityTokenHandler();
//            var token = tokenHandler.CreateToken(tokenDescriptor);

//            var modelToken = new TokenModel()
//            {
//                id = user.Id,
//                token = tokenHandler.WriteToken(token),
//                userName = $"{user.Surname} {user.Name} {user.Patronymic}",
//                roles = roles
//            };

//            return modelToken;
//        }
//    }

//}
