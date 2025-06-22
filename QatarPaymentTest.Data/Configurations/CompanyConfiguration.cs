using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QatarPaymentTest.Models.Entities;

namespace QatarPaymentTest.Data.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            // Primary Key
            builder.HasKey(c => c.Id);

            // Properties
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("citext");

            // Indexes
            builder.HasIndex(c => c.Name)
                .IsUnique()
                .HasMethod("btree");

            // Add a separate trigram index for text search
            builder.HasIndex(c => c.Name)
                .HasMethod("gist")
                .HasOperators(new[] { "gist_trgm_ops" })
                .HasDatabaseName("IX_Companies_Name_Trigram");

            // Seed Data
            builder.HasData(
                new Company
                {
                    Id = 1,
                    Name = "Qatar Airways",
                    CreatedAt = DateTime.UtcNow
                },
                new Company
                {
                    Id = 2,
                    Name = "Qatar National Bank",
                    CreatedAt = DateTime.UtcNow
                },
                new Company
                {
                    Id = 3,
                    Name = "Ooredoo Qatar",
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
