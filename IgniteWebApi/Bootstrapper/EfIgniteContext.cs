using IgniteWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace IgniteWebApi.Bootstrapper
{
    public class EfIgniteContext : DbContext
    {
        bool useInnerConnectionString;
        string innerConnectionString;
        public EfIgniteContext(string connectionString)
        {
            useInnerConnectionString = true;
            this.innerConnectionString = connectionString;
        }

        public EfIgniteContext(DbContextOptions<EfIgniteContext> options)
            : base(options)
        { }

        public virtual DbSet<Zone> Zones { get; set; }
        public virtual DbSet<Vehicle> Vehicles { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<GeoPoint> GeoPoints { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(innerConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Device>()
                .ToTable("Tbl_Device");

            modelBuilder.Entity<Zone>()
                .ToTable("Tbl_Zone");

            modelBuilder.Entity<Vehicle>()
                .ToTable("Tbl_Vehicle");

            modelBuilder.Entity<GeoPoint>()
                .ToTable("Tbl_GeoPoint");
        }
    }
}
