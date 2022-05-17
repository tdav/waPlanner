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
        ValueTask<Answer<viFullAnalysis[]>> GetAllAnalysis();
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

        public async ValueTask<Answer<viFullAnalysis[]>> GetAllAnalysis()
        {
            try
            {
                int org_id = accessor.GetOrgId();
                var analysis = await db.tbAnalizeResults
                    .AsNoTracking()
                    .Where(x => x.OrganizationId == org_id && x.Status == 1)
                    .Select(x => new viFullAnalysis
                    {
                        UserId = x.UserId,
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
                logger.LogError($"FileService.SaveAnalizeResultFile Error:{ex.Message}");
                return new Answer<viFullAnalysis[]>(false, "Ошибка программы", null);
            }
        }
    }
}
