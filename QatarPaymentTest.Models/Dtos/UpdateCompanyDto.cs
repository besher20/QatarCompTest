using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QatarPaymentTest.Models.Dtos
{
    public class UpdateCompanyDto
    {
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        public Dictionary<string, object?> CustomFields { get; set; } = new();
    }

}
