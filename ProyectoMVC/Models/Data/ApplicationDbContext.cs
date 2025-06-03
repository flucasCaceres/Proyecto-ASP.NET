using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ProyectoMVC.Models;

namespace ProyectoMVC.Models.Data
{
    public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(){}
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.name)
                .HasMaxLength(50)
                .HasColumnName("name");

                entity.Property(e => e.secondName)
                .HasMaxLength(50)
                .HasColumnName("secondName");

                entity.Property(e => e.registerDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("registerDate");

                entity.Property(e => e.lastLogin)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("lastLogin");

                entity.Property(e => e.active)
                .HasDefaultValue(true)
                .HasColumnName("active");
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
