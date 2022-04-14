using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;
using waPlanner.Extensions;
using waPlanner.ModelViews;

namespace waPlanner.Services
{
    public interface IUserService
    {
        Task UpdateAsync(viPatient patient);
        Task SetStatusAsync(viPatient viPatient, int status);
        Task<int> AddAsync(viPatient patient);

        Task<viPatient[]> GetAllAsync();
        Task<viPatient> GetAsync(int user_id);        
    }

    public class UserService : IUserService
    {
        private readonly MyDbContext db;
        private readonly IHttpContextAccessorExtensions accessor;
        public UserService(MyDbContext myDb, IHttpContextAccessorExtensions accessor)
        {
            this.accessor = accessor;
            this.db = myDb;
        }

        public async Task UpdateAsync(viPatient vipatient)
        {
            var patient = await db.tbUsers.FindAsync(vipatient.Id);
            int user_id = accessor.GetId();

            if (vipatient.Phone is not null)
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
            patient.UpdateUser = user_id;
            await db.SaveChangesAsync();    
        }

        public async Task SetStatusAsync(viPatient viPatient, int status)
        {
            int user_id = accessor.GetId();
            var patient = await db.tbUsers.FindAsync(viPatient.Id);
            patient.Status = status;
            patient.UpdateDate = DateTime.Now;
            patient.UpdateUser = user_id;
            await db.SaveChangesAsync();
        }

        public async Task<int> AddAsync(viPatient patient)
        {
            var user_id = accessor.GetId();
            var newPatient = new tbUser
            {
                Name = patient.Name,
                Surname = patient.Surname,
                Patronymic = patient.Patronymic,
                Gender = patient.Gender,
                BirthDay = patient.BirthDay,
                TelegramId = 0,
                CreateDate = DateTime.Now,
                CreateUser = user_id,
                Status = 1
            };
            await db.tbUsers.AddAsync(newPatient);
            await db.SaveChangesAsync();

            return newPatient.Id;
        }

        public async Task<viPatient[]> GetAllAsync()
        {
            int org_id = accessor.GetOrgId();
            return await db.tbSchedulers
                .Include(x => x.User)
                .AsNoTracking()
                .Where(x => x.OrganizationId == org_id)
                .Select(x => new viPatient
                {
                    Id = x.User.Id,
                    Name = x.User.Name,
                    AdInfo = x.AdInfo ?? "Нет данных",
                    Phone = x.User.PhoneNum,
                    BirthDay = x.User.BirthDay,
                    Gender = x.User.Gender ?? "Нет данных"
                })
                .ToArrayAsync();
        }

        public async Task<viPatient> GetAsync(int user_id)
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
    }
}

