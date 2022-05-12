using System.Threading.Tasks;
using waPlanner.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using waPlanner.ModelViews;
using System;
using waPlanner.TelegramBot;
using waPlanner.Extensions;
using Microsoft.Extensions.Logging;
using waPlanner.Interfaces;

namespace waPlanner.Services
{
    public interface IInfoService
    {
        Task<Answer<int>> GetTotalTodayAppointments();
        Task<Answer<int>> GetTotalWeekAppointments();
        Task<Answer<List<viAppointmentsModel>>> GetTodayAppointments();
        Task<Answer<List<viRecentSchedulers>>> GetRecentUsers();
        Task<Answer<List<viSchedulerDiagramma>>> GetSchedulerDiagramma();
    }
    public class InfoService : IInfoService, IAutoRegistrationScopedLifetimeService
    {
        private readonly MyDbContext db;
        private readonly IHttpContextAccessorExtensions accessor;
        private readonly ILogger<InfoService> logger;
        public InfoService(MyDbContext db, IHttpContextAccessorExtensions accessor, ILogger<InfoService> logger)
        {
            this.accessor = accessor;
            this.db = db;
            this.logger = logger;
        }
        public async Task<Answer<int>> GetTotalTodayAppointments()
        {
            try
            {
                int org_id = accessor.GetOrgId();
                var totalTodayAppointments = await db.tbSchedulers.CountAsync(x => x.CreateDate.Date == DateTime.Now.Date && x.OrganizationId == org_id);
                return new Answer<int>(true, "", totalTodayAppointments);
            }
            catch (Exception e)
            {
                logger.LogError($"InfoService.GetTotalTodayAppointments Error:{e.Message}");
                return new Answer<int>(false, "Ошибка программы", 0);
            }
            
        }

        public async Task<Answer<int>> GetTotalWeekAppointments()
        {
            try
            {
                int org_id = accessor.GetOrgId();
                var totalWeekAppointments = await db.tbSchedulers.CountAsync(x => x.AppointmentDateTime.Date >= DateTime.Now.Date.AddDays(-7) &&
                                                             x.AppointmentDateTime.Date <= DateTime.Now.Date &&
                                                             x.OrganizationId == org_id);
                return new Answer<int>(true, "", totalWeekAppointments);
            }
            catch (Exception e)
            {
                logger.LogError($"InfoService.GetTotalWeekAppointments Error:{e.Message}");
                return new Answer<int>(false, "Ошибка программы", 0);
            }
        }

        public async Task<Answer<List<viAppointmentsModel>>> GetTodayAppointments()
        {
            try
            {
                int org_id = accessor.GetOrgId();
                var todayAppointments = await db.tbSchedulers
                    .AsNoTracking()
                    .Include(x => x.User)
                    .Include(x => x.Organization)
                    .Where(x => x.OrganizationId == org_id && x.AppointmentDateTime.Date == DateTime.Now.Date)
                    .Select(x => new viAppointmentsModel
                    {
                        Id = x.Id,
                        AppointmentTime = x.AppointmentDateTime,
                        PatientId = x.User.Id,
                        PatientName = $"{x.User.Surname} {x.User.Name} {x.User.Patronymic}",
                        StaffId = x.Staff.Id,
                        StaffName = $"{x.Staff.Surname} {x.Staff.Name} {x.Staff.Patronymic}",
                        Symptoms = x.AdInfo
                    })
                    .Take(20)
                    .ToListAsync();
                return new Answer<List<viAppointmentsModel>>(true, "", todayAppointments);
            }
            catch (Exception e)
            {
                logger.LogError($"InfoService.GetTodayAppointments Error:{e.Message}");
                return new Answer<List<viAppointmentsModel>>(false, "Ошибка программы", null);
            }
        }

        public async Task<Answer<List<viRecentSchedulers>>> GetRecentUsers()
        {
            try
            {
                int org_id = accessor.GetOrgId();
                var recentUsers = await db.tbSchedulers
                    .AsNoTracking()
                    .Include(x => x.User)
                    .Where(x => x.OrganizationId == org_id)
                    .OrderByDescending(x => x.CreateDate)
                    .Select(x => new viRecentSchedulers
                    {
                        Id = x.Id,
                        PersonName = x.User.Name,
                        DpInfo = x.AdInfo,
                        CreateDate = x.CreateDate
                    })
                    .Take(20)
                    .ToListAsync();
                return new Answer<List<viRecentSchedulers>>(true, "", recentUsers);
            }
            catch (Exception e)
            {
                logger.LogError($"InfoService.GetRecentUsers Error:{e.Message}");
                return new Answer<List<viRecentSchedulers>>(false, "Ошибка программы", null);
            }
        }

        public async Task<Answer<List<viSchedulerDiagramma>>> GetSchedulerDiagramma()
        {
            try
            {
                int org_id = accessor.GetOrgId();
                var schedulerDiagramma = await db.tbSchedulers
                    .AsNoTracking()
                    .Include(x => x.Category)
                    .Where(x => x.OrganizationId == org_id &&
                           x.AppointmentDateTime >= DateTime.Now.Date.AddMonths(-1) &&
                           x.AppointmentDateTime.Date.Month <= DateTime.Now.Date.Month)
                    .GroupBy(g => new { g.Category.NameUz, g.AppointmentDateTime.Date })
                    .Select(x => new viSchedulerDiagramma
                    {
                        Cnt = x.Count(),
                        Category = x.Key.NameUz,
                        AppointmentDate = x.Key.Date
                    })
                    .ToListAsync();
                return new Answer<List<viSchedulerDiagramma>>(true, "", schedulerDiagramma);
            }
            catch (Exception e)
            {
                logger.LogError($"InfoService.GetSchedulerDiagramma Error:{e.Message}");
                return new Answer<List<viSchedulerDiagramma>>(false, "Ошибка программы", null);
            }
        }
    }
}
