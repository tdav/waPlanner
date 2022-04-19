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
        Task SetStatusAsync(int patient, int status);
        Task<viPatient> AddAsync(viPatient patient);

        Task<viPatient[]> GetAllAsync();
        Task<viPatient> GetAsync(int user_id);        
    }

    public class UserService : IUserService
    {
        private readonly MyDbContext db;
        private readonly IHttpContextAccessorExtensions accessor;
        public UserService(MyDbContext db, IHttpContextAccessorExtensions accessor)
        {
            this.accessor = accessor;
            this.db = db;
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

        public async Task SetStatusAsync(int patient_id, int status)
        {
            int user_id = accessor.GetId();
            var patient = await db.tbUsers.FindAsync(patient_id);
            patient.Status = status;
            patient.UpdateDate = DateTime.Now;
            patient.UpdateUser = user_id;
            await db.SaveChangesAsync();
        }

        public async Task<viPatient> AddAsync(viPatient patient)
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

            return new viPatient
            {
                Id = patient.Id,
                Name = patient.Name,
                Patronymic = patient.Patronymic,
                Surname = patient.Surname,
                Gender = patient.Gender,
                Phone = patient.Phone,
                BirthDay = patient.BirthDay,
            };
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
                    AdInfo = x.AdInfo,
                    Phone = x.User.PhoneNum,
                    BirthDay = x.User.BirthDay,
                    Gender = x.User.Gender
                })
                .Distinct()
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
                    Gender = x.Gender,
                    Phone = x.PhoneNum,
                    BirthDay = x.BirthDay,
                })
                .FirstOrDefaultAsync();
        }
    }
}

