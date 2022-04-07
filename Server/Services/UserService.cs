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
        Task InsertAsync(tbUser user);
        Task UpdateAsync(tbUser user);
        Task UpdatePatient(int patient_id, viPatient patient);
        Task UpdatePatientStatus(int patient_id, byte status);
        void Delete(int id);
        Task<tbUser> GetUserByIdAsync(int id);
        Task<tbUser[]> GetAllAsync();
        Task AddPatientsAsync(viPatient patient);
        Task<List<viPatient>> GetPateintsAsync(int organization_id);
    }
    public class UserService : IUserService
    {
        private readonly MyDbContext db;
        public UserService(MyDbContext myDb)
        {
            this.db = myDb;
        }

        public async Task InsertAsync(tbUser user)
        {
            user.CreateDate = DateTime.Now;
            user.CreateUser = 1;
            user.Status = 1;
            await db.tbUsers.AddAsync(user);
            await db.SaveChangesAsync();
        }

        public async Task UpdateAsync(tbUser user)
        {
            user.UpdateDate = DateTime.Now;
            user.UpdateUser = 1;
            db.tbUsers.Update(user);
            await db.SaveChangesAsync();
        }

        public async Task UpdatePatient(int patient_id, viPatient vipatient)
        {
            var patient = await db.tbUsers.FindAsync(patient_id);
            patient.Name = vipatient.Name;
            patient.Gender = vipatient.Gender;
            patient.AdInfo = vipatient.AdInfo;
            patient.PhoneNum = vipatient.Phone;
            patient.BirthDay = vipatient.BirthDay;
            patient.UserTypeId = (int)UserTypes.TELEGRAM_USER;
            patient.Password = vipatient.Password;
            patient.Status = vipatient.Status;
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

        public void Delete(int id)
        {
            tbUser user = db.tbUsers.Find(id);
            db.tbUsers.Remove(user);
            db.SaveChanges();
        }

        public async Task<tbUser[]> GetAllAsync()
        {
            return await db.tbUsers.AsNoTracking().ToArrayAsync();
        }

        public async Task<tbUser> GetUserByIdAsync(int id)
        {
            return await db.tbUsers.FindAsync(id);
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

        public async Task<List<viPatient>> GetPateintsAsync(int organization_id)
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
    }
}

