using HM_byDH.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace HM_byDH.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<FoodItem> FoodItems { get; set; }
        public DbSet<FoodEntry> FoodEntries { get; set; }
        public DbSet<WaterIntake> WaterIntakes { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; } // Thêm DbSet cho UserSettings
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình quan hệ
            modelBuilder.Entity<FoodEntry>()
                .HasOne(fe => fe.User)
                .WithMany()
                .HasForeignKey(fe => fe.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FoodEntry>()
                .HasOne(fe => fe.FoodItem)
                .WithMany()
                .HasForeignKey(fe => fe.FoodItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WaterIntake>()
                .HasOne(wi => wi.User)
                .WithMany()
                .HasForeignKey(wi => wi.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserSettings>()
            .HasOne(us => us.User)
            .WithOne()
            .HasForeignKey<UserSettings>(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
