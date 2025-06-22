using QatarPaymentTest.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QatarPaymentTest.Repositories.Interface
{
    public interface ICompanyRepository : IGenericRepository<Company>
    {
        Task<(IEnumerable<Company> Items, int TotalCount, int TotalPages)> GetPagedAsync(
            int page,
            int pageSize,
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true,
            bool includeCustomFields = false,
            bool includeContacts = false);

        Task<Company?> GetByIdWithDetailsAsync(int id, bool includeCustomFields = false, bool includeContacts = false);
        
        Task<bool> IsNameUniqueAsync(string name, int? excludeId = null);
        
        Task<IEnumerable<Company>> GetByCustomFieldValueAsync(int customFieldId, string value);
        
        Task<int> GetTotalCountAsync(string? searchTerm = null);
        
        Task<bool> HasRelatedDataAsync(int id);
        
        Task<bool> SoftDeleteAsync(int id);
        
        Task<IEnumerable<Company>> GetRecentlyModifiedAsync(int count = 10);

        new Task<bool> UpdateAsync(Company entity);
        new Task<bool> DeleteAsync(Company entity);
    }
}
