using QatarPaymentTest.Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QatarPaymentTest.Models.Dtos
{
    public class CustomFieldDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string EntityType { get; set; } = string.Empty;

        public CustomFieldType FieldType { get; set; }
        public bool IsRequired { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public string? DefaultValue { get; set; }
        public string? ValidationRegex { get; set; }
        public List<string>? AllowedValues { get; set; }
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }

    public class CreateCustomFieldDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string EntityType { get; set; } = string.Empty;

        public CustomFieldType FieldType { get; set; }
        public bool IsRequired { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public string? DefaultValue { get; set; }
        public string? ValidationRegex { get; set; }
        public List<string>? AllowedValues { get; set; }
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
    }


    public class CustomFieldUsageDto
    {
        public int CustomFieldId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public int TotalUsageCount { get; set; }
        public Dictionary<string, int> ValueDistribution { get; set; } = new();
        public DateTime LastUsed { get; set; }
    }
}
