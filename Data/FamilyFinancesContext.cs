using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FamilyFinances.Models;
using Microsoft.EntityFrameworkCore;

namespace FamilyFinances.Data
{
    public class FamilyFinancesContext : DbContext
    {
        public FamilyFinancesContext(DbContextOptions<FamilyFinancesContext> options) : base(options)
        {
        }

        public DbSet<Expense> Expenses { get; set; }
        public DbSet<PaySource> PaySources { get; set; }
        public DbSet<PurchaseCategory> PurchaseCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Expense>().ToTable("Expense");
            modelBuilder.Entity<PaySource>().ToTable("PaySource");
            modelBuilder.Entity<PurchaseCategory>().ToTable("PurchaseCategory");


            //modelBuilder.Entity<Department>()
            //    .Property(p => p.RowVersion).IsConcurrencyToken();

//            modelBuilder.Entity<CourseAssignment>()
//                .HasKey(c => new { c.CourseID, c.InstructorID });

            // See also https://docs.microsoft.com/en-us/ef/core/modeling/#use-fluent-api-to-configure-a-model
            //modelBuilder.Entity<Blog>()
            //    .Property(b => b.Url)
            //    .IsRequired();
        }

        public DbSet<FamilyFinances.Models.Inpayment> Inpayment { get; set; }
    }
}
