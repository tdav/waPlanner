using System.Threading.Tasks;
using waPlanner.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using waPlanner.ModelViews;
using System;
using waPlanner.TelegramBot;

namespace waPlanner.Services
{
    public interface IInfoService
    {
        Task<int> GetTotalTodayAppointments(int organization_id);
        Task<int> GetTotalWeekAppointments(int organization_id);
        Task<List<viAppointmentsModel>> GetTodayAppointments(int organization_id);
        Task<List<viRecentSchedulers>> GetRecentUsers(int organization_id);
        Task<List<viSchedulerDiagramma>> GetSchedulerDiagramma(int organization_id);
    }
    public class InfoService : IInfoService
    {
        private readonly MyDbContext db;
        public InfoService(MyDbContext db)
        {
            this.db = db;
        }
        public async Task<int> GetTotalTodayAppointments(int organization_id)
        {
            return await db.tbSchedulers.CountAsync(x => x.AppointmentDateTime.Date == DateTime.Now.Date && x.OrganizationId == organization_id);
        }

        public async Task<int> GetTotalWeekAppointments(int organization_id)
        {
            return await db.tbSchedulers.CountAsync(x => x.AppointmentDateTime.Date >= DateTime.Now.Date.AddDays(-7) &&
                                                         x.AppointmentDateTime.Date <= DateTime.Now.Date &&
                                                         x.OrganizationId == organization_id);
        }

        public async Task<List<viAppointmentsModel>> GetTodayAppointments(int organization_id)
        {
            return await db.tbSchedulers
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.Organization)
                .Where(x => x.OrganizationId == organization_id && x.AppointmentDateTime.Date == DateTime.Now.Date)
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

        public async Task<List<viRecentSchedulers>> GetRecentUsers(int organization_id)
        {
            return await db.tbSchedulers
                .AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.OrganizationId == organization_id)
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

        public async Task<List<viSchedulerDiagramma>> GetSchedulerDiagramma(int organization_id)
        {
            return await db.tbSchedulers
                .AsNoTracking()
                .Include(x => x.Category)
                .Where(x => x.OrganizationId == organization_id &&
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
