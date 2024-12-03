using Microsoft.EntityFrameworkCore;
using PropertyWebApp.Models;

namespace PropertyWebApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Issue> Issues { get; set; }
        public DbSet<IssueImage> IssueImages { get; set; }
        public DbSet<IssueStatus> IssueStatus { get; set; }
        public DbSet<MonthlyPayment> MonthlyPayments { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }
        public DbSet<PropertyType> PropertyTypes { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<RentCosts> RentCosts { get; set; }
        public DbSet<Repair> Repair { get; set; }
        public DbSet<RepairCosts> RepairCosts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TaggedIssue> TaggedIssues { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UtilitiesCosts> UtilitiesCosts { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // IssueStatus -> Issue (One-to-Many)
            modelBuilder.Entity<IssueStatus>()
                .HasMany(s => s.Issues)
                .WithOne(i => i.Status)
                .HasForeignKey(i => i.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // Property -> PropertyType (Many-to-One)
            modelBuilder.Entity<Property>()
                .HasOne(p => p.PropertyType)
                .WithMany(pt => pt.Properties)
                .HasForeignKey(p => p.TypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Property -> PropertyImage (One-to-Many)
            modelBuilder.Entity<PropertyImage>()
                .HasOne(pi => pi.Property)
                .WithMany(p => p.PropertyImages)
                .HasForeignKey(pi => pi.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Property -> Issue (One-to-Many)
            modelBuilder.Entity<Property>()
                .HasMany(p => p.Issues)
                .WithOne(i => i.Property)
                .HasForeignKey(i => i.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Property -> Rental (One-to-Many)
            modelBuilder.Entity<Property>()
                .HasMany(p => p.Rentals)
                .WithOne(r => r.Property)
                .HasForeignKey(r => r.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            // RepairCosts -> Repair (Many-to-One)
            modelBuilder.Entity<RepairCosts>()
                .HasOne(rc => rc.Repair)
                .WithMany()
                .HasForeignKey(rc => rc.RepairId)
                .OnDelete(DeleteBehavior.Cascade);

            // RepairCosts -> Rental (Many-to-One)
            modelBuilder.Entity<RepairCosts>()
                .HasOne(rc => rc.Rental)
                .WithMany()
                .HasForeignKey(rc => rc.RentalId)
                .OnDelete(DeleteBehavior.Restrict);

            // TaggedIssue -> Issue (Many-to-One)
            modelBuilder.Entity<TaggedIssue>()
                .HasOne(ti => ti.Issue)
                .WithMany(i => i.TaggedIssues)
                .HasForeignKey(ti => ti.IssueId)
                .OnDelete(DeleteBehavior.Cascade);

            // TaggedIssue -> Tag (Many-to-One)
            modelBuilder.Entity<TaggedIssue>()
                .HasOne(ti => ti.Tag)
                .WithMany(t => t.TaggedIssues)
                .HasForeignKey(ti => ti.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Issue>()
    .HasOne(i => i.Rental)
    .WithMany(r => r.Issues) // Ak `Rental` má navigačnú kolekciu `Issues`
    .HasForeignKey(i => i.RentalId)
    .OnDelete(DeleteBehavior.Restrict);
        }






    }
}
