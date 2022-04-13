using System.Threading.Tasks;
using waPlanner.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using waPlanner.ModelViews;
using System;
using waPlanner.TelegramBot;
using waPlanner.Extensions;

namespace waPlanner.Services
{
    public interface IInfoService
    {
        Task<int> GetTotalTodayAppointments();
        Task<int> GetTotalWeekAppointments();
        Task<List<viAppointmentsModel>> GetTodayAppointments();
        Task<List<viRecentSchedulers>> GetRecentUsers();
        Task<List<viSchedulerDiagramma>> GetSchedulerDiagramma();
    }
    public class InfoService : IInfoService
    {
        private readonly MyDbContext db;
        private readonly IHttpContextAccessorExtensions accessor;
        public InfoService(MyDbContext db, IHttpContextAccessorExtensions accessor)
        {
            this.accessor = accessor;
            this.db = db;
        }
        public async Task<int> GetTotalTodayAppointments()
        {
            int org_id = accessor.GetOrgId();
            return await db.tbSchedulers.CountAsync(x => x.AppointmentDateTime.Date == DateTime.Now.Date && x.OrganizationId == org_id);
        }

        public async Task<int> GetTotalWeekAppointments()
        {
            int org_id = accessor.GetOrgId();
            return await db.tbSchedulers.CountAsync(x => x.AppointmentDateTime.Date >= DateTime.Now.Date.AddDays(-7) &&
                                                         x.AppointmentDateTime.Date <= DateTime.Now.Date &&
                                                         x.OrganizationId == org_id);
        }

        public async Task<List<viAppointmentsModel>> GetTodayAppointments()
        {
            int org_id = accessor.GetOrgId();
            return await db.tbSchedulers
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.Organization)
                .Where(x => x.OrganizationId == org_id && x.AppointmentDateTime.Date == DateTime.Now.Date)
                .Select(x => new viAppointmentsModel
                {
                    Id = x.Id,
                    PatientId = x.User.Id,
                    PatientName = x.User.Name,
                    StaffId = x.Staff.Id,
                    StaffName = $"{x.Staff.Name} {x.Staff.Surname}",
                    Symptoms = x.AdInfo ?? "Нет данных"
                })
                .Take(20)
                .ToListAsync();
        }

        public async Task<List<viRecentSchedulers>> GetRecentUsers()
        {
            int org_id = accessor.GetOrgId();
            return await db.tbSchedulers
                .AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.OrganizationId == org_id)
                .OrderByDescending(x => x.CreateDate)
                .Select(x => new viRecentSchedulers
                {
                    Id = x.Id,
                    PersonName = x.User.Name,
                    DpInfo = x.AdInfo ?? "Нет данных",
                    CreateDate = x.CreateDate
                })
                .Take(20)
                .ToListAsync();

        }

        public async Task<List<viSchedulerDiagramma>> GetSchedulerDiagramma()
        {
            int org_id = accessor.GetOrgId();
            return await db.tbSchedulers
                .AsNoTracking()
                .Include(x => x.Category)
                .Where(x => x.OrganizationId == org_id &&
                       x.CreateDate >= DateTime.Now.Date.AddMonths(-1) &&
                       x.CreateDate <= DateTime.Now)
                .GroupBy(g => new { g.Category.NameUz, g.CreateDate.Date })
                .Select(x => new viSchedulerDiagramma
                {
                    Cnt = x.Count(),
                    Category = x.Key.NameUz,
                    CreateDate = x.Key.Date
                })
                .ToListAsync();
        }
    }
}
