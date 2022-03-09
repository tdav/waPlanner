using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace waPlanner.Database
{
    public class MyDbContextFactory : IDesignTimeDbContextFactory<MyDbContext>
    {
        public MyDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<MyDbContext>();
            var connectionString = "Host=127.0.0.1;Database=market_db;Username=postgres;Password=grand_online_orders;Pooling=true;";
            options.UseNpgsql(connectionString, b => b.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                   .EnableDetailedErrors()
                   .EnableSensitiveDataLogging()
                   .UseSnakeCaseNamingConvention();

            return new MyDbContext(options.Options);
        }
    }
}
