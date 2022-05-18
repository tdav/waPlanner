using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Linq;
using Toolbelt.ComponentModel.DataAnnotations;
using waPlanner.Database.Models;

namespace waPlanner.Database
{
    public partial class MyDbContext : DbContext
    {

        #region dbSet
        public DbSet<spCategory> spCategories { get; set; }
        public DbSet<spOrganization> spOrganizations { get; set; }
        public DbSet<spRole> spRoles { get; set; }
        public DbSet<spSetup> spSetups { get; set; }
        public DbSet<spSpecialization> spSpecializations{ get; set; }
        public DbSet<tbAnalizeResult> tbAnalizeResults{ get; set; }
        public DbSet<tbFavorites> tbFavorites { get; set; }
        public DbSet<tbRating> tbRatings { get; set; }
        public DbSet<tbScheduler> tbSchedulers { get; set; }
        public DbSet<tbStaff> tbStaffs { get; set; }
        public DbSet<tbUser> tbUsers { get; set; }
        
        #endregion

        public MyDbContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            //if (!options.IsConfigured)
            {
                var connectionString = "Host=127.0.0.1;Database=planner_db;Username=postgres;Password=1;Pooling=true;";
                
                options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
                options.EnableDetailedErrors();
            }
        }
 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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