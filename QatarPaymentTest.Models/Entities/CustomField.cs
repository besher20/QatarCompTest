using QatarPaymentTest.Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QatarPaymentTest.Models.Entities
{
    public class CustomField
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(50)]
        public string EntityType { get; set; } = string.Empty;

        public CustomFieldType FieldType { get; set; }

        public bool IsRequired { get; set; }

        [StringLength(1000)]
        public string? DefaultValue { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        public virtual ICollection<ContactCustomFieldValue> ContactValues { get; set; } = new List<ContactCustomFieldValue>();
        public virtual ICollection<CompanyCustomFieldValue> CompanyValues { get; set; } = new List<CompanyCustomFieldValue>();
    }
}
