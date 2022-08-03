using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;
using waPlanner.Extensions;
using waPlanner.Interfaces;
using waPlanner.ModelViews;
using waPlanner.Utils;

namespace waPlanner.Services
{
    public interface IOrganizationService
    {
        Task<Answer<long>> InsertOrganizationAsync(viOrganization organization);
        Task<AnswerBasic> UpdateOrganizationAsync(spOrganization organziation);
        Task<AnswerBasic> UpdateOrganizationStatus(viSetStatus status);
        Task<Answer<spOrganization>> GetOrgByIdAsync(int id);
        Task<Answer<spOrganization[]>> GetAllOrgsAsync();
    }
    public class OrganizationService : IOrganizationService, IAutoRegistrationScopedLifetimeService
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

                if (organization.BreakTimeStart.HasValue)
                    addOrganization.BreakTimeStart = organization.BreakTimeStart.Value;

                if (organization.BreakTimeEnd.HasValue)
                    addOrganization.BreakTimeEnd = organization.BreakTimeEnd.Value;

                if (organization.Rating.HasValue)
                    addOrganization.Rating = organization.Rating.Value;

                addOrganization.SpecializationId = organization.SpecializationId.Value;
                addOrganization.WorkStart = organization.WorkStart;
                addOrganization.WorkEnd = organization.WorkEnd;
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

        public async Task<AnswerBasic> UpdateOrganizationAsync(spOrganization organization)
        {
            try
            {
                int user_id = accessor.GetId();
                var updatedOrganization = await db.spOrganizations.FindAsync(organization.Id);

                updatedOrganization.ChatId = organization.ChatId;

                updatedOrganization.Latitude = organization.Latitude;

                updatedOrganization.Longitude = organization.Longitude;

                if (organization.SpecializationId.HasValue)
                    updatedOrganization.SpecializationId = organization.SpecializationId.Value;

                updatedOrganization.Status = organization.Status;

                updatedOrganization.BreakTimeStart = organization.BreakTimeStart;

                updatedOrganization.BreakTimeEnd = organization.BreakTimeEnd;

                if (organization.Name is not null)
                    updatedOrganization.Name = organization.Name;

                if (organization.PhotoPath is not null)
                    updatedOrganization.PhotoPath = organization.PhotoPath;

                if (organization.SpecializationId.HasValue)
                    updatedOrganization.SpecializationId = organization.SpecializationId.Value;

                if (organization.Info is not null)
                    updatedOrganization.Info = organization.Info;

                if (organization.Address is not null)
                    updatedOrganization.Address = organization.Address;

                if (organization.WorkStart.HasValue)
                    updatedOrganization.WorkStart = organization.WorkStart.Value;

                if (organization.WorkEnd.HasValue)
                    updatedOrganization.WorkEnd = organization.WorkEnd.Value;

                if (organization.MessageLt is not null)
                    updatedOrganization.MessageLt = organization.MessageLt;

                if (organization.MessageRu is not null)
                    updatedOrganization.MessageRu = organization.MessageRu;

                if (organization.MessageUz is not null)
                    updatedOrganization.MessageUz = organization.MessageUz;

                if (organization.Rating.HasValue)
                    updatedOrganization.Rating = organization.Rating.Value;

                updatedOrganization.UpdateDate = DateTime.Now;
                updatedOrganization.UpdateUser = user_id;
                updatedOrganization.Status = 1;
                await db.SaveChangesAsync();

                return new AnswerBasic(true, "");
            }
            catch (Exception e)
            {
                logger.LogError($"OrganizationService.UpdateOrganizationAsync Error: {e.Message} Model: {organization.ToJson()}");
                return new AnswerBasic(false, "Ошибка программы");
            }
        }

        public async Task<AnswerBasic> UpdateOrganizationStatus(viSetStatus status)
        {
            try
            {
                int user_id = accessor.GetId();
                var organiztion = await db.spOrganizations.FindAsync(status.Id);

                organiztion.Status = status.Status;
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
                var organization = await db.spOrganizations.AsNoTracking().FirstOrDefaultAsync(x => x.Status == 1 && x.Id == organization_id);
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
                var organizations = await db.spOrganizations.AsNoTracking().Where(x => x.Status == 1).ToArrayAsync();
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
