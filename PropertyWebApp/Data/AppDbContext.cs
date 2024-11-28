using Microsoft.EntityFrameworkCore;
using PropertyWebApp.Models;

namespace PropertyWebApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Property> Properties { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<IssueStatus> IssueStatuses { get; set; }
        public DbSet<IssueImage> IssueImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Definovanie vzťahov a pravidiel
            modelBuilder.Entity<Issue>()
                .HasOne(i => i.Status)
                .WithMany(s => s.Issues)
                .HasForeignKey(i => i.StatusId);

            //modelBuilder.Entity<Issue>()
                //.HasOne(i => i.Rental)
                //.WithMany(r => r.Issues)
               //.HasForeignKey(i => i.RentalId);

            modelBuilder.Entity<IssueImage>()
                .HasOne(ii => ii.Issue)
                .WithMany(i => i.Images)
                .HasForeignKey(ii => ii.IssueId);
        }
    }
}
