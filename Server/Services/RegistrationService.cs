using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;
using waPlanner.ModelViews;
using waPlanner.TelegramBot;
using waPlanner.Utils;

namespace waPlanner.Services
{
    public interface IRegistrationService
    {
        Task<AnswerBasic> RegisterAsync(viRegistration viRegistration);
    }
    public class RegistrationService: IRegistrationService
    {
        private readonly ILogger<RegistrationService> logger;
        private readonly MyDbContext db;
        private readonly IServiceProvider provider;

        public RegistrationService(MyDbContext db, ILogger<RegistrationService> logger, IServiceProvider provider)
        {
            this.db = db;
            this.logger = logger;
            this.provider = provider;
        }

        public async Task<AnswerBasic> RegisterAsync(viRegistration viRegistration)
        {
            try
            {
                using (var scope = provider.CreateScope())
                {
                    var staff = new tbStaff
                    {
                        Name = viRegistration.Name,
                        Surname = viRegistration.Surname,
                        Patronymic = viRegistration.Patronymic,
                        PhoneNum = viRegistration.Phone,
                        Password = CHash.EncryptMD5(viRegistration.Password),
                        BirthDay = DateTime.Now,
                        RoleId = (int)UserRoles.ADMIN,
                        Status = 1,
                        CreateDate = DateTime.Now,
                        CreateUser = 0
                    };
                    
                    var telegramGroupCreator = scope.ServiceProvider.GetService<ITelegramGroupCreatorService>();
                    
                    var organization = new spOrganization();
                    organization.Name = viRegistration.OrganizationName;

                    var chatid = await telegramGroupCreator.CreateGroup(staff.PhoneNum, organization.Name);

                    organization.SpecializationId = viRegistration.SpecializationId;
                    organization.ChatId = chatid.Data;
                    organization.Address = viRegistration.Address;
                    organization.BreakTimeStart = viRegistration.BreakTimeStart;
                    organization.BreakTimeEnd = viRegistration.BreakTimeEnd;
                    organization.WorkStart = viRegistration.WorkStart;
                    organization.WorkEnd = viRegistration.WorkEnd;
                    organization.CreateDate = DateTime.Now;
                    organization.CreateUser = staff.Id;
                    organization.Status = 1;

                    await db.spOrganizations.AddAsync(organization);
                    await db.SaveChangesAsync();

                    staff.OrganizationId = organization.Id;
                    await db.tbStaffs.AddAsync(staff);
                    await db.SaveChangesAsync();
                }

                return new AnswerBasic(true, "");
            }
            catch (Exception e)
            {
                logger.LogError($"InfoService.GetTotalTodayAppointments Error:{e.Message}");
                return new AnswerBasic(false, "Ошибка программы");
            }
        }

    }
}
