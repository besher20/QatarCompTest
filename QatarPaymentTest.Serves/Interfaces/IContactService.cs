using QatarPaymentTest.Models.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QatarPaymentTest.Services.Interfaces
{
    public interface IContactService
    {
        Task<(IEnumerable<ContactDto> Items, int TotalCount, int TotalPages)> GetAllAsync(
            int page = 1,
            int pageSize = 10,
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true,
            bool includeInactive = false,
            bool includeDeleted = false,
            int? companyId = null);

        Task<ContactDto?> GetByIdAsync(int id, bool includeCompanies = true, bool includeCustomFields = true);
        Task<ContactDto> CreateAsync(CreateContactDto dto);
        Task<bool> UpdateAsync(int id, UpdateContactDto dto);
        Task<bool> DeleteAsync(int id);
        Task<ContactStatisticsDto> GetStatisticsAsync();
        
        Task<(IEnumerable<ContactDto> Items, int TotalCount, int TotalPages)> GetByCompanyAsync(
            int companyId,
            int page = 1,
            int pageSize = 10,
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true);

        Task<bool> ExistsAsync(int id);
        Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
        
        Task<(IEnumerable<ContactDto> Items, int TotalCount, int TotalPages)> SearchAsync(
            string? searchTerm,
            Dictionary<string, object>? customFieldFilters,
            int page = 1,
            int pageSize = 10,
            string? sortBy = null,
            bool ascending = true);

        Task<bool> RestoreAsync(int id);
        Task<bool> ValidateContactDataAsync(CreateContactDto dto);
        Task<bool> ValidateContactDataAsync(UpdateContactDto dto, int contactId);
    }
}
