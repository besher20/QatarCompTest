using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QatarPaymentTest.Models.Entities;
using QatarPaymentTest.Models.Enum;

namespace QatarPaymentTest.Data.Configurations
{
    public class CustomFieldConfiguration : IEntityTypeConfiguration<CustomField>
    {
        public void Configure(EntityTypeBuilder<CustomField> builder)
        {
            // Primary Key
            builder.HasKey(c => c.Id);

            // Properties
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("citext");

            builder.Property(c => c.EntityType)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnType("citext");

            builder.Property(c => c.Description)
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(c => new { c.Name, c.EntityType })
                .IsUnique()
                .HasMethod("btree");

            // Add a separate trigram index for text search
            builder.HasIndex(c => c.Name)
                .HasMethod("gist")
                .HasOperators(new[] { "gist_trgm_ops" })
                .HasDatabaseName("IX_CustomFields_Name_Trigram");

            // Seed Data
            builder.HasData(
                new CustomField
                {
                    Id = 1,
                    Name = "Email",
                    EntityType = "Company",
                    FieldType = CustomFieldType.Text,
                    IsRequired = true,
                    Description = "Company email address",
                    CreatedAt = DateTime.UtcNow
                },
                new CustomField
                {
                    Id = 2,
                    Name = "Phone",
                    EntityType = "Company",
                    FieldType = CustomFieldType.Text,
                    IsRequired = true,
                    Description = "Company phone number",
                    CreatedAt = DateTime.UtcNow
                },
                new CustomField
                {
                    Id = 3,
                    Name = "Email",
                    EntityType = "Contact",
                    FieldType = CustomFieldType.Text,
                    IsRequired = true,
                    Description = "Contact email address",
                    CreatedAt = DateTime.UtcNow
                },
                new CustomField
                {
                    Id = 4,
                    Name = "Mobile",
                    EntityType = "Contact",
                    FieldType = CustomFieldType.Text,
                    IsRequired = true,
                    Description = "Contact mobile number",
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
