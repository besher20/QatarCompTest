using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QatarPaymentTest.Models.Dtos
{
    public class CompanyDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public string? Website { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public bool IsDeleted { get; set; }

        public Dictionary<int, string> CustomFields { get; set; } = new();

        // Change to simplified contact info to avoid circular references
        public ICollection<ContactSummaryDto> Contacts { get; set; } = new List<ContactSummaryDto>();
    }

    public class ContactSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
