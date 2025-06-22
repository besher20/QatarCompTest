using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace QatarPaymentTest.Models.Entities
{
    public class ContactCustomFieldValue
    {
        public int Id { get; set; }
        
        [Required]
        public int ContactId { get; set; }
        
        [Required]
        public int CustomFieldId { get; set; }
        
        public string? Value { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual Contact Contact { get; set; } = null!;
        public virtual CustomField CustomField { get; set; } = null!;
    }
}
