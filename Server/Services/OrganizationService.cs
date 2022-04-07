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
        Task<tbOrganization> GetOrgByIdAsync(int id);
        Task<tbOrganization[]> GetAllOrgsAsync();
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
            var addOrganization = new tbOrganization();

            if (organization.ChatId.HasValue)
                addOrganization.ChatId = organization.ChatId.Value;

            if (organization.latitude.HasValue)
                addOrganization.Latitude = organization.latitude.Value;

            if (organization.longitude.HasValue)
                addOrganization.Longitude = organization.longitude.Value;

            if (organization.TypeId.HasValue)
                addOrganization.TypeId = organization.TypeId.Value;

            addOrganization.CreateDate = DateTime.Now;
            addOrganization.CreateUser = 1;
            addOrganization.Status = organization.Status;
            addOrganization.Address = organization.address;
            await db.tbOrganizations.AddAsync(addOrganization);
            await db.SaveChangesAsync();


        }
        public async Task UpdateOrganizationAsync(int organization_id, viOrganization organziation)
        {
            var updatedOrganization = await db.tbOrganizations.FindAsync(organization_id);

            if (organziation.ChatId.HasValue)
                updatedOrganization.ChatId = organziation.ChatId.Value;

            if (organziation.latitude.HasValue)
                updatedOrganization.Latitude = organziation.latitude.Value;

            if (organziation.longitude.HasValue)
                updatedOrganization.Longitude = organziation.longitude.Value;

            if (organziation.TypeId.HasValue)
                updatedOrganization.TypeId = organziation.TypeId.Value;

            updatedOrganization.Status = organziation.Status;
            updatedOrganization.UpdateDate = DateTime.Now;
            updatedOrganization.UpdateUser = 1;
            await db.SaveChangesAsync();
        }

        public async Task UpdateOrganizationStatus(int organizatin_id, int status)
        {
            var organiztion = await db.tbOrganizations.FindAsync(organizatin_id);
            organiztion.Status = status;
            organiztion.UpdateDate = DateTime.Now;
            organiztion.UpdateUser = 1;
            await db.SaveChangesAsync();
        }
        public async Task<tbOrganization> GetOrgByIdAsync(int id)
        {
            return await db.tbOrganizations.AsNoTracking().FirstOrDefaultAsync(x => x.Status == 1);
        }
        public async Task<tbOrganization[]> GetAllOrgsAsync()
        {
            return await db.tbOrganizations.Where(x => x.Status == 1).AsNoTracking().ToArrayAsync();
        }
    }
}
