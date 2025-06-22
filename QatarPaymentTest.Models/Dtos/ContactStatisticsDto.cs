using System;

namespace QatarPaymentTest.Models.Dtos
{
    public class ContactStatisticsDto
    {
        public int TotalContacts { get; set; }
        public int ActiveContacts { get; set; }
        public int InactiveContacts { get; set; }
        public int ContactsWithCompany { get; set; }
        public int ContactsWithoutCompany { get; set; }
        public int PrimaryContacts { get; set; }

        public DateTime? LastContactCreated { get; set; }
        public DateTime? LastContactUpdated { get; set; }
        public int DeletedContacts { get; set; }
        public Dictionary<string, int> ContactsByCompany { get; set; } = new();
        public Dictionary<string, int> ContactsByStatus { get; set; } = new();
    }
} 