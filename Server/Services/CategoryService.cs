using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;
using waPlanner.Extensions;
using waPlanner.Interfaces;
using waPlanner.ModelViews;
using waPlanner.TelegramBot;
using waPlanner.Utils;

namespace waPlanner.Services
{
    public interface ICategoryService
    {
        Task<Answer<int>> AddCategoryAsync(viCategory value);
        Task<AnswerBasic> UpdateAsync(viCategory value);
        Task<Answer<viCategory[]>> GetAllCategoriesAsync();
        Task<AnswerBasic> ChangeCategoryStatus(viSetStatus status);
        Task<Answer<viCategory>> GetCategoryByIdAsync(int category_id);
        Task<Answer<viCategory[]>> SearchCategory(string name);
    }

    public class CategoryService : ICategoryService, IAutoRegistrationScopedLifetimeService
    {
        private readonly MyDbContext db;
        private readonly IHttpContextAccessorExtensions accessor;
        private readonly ILogger<CategoryService> logger;
        public CategoryService(MyDbContext db, IHttpContextAccessorExtensions accessor, ILogger<CategoryService> logger)
        {
            this.db = db;
            this.accessor = accessor;
            this.logger = logger;
        }

        public async Task<Answer<viCategory>> GetCategoryByIdAsync(int category_id)
        {
            try
            {
                var category = await db.spCategories
                .AsNoTracking()
                .Where(x => x.Id == category_id)
                .Select(x => new viCategory
                {
                    Id = x.Id,
                    NameLt = x.NameLt,
                    NameRu = x.NameRu,
                    NameUz = x.NameUz,
                    OrganizationId = x.OrganizationId,
                    Organization = x.Organization.Name,
                    Status = x.Status,
                })
                .FirstOrDefaultAsync();
                return new Answer<viCategory>(true, "", category);
            }
            catch (Exception e)
            {
                logger.LogError($"InfoService.GetTotalTodayAppointments Error:{e.Message}");
                return new Answer<viCategory>(false, "Ошибка программы", null);
            }
        }

        public async Task<Answer<viCategory[]>> GetAllCategoriesAsync()
        {
            try
            {
                int org_id = accessor.GetOrgId();
                var categories = await db.spCategories
                    .AsNoTracking()
                    .Where(x => x.OrganizationId == org_id)
                    .Select(x => new viCategory
                    {
                        Id = x.Id,
                        NameLt = x.NameLt,
                        NameRu = x.NameRu,
                        NameUz = x.NameUz,
                        OrganizationId = org_id,
                        Organization = x.Organization.Name,
                        Status = x.Status,
                    })
                    .ToArrayAsync();
                return new Answer<viCategory[]>(true, "", categories);
            }
            catch (Exception e)
            {
                logger.LogError($"InfoService.GetTotalTodayAppointments Error:{e.Message}");
                return new Answer<viCategory[]>(false, "Ошибка программы", null);
            }
        }

        public async Task<Answer<int>> AddCategoryAsync(viCategory value)
        {
            try
            {
                var category = new spCategory();
                int user_id = accessor.GetId();
                int org_id = accessor.GetOrgId();
                int role_id = accessor.GetRoleId();

                if (role_id == (int)UserRoles.SUPER_ADMIN)
                    org_id = value.OrganizationId.Value;

                category.Status = 1;
                category.OrganizationId = org_id;
                category.NameRu = value.NameRu;
                category.NameUz = value.NameUz;
                category.NameLt = value.NameLt;
                category.CreateDate = DateTime.Now;
                category.CreateUser = user_id;
                await db.spCategories.AddAsync(category);
                await db.SaveChangesAsync();

                return new Answer<int>(true, "", category.Id);
            }
            catch (Exception e)
            {
                logger.LogError($"InfoService.GetTotalTodayAppointments Error:{e.Message} Model: {value.ToJson()}");
                return new Answer<int>(false, "Ошибка программы", 0);
            }
        }

        public async Task<AnswerBasic> UpdateAsync(viCategory value)
        {
            try
            {
                var category = await db.spCategories.FindAsync(value.Id);
                int org_id = accessor.GetOrgId();
                int user_id = accessor.GetId();

                category.OrganizationId = org_id;

                category.Status = value.Status.Value;

                category.NameRu = value.NameRu;
                category.NameUz = value.NameUz;
                category.NameLt = value.NameLt;

                category.UpdateDate = DateTime.Now;
                category.UpdateUser = user_id;
                await db.SaveChangesAsync();
                return new AnswerBasic(true, "");
            }
            catch (Exception e)
            {
                logger.LogError($"InfoService.GetTotalTodayAppointments Error:{e.Message} Model: {value.ToJson()}");
                return new AnswerBasic(false, "Ошибка программы");
            }
        }

        public async Task<AnswerBasic> ChangeCategoryStatus(viSetStatus status)
        {
            try
            {
                int user_id = accessor.GetId();
                int org_id = accessor.GetOrgId();
                var get_category = await db.spCategories.AsNoTracking().FirstAsync(c => c.Id == status.Id && c.OrganizationId == org_id);
                var category = await db.spCategories.FindAsync(get_category.Id);
                category.Status = status.Status;
                category.UpdateDate = DateTime.Now;
                category.UpdateUser = user_id;
                await db.SaveChangesAsync();
                return new AnswerBasic(true, "");
            }
            catch (Exception e)
            {
                logger.LogError($"InfoService.GetTotalTodayAppointments Error:{e.Message}");
                return new AnswerBasic(false, "Ошибка программы");
            }
        }

        public async Task<Answer<viCategory[]>> SearchCategory(string name)
        {
            try
            {
                int org_id = accessor.GetOrgId();
                var search = await (from s in db.spCategories
                                    where EF.Functions.ILike(s.NameUz, $"%{name}%")
                                    || EF.Functions.ILike(s.NameRu, $"%{name}%")
                                    || EF.Functions.ILike(s.NameLt, $"%{name}%")
                                    select s)
                            .AsNoTracking()
                            .Where(x => x.Status == 1 && x.OrganizationId == org_id)
                            .Select(x => new viCategory
                            {
                                Id = x.Id,
                                NameUz = x.NameUz,
                                NameLt = x.NameLt,
                                NameRu = x.NameRu,
                                OrganizationId = x.OrganizationId,
                                Organization = x.Organization.Name,
                                Status = x.Status
                            })
                            .ToArrayAsync();
                return new Answer<viCategory[]>(true, "", search);
            }
            catch (Exception e)
            {
                logger.LogError($"InfoService.GetTotalTodayAppointments Error:{e.Message}");
                return new Answer<viCategory[]>(false, "Ошибка программы", null);
            }
        }
    }
}
