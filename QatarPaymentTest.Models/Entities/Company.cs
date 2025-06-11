using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QatarPaymentTest.Models.Entities
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        public virtual ICollection<CompanyCustomFieldValue> CustomFieldValues { get; set; } = new List<CompanyCustomFieldValue>();
    }
}
