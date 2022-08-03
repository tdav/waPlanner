using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
    public interface IUserService
    {
        Task<AnswerBasic> UpdateAsync(viPatient patient);
        Task<AnswerBasic> SetStatusAsync(viSetStatus status);
        Task<Answer<viPatient>> AddAsync(viPatient patient);
        Task<Answer<List<viPatient>>> GetAllAsync();
        Task<Answer<viPatient>> GetAsync(int user_id);
        Task<Answer<List<viPatient>>> SearchUserAsync(string name);
    }

    public class UserService : IUserService, IAutoRegistrationScopedLifetimeService
    {
        private readonly MyDbContext db;
        private readonly IHttpContextAccessorExtensions accessor;
        private readonly ILogger<UserService> logger;
        public UserService(MyDbContext db, IHttpContextAccessorExtensions accessor, ILogger<UserService> logger)
        {
            this.accessor = accessor;
            this.db = db;
            this.logger = logger;
        }

        public async Task<AnswerBasic> UpdateAsync(viPatient vipatient)
        {
            try
            {
                int user_id = accessor.GetId();
                var patient = await db.tbUsers.FindAsync(vipatient.Id);

                patient.PhoneNum = vipatient.Phone;
                patient.Name = vipatient.Name;
                patient.Surname = vipatient.Surname;
                patient.Patronymic = vipatient.Patronymic;
                patient.Gender = vipatient.Gender;
                patient.BirthDay = vipatient.BirthDay.Value;
                patient.Status = vipatient.Status.Value;
                patient.UpdateDate = DateTime.Now;
                patient.UpdateUser = user_id;
                await db.SaveChangesAsync();
                return new AnswerBasic(true, "");
            }
            catch (Exception e)
            {
                logger.LogError($"UserService.UpdateAsync Error:{e.Message} Model:{vipatient.ToJson()}");
                return new AnswerBasic(false, "Ошибка программы");
            }

        }

        public async Task<AnswerBasic> SetStatusAsync(viSetStatus status)
        {
            try
            {
                int user_id = accessor.GetId();
                var patient = await db.tbUsers.FindAsync(status.Id);
                patient.Status = status.Status;
                patient.UpdateDate = DateTime.Now;
                patient.UpdateUser = user_id;
                await db.SaveChangesAsync();
                return new AnswerBasic(true, "");
            }
            catch (Exception e)
            {
                logger.LogError($"UserService.SetStatusAsync Error:{e.Message}");
                return new AnswerBasic(false, "Ошибка программы");
            }

        }

        public async Task<Answer<viPatient>> AddAsync(viPatient patient)
        {
            try
            {
                var user_id = accessor.GetId();
                var newPatient = new tbUser
                {
                    Name = patient.Name,
                    Surname = patient.Surname,
                    Patronymic = patient.Patronymic,
                    Gender = patient.Gender,
                    BirthDay = patient.BirthDay,
                    PhoneNum = patient.Phone,
                    TelegramId = 0,
                    CreateDate = DateTime.Now,
                    CreateUser = user_id,
                    Status = 1
                };
                await db.tbUsers.AddAsync(newPatient);
                await db.SaveChangesAsync();

                var return_patient = new viPatient
                {
                    Id = newPatient.Id,
                    Name = patient.Name,
                    Patronymic = patient.Patronymic,
                    Surname = patient.Surname,
                    Gender = patient.Gender,
                    Phone = patient.Phone,
                    BirthDay = patient.BirthDay,
                };
                return new Answer<viPatient>(true, "", return_patient);
            }
            catch (Exception e)
            {
                logger.LogError($"UserService.AddAsync Error:{e.Message} Model: {patient.ToJson()}");
                return new Answer<viPatient>(false, "Ошибка программы", null);
            }
        }

        public async Task<Answer<List<viPatient>>> GetAllAsync()
        {
            try
            {
                int org_id = accessor.GetOrgId();
                var patients = await db.tbSchedulers
                    .Include(x => x.User)
                    .AsNoTracking()
                    .Where(x => x.OrganizationId == org_id && x.User.Status == 1)
                    .Select(x => new viPatient
                    {
                        Id = x.User.Id,
                        Name = x.User.Name,
                        Surname = x.User.Surname,
                        Patronymic = x.User.Patronymic,
                        Phone = x.User.PhoneNum,
                        BirthDay = x.User.BirthDay,
                        Gender = x.User.Gender,
                        Status = x.User.Status
                    })
                    .Distinct()
                    .ToListAsync();
                return new Answer<List<viPatient>>(true, "", patients);
            }
            catch (Exception e)
            {
                logger.LogError($"UserService.GetAllAsync Error:{e.Message}");
                return new Answer<List<viPatient>>(false, "Ошибка программы", null);
            }
        }

        public async Task<Answer<viPatient>> GetAsync(int user_id)
        {
            try
            {
                var user = await db.tbUsers
                .AsNoTracking()
                .Where(x => x.Id == user_id && x.Status == 1)
                .Select(x => new viPatient
                {
                    Id = x.Id,
                    Name = x.Name,
                    Patronymic = x.Patronymic,
                    Surname = x.Surname,
                    Gender = x.Gender,
                    Phone = x.PhoneNum,
                    BirthDay = x.BirthDay,
                    Status = x.Status
                })
                .FirstOrDefaultAsync();
                return new Answer<viPatient>(true, "Ошибка программы", user);
            }
            catch (Exception e)
            {
                logger.LogError($"UserService.GetAsync Error:{e.Message}");
                return new Answer<viPatient>(false, "Ошибка программы", null);
            }
        }

        public async Task<Answer<List<viPatient>>> SearchUserAsync(string name)
        {
            try
            {
                int org_id = accessor.GetOrgId();
                var search = await (from s in db.tbSchedulers
                                    where EF.Functions.ILike(s.User.Surname, $"%{name}%")
                                    || EF.Functions.ILike(s.User.Name, $"%{name}%")
                                    || EF.Functions.ILike(s.User.Patronymic, $"%{name}%")
                                    select s)
                            .AsNoTracking()
                            .Where(x => x.Status == 1 && x.OrganizationId == org_id)
                            .Select(x => new viPatient
                            {
                                Id = x.User.Id,
                                Name = x.User.Name,
                                Surname = x.User.Surname,
                                BirthDay = x.User.BirthDay,
                                Phone = x.User.PhoneNum,
                                Patronymic = x.User.Patronymic,
                                Gender = x.User.Gender
                            })
                            .Distinct()
                            .ToListAsync();
                return new Answer<List<viPatient>>(true, "Ошибка программы", search);
            }
            catch (Exception e)
            {
                logger.LogError($"UserService.SearchUserAsync Error:{e.Message}");
                return new Answer<List<viPatient>>(false, "Ошибка программы", null);
            }

        }
    }
}

