using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using QatarPaymentTest.Models.Entities;
using Microsoft.EntityFrameworkCore;
using QatarPaymentTest.Models.Enum;

namespace QatarPaymentTest.Data.DbContextApp
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<CustomField> CustomFields { get; set; }
        public DbSet<CompanyCustomFieldValue> CompanyCustomFieldValues { get; set; }
        public DbSet<ContactCustomFieldValue> ContactCustomFieldValues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Company
            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasMany(e => e.Contacts)
                      .WithMany(e => e.Companies)
                      .UsingEntity("CompanyContact");
            });

            // Configure Contact
            modelBuilder.Entity<Contact>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            // Configure CustomField
            modelBuilder.Entity<CustomField>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.Name, e.EntityType }).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.EntityType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.FieldType).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            // Configure CompanyCustomFieldValue
            modelBuilder.Entity<CompanyCustomFieldValue>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.CompanyId, e.CustomFieldId }).IsUnique();

                entity.HasOne(e => e.Company)
                      .WithMany(e => e.CustomFieldValues)
                      .HasForeignKey(e => e.CompanyId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.CustomField)
                      .WithMany(e => e.CompanyValues)
                      .HasForeignKey(e => e.CustomFieldId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure ContactCustomFieldValue
            modelBuilder.Entity<ContactCustomFieldValue>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.ContactId, e.CustomFieldId }).IsUnique();

                entity.HasOne(e => e.Contact)
                      .WithMany(e => e.CustomFieldValues)
                      .HasForeignKey(e => e.ContactId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.CustomField)
                      .WithMany(e => e.ContactValues)
                      .HasForeignKey(e => e.CustomFieldId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed data
           // SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed some initial companies
            modelBuilder.Entity<Company>().HasData(
                new Company { Id = 1, Name = "Microsoft Corporation"  },
                new Company { Id = 2, Name = "Google LLC"  },
                new Company { Id = 3, Name = "Apple Inc." }
            );

            // Seed some initial contacts
            modelBuilder.Entity<Contact>().HasData(
                new Contact { Id = 1, Name = "أحمد محمد"  },
                new Contact { Id = 2, Name = "سارة أحمد"  },
                new Contact { Id = 3, Name = "محمد علي"  }
            );

            // Seed some custom fields
            modelBuilder.Entity<CustomField>().HasData(
                new CustomField
                {
                    Id = 1,
                    Name = "تاريخ الميلاد",
                    EntityType = "Contact",
                    FieldType = CustomFieldType.Date,
                    Description = "تاريخ ميلاد جهة الاتصال",
                  
                },
                new CustomField
                {
                    Id = 2,
                    Name = "رقم الهاتف",
                    EntityType = "Contact",
                    FieldType = CustomFieldType.Text,
                    Description = "رقم هاتف جهة الاتصال",
                  
                },
                new CustomField
                {
                    Id = 3,
                    Name = "عدد الموظفين",
                    EntityType = "Company",
                    FieldType = CustomFieldType.Number,
                    Description = "عدد الموظفين في الشركة",
                  
                }
            );
        }
    }
}
