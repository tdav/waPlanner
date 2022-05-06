using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Extensions;
using waPlanner.ModelViews;

namespace waPlanner.Services
{
    public interface ISetupService
    {
        Task<Answer<viSetup>> Get();
        Task<AnswerBasic> Set(viSetup v);
    }

    public class SetupService : ISetupService
    {
        private readonly MyDbContext db;
        private readonly IHttpContextAccessorExtensions accessor;
        private readonly ILogger<SetupService> logger;

        public SetupService(MyDbContext db, IHttpContextAccessorExtensions accessor, ILogger<SetupService> logger)
        {
            this.db = db;
            this.accessor = accessor;
            this.logger = logger;
        }
        public async Task<Answer<viSetup>> Get()
        {
            try
            {
                int userId = accessor.GetId();
                var s = await db.spSetups.AsNoTracking().FirstOrDefaultAsync(x => x.StaffId == userId);
                var setup = viSetup.FromJson(s.Text);
                return new Answer<viSetup>(true, "", setup);
            }
            catch (Exception e)
            {
                logger.LogError($"StaffService.GetStaffByOrganizationId Error:{e.Message}");
                return new Answer<viSetup>(false, "Ошибка программы", null);
            }
           
        }

        public async Task<AnswerBasic> Set(viSetup v)
        {
            try
            {
                int userId = accessor.GetId();
                var s = await db.spSetups.FindAsync(userId);

                if (s == null)
                {
                    s = new Database.Models.spSetup();
                    s.Status = 1;
                    s.CreateDate = System.DateTime.Now;
                    s.StaffId = userId;
                }
                else
                {
                    s.UpdateUser = userId;
                    s.UpdateDate = System.DateTime.Now;
                }

                s.Text = v.ToString();
                await db.spSetups.AddAsync(s);
                await db.SaveChangesAsync();
                return new AnswerBasic(true, "");
            }
            catch (Exception e)
            {
                logger.LogError($"StaffService.GetStaffByOrganizationId Error:{e.Message}");
                return new AnswerBasic(false, "Ошибка программы");
            }
            
        }
    }
}
