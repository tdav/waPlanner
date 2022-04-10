using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;
using waPlanner.ModelViews;

namespace waPlanner.Services
{
    public interface IOrganizationService
    {
        Task InsertOrganizationAsync(viOrganization organization);
        Task UpdateOrganizationAsync(int organization_id, viOrganization organziation);
        Task UpdateOrganizationStatus(int organization_id, int status);
        Task<spOrganization> GetOrgByIdAsync(int id);
        Task<spOrganization[]> GetAllOrgsAsync();
    }
    public class OrganizationService: IOrganizationService
    {
        private readonly MyDbContext db;
        public OrganizationService(MyDbContext db)
        {
            this.db = db;
        }
        public async Task InsertOrganizationAsync(viOrganization organization)
        {
            var addOrganization = new spOrganization();

            if (organization.ChatId.HasValue)
                addOrganization.ChatId = organization.ChatId.Value;

            if (organization.latitude.HasValue)
                addOrganization.Latitude = organization.latitude.Value;

            if (organization.longitude.HasValue)
                addOrganization.Longitude = organization.longitude.Value;

            if(organization.SpecializationId.HasValue)
                addOrganization.SpecializationId = organization.SpecializationId.Value;

            addOrganization.Name = organization.Name;
            addOrganization.CreateDate = DateTime.Now;
            addOrganization.CreateUser = 1;
            addOrganization.Status = organization.Status.Value;
            addOrganization.Address = organization.address;
            await db.spOrganizations.AddAsync(addOrganization);
            await db.SaveChangesAsync();


        }
        public async Task UpdateOrganizationAsync(int organization_id, viOrganization organization)
        {
            var updatedOrganization = await db.spOrganizations.FindAsync(organization_id);

            if (organization.ChatId.HasValue)
                updatedOrganization.ChatId = organization.ChatId.Value;

            if (organization.latitude.HasValue)
                updatedOrganization.Latitude = organization.latitude.Value;

            if (organization.longitude.HasValue)
                updatedOrganization.Longitude = organization.longitude.Value;

            if(organization.SpecializationId.HasValue)
                updatedOrganization.SpecializationId = organization.SpecializationId.Value;

            if(organization.Status.HasValue)
                updatedOrganization.Status = organization.Status.Value;

            updatedOrganization.Name = organization.Name;
            updatedOrganization.UpdateDate = DateTime.Now;
            updatedOrganization.UpdateUser = 1;
            await db.SaveChangesAsync();
        }

        public async Task UpdateOrganizationStatus(int organization_id, int status)
        {
            var organiztion = await db.spOrganizations.FindAsync(organization_id);
            organiztion.Status = status;
            organiztion.UpdateDate = DateTime.Now;
            organiztion.UpdateUser = 1;
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
