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
    public interface ISchedulerService
    {
        Task InsertSchedulerAsync(viScheduler scheduler);
        Task UpdateSchedulerAsync(int scheduler_id, viScheduler scheduler);
        Task UpdateSchedulerStatus(int scheduler_id, byte status);        
        Task<viScheduler> GetSchedulerByIdAsync(int id);
        Task<List<viScheduler>> GetAllSchedulersByOrgAsync(int organization_id);
    }
    public class SchedulerService: ISchedulerService
    {
        private readonly MyDbContext db;
        public SchedulerService(MyDbContext db)
        {
            this.db = db; 
        }

        public async Task InsertSchedulerAsync(viScheduler scheduler)
        {
            var addScheduler = new tbScheduler();

            if (scheduler.UserId.HasValue)
                addScheduler.UserId = scheduler.UserId.Value;

            if (scheduler.StaffId.HasValue)
                addScheduler.DoctorId = scheduler.StaffId.Value;

            if(scheduler.AppointmentDateTime.HasValue)
                addScheduler.AppointmentDateTime = scheduler.AppointmentDateTime.Value;

            if(scheduler.CategoryId.HasValue)
                addScheduler.CategoryId = scheduler.CategoryId.Value;

            if(scheduler.OrganizationId.HasValue)
                addScheduler.OrganizationId = scheduler.OrganizationId.Value;

            addScheduler.AdInfo = scheduler.AdInfo;
            addScheduler.Status = 1;
            addScheduler.CreateUser = 1;
            addScheduler.CreateDate = DateTime.Now;
            await db.tbSchedulers.AddAsync(addScheduler);
            await db.SaveChangesAsync();
        }
        public async Task UpdateSchedulerAsync(int scheduler_id, viScheduler scheduler)
        {
            var updateScheduler = await db.tbSchedulers.FindAsync(scheduler_id);

            if(scheduler.UserId.HasValue)
                updateScheduler.UserId = scheduler.UserId.Value;

            if (scheduler.StaffId.HasValue)
                updateScheduler.DoctorId = scheduler.StaffId.Value;

            if(scheduler.CategoryId.HasValue)
                updateScheduler.CategoryId = scheduler.CategoryId.Value;

            if(scheduler.OrganizationId.HasValue)
                updateScheduler.OrganizationId = scheduler.OrganizationId.Value;

            if(scheduler.AppointmentDateTime.HasValue)
                updateScheduler.AppointmentDateTime = scheduler.AppointmentDateTime.Value;

            if(scheduler.Status.HasValue)
                updateScheduler.Status = scheduler.Status.Value;

            updateScheduler.AdInfo = scheduler.AdInfo;
            updateScheduler.UpdateDate = DateTime.Now;
            updateScheduler.UpdateUser = 1;
            await db.SaveChangesAsync();
        }
        public async Task UpdateSchedulerStatus(int scheduler_id, byte status)
        {
            var sh = await db.tbSchedulers.FindAsync(scheduler_id);
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
                    UserId = x.UserId,
                    User = $"{x.User.Name} {x.User.Surname} {x.User.Patronymic}",
                    StaffId = x.DoctorId,
                    Staff = $"{x.Doctor.Name} {x.Doctor.Surname} {x.Doctor.Patronymic}",
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

        public async Task<List<viScheduler>> GetAllSchedulersByOrgAsync(int organization_id)
        {
            return await db.tbSchedulers
                .AsNoTracking()
                .Where(x => x.OrganizationId == organization_id)
                .Select(x => new viScheduler
                {
                    UserId = x.UserId,
                    User = $"{x.User.Name} {x.User.Surname} {x.User.Patronymic}",
                    StaffId = x.DoctorId,
                    Staff = $"{x.Doctor.Name} {x.Doctor.Surname} {x.Doctor.Patronymic}",
                    CategoryId = x.CategoryId,
                    Category = x.Category.NameUz,
                    OrganizationId = x.OrganizationId,
                    Organization = x.Organization.Name,
                    AppointmentDateTime = x.AppointmentDateTime,
                    AdInfo = x.AdInfo,
                    Status = x.Status
                })
                .ToListAsync();
        }
    }
}
