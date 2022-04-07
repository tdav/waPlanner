using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;

namespace waPlanner.Services
{
    public interface ICategoryService
    {
        Task InsertAsync(spCategory value);
        Task UpdateAsync(spCategory value);
        Task<spCategory[]> GetAllAsync();
        //Task<spCategory> GetCategoryByIdAsync(int id);
    }

    public class CategoryService : ICategoryService
    {
        private readonly MyDbContext myDb;

        public CategoryService(MyDbContext myDb)
        {
            this.myDb = myDb;
        }

        //public async Task<spCategory> GetCategoryByIdAsync(int id)
        //{
            
        //}

        public async Task<spCategory[]> GetAllAsync()
        {
            return await myDb.spCategories.AsNoTracking().ToArrayAsync();
        }

        public async Task InsertAsync(spCategory value)
        {
            value.CreateDate = DateTime.Now;
            value.CreateUser=1;
            value.Status = 1;
            await myDb.spCategories.AddAsync(value);
            await myDb.SaveChangesAsync();
        }

        public async Task UpdateAsync(spCategory value)
        {
            value.UpdateDate = DateTime.Now;
            value.UpdateUser = 1;
            myDb.spCategories.Update(value);
            await myDb.SaveChangesAsync();
        }
    }
}
