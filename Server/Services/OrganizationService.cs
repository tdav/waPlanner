using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;
using waPlanner.Extensions;
using waPlanner.ModelViews;


namespace waPlanner.Services
{
    public interface IOrganizationService
    {
        Task<int> InsertOrganizationAsync(viOrganization organization, string phoneNum);
        Task UpdateOrganizationAsync(viOrganization organziation);
        Task UpdateOrganizationStatus(int org_id, int status);
        Task<spOrganization> GetOrgByIdAsync(int id);
        Task<spOrganization[]> GetAllOrgsAsync();
    }
    public class OrganizationService: IOrganizationService
    {
        private readonly MyDbContext db;
        private readonly IHttpContextAccessorExtensions accessor;
        public OrganizationService(MyDbContext db, IHttpContextAccessorExtensions accessor)
        {
            this.accessor = accessor;
            this.db = db;
        }
        public async Task<int> InsertOrganizationAsync(viOrganization organization, string phoneNum)
        {
            int user_id = accessor.GetId();
            var addOrganization = new spOrganization();

            if (organization.ChatId.HasValue)
                addOrganization.ChatId = organization.ChatId.Value;

            if (organization.Latitude.HasValue)
                addOrganization.Latitude = organization.Latitude.Value;

            if (organization.Longitude.HasValue)
                addOrganization.Longitude = organization.Longitude.Value;

            if(organization.SpecializationId.HasValue)
                addOrganization.SpecializationId = organization.SpecializationId.Value;

            if (organization.DinnerTimeStart.HasValue)
                addOrganization.BreakTimeStart = organization.DinnerTimeStart.Value;

            if (organization.DinnerTimeEnd.HasValue)
                addOrganization.BreakTimeEnd = organization.DinnerTimeEnd.Value;

            addOrganization.WorkStart = organization.WorkTimeStart;
            addOrganization.WorkEnd = organization.WorkTimeEnd;
            addOrganization.Name = organization.Name;
            addOrganization.ChatId = await ClientTelegram.ClientTelegram.CreateClientGroup(addOrganization.Name, phoneNum);
            addOrganization.CreateDate = DateTime.Now;
            addOrganization.CreateUser = user_id;
            addOrganization.Status = 1;
            addOrganization.Address = organization.Address;
            await db.spOrganizations.AddAsync(addOrganization);
            await db.SaveChangesAsync();

            return addOrganization.Id;
        }
        public async Task UpdateOrganizationAsync(viOrganization organization)
        {
            int user_id = accessor.GetId();
            var updatedOrganization = await db.spOrganizations.FindAsync(organization.Id);

            if (organization.ChatId.HasValue)
                updatedOrganization.ChatId = organization.ChatId.Value;

            if (organization.Latitude.HasValue)
                updatedOrganization.Latitude = organization.Latitude.Value;

            if (organization.Longitude.HasValue)
                updatedOrganization.Longitude = organization.Longitude.Value;

            if(organization.SpecializationId.HasValue)
                updatedOrganization.SpecializationId = organization.SpecializationId.Value;

            if(organization.Status.HasValue)
                updatedOrganization.Status = organization.Status.Value;

            if (organization.DinnerTimeStart.HasValue)
                updatedOrganization.BreakTimeStart = organization.DinnerTimeStart.Value;

            if (organization.DinnerTimeEnd.HasValue)
                updatedOrganization.BreakTimeEnd = organization.DinnerTimeEnd.Value;

            if (organization.Name is not null)
                updatedOrganization.Name = organization.Name;

            updatedOrganization.UpdateDate = DateTime.Now;
            updatedOrganization.UpdateUser = user_id;
            await db.SaveChangesAsync();
        }

        public async Task UpdateOrganizationStatus(int org_id, int status)
        {
            int user_id = accessor.GetId();
            var organiztion = await db.spOrganizations.FindAsync(org_id);
            organiztion.Status = status;
            organiztion.UpdateDate = DateTime.Now;
            organiztion.UpdateUser = user_id;
            await db.SaveChangesAsync();
        }

        public async Task<spOrganization> GetOrgByIdAsync(int organization_id)
        {
            return await db.spOrganizations.AsNoTracking().FirstAsync(x => x.Status == 1 && x.Id == organization_id);
        }
        public async Task<spOrganization[]> GetAllOrgsAsync()
        {
            return await db.spOrganizations.Where(x => x.Status == 1).AsNoTracking().ToArrayAsync();
        }
    }
}
