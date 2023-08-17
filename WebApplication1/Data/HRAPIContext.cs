using HR_API.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace HR_API.Data
{
    public class HRAPIDbContext : DbContext
    {
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<SalaryReport> SalaryReports { get; set; }

        public DbSet<Salary> Salaries { get; set; }

        public   HRAPIDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>()
                .HasIndex(role => role.Name)
                .IsUnique(true);
            
            modelBuilder.Entity<Role>().
                Property(x => x.Name).HasConversion(typeof(string));
        }
        
    }
}
