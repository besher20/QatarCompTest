using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QatarPaymentTest.Models.Dtos
{
    public class ContactDto
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
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsPrimary { get; set; }
        public bool IsInactive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public List<CompanyDto> Companies { get; set; } = new();
        public Dictionary<int, string> CustomFieldValues { get; set; } = new();
    }


 
}
