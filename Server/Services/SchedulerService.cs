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
    public interface ISchedulerService
    {
        Task<int> AddSchedulerAsync(viScheduler scheduler);
        Task UpdateSchedulerAsync(viScheduler scheduler);
        Task UpdateSchedulerStatus(viScheduler scheduler, int status);        
        Task<viScheduler> GetSchedulerByIdAsync(int id);
        Task<viEvents[]> GetAllSchedulersByOrgAsync();
    }
    public class SchedulerService: ISchedulerService
    {
        private readonly MyDbContext db;
        private readonly IHttpContextAccessorExtensions accessor;
        public SchedulerService(MyDbContext db, IHttpContextAccessorExtensions accessor)
        {
            this.accessor = accessor;
            this.db = db; 
        }

        public async Task<int> AddSchedulerAsync(viScheduler scheduler)
        {
            var addScheduler = new tbScheduler();
            int org_id = accessor.GetOrgId();
            int user_id = accessor.GetId();

            if (scheduler.UserId.HasValue)
                addScheduler.UserId = scheduler.UserId.Value;

            if (scheduler.StaffId.HasValue)
                addScheduler.StaffId = scheduler.StaffId.Value;

            if(scheduler.AppointmentDateTime.HasValue)
                addScheduler.AppointmentDateTime = scheduler.AppointmentDateTime.Value;

            if(scheduler.CategoryId.HasValue)
                addScheduler.CategoryId = scheduler.CategoryId.Value;

            addScheduler.OrganizationId = org_id;

            addScheduler.AdInfo = scheduler.AdInfo;
            addScheduler.Status = 1;
            addScheduler.CreateUser = user_id;
            addScheduler.CreateDate = DateTime.Now;
            await db.tbSchedulers.AddAsync(addScheduler);
            await db.SaveChangesAsync();

            return addScheduler.Id;
        }
        public async Task UpdateSchedulerAsync(viScheduler scheduler)
        {
            var updateScheduler = await db.tbSchedulers.FindAsync(scheduler.SchedulerId);
            int org_id = accessor.GetOrgId();

            if (scheduler.UserId.HasValue)
                updateScheduler.UserId = scheduler.UserId.Value;

            if (scheduler.StaffId.HasValue)
                updateScheduler.StaffId = scheduler.StaffId.Value;

            if(scheduler.CategoryId.HasValue)
                updateScheduler.CategoryId = scheduler.CategoryId.Value;

            updateScheduler.OrganizationId = org_id;

            if(scheduler.AppointmentDateTime.HasValue)
                updateScheduler.AppointmentDateTime = scheduler.AppointmentDateTime.Value;

            if(scheduler.Status.HasValue)
                updateScheduler.Status = scheduler.Status.Value;

            updateScheduler.AdInfo = scheduler.AdInfo;
            updateScheduler.UpdateDate = DateTime.Now;
            updateScheduler.UpdateUser = 1;
            await db.SaveChangesAsync();
        }
        public async Task UpdateSchedulerStatus(viScheduler scheduler, int status)
        {
            var sh = await db.tbSchedulers.FindAsync(scheduler.SchedulerId);
            sh.Status = status;
            sh.UpdateUser = 1;
            sh.UpdateDate = DateTime.Now;
            await db.SaveChangesAsync();
        }
     
        public async Task<viScheduler> GetSchedulerByIdAsync(int id)
        {
            return await db.tbSchedulers
                .AsNoTracking()
                .Where(x => x.Id == id && x.Status == 1)
                .Select(x => new viScheduler
                {
                    SchedulerId = x.Id,
                    UserId = x.UserId,
                    User = $"{x.User.Name} {x.User.Surname} {x.User.Patronymic}",
                    StaffId = x.StaffId,
                    Staff = $"{x.Staff.Name} {x.Staff.Surname} {x.Staff.Patronymic}",
                    CategoryId = x.CategoryId,
                    Category = x.Category.NameUz,
                    OrganizationId = x.OrganizationId,
                    Organization = x.Organization.Name,
                    AppointmentDateTime = x.AppointmentDateTime,
                    AdInfo = x.AdInfo,
                    Status = x.Status
                })
                .FirstAsync();
        }

        public async Task<viEvents[]> GetAllSchedulersByOrgAsync()
        {
            int org_id = accessor.GetOrgId();
            return await db.tbSchedulers
                .AsNoTracking()
                .Where(x => x.OrganizationId == org_id && x.AppointmentDateTime.Date >= DateTime.Now.AddMonths(-3) &&
                                                                   x.AppointmentDateTime.Date.Month <= DateTime.Now.Date.Month)
                .Select(x => new viEvents
                {
                    Id = x.Id,
                    StaffId = x.StaffId,
                    Staff = $"{x.Staff.Surname} {x.Staff.Name} {x.Staff.Patronymic}",
                    UserId = x.UserId,
                    User = $"{x.User.Name} {x.User.Surname} {x.User.Patronymic}",
                    Start = x.AppointmentDateTime,
                    End = x.AppointmentDateTime.AddMinutes(30),
                    AdInfo = x.AdInfo
                })
                .ToArrayAsync();
        }
    }
}
