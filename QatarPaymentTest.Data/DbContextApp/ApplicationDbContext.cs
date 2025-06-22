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

            // Enable extensions
            modelBuilder.HasPostgresExtension("citext");
            modelBuilder.HasPostgresExtension("pg_trgm");

            // Configure Company
            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // Unique index with btree
                entity.HasIndex(e => e.Name)
                      .IsUnique()
                      .HasMethod("btree")
                      .HasDatabaseName("IX_Companies_Name_Unique");

                // Text search index with gist (non-unique)
                entity.HasIndex(e => e.Name)
                      .HasMethod("gist")
                      .HasOperators("gist_trgm_ops")
                      .HasDatabaseName("IX_Companies_Name_Search");

                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(200)
                      .HasColumnType("citext");

                entity.Property(e => e.Description)
                      .HasMaxLength(1000);

                entity.Property(e => e.CreatedAt)
                      .IsRequired()
                      .HasColumnType("timestamp with time zone");

                entity.Property(e => e.IsDeleted)
                      .HasDefaultValue(false);

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.HasMany(e => e.Contacts)
                      .WithMany(e => e.Companies)
                      .UsingEntity(
                          "CompanyContacts",
                          l => l.HasOne(typeof(Contact)).WithMany().HasForeignKey("ContactId"),
                          r => r.HasOne(typeof(Company)).WithMany().HasForeignKey("CompanyId"),
                          j => j.HasKey("CompanyId", "ContactId")
                      );
            });

            // Configure Contact
            modelBuilder.Entity<Contact>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // Unique index with btree
                entity.HasIndex(e => e.Email)
                      .IsUnique()
                      .HasMethod("btree")
                      .HasDatabaseName("IX_Contacts_Email_Unique");

                // Text search index with gist (non-unique)
                entity.HasIndex(e => e.Email)
                      .HasMethod("gist")
                      .HasOperators("gist_trgm_ops")
                      .HasDatabaseName("IX_Contacts_Email_Search");

                entity.Property(e => e.FirstName)
                      .IsRequired()
                      .HasMaxLength(100)
                      .HasColumnType("citext");

                entity.Property(e => e.LastName)
                      .IsRequired()
                      .HasMaxLength(100)
                      .HasColumnType("citext");

                entity.Property(e => e.Email)
                      .IsRequired()
                      .HasMaxLength(200)
                      .HasColumnType("citext");

                entity.Property(e => e.CreatedAt)
                      .IsRequired()
                      .HasColumnType("timestamp with time zone");

                entity.Property(e => e.IsDeleted)
                      .HasDefaultValue(false);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Configure CustomField
            modelBuilder.Entity<CustomField>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.HasIndex(e => new { e.Name, e.EntityType })
                      .IsUnique()
                      .HasMethod("btree");

                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(100)
                      .HasColumnType("citext");

                entity.Property(e => e.EntityType)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(e => e.FieldType)
                      .IsRequired()
                      .HasConversion<string>();

                entity.Property(e => e.CreatedAt)
                      .IsRequired()
                      .HasColumnType("timestamp with time zone");

                entity.Property(e => e.IsDeleted)
                      .HasDefaultValue(false);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Configure CompanyCustomFieldValue
            modelBuilder.Entity<CompanyCustomFieldValue>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.HasIndex(e => new { e.CompanyId, e.CustomFieldId })
                      .IsUnique()
                      .HasMethod("btree");

                entity.HasIndex(e => e.Value)
                      .HasMethod("gin")
                      .HasOperators("gin_trgm_ops");

                entity.Property(e => e.Value)
                      .HasColumnType("text");

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
                
                entity.HasIndex(e => new { e.ContactId, e.CustomFieldId })
                      .IsUnique()
                      .HasMethod("btree");

                entity.HasIndex(e => e.Value)
                      .HasMethod("gin")
                      .HasOperators("gin_trgm_ops");

                entity.Property(e => e.Value)
                      .HasColumnType("text");

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
            //SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            try
            {
                // 1. Seed Companies
                modelBuilder.Entity<Company>().HasData(
                    new Company 
                    { 
                        Id = 1, 
                        Name = "Microsoft Corporation", 
                        CreatedAt = seedDate,
                        Description = "Leading technology company",
                        IsDeleted = false
                    },
                    new Company 
                    { 
                        Id = 2, 
                        Name = "Google LLC", 
                        CreatedAt = seedDate,
                        Description = "Search and technology giant",
                        IsDeleted = false
                    },
                    new Company 
                    { 
                        Id = 3, 
                        Name = "Apple Inc.", 
                        CreatedAt = seedDate,
                        Description = "Consumer electronics and software",
                        IsDeleted = false
                    }
                );

                // 2. Seed Contacts
                modelBuilder.Entity<Contact>().HasData(
                    new Contact 
                    { 
                        Id = 1, 
                        FirstName = "أحمد",
                        LastName = "محمد",
                        Email = "ahmed.m@example.com",
                        CreatedAt = seedDate,
                        IsPrimary = true,
                        IsInactive = false,
                        IsDeleted = false
                    },
                    new Contact 
                    { 
                        Id = 2, 
                        FirstName = "سارة",
                        LastName = "أحمد",
                        Email = "sara.a@example.com",
                        CreatedAt = seedDate,
                        IsPrimary = true,
                        IsInactive = false,
                        IsDeleted = false
                    },
                    new Contact 
                    { 
                        Id = 3, 
                        FirstName = "محمد",
                        LastName = "علي",
                        Email = "mohamed.a@example.com",
                        CreatedAt = seedDate,
                        IsPrimary = true,
                        IsInactive = false,
                        IsDeleted = false
                    }
                );

                // 3. Seed Custom Fields
                modelBuilder.Entity<CustomField>().HasData(
                    new CustomField
                    {
                        Id = 1,
                        Name = "تاريخ الميلاد",
                        EntityType = "Contact",
                        FieldType = CustomFieldType.Date,
                        Description = "تاريخ ميلاد جهة الاتصال",
                        CreatedAt = seedDate,
                        IsRequired = true,
                        IsDeleted = false
                    },
                    new CustomField
                    {
                        Id = 2,
                        Name = "رقم الهاتف",
                        EntityType = "Contact",
                        FieldType = CustomFieldType.Text,
                        Description = "رقم هاتف جهة الاتصال",
                        CreatedAt = seedDate,
                        IsRequired = true,
                        IsDeleted = false
                    },
                    new CustomField
                    {
                        Id = 3,
                        Name = "عدد الموظفين",
                        EntityType = "Company",
                        FieldType = CustomFieldType.Number,
                        Description = "عدد الموظفين في الشركة",
                        CreatedAt = seedDate,
                        IsRequired = false,
                        IsDeleted = false
                    }
                );

                // 4. Seed Company-Contact Relationships
                modelBuilder.Entity("CompanyContacts").HasData(
                    new { CompanyId = 1, ContactId = 1 },
                    new { CompanyId = 2, ContactId = 2 },
                    new { CompanyId = 3, ContactId = 3 }
                );

                // 5. Seed Contact Custom Field Values
                modelBuilder.Entity<ContactCustomFieldValue>().HasData(
                    new ContactCustomFieldValue 
                    { 
                        Id = 1, 
                        ContactId = 1, 
                        CustomFieldId = 1, 
                        Value = "1990-01-01",
                        CreatedAt = seedDate 
                    },
                    new ContactCustomFieldValue 
                    { 
                        Id = 2, 
                        ContactId = 1, 
                        CustomFieldId = 2, 
                        Value = "+974 3000 0001",
                        CreatedAt = seedDate 
                    },
                    new ContactCustomFieldValue 
                    { 
                        Id = 3, 
                        ContactId = 2, 
                        CustomFieldId = 1, 
                        Value = "1992-05-15",
                        CreatedAt = seedDate 
                    },
                    new ContactCustomFieldValue 
                    { 
                        Id = 4, 
                        ContactId = 2, 
                        CustomFieldId = 2, 
                        Value = "+974 3000 0002",
                        CreatedAt = seedDate 
                    },
                    new ContactCustomFieldValue 
                    { 
                        Id = 5, 
                        ContactId = 3, 
                        CustomFieldId = 1, 
                        Value = "1988-12-30",
                        CreatedAt = seedDate 
                    },
                    new ContactCustomFieldValue 
                    { 
                        Id = 6, 
                        ContactId = 3, 
                        CustomFieldId = 2, 
                        Value = "+974 3000 0003",
                        CreatedAt = seedDate 
                    }
                );

                // 6. Seed Company Custom Field Values
                modelBuilder.Entity<CompanyCustomFieldValue>().HasData(
                    new CompanyCustomFieldValue 
                    { 
                        Id = 1, 
                        CompanyId = 1, 
                        CustomFieldId = 3, 
                        Value = "150000",
                        CreatedAt = seedDate 
                    },
                    new CompanyCustomFieldValue 
                    { 
                        Id = 2, 
                        CompanyId = 2, 
                        CustomFieldId = 3, 
                        Value = "180000",
                        CreatedAt = seedDate 
                    },
                    new CompanyCustomFieldValue 
                    { 
                        Id = 3, 
                        CompanyId = 3, 
                        CustomFieldId = 3, 
                        Value = "160000",
                        CreatedAt = seedDate 
                    }
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Error seeding data: {ex.Message}", ex);
            }
        }
    }
}
