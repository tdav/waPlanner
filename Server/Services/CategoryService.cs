using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;

namespace waPlanner.Services
{
    public interface ICategoryService
    {
        Task InsertAsync(spCategory value);
        Task UpdateAsync(spCategory value);
        void Delete(spCategory value);
        void Delete(int id);
        Task<spCategory[]> GetAllAsync();
        Task<spCategory> GetAsync(int id);
    }

    public class CategoryService : ICategoryService
    {
        private readonly MyDbContext myDb;

        public CategoryService(MyDbContext myDb)
        {
            this.myDb = myDb;
        }

        public void Delete(spCategory value)
        {
            myDb.spCategories.Remove(value);
            myDb.SaveChanges();
        }

        public void Delete(int id)
        {
            var value = myDb.spCategories.Find(id);
            myDb.Remove(value);
            myDb.SaveChanges();
        }

        public async Task<spCategory> GetAsync(int id)
        {
            return await myDb.spCategories.FindAsync(id);
        }

        public async Task<spCategory[]> GetAllAsync()
        {
            return await myDb.spCategories.ToArrayAsync();
        }

        public async Task InsertAsync(spCategory value)
        {
            await myDb.spCategories.AddAsync(value);
            await myDb.SaveChangesAsync();
        }

        public async Task UpdateAsync(spCategory value)
        {
            myDb.spCategories.Update(value);
            await myDb.SaveChangesAsync();
        }
    }
}
