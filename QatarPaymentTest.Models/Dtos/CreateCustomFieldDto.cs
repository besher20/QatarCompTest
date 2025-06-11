using QatarPaymentTest.Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QatarPaymentTest.Models.Dtos
{
    public class CreateCustomFieldDto
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(Company|Contact)$", ErrorMessage = "EntityType must be either 'Company' or 'Contact'")]
        public string EntityType { get; set; } = string.Empty;

        [Required]
        public CustomFieldType FieldType { get; set; }

        public bool IsRequired { get; set; } = false;

        [StringLength(500)]
        public string? Description { get; set; }
    }

}
