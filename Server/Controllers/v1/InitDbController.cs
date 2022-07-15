using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;
using waPlanner.TelegramBot;
using waPlanner.Utils;

namespace waPlanner.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("[controller]")]
    [SwaggerTag("Добавить по-умолчанию")]
    public class InitDbController : ControllerBase
    {
        private readonly ILogger<InitDbController> logger;

        public InitDbController(ILogger<InitDbController> logger)
        {
            this.logger = logger;
        }


#if DEBUG
        [HttpPost]
        public async ValueTask<string> Post([FromServices] MyDbContext db)
        {
            var tran = await db.Database.BeginTransactionAsync();

            try
            {

                #region Add Roles
                db.spRoles.Add(new spRole() { Id = 1, Name = "SuperAdmin", Status = 1, CreateDate = DateTime.Now, CreateUser = 1 });
                db.spRoles.Add(new spRole() { Id = 2, Name = "OrgAdmin", Status = 1, CreateDate = DateTime.Now, CreateUser = 1 });
                db.spRoles.Add(new spRole() { Id = 3, Name = "Staff", Status = 1, CreateDate = DateTime.Now, CreateUser = 1 });
                await db.SaveChangesAsync();
                #endregion

                #region Add User
                db.tbStaffs.Add(new tbStaff()
                {
                    Id = 1,
                    BirthDay = DateTime.Now,
                    RoleId = (int)UserRoles.SUPER_ADMIN,
                    Gender = "m",
                    PhoneNum = "+998977764669",
                    Password = CHash.EncryptMD5("kozim8386838"),
                    Name = "admin",
                    Surname = "admin",
                    Patronymic = "admin",
                    Availability = new int[] { 0 },
                    PhotoUrl = "string",
                    AdInfo = "string",
                    Status = 1,
                    CreateDate = DateTime.Now,
                    CreateUser = 1
                });
                await db.SaveChangesAsync();
                #endregion

                await tran.CommitAsync();
            }
            catch (Exception ee)
            {
                await tran.RollbackAsync();
                logger.LogError($"InitDbController.Post Error:{ee.Message}");
                return "Error";
            }

            return "OK";
        }
#endif

    }
}