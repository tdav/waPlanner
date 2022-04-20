using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using waPlanner.Database;

namespace waPlanner.Extensions
{
    public static class MyDbContextService
    {
        public static void AddMyDatabaseService(this IServiceCollection services, IConfiguration conf)
        {
            var connStr = conf.GetConnectionString("ConnectionString");
            services.AddDbContext<MyDbContext>(o => { o.UseNpgsql(connStr); });
                            }


        public static void UpdateMigrateDatabase(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                        .GetRequiredService<IServiceScopeFactory>()
                        .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<MyDbContext>())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}
