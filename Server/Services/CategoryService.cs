using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;
using waPlanner.ModelViews;

namespace waPlanner.Services
{
    public interface ICategoryService
    {
        Task AddCategoryAsync(viCategory value);
        Task UpdateAsync(int category_id, viCategory value);
        Task<List<viCategory>> GetAllCategoryByOrgAsync(int organization_id);
        Task ChangeCategoryStatus(int category_id, int status);
        Task<viCategory> GetCategoryByIdAsync(int category_id);
    }

    public class CategoryService : ICategoryService
    {
        private readonly MyDbContext db;

        public CategoryService(MyDbContext db)
        {
            this.db = db;
        }

        public async Task<viCategory> GetCategoryByIdAsync(int category_id)
        {
            return await db.spCategories
                .AsNoTracking()
                .Where(x => x.Id == category_id)
                .Select(x => new viCategory
                {
                    NameLt = x.NameLt,
                    NameRu = x.NameRu,
                    NameUz = x.NameUz,
                    OrganizationId = x.OrganizationId,
                    Organization = x.Organization.Name,
                    Status = x.Status,
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<viCategory>> GetAllCategoryByOrgAsync(int organization_id)
        {
            return await db.spCategories
                .AsNoTracking()
                .Where(x => x.OrganizationId == organization_id)
                .Select(x => new viCategory
                {
                    NameLt = x.NameLt,
                    NameRu = x.NameRu,
                    NameUz = x.NameUz,
                    OrganizationId = organization_id,
                    Organization = x.Organization.Name,
                    Status = x.Status,
                })
                .ToListAsync();
        }

        public async Task AddCategoryAsync(viCategory value)
        {
            var category = new spCategory();
            
            if(value.OrganizationId.HasValue)
                category.OrganizationId = value.OrganizationId;

            if(value.Status.HasValue)
                category.Status = value.Status.Value;

            category.NameRu = value.NameRu;
            category.NameUz = value.NameUz;
            category.NameLt = value.NameLt;
            category.CreateDate = DateTime.Now;
            category.CreateUser = 1;
            db.spCategories.Add(category);
            await db.SaveChangesAsync();
        }

        public async Task UpdateAsync(int category_id, viCategory value)
        {
            var category = await db.spCategories.FindAsync(category_id);

            if (value.OrganizationId.HasValue)
                category.OrganizationId = value.OrganizationId;

            if (value.Status.HasValue)
                category.Status = value.Status.Value;

            if (value.NameRu is not null)
                category.NameRu = value.NameRu;

            if (value.NameUz is not null)
                category.NameUz = value.NameUz;

            if (value.NameLt is not null)
                category.NameLt = value.NameLt;

            category.UpdateDate = DateTime.Now;
            category.UpdateUser = 1;
            await db.SaveChangesAsync();
        }

        public async Task ChangeCategoryStatus(int category_id, int status)
        {
            var category = await db.spCategories.FindAsync(category_id);
            category.Status = status;
            category.UpdateDate = DateTime.Now;
            category.UpdateUser = 1;
            await db.SaveChangesAsync();
        }
    }
}
