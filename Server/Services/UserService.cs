using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;
using waPlanner.Extensions;
using waPlanner.ModelViews;
using waPlanner.Utils;

namespace waPlanner.Services
{
    public interface IUserService
    {
        Task<AnswerBasic> UpdateAsync(viPatient patient);
        Task<AnswerBasic> SetStatusAsync(int patient, int status);
        Task<Answer<viPatient>> AddAsync(viPatient patient);
        Task<Answer<viPatient[]>> GetAllAsync();
        Task<Answer<viPatient>> GetAsync(int user_id);
        Task<Answer<viPatient[]>> SearchUserAsync(string name);
    }

    public class UserService : IUserService
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

                if (vipatient.Phone is not null)
                    patient.PhoneNum = vipatient.Phone;

                if (vipatient.Name is not null)
                    patient.Name = vipatient.Name;

                if (vipatient.Surname is not null)
                    patient.Surname = vipatient.Surname;

                if (vipatient.Patronymic is not null)
                    patient.Patronymic = vipatient.Patronymic;

                if (vipatient is not null)
                    patient.Gender = vipatient.Gender;

                if (vipatient.BirthDay.HasValue)
                    patient.BirthDay = vipatient.BirthDay.Value;

                if (vipatient.Status.HasValue)
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

        public async Task<AnswerBasic> SetStatusAsync(int patient_id, int status)
        {
            try
            {
                int user_id = accessor.GetId();
                var patient = await db.tbUsers.FindAsync(patient_id);
                patient.Status = status;
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
                logger.LogError($"UserService.AddAsync Error:{e.Message}");
                return new Answer<viPatient>(false, "Ошибка программы", null);
            }
        }

        public async Task<Answer<viPatient[]>> GetAllAsync()
        {
            try
            {
                int org_id = accessor.GetOrgId();
                var patients = await db.tbSchedulers
                    .Include(x => x.User)
                    .AsNoTracking()
                    .Where(x => x.OrganizationId == org_id && x.Status == 1)
                    .Select(x => new viPatient
                    {
                        Id = x.User.Id,
                        Name = x.User.Name,
                        Surname = x.User.Surname,
                        Patronymic = x.User.Patronymic,
                        AdInfo = x.AdInfo,
                        Phone = x.User.PhoneNum,
                        BirthDay = x.User.BirthDay,
                        Gender = x.User.Gender
                    })
                    .Distinct()
                    .ToArrayAsync();
                return new Answer<viPatient[]>(true, "", patients);
            }
            catch (Exception e)
            {
                logger.LogError($"UserService.GetAllAsync Error:{e.Message}");
                return new Answer<viPatient[]>(false, "Ошибка программы", null);
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

        public async Task<Answer<viPatient[]>> SearchUserAsync(string name)
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
                            .ToArrayAsync();
                return new Answer<viPatient[]>(true, "Ошибка программы", search);
            }
            catch (Exception e)
            {
                logger.LogError($"UserService.SearchUserAsync Error:{e.Message}");
                return new Answer<viPatient[]>(false, "Ошибка программы", null);
            }
            
        }
    }
}

