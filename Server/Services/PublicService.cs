using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Interfaces;
using waPlanner.ModelViews;
using waPlanner.TelegramBot;

namespace waPlanner.Services
{
    public interface IPublicService
    {
        ValueTask<Answer<viOrganization[]>> GetOrganizationsBySpecId(int specId);
        ValueTask<Answer<viStaff[]>> GetStaffsByOrgId(int orgId);
        ValueTask<Answer<viCategory[]>> GetCategoriesByOrgId(int orgId);
    }
    public class PublicService: IPublicService, IAutoRegistrationScopedLifetimeService
    {
        private readonly MyDbContext db;
        private readonly ILogger<CategoryService> logger;

        public PublicService(MyDbContext db, ILogger<CategoryService> logger)
        {
            this.db = db;
            this.logger = logger;
        }

        public async ValueTask<Answer<viOrganization[]>> GetOrganizationsBySpecId(int specId)
        {
            try
            {
                var orgs = await db.spOrganizations
                    .AsNoTracking()
                    .Where(x => x.Status == 1 && x.SpecializationId == specId)
                    .Select(x => new viOrganization
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Address = x.Address,
                        Latitude = x.Latitude,
                        Longitude = x.Longitude,
                        SpecializationId = x.SpecializationId,
                        Specialization = x.Specialization.NameRu,
                        PhotoPath = x.PhotoPath,
                    })
                    .ToArrayAsync();

                return new Answer<viOrganization[]>(true, "", orgs);
            }
            catch (Exception ex)
            {
                logger.LogError($"PublicService.GetOrganizationsBySpecId Error:{ex.Message}");
                return new Answer<viOrganization[]>(false, "Ошибка программы", null);
            }
        }

        public async ValueTask<Answer<viStaff[]>> GetStaffsByOrgId(int orgId)
        {
            try
            {
                var staffs = await db.tbStaffs
                    .AsNoTracking()
                    .Where(x => x.Status == 1 && x.OrganizationId == orgId && x.RoleId == (int)UserRoles.STAFF)
                    .Select(x => new viStaff
                    {
                        Id = x.Id,
                        Availability = x.Availability,
                        Name = x.Name,
                        Surname = x.Surname,
                        Patronymic = x.Patronymic,
                        Gender = x.Gender,
                        PhoneNum = x.PhoneNum,
                        BirthDay = x.BirthDay,
                        PhotoUrl = x.PhotoUrl,
                        Rating = x.Rating,
                        PeriodTime = x.PeriodTime,
                        CategoryId = x.CategoryId,
                        Category = x.Category.NameRu,
                        Experience = x.Experience,
                        TelegramId = x.TelegramId,
                        OrganizationId = x.OrganizationId,
                        Organization = x.Organization.Name
                    })
                    .ToArrayAsync();

                return new Answer<viStaff[]>(true, "", staffs);
            }
            catch (Exception ex)
            {
                logger.LogError($"PublicService.GetStaffsByOrgId Error:{ex.Message}");
                return new Answer<viStaff[]>(false, "Ошибка программы", null);
            }
        }

        public async ValueTask<Answer<viCategory[]>> GetCategoriesByOrgId(int orgId)
        {
            try
            {
                var categories = await db.spCategories
                    .AsNoTracking()
                    .Where(x => x.Status == 1 && x.OrganizationId == orgId)
                    .Select(x => new viCategory
                    {
                        Id = x.Id,
                        NameLt = x.NameLt,
                        NameRu = x.NameRu,
                        NameUz = x.NameUz,
                        Organization = x.Organization.Name,
                        OrganizationId = x.OrganizationId
                    })
                    .ToArrayAsync();

                return new Answer<viCategory[]>(true, "", categories);
            }
            catch (Exception ex)
            {
                logger.LogError($"PublicService.GetCategoriesByOrgId Error:{ex.Message}");
                return new Answer<viCategory[]>(false, "Ошибка программы", null);
            }
        }
    }
}
