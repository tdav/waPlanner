using Microsoft.Extensions.DependencyInjection;
using System;
using waPlanner.Controllers.v1;
using waPlanner.Interfaces;
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
            services.AddScoped<IGenerateQrCodeService, GenerateQrCodeService>();
            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddSingleton<ITelegramGroupCreatorService, TelegramGroupCreatorService>();



            services.Scan(s => s.FromAssemblyOf<IAutoRegistrationScopedLifetimeService>()
               .AddClasses(c => c.AssignableTo<IAutoRegistrationScopedLifetimeService>())
                   .AsImplementedInterfaces()
                   .WithScopedLifetime());

        }
    }
}
