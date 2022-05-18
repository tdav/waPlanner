using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Extensions;
using waPlanner.Interfaces;
using waPlanner.ModelViews;


namespace waPlanner.Services
{
    public interface IAnalysisService
    {
        ValueTask<Answer<viFullAnalysis[]>> GetStaffAllAnalysis(int user_id);
        ValueTask<AnswerBasic> DeleteAnylysis(int id);
    }

    public class AnalysisService: IAnalysisService, IAutoRegistrationScopedLifetimeService
    {
        private readonly MyDbContext db;
        private readonly IHttpContextAccessorExtensions accessor;
        private readonly ILogger<CategoryService> logger;

        public AnalysisService(MyDbContext db, IHttpContextAccessorExtensions accessor, ILogger<CategoryService> logger)
        {
            this.db = db;
            this.accessor = accessor;
            this.logger = logger;
        }

        public async ValueTask<Answer<viFullAnalysis[]>> GetStaffAllAnalysis(int user_id)
        {
            try
            {
                int org_id = accessor.GetOrgId();
                var analysis = await db.tbAnalizeResults
                    .AsNoTracking()
                    .Where(x => x.OrganizationId == org_id && x.Status == 1 && x.UserId == user_id)
                    .Select(x => new viFullAnalysis
                    {
                        Id = x.Id,
                        User = x.User,
                        StaffId = x.StaffId.Value,
                        OrganizationId = org_id,
                        AdInfo = x.AdInfo,
                        FileUrl = x.Url
                    })
                    .ToArrayAsync();

                return new Answer<viFullAnalysis[]>(true, "", analysis);
            }
            catch (Exception ex)
            {
                logger.LogError($"AnalysisService.GetAllAnalysis Error:{ex.Message}");
                return new Answer<viFullAnalysis[]>(false, "Ошибка программы", null);
            }
        }

        public async ValueTask<AnswerBasic> DeleteAnylysis(int id)
        {
            try
            {
                var userAnalys = await db.tbAnalizeResults.FindAsync(id);

                if (userAnalys is null) return new AnswerBasic(false, "Такого анализа нет");

                userAnalys.Status = 0;
                await db.SaveChangesAsync();

                return new AnswerBasic(true, "");
            }
            catch (Exception ex)
            {
                logger.LogError($"AnalysisService.ChangeAnylysis Error:{ex.Message}");
                return new AnswerBasic(false, "Ошибка программы");

            }
        }
    }
}
