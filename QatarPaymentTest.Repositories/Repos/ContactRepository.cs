using Microsoft.EntityFrameworkCore;
using QatarPaymentTest.Data.DbContextApp;
using QatarPaymentTest.Models.Entities;
using QatarPaymentTest.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QatarPaymentTest.Repositories.Repos
{
    public class ContactRepository : GenericRepository<Contact>, IContactRepository
    {
        public ContactRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Contact?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Companies)
                .Include(c => c.CustomFieldValues)
                    .ThenInclude(cfv => cfv.CustomField)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Contact>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(c => c.Companies)
                .Include(c => c.CustomFieldValues)
                    .ThenInclude(cfv => cfv.CustomField)
                .ToListAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            var query = _dbSet.Where(c => c.Name.ToLower() == name.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

   


        public async Task<(IEnumerable<Contact> Items, int TotalCount)> SearchAsync(
       string? searchTerm,
       Dictionary<string, object>? customFieldFilters,
       int pageNumber,
       int pageSize)
        {
            var query = _dbSet
                .Include(c => c.Companies)
                .Include(c => c.CustomFieldValues)
                    .ThenInclude(cfv => cfv.CustomField)
                .AsQueryable();

            // Apply search term filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c => c.Name.Contains(searchTerm));
            }

            // Apply custom field filters
            if (customFieldFilters != null && customFieldFilters.Any())
            {
                foreach (var filter in customFieldFilters)
                {
                    var fieldName = filter.Key;
                    var fieldValue = filter.Value.ToString();

                    query = query.Where(c => c.CustomFieldValues
                        .Any(cfv => cfv.CustomField.Name == fieldName &&
                                   cfv.Value != null &&
                                   cfv.Value.Contains(fieldValue!)));
                }
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(c => c.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    } 
}
