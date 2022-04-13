using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;
using waPlanner.ModelViews;
using waPlanner.TelegramBot;

namespace waPlanner.Services
{
    public interface IUserService
    {
        Task UpdatePatient(viPatient patient);
        Task UpdatePatientStatus(viPatient viPatient, byte status);
        Task AddPatientsAsync(viPatient patient);
        Task<List<viPatient>> GetAllPateintsAsync(int organization_id);
        Task<viPatient> GetPatientAsync(int user_id);
        Task<viPatient[]> GetUsers();
    }
    public class UserService : IUserService
    {
        private readonly MyDbContext db;
        public UserService(MyDbContext myDb)
        {
            this.db = myDb;
        }

        public async Task UpdatePatient(viPatient vipatient)
        {
            var patient = await db.tbUsers.FindAsync(vipatient.Id);

            if(vipatient.Phone is not null)
                patient.PhoneNum = vipatient.Phone;

            if(vipatient.Name is not null)
                patient.Name = vipatient.Name;

            if(vipatient.Surname is not null)
                patient.Surname = vipatient.Surname;

            if(vipatient.Patronymic is not null)
                patient.Patronymic = vipatient.Patronymic;

            if(vipatient is not null)
                patient.Gender = vipatient.Gender;

            if(vipatient.BirthDay.HasValue)
                patient.BirthDay = vipatient.BirthDay.Value;

            if(vipatient.Status.HasValue)
                patient.Status = vipatient.Status.Value;
            patient.UpdateDate = DateTime.Now;
            patient.UpdateUser = 1;
            await db.SaveChangesAsync();    
        }

        public async Task UpdatePatientStatus(viPatient viPatient, byte status)
        {
            var patient = await db.tbUsers.FindAsync(viPatient.Id);
            patient.Status = status;
            patient.UpdateDate = DateTime.Now;
            patient.UpdateUser = 1;
            await db.SaveChangesAsync();
        }

        public async Task AddPatientsAsync(viPatient patient)
        {
            var newPatient = new tbUser
            {
                Name = patient.Name,
                Surname = patient.Surname,
                Patronymic = patient.Patronymic,
                Gender = patient.Gender,
                BirthDay = patient.BirthDay,
                TelegramId = 0,
                CreateDate = DateTime.Now,
                CreateUser = 1,
                Status = 1
            };
            await db.tbUsers.AddAsync(newPatient);
            await db.SaveChangesAsync();
        }

        public async Task<List<viPatient>> GetAllPateintsAsync(int organization_id)
        {
            return await db.tbSchedulers
                .Include(x => x.User)
                .AsNoTracking()
                .Where(x => x.OrganizationId == organization_id)
                .Select(x => new viPatient
                {
                    Id = x.User.Id,
                    Name = x.User.Name,
                    AdInfo = x.AdInfo ?? "Нет данных",
                    Phone = x.User.PhoneNum,
                    BirthDay = x.User.BirthDay,
                    Gender = x.User.Gender ?? "Нет данных"
                })
                .ToListAsync();
        }

        public async Task<viPatient> GetPatientAsync(int user_id)
        {
            return await db.tbUsers
                .AsNoTracking()
                .Where(x => x.Id == user_id)
                .Select(x => new viPatient
                {
                    Id = x.Id,
                    Name = x.Name,
                    Patronymic = x.Patronymic,
                    Surname = x.Surname,
                    Gender = x.Gender ?? "Нет данных",
                    Phone = x.PhoneNum ?? "Нет Данных",
                    BirthDay = x.BirthDay,
                })
                .FirstOrDefaultAsync();
        }

        public async Task<viPatient[]> GetUsers()
        {
            return await db.tbUsers
                .AsNoTracking()
                .Where(x => x.Status == 1)
                .Select(x => new viPatient
                {
                    Id = x.Id,
                    Name = x.Name,
                    Patronymic = x.Patronymic,
                    Surname = x.Surname,
                    Gender = x.Gender ?? "Нет данных",
                    Phone = x.PhoneNum ?? "Нет Данных",
                    BirthDay = x.BirthDay,
                })
                .ToArrayAsync();
        }
    }
}

