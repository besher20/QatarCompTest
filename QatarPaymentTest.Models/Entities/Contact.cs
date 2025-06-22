using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QatarPaymentTest.Models.Entities
{
    public class Contact
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        public string Name => $"{FirstName} {LastName}".Trim();

        [Required]
        [StringLength(200)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsPrimary { get; set; }
        public bool IsInactive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        public virtual ICollection<Company> Companies { get; set; } = new List<Company>();
        public virtual ICollection<ContactCustomFieldValue> CustomFieldValues { get; set; } = new List<ContactCustomFieldValue>();
    }
}
