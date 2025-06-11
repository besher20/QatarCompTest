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
        Task<Company?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Company>> GetAllWithDetailsAsync();
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
        Task<(IEnumerable<Company> Items, int TotalCount)> SearchAsync(
            string? searchTerm,
            Dictionary<string, object>? customFieldFilters,
            int pageNumber,
            int pageSize);
    }
}
