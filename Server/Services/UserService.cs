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
        Task UpdatePatient(int patient_id, viPatient patient);
        Task UpdatePatientStatus(int patient_id, byte status);
        Task AddPatientsAsync(viPatient patient);
        Task<List<viPatient>> GetAllPateintsAsync(int organization_id);
        Task<viPatient> GetPatientAsync(int user_id);
    }
    public class UserService : IUserService
    {
        private readonly MyDbContext db;
        public UserService(MyDbContext myDb)
        {
            this.db = myDb;
        }

        public async Task UpdatePatient(int patient_id, viPatient vipatient)
        {
            var patient = await db.tbUsers.FindAsync(patient_id);

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

            if(vipatient.AdInfo is not null)
                patient.AdInfo = vipatient.AdInfo;

            if(vipatient.BirthDay.HasValue)
                patient.BirthDay = vipatient.BirthDay.Value;

            if(vipatient.Password is not null)
                patient.Password = vipatient.Password;

            if(vipatient.Status.HasValue)
                patient.Status = vipatient.Status.Value;

            patient.UserTypeId = (int)UserTypes.TELEGRAM_USER;
            patient.UpdateDate = DateTime.Now;
            patient.UpdateUser = 1;
            await db.SaveChangesAsync();    
        }

        public async Task UpdatePatientStatus(int patient_id, byte status)
        {
            var patient = await db.tbUsers.FindAsync(patient_id);
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
                Gender = patient.Gender,
                AdInfo = patient.AdInfo,
                PhoneNum = patient.Phone,
                Password = "123456",
                UserTypeId = (int)UserTypes.TELEGRAM_USER,
                BirthDay = patient.BirthDay,
                TelegramId = 0,
                CreateDate = DateTime.Now,
                CreateUser = 1
            };
            await db.tbUsers.AddAsync(newPatient);
            await db.SaveChangesAsync();
        }

        public async Task<List<viPatient>> GetAllPateintsAsync(int organization_id)
        {
            return await db.tbSchedulers
                .Include(x => x.User)
                .AsNoTracking()
                .Where(x => x.User.UserTypeId == (int)UserTypes.TELEGRAM_USER && x.OrganizationId == organization_id)
                .Select(x => new viPatient
                {
                    Id = x.User.Id,
                    Name = x.User.Name,
                    AdInfo = x.AdInfo,
                    Phone = x.User.PhoneNum,
                    BirthDay = x.User.BirthDay,
                    Gender = x.User.Gender
                })
                .ToListAsync();
        }

        public async Task<viPatient> GetPatientAsync(int user_id)
        {
            return await db.tbUsers
                .AsNoTracking()
                .Where(x => x.Id == user_id && x.UserTypeId == (int)UserTypes.TELEGRAM_USER)
                .Select(x => new viPatient
                {
                    Name = x.Name,
                    Patronymic = x.Patronymic,
                    Surname = x.Surname,
                    Gender = x.Gender,
                    AdInfo = x.Gender,
                    Phone = x.PhoneNum,
                    BirthDay = x.BirthDay,
                })
                .FirstAsync();
        }
    }
}

