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
        Task<Contact?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Contact>> GetAllWithDetailsAsync();
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
        Task<(IEnumerable<Contact> Items, int TotalCount)> SearchAsync(
            string? searchTerm,
            Dictionary<string, object>? customFieldFilters,
            int pageNumber,
            int pageSize);
    }
}
