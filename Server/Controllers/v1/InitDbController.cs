using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;

namespace waPlanner.Controllers.v1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("[controller]")]
    [SwaggerTag("Фойдаланувчилар тури")]
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
                db.spRoles.Add(new spRole() { Id = 2, Name = "Admin", Status = 1, CreateDate = DateTime.Now, CreateUser = 1 });
                db.spRoles.Add(new spRole() { Id = 3, Name = "User", Status = 1, CreateDate = DateTime.Now, CreateUser = 1 });
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