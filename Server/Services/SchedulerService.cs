using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;

namespace waPlanner.Services
{
    public interface ISchedulerService
    {
        Task InsertAsync(tbScheduler scheduler);
        Task UpdateAsync(tbScheduler scheduler);
        void Delete(tbScheduler scheduler);
        void Delete(int id);
        Task<tbScheduler> GetSchedulerByIdAsync(int id);
        Task<tbScheduler[]> GetAllSchedulersAsync();
    }
    public class SchedulerService: ISchedulerService
    {
        private readonly MyDbContext db;
        public SchedulerService(MyDbContext db)
        {
            this.db = db;
        }
        public async Task InsertAsync(tbScheduler scheduler)
        {
            await db.tbSchedulers.AddAsync(scheduler);
            await db.SaveChangesAsync();
        }
        public async Task UpdateAsync(tbScheduler scheduler)
        {
            db.tbSchedulers.Update(scheduler);
            await db.SaveChangesAsync();
        }
        public void Delete(tbScheduler scheduler)
        {
            db.tbSchedulers.Remove(scheduler);
            db.SaveChanges();
        }
        public void Delete(int id)
        {
            var scheduler = db.tbSchedulers.Find(id);
            db.tbSchedulers.Remove(scheduler);
            db.SaveChanges();
        }
        public async Task<tbScheduler> GetSchedulerByIdAsync(int id)
        {
            return await db.tbSchedulers.FindAsync(id);
        }
        public async Task<tbScheduler[]> GetAllSchedulersAsync()
        {
            return await db.tbSchedulers.ToArrayAsync();
        }
    }
}
