using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;
using waPlanner.Extensions;
using waPlanner.ModelViews;
using waPlanner.TelegramBot;

namespace waPlanner.Services
{
    public interface ICategoryService
    {
        Task<int> AddCategoryAsync(viCategory value);
        Task UpdateAsync(viCategory value);
        Task<viCategory[]> GetAllCategoriesAsync();
        Task ChangeCategoryStatus(viCategory value, int status);
        Task<viCategory> GetCategoryByIdAsync(int category_id);
    }

    public class CategoryService : ICategoryService
    {
        private readonly MyDbContext db;
        private readonly IHttpContextAccessorExtensions accessor;
        public CategoryService(MyDbContext db, IHttpContextAccessorExtensions accessor)
        {
            this.db = db;
            this.accessor = accessor;
        }

        public async Task<viCategory> GetCategoryByIdAsync(int category_id)
        {
            return await db.spCategories
                .AsNoTracking()
                .Where(x => x.Id == category_id)
                .Select(x => new viCategory
                {
                    Id = x.Id,
                    NameLt = x.NameLt,
                    NameRu = x.NameRu,
                    NameUz = x.NameUz,
                    OrganizationId = x.OrganizationId,
                    Organization = x.Organization.Name,
                    Status = x.Status,
                })
                .FirstOrDefaultAsync();
        }

        public async Task<viCategory[]> GetAllCategoriesAsync()
        {
            int org_id = accessor.GetOrgId();
            return await db.spCategories
                .AsNoTracking()
                .Where(x => x.OrganizationId == org_id)
                .Select(x => new viCategory
                {
                    Id = x.Id,
                    NameLt = x.NameLt,
                    NameRu = x.NameRu,
                    NameUz = x.NameUz,
                    OrganizationId = org_id,
                    Organization = x.Organization.Name,
                    Status = x.Status,
                })
                .ToArrayAsync();
        }

        public async Task<int> AddCategoryAsync(viCategory value)
        {
            var category = new spCategory();
            int user_id = accessor.GetId();
            int org_id = accessor.GetOrgId();
            int role_id = accessor.GetRoleId();

            if (role_id == (int)UserRoles.SUPER_ADMIN)
                org_id = value.OrganizationId.Value;

            category.Status = 1;
            category.OrganizationId = org_id;
            category.NameRu = value.NameRu;
            category.NameUz = value.NameUz;
            category.NameLt = value.NameLt;
            category.CreateDate = DateTime.Now;
            category.CreateUser = user_id;
            db.spCategories.Add(category);
            await db.SaveChangesAsync();

            return category.Id;
        }

        public async Task UpdateAsync(viCategory value)
        {
            var category = await db.spCategories.FindAsync(value.Id);
            int org_id = accessor.GetOrgId();
            int user_id = accessor.GetId();

            category.OrganizationId = org_id;

            if (value.Status.HasValue)
                category.Status = value.Status.Value;

            if (value.NameRu is not null)
                category.NameRu = value.NameRu;

            if (value.NameUz is not null)
                category.NameUz = value.NameUz;

            if (value.NameLt is not null)
                category.NameLt = value.NameLt;

            category.UpdateDate = DateTime.Now;
            category.UpdateUser = user_id;
            await db.SaveChangesAsync();
        }

        public async Task ChangeCategoryStatus(viCategory value, int status)
        {
            int user_id = accessor.GetId();
            int org_id = accessor.GetOrgId();
            var get_category = await db.spCategories.AsNoTracking().FirstAsync(c => c.Id == value.Id && c.OrganizationId == org_id);
            var category = await db.spCategories.FindAsync(get_category.Id);
            category.Status = status;
            category.UpdateDate = DateTime.Now;
            category.UpdateUser = user_id;
            await db.SaveChangesAsync();
        }
    }
}
