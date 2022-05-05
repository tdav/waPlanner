using Microsoft.Extensions.DependencyInjection;
using waPlanner.Controllers.v1;
using waPlanner.Services;
using waPlanner.TelegramBot.Utils;

namespace waPlanner.Extensions
{
    public static class MyService
    {
        public static void AddMyService(this IServiceCollection services)
        {
            services.AddScoped<IDbManipulations, DbManipulations>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISchedulerService, SchedulerService>();
            services.AddScoped<ISetupService, SetupService>();
            services.AddScoped<IOrganizationService, OrganizationService>();
            services.AddScoped<IStaffService, StaffService>();
            services.AddScoped<IInfoService, InfoService>();
            services.AddScoped<ISpecializationService, SpecializationService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IGenerateQrCode, GenerateQrCode>();
            services.AddSingleton<ITelegramGroupCreatorService, TelegramGroupCreatorService>();
        }
    }
}
