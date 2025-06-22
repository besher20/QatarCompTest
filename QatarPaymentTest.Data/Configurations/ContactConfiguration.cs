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
    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            // Primary Key
            builder.HasKey(c => c.Id);

            // Properties
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("citext");
            
            builder.Property(c => c.Email)
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
                .HasDatabaseName("IX_Contacts_Name_Trigram");

            builder.HasIndex(c => c.Email)
                .IsUnique()
                .HasMethod("btree");

            // Add a separate trigram index for email search
            builder.HasIndex(c => c.Email)
                .HasMethod("gist")
                .HasOperators(new[] { "gist_trgm_ops" })
                .HasDatabaseName("IX_Contacts_Email_Trigram");

            // Seed Data
            builder.HasData(
                new Contact
                {
                    Id = 1,
                    FirstName = "Mohammed Al-Thani",
                    CreatedAt = DateTime.UtcNow
                },
                new Contact
                {
                    Id = 2,
                    FirstName = "Fatima Al-Sayed",
                    CreatedAt = DateTime.UtcNow
                },
                new Contact
                {
                    Id = 3,
                    FirstName = "Ahmed Al-Kuwari",
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
