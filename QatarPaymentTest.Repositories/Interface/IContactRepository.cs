using QatarPaymentTest.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QatarPaymentTest.Repositories.Interface
{
    public interface IContactRepository : IGenericRepository<Contact>
    {
        Task<(IEnumerable<Contact> Items, int TotalCount, int TotalPages)> GetPagedAsync(
            int page,
            int pageSize,
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true,
            int? companyId = null);

        Task<Contact?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Contact>> GetByCompanyIdAsync(int companyId);
        Task<IEnumerable<Contact>> GetPrimaryContactsAsync();
        Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
        new Task<IEnumerable<Contact>> GetAllAsync();
        new Task<bool> DeleteAsync(Contact contact);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
        Task<(IEnumerable<Contact> Items, int TotalCount)> SearchAsync(
            string? searchTerm,
            Dictionary<string, object>? customFieldFilters,
            int pageNumber,
            int pageSize);

        // Generic query methods for related entities
        IQueryable<TEntity> QueryRelated<TEntity>() where TEntity : class;
    }
}
