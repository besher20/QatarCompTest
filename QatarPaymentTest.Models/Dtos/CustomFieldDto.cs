using QatarPaymentTest.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QatarPaymentTest.Models.Dtos
{
    public class CustomFieldDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public CustomFieldType FieldType { get; set; }
        public bool IsRequired { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
