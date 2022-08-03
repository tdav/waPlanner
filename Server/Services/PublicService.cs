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
        ValueTask<Answer<viOrganization[]>> GetOrganizations();
        ValueTask<Answer<viOrganization>> GetOrganizationById(int id);
        ValueTask<Answer<viPublicSearch>> PublicSearch(string param);
    }
    public class PublicService : IPublicService, IAutoRegistrationScopedLifetimeService
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
                        OrganizationInfo = x.Info,
                        WorkStart = x.WorkStart,
                        WorkEnd = x.WorkEnd,
                        BreakTimeStart = x.BreakTimeStart,
                        BreakTimeEnd = x.BreakTimeEnd,
                        Rating = x.Rating.Value
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

        public async ValueTask<Answer<viOrganization[]>> GetOrganizations()
        {
            try
            {
                var orgs = await db.spOrganizations
                    .AsNoTracking()
                    .Where(x => x.Status == 1)
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
                        OrganizationInfo = x.Info,
                        WorkStart = x.WorkStart,
                        WorkEnd = x.WorkEnd,
                        BreakTimeStart = x.BreakTimeStart,
                        BreakTimeEnd = x.BreakTimeEnd,
                        Rating = x.Rating.Value
                    })
                    .ToArrayAsync();

                return new Answer<viOrganization[]>(true, "", orgs);
            }
            catch (Exception ex)
            {
                logger.LogError($"PublicService.GetOrganizations Error:{ex.Message}");
                return new Answer<viOrganization[]>(false, "Ошибка программы", null);
            }
        }

        public async ValueTask<Answer<viOrganization>> GetOrganizationById(int id)
        {
            try
            {
                var org = await db.spOrganizations
                    .AsNoTracking()
                    .Where(x => x.Status == 1 && x.Id == id)
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
                        OrganizationInfo = x.Info,
                        WorkStart = x.WorkStart,
                        WorkEnd = x.WorkEnd,
                        BreakTimeStart = x.BreakTimeStart,
                        BreakTimeEnd = x.BreakTimeEnd,
                        Rating = x.Rating.Value
                    })
                    .FirstOrDefaultAsync();

                return new Answer<viOrganization>(true, "", org);
            }
            catch (Exception ex)
            {
                logger.LogError($"PublicService.GetOrganizationById Error:{ex.Message}");
                return new Answer<viOrganization>(false, "Ошибка программы", null);
            }
        }

        public async ValueTask<Answer<viPublicSearch>> PublicSearch(string param)
        {
            try
            {
                var search_staffs = await (from s in db.tbStaffs
                                           where EF.Functions.ILike(s.Surname, $"%{param}%")
                                           || EF.Functions.ILike(s.Name, $"%{param}%")
                                           || EF.Functions.ILike(s.Patronymic, $"%{param}%")
                                           select s)
                                        .AsNoTracking()
                                        .Where(x => x.Status == 1 && x.RoleId == (int)UserRoles.STAFF)
                                        .Select(x => new viStaff
                                        {
                                            Id = x.Id,
                                            Name = x.Name,
                                            Surname = x.Surname,
                                            BirthDay = x.BirthDay,
                                            PhoneNum = x.PhoneNum,
                                            Patronymic = x.Patronymic,
                                            TelegramId = x.TelegramId,
                                            Online = x.Online.Value,
                                            Availability = x.Availability,
                                            Experience = x.Experience,
                                            OrganizationId = x.OrganizationId,
                                            Organization = x.Organization.Name,
                                            CategoryId = x.CategoryId,
                                            Category = x.Category.NameUz,
                                            RoleId = x.RoleId,
                                            PhotoUrl = x.PhotoUrl,
                                            Gender = x.Gender
                                        })
                                        .ToArrayAsync();

                var search_category = await (from s in db.spCategories
                                             where EF.Functions.ILike(s.NameLt, $"%{param}%")
                                             || EF.Functions.ILike(s.NameRu, $"%{param}%")
                                             || EF.Functions.ILike(s.NameUz, $"%{param}%")
                                             select s)
                                       .AsNoTracking()
                                       .Where(x => x.Status == 1)
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

                var search_org = await (from s in db.spOrganizations
                                        where EF.Functions.ILike(s.Name, $"%{param}%")
                                        select s)
                                  .AsNoTracking()
                                  .Where(x => x.Status == 1)
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
                                      OrganizationInfo = x.Info,
                                      WorkStart = x.WorkStart,
                                      WorkEnd = x.WorkEnd,
                                      BreakTimeStart = x.BreakTimeStart,
                                      BreakTimeEnd = x.BreakTimeEnd,
                                      Rating = x.Rating.Value
                                  })
                                  .ToArrayAsync();

                var all = new viPublicSearch
                {
                    StaffList = search_staffs,
                    CategoryList = search_category,
                    OrganizationList = search_org
                };

                return new Answer<viPublicSearch>(true, "", all);
            }
            catch (Exception ex)
            {
                logger.LogError($"PublicService.PublicSearch Error:{ex.Message}");
                return new Answer<viPublicSearch>(false, "Сбой в программе", null);
            }
        }
    }
}
