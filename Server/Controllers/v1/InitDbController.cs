using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;
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


//#if DEBUG
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

                #region Add Specialization
                db.spSpecializations.Add(new spSpecialization() { Id = 1, NameLt = "admin", NameRu = "admin", NameUz = "admin", CreateDate = DateTime.Now, CreateUser = 1 });
                await db.SaveChangesAsync();
                #endregion

                #region Add Org
                db.spOrganizations.Add(new spOrganization()
                {
                    Id = 1,
                    Name = "admin",
                    ChatId = 0,
                    Address = "admin",
                    Latitude = 0,
                    Longitude = 0,
                    CreateDate = DateTime.Now,
                    CreateUser = 0,
                    SpecializationId = 1
                });
                await db.SaveChangesAsync();
                #endregion

                #region Add Cat
                db.spCategories.Add(new spCategory()
                {
                    Id = 1,
                    NameUz = "admin",
                    NameRu = "admin",
                    NameLt = "admin",
                    OrganizationId = 1,
                    CreateDate = DateTime.Now,
                    CreateUser = 1
                });
                await db.SaveChangesAsync();
                #endregion

                #region Add User
                db.tbStaffs.Add(new tbStaff()
                {
                    Id = 1,
                    BirthDay = DateTime.Now,
                    CategoryId = 1,
                    OrganizationId = 1,
                    RoleId = 1,
                    Gender = "m",
                    PhoneNum = "+998977764669",
                    Password = CHash.EncryptMD5("string"),
                    Name = "admin", 
                    Surname = "admin",
                    Patronymic = "admin",
                    Availability = new int[]{0},
                    PhotoUrl = "string",
                    AdInfo = "string",
                    Status = 1, 
                    CreateDate = DateTime.Now, 
                    CreateUser = 1
                });;

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
//#endif

    }
}