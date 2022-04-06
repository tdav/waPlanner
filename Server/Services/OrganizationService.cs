using Arch.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;

namespace waPlanner.Services
{
    public interface IOrganizationService
    {
        Task InsertAsync(tbOrganization organization);
        Task UpdateAsync(tbOrganization organization);
        void Delete(int id);
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
        public async Task InsertAsync(tbOrganization organization)
        {
            organization.CreateDate = DateTime.Now;
            organization.CreateUser = 1;
            organization.Status = 1;
            await db.tbOrganizations.AddAsync(organization);
            await db.SaveChangesAsync();
        }
        public async Task UpdateAsync(tbOrganization organization)
        {
            organization.UpdateDate = DateTime.Now;
            organization.UpdateUser = 1;
            db.tbOrganizations.Update(organization);
            await db.SaveChangesAsync();
        }
        public void Delete(int id)
        {
            var organizationId = db.tbOrganizations.Find(id);
            db.tbOrganizations.Remove(organizationId);
            db.SaveChanges();
        }
        public async Task<tbOrganization> GetOrgByIdAsync(int id)
        {
            return await db.tbOrganizations.FindAsync(id);
        }
        public async Task<tbOrganization[]> GetAllOrgsAsync()
        {
            return await db.tbOrganizations.AsNoTracking().ToArrayAsync();
        }
    }
}
