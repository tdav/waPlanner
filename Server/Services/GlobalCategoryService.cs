using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;

namespace waPlanner.Services
{
    public interface IGlobalCategoryService
    {
        Task InsertAsync(spGlobalCategory GlobalCategory);
        Task UpdateAsync(spGlobalCategory GlobalCategory);
        void Delete (spGlobalCategory GlobalCategory);
        void Delete(int id);
        Task<spGlobalCategory> GetByIdAsync(int id);
        Task<spGlobalCategory[]> GetAllAsync();
    }
    public class GlobalCategoryService: IGlobalCategoryService
    {
        private readonly MyDbContext db;
        public GlobalCategoryService(MyDbContext db)
        {
            this.db = db;
        }
        public async Task InsertAsync(spGlobalCategory globalCategory)
        {
            await db.spGlobalCategories.AddAsync(globalCategory);
            await db.SaveChangesAsync();
        }
        public async Task UpdateAsync(spGlobalCategory globalCategory)
        {
            db.spGlobalCategories.Update(globalCategory);
            await db.SaveChangesAsync();
        }
        public void Delete(spGlobalCategory globalCategory)
        {
            db.spGlobalCategories.Remove(globalCategory);
            db.SaveChanges();
        }
        public void Delete(int id)
        {
            var globalCategory = db.spGlobalCategories.Find(id);
            db.spGlobalCategories.Remove(globalCategory);
            db.SaveChanges();
        }
        public async Task<spGlobalCategory> GetByIdAsync(int id)
        {
            return await db.spGlobalCategories.FindAsync(id);
        }
        public async Task<spGlobalCategory[]> GetAllAsync()
        {
            return await db.spGlobalCategories.ToArrayAsync();
        }
    }
}
