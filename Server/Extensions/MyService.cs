using Microsoft.Extensions.DependencyInjection;
using waPlanner.Interfaces;

namespace waPlanner.Extensions
{
    public static class MyService
    {
        public static void AddMyService(this IServiceCollection services)
        {
            services.Scan(s => s.FromAssemblyOf<IAutoRegistrationScopedLifetimeService>()
               .AddClasses(c => c.AssignableTo<IAutoRegistrationScopedLifetimeService>())
                   .AsImplementedInterfaces()
                   .WithScopedLifetime());

            services.Scan(s => s.FromAssemblyOf<IAutoRegistrationSingletonLifetimeService>()
               .AddClasses(c => c.AssignableTo<IAutoRegistrationSingletonLifetimeService>())
                   .AsImplementedInterfaces()
                   .WithSingletonLifetime());

        }
    }
}
