using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;
using waPlanner.Extensions;
using waPlanner.Interfaces;
using waPlanner.ModelViews;
using waPlanner.Utils;

namespace waPlanner.Services
{
    public interface ISchedulerService
    {
        Task<Answer<int>> AddSchedulerAsync(viScheduler scheduler);
        Task<Answer<viScheduler>> UpdateSchedulerAsync(viScheduler scheduler);
        Task<AnswerBasic> UpdateSchedulerStatus(viSetStatus status);
        Task<Answer<viScheduler[]>> GetSchedulerByUserIdAsync(int user_id);
        Task<Answer<viEvents[]>> GetAllSchedulersByOrgAsync();
        Task<Answer<TimeSpan[]>> GetStaffBusyTime(int staff_id, DateTime date);
        Task<Answer<viEvents[]>> SearchScheduler(string staff_name);

    }
    public class SchedulerService : ISchedulerService, IAutoRegistrationScopedLifetimeService
    {
        private readonly MyDbContext db;
        private readonly IHttpContextAccessorExtensions accessor;
        private readonly ILogger<SchedulerService> logger;
        public SchedulerService(MyDbContext db, IHttpContextAccessorExtensions accessor, ILogger<SchedulerService> logger)
        {
            this.accessor = accessor;
            this.db = db;
            this.logger = logger;
        }

        public async Task<Answer<int>> AddSchedulerAsync(viScheduler scheduler)
        {
            try
            {
                int org_id = accessor.GetOrgId();
                int user_id = accessor.GetId();
                var addScheduler = new tbScheduler
                {
                    UserId = scheduler.UserId.Value,
                    StaffId = scheduler.StaffId.Value,
                    AppointmentDateTime = scheduler.AppointmentDateTime.Value,
                    CategoryId = scheduler.CategoryId.Value,
                    OrganizationId = org_id,
                    AdInfo = scheduler.AdInfo,
                    Status = 1,
                    CreateUser = user_id,
                    CreateDate = DateTime.Now
                };
                await db.tbSchedulers.AddAsync(addScheduler);
                await db.SaveChangesAsync();

                int sch_id = addScheduler.Id;

                return new Answer<int>(true, "", sch_id);
            }
            catch (Exception e)
            {
                logger.LogError($"SchedulerService.AddSchedulerAsync Error:{e.Message} Model: {scheduler.ToJson()}");
                return new Answer<int>(false, "Ошибка программы", 0);
            }

        }
        public async Task<Answer<viScheduler>> UpdateSchedulerAsync(viScheduler scheduler)
        {
            try
            {
                var updateScheduler = await db.tbSchedulers.FindAsync(scheduler.SchedulerId);
                int org_id = accessor.GetOrgId();
                int user_id = accessor.GetId();

                updateScheduler.UserId = scheduler.UserId.Value;
                updateScheduler.StaffId = scheduler.StaffId.Value;
                updateScheduler.CategoryId = scheduler.CategoryId.Value;
                updateScheduler.OrganizationId = org_id;
                updateScheduler.AppointmentDateTime = scheduler.AppointmentDateTime.Value;
                updateScheduler.Status = scheduler.Status.Value;

                updateScheduler.AdInfo = scheduler.AdInfo;
                updateScheduler.UpdateDate = DateTime.Now;
                updateScheduler.UpdateUser = user_id;
                await db.SaveChangesAsync();

                return new Answer<viScheduler>(true, "", null);
            }
            catch (Exception e)
            {
                logger.LogError($"SchedulerService.UpdateSchedulerAsync Error:{e.Message} Model: {scheduler.ToJson()}");
                return new Answer<viScheduler>(false, "Ошибка программы", null);
            }

        }
        public async Task<AnswerBasic> UpdateSchedulerStatus(viSetStatus status)
        {
            try
            {
                int user_id = accessor.GetId();
                var sh = await db.tbSchedulers.FindAsync(status.Id);
                sh.Status = status.Status;
                sh.UpdateUser = user_id;
                sh.UpdateDate = DateTime.Now;
                await db.SaveChangesAsync();
                return new AnswerBasic(true, "");
            }
            catch (Exception e)
            {
                logger.LogError($"SchedulerService.UpdateSchedulerStatus Error:{e.Message}");
                return new AnswerBasic(false, "Ошибка программы");
            }

        }

        public async Task<Answer<viScheduler[]>> GetSchedulerByUserIdAsync(int user_id)
        {
            try
            {
                var scheduler = await db.tbSchedulers
                                .AsNoTracking()
                                .Where(x => x.UserId == user_id && x.Status == 1)
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
                                .ToArrayAsync();
                return new Answer<viScheduler[]>(true, "", scheduler);
            }
            catch (Exception e)
            {
                logger.LogError($"SchedulerService.GetSchedulerByIdAsync Error:{e.Message}");
                return new Answer<viScheduler[]>(false, "Ошибка программы", null);
            }

        }

        public async Task<Answer<viEvents[]>> GetAllSchedulersByOrgAsync()
        {
            try
            {
                int org_id = accessor.GetOrgId();
                var events = await db.tbSchedulers
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
                        UserPhoneNum = x.User.PhoneNum,
                        Start = x.AppointmentDateTime,
                        End = x.AppointmentDateTime.AddMinutes(30),
                        AdInfo = x.AdInfo
                    })
                    .ToArrayAsync();
                return new Answer<viEvents[]>(true, "", events);
            }
            catch (Exception e)
            {
                logger.LogError($"SchedulerService.GetAllSchedulersByOrgAsync Error:{e.Message}");
                return new Answer<viEvents[]>(false, "Ошибка программы", null);
            }

        }

        public async Task<Answer<TimeSpan[]>> GetStaffBusyTime(int staff_id, DateTime date)
        {
            try
            {
                int org_id = accessor.GetOrgId();
                var times = await db.tbSchedulers
                    .AsNoTracking()
                    .Where(x => x.OrganizationId == org_id && x.StaffId == staff_id && x.AppointmentDateTime.Date == date.Date)
                    .Select(x => x.AppointmentDateTime.TimeOfDay)
                    .ToArrayAsync();
                return new Answer<TimeSpan[]>(true, "", times);
            }
            catch (Exception e)
            {
                logger.LogError($"SchedulerService.GetStaffBusyTime Error:{e.Message}");
                return new Answer<TimeSpan[]>(false, "Ошибка программы", null);
            }
        }

        public async Task<Answer<viEvents[]>> SearchScheduler(string staff_name)
        {
            try
            {
                int org_id = accessor.GetOrgId();

                var search = await (from s in db.tbSchedulers
                                    where EF.Functions.ILike(s.Staff.Surname, $"%{staff_name}%")
                                    || EF.Functions.ILike(s.Staff.Name, $"%{staff_name}%")
                                    || EF.Functions.ILike(s.Staff.Patronymic, $"%{staff_name}%")
                                    select s)
                                    .AsNoTracking()
                                    .Where(x => x.Status == 1 && x.Staff.Status == 1 && x.OrganizationId == org_id)
                                    .Select(x => new viEvents
                                    {
                                        Id = x.Id,
                                        StaffId = x.StaffId,
                                        Staff = $"{x.Staff.Surname} {x.Staff.Name} {x.Staff.Patronymic}",
                                        UserId = x.UserId,
                                        User = $"{x.User.Name} {x.User.Surname} {x.User.Patronymic}",
                                        UserPhoneNum = x.User.PhoneNum,
                                        Start = x.AppointmentDateTime,
                                        End = x.AppointmentDateTime.AddMinutes(30),
                                        AdInfo = x.AdInfo
                                    })
                                    .ToArrayAsync();
                return new Answer<viEvents[]>(true, "", search);
            }
            catch (Exception e)
            {
                logger.LogError($"SchedulerService.SearchScheduler Error:{e.Message}");
                return new Answer<viEvents[]>(false, "Ошибка программы", null);
            }
        }
    }
}
