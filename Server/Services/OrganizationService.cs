using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;
using waPlanner.Extensions;
using waPlanner.ModelViews;
using waPlanner.Utils;

namespace waPlanner.Services
{
    public interface IOrganizationService
    {
        Task<Answer<long>> InsertOrganizationAsync(viOrganization organization);
        Task<AnswerBasic> UpdateOrganizationAsync(viOrganization organziation);
        Task<AnswerBasic> UpdateOrganizationStatus(int org_id, int status);
        Task<Answer<spOrganization>> GetOrgByIdAsync(int id);
        Task<Answer<spOrganization[]>> GetAllOrgsAsync();
    }
    public class OrganizationService : IOrganizationService
    {
        private readonly MyDbContext db;
        private readonly ILogger<OrganizationService> logger;
        private readonly IHttpContextAccessorExtensions accessor;
        public OrganizationService(MyDbContext db, IHttpContextAccessorExtensions accessor, ILogger<OrganizationService> logger)
        {
            this.accessor = accessor;
            this.db = db;
            this.logger = logger;
        }
        public async Task<Answer<long>> InsertOrganizationAsync(viOrganization organization)
        {
            try
            {
                int user_id = accessor.GetId();

                var addOrganization = new spOrganization();

                if (organization.Latitude.HasValue)
                    addOrganization.Latitude = organization.Latitude.Value;

                if (organization.Longitude.HasValue)
                    addOrganization.Longitude = organization.Longitude.Value;

                if (organization.SpecializationId.HasValue)
                    addOrganization.SpecializationId = organization.SpecializationId.Value;

                if (organization.DinnerTimeStart.HasValue)
                    addOrganization.BreakTimeStart = organization.DinnerTimeStart.Value;

                if (organization.DinnerTimeEnd.HasValue)
                    addOrganization.BreakTimeEnd = organization.DinnerTimeEnd.Value;

                addOrganization.WorkStart = organization.WorkTimeStart;
                addOrganization.WorkEnd = organization.WorkTimeEnd;
                addOrganization.Name = organization.Name;
                addOrganization.ChatId = 0;
                addOrganization.CreateDate = DateTime.Now;
                addOrganization.CreateUser = user_id;
                addOrganization.Status = 1;
                addOrganization.Address = organization.Address;
                await db.spOrganizations.AddAsync(addOrganization);
                await db.SaveChangesAsync();

                return new Answer<long>(true, "", addOrganization.Id);
                }

            catch (Exception ee)
            {
                logger.LogError($"OrganizationService.InsertOrganizationAsync Error:{ee.Message} Model:{organization.ToJson()}");
                return new Answer<long>(false, "Ошибка программы", 0);
            }
        }

        public async Task<AnswerBasic> UpdateOrganizationAsync(viOrganization organization)
        {
            try
            {
                int user_id = accessor.GetId();
                var updatedOrganization = await db.spOrganizations.FindAsync(organization.Id);

                if (organization.ChatId.HasValue)
                    updatedOrganization.ChatId = organization.ChatId.Value;

                if (organization.Latitude.HasValue)
                    updatedOrganization.Latitude = organization.Latitude.Value;

                if (organization.Longitude.HasValue)
                    updatedOrganization.Longitude = organization.Longitude.Value;

                if (organization.SpecializationId.HasValue)
                    updatedOrganization.SpecializationId = organization.SpecializationId.Value;

                if (organization.Status.HasValue)
                    updatedOrganization.Status = organization.Status.Value;

                if (organization.DinnerTimeStart.HasValue)
                    updatedOrganization.BreakTimeStart = organization.DinnerTimeStart.Value;

                if (organization.DinnerTimeEnd.HasValue)
                    updatedOrganization.BreakTimeEnd = organization.DinnerTimeEnd.Value;

                if (organization.Name is not null)
                    updatedOrganization.Name = organization.Name;

                updatedOrganization.WorkStart = organization.WorkTimeStart;
                updatedOrganization.WorkEnd = organization.WorkTimeEnd;
                updatedOrganization.MessageRu = organization.MessageRu;
                updatedOrganization.MessageLt = organization.MessageLt;
                updatedOrganization.MessageUz = organization.MessageUz;
                updatedOrganization.UpdateDate = DateTime.Now;
                updatedOrganization.UpdateUser = user_id;
                await db.SaveChangesAsync();

                return new AnswerBasic(true, "");
            }
            catch (Exception e)
            {
                logger.LogError($"OrganizationService.UpdateOrganizationAsync Error: {e.Message} Model: {organization.ToJson()}");
                return new AnswerBasic(false, "Ошибка программы");
            }
        }

        public async Task<AnswerBasic> UpdateOrganizationStatus(int org_id, int status)
        {
            try
            {
                int user_id = accessor.GetId();
                var organiztion = await db.spOrganizations.FindAsync(org_id);
            
                organiztion.Status = status;
                organiztion.UpdateDate = DateTime.Now;
                organiztion.UpdateUser = user_id;
                await db.SaveChangesAsync();
                return new AnswerBasic(true, "");
            } 
            catch (Exception e)
            {
                logger.LogError($"OrganizationService.UpdateOrganizationStatus Error: {e.Message}");
                return new AnswerBasic(false, "Ошибка в программе");
            }
        }

        public async Task<Answer<spOrganization>> GetOrgByIdAsync(int organization_id)
        {
            try
            {
                var organization = await db.spOrganizations.AsNoTracking().FirstAsync(x => x.Status == 1 && x.Id == organization_id);
                return new Answer<spOrganization>(true, "", organization);
            }
            catch (Exception e)
            {
                logger.LogError($"OrganizationService.GetOrgByIdAsync Error: {e.Message}");
                return new Answer<spOrganization>(false, "Ошибка в программе", null);
            }
        }

        public async Task<Answer<spOrganization[]>> GetAllOrgsAsync()
        {
            try
            {
                var organizations = await db.spOrganizations.Where(x => x.Status == 1).AsNoTracking().ToArrayAsync();
                return new Answer<spOrganization[]>(true, "", organizations);
            }
            catch (Exception e)
            {
                logger.LogError($"OrganizationService.GetAllOrgsAsync Error: {e.Message}");
                return new Answer<spOrganization[]>(false, "Ошибка в программе", null);
            }
        }
    }
}
