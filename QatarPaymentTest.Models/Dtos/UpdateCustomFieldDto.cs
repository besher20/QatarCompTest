using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QatarPaymentTest.Models.Dtos
{
    public class UpdateCustomFieldDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        public bool IsRequired { get; set; } = false;

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
