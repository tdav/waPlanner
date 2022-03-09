using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Linq;
using Toolbelt.ComponentModel.DataAnnotations;
using waPlanner.Database.Models;

namespace waPlanner.Database
{
    public partial class MyDbContext : DbContext
    {
        //private readonly IHttpContextAccessor accessor;
        //private readonly IConfiguration config;

        #region dbSet
        public DbSet<spCategory> spCategories { get; set; }
        public DbSet<spUserType> spUserTypes { get; set; }
        public DbSet<tbScheduler> tbSchedulers { get; set; }
        public DbSet<tbUser> tbUsers { get; set; }
        #endregion

        public MyDbContext(DbContextOptions options) : base(options)//IHttpContextAccessor _accessor,, IConfiguration _config
        {
            // accessor = _accessor;
            // config = _config;
            // this.ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            //if (!options.IsConfigured)
            {
                //var connectionString = "Host=127.0.0.1;Database=planner_db;Username=postgres;Password=1;Pooling=true;";
                //options.UseNpgsql(connectionString,
                //            b => b.EnableDetailedErrors()
                //                  .EnableSensitiveDataLogging()
                //                  .UseSnakeCaseNamingConvention()
                //                  .EnableServiceProviderCaching());

            }
        }

        #region Audit Base Model
        //public override int SaveChanges(bool acceptAllChangesOnSuccess)
        //{
        //    AuditEvent();
        //    return base.SaveChanges(acceptAllChangesOnSuccess);
        //}
        //public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        //{
        //    AuditEvent();
        //    return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        //}
        //public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        //{
        //    AuditEvent();
        //    return base.SaveChangesAsync(cancellationToken);
        //}
        //public override int SaveChanges()
        //{
        //    AuditEvent();
        //    return base.SaveChanges();
        //}

        //public int GetId()
        //{
        //    //var user = accessor.HttpContext.User.FindFirst(ClaimTypes.Sid);
        //    //if (user == null)
        //    //    return 1;  //Это когда новый пользователи
        //    //else
        //    //    return user.Value.ToInt();

        //    return 1;
        //}

        //private void AuditEvent()
        //{
        //    ChangeTracker.DetectChanges();

        //    var addList = ChangeTracker.Entries().Where(x => x.State == EntityState.Added);

        //    var UserId = GetId();

        //    foreach (var item in addList)
        //    {
        //        if (item.Entity is IBaseModel entity)
        //        {
        //            entity.Status = 1;
        //            entity.CreateUser = UserId;
        //            entity.CreateDate = DateTime.Now;
        //        }
        //    }

        //    var updateList = ChangeTracker.Entries().Where(x => x.State == EntityState.Modified);

        //    foreach (var item in updateList)
        //    {
        //        if (item.Entity is IBaseModel entity)
        //        {
        //            entity.UpdateUser = UserId;
        //            entity.UpdateDate = DateTime.Now;
        //        }
        //    }
        //}
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension("postgis");

            //modelBuilder.Seed();
            modelBuilder.BuildIndexesFromAnnotations();

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        public DbConnection GetDbConnection()
        {
            return Database.GetDbConnection();
        }

    }
}