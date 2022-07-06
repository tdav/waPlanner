using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;
using waPlanner.Interfaces;
using waPlanner.ModelViews;
using waPlanner.TelegramBot;
using waPlanner.Utils;

namespace waPlanner.Services
{
    public interface IRegistrationService
    {
        Task<Answer<viRegistration>> RegistrAsync(viRegistration viRegistration);
    }
    public class RegistrationService: IRegistrationService, IAutoRegistrationScopedLifetimeService
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

        public async Task<Answer<viRegistration>> RegistrAsync(viRegistration viRegistration)
        {
            try
            {
                var check = db.tbStaffs
                    .AsNoTracking()
                    .Where(x => x.RoleId == (int)UserRoles.ADMIN)
                    .Any(x => x.PhoneNum == viRegistration.Phone);

                if (check)
                    return new Answer<viRegistration>(false, "Такой номер уже зарегистрирован", null);

                using (var scope = provider.CreateScope())
                {
                    var staff = new tbStaff
                    {
                        Name = viRegistration.Name,
                        Surname = viRegistration.Surname,
                        Patronymic = viRegistration.Patronymic,
                        PhoneNum = viRegistration.Phone,
                        Password = CHash.EncryptMD5(viRegistration.Password),
                        BirthDay = new DateTime(1970, 01, 27, 14, 0, 0),
                        RoleId = (int)UserRoles.ADMIN,
                        Status = 1,
                        CreateDate = DateTime.Now,
                        CreateUser = 0
                    };
                    
                    var telegramGroupCreator = scope.ServiceProvider.GetService<ITelegramGroupCreatorService>();
                    
                    var organization = new spOrganization();
                    organization.Name = viRegistration.OrganizationName;

                    var chatIds = await telegramGroupCreator.CreateGroup(staff.PhoneNum, organization.Name);

                    organization.SpecializationId = viRegistration.SpecializationId;
                    organization.ChatId = chatIds.Data[0];
                    organization.Address = "address";
                    organization.BreakTimeStart = new DateTime(2022, 01, 01, 13, 0, 0);
                    organization.BreakTimeEnd = new DateTime(2022, 01, 01, 14, 0, 0);
                    organization.WorkStart = new DateTime(2022, 01, 01, 09, 0, 0);
                    organization.WorkEnd = new DateTime(2022, 01, 01, 18, 0, 0);
                    organization.Latitude = 0;
                    organization.Longitude = 0;
                    organization.MessageRu = "Выберите что-то";
                    organization.MessageUz = "Nimadur tanlang";
                    organization.MessageLt = "Nimadur tanlang";
                    organization.CreateDate = DateTime.Now;
                    organization.CreateUser = staff.Id;
                    organization.Status = 1;
                    organization.OrderIndex = 1;

                    await db.spOrganizations.AddAsync(organization);
                    await db.SaveChangesAsync();

                    staff.TelegramId = chatIds.Data[1];
                    staff.OrganizationId = organization.Id;
                    await db.tbStaffs.AddAsync(staff);
                    await db.SaveChangesAsync();
                }

                return new Answer<viRegistration>(true, "", viRegistration);
            }
            catch (Exception e)
            {
                logger.LogError($"InfoService.RegistrAsync Error:{e.Message} Model: {viRegistration.ToJson()}");
                return new Answer<viRegistration>(false, "Ошибка программы", null);
            }
        }

    }
}
