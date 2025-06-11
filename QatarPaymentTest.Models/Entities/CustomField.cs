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

        [Required]
        [StringLength(50)]
        public string EntityType { get; set; } = string.Empty; // "Company" or "Contact"

        public CustomFieldType FieldType { get; set; }

        public bool IsRequired { get; set; } = false;

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<CompanyCustomFieldValue> CompanyValues { get; set; } = new List<CompanyCustomFieldValue>();
        public virtual ICollection<ContactCustomFieldValue> ContactValues { get; set; } = new List<ContactCustomFieldValue>();
    }
}
