using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QatarPaymentTest.Models.Entities
{
    public class CompanyCustomFieldValue
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int CustomFieldId { get; set; }
        public string? Value { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual Company Company { get; set; } = null!;
        public virtual CustomField CustomField { get; set; } = null!;
    }
}
