using Microsoft.EntityFrameworkCore;
using QatarPaymentTest.Data.DbContextApp;
using QatarPaymentTest.Models.Entities;
using QatarPaymentTest.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;

namespace QatarPaymentTest.Repositories.Repos
{
    public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
    {
        private readonly ILogger<CompanyRepository> _logger;

        public CompanyRepository(ApplicationDbContext context, ILogger<CompanyRepository> logger) : base(context)
        {
            _logger = logger;
        }

        public async Task<(IEnumerable<Company> Items, int TotalCount, int TotalPages)> GetPagedAsync(
            int page,
            int pageSize,
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true,
            bool includeCustomFields = false,
            bool includeContacts = false)
        {
            _logger.LogInformation("Building query with parameters: page={Page}, pageSize={PageSize}, searchTerm={SearchTerm}", 
                page, pageSize, searchTerm ?? "none");

            var query = _context.Companies.AsNoTracking();

            if (includeCustomFields)
            {
                query = query.Include(c => c.CustomFieldValues);
                _logger.LogInformation("Including CustomFieldValues in query");
            }

            if (includeContacts)
            {
                query = query.Include(c => c.Contacts);
                _logger.LogInformation("Including Contacts in query");
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(searchTerm));
                _logger.LogInformation("Applied search filter for term: {SearchTerm}", searchTerm);
            }

            // Calculate total count before applying pagination
            var totalCount = await query.CountAsync();
            _logger.LogInformation("Total count before pagination: {TotalCount}", totalCount);

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query = sortBy.ToLower() switch
                {
                    "name" => ascending ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name),
                    "createdat" => ascending ? query.OrderBy(c => c.CreatedAt) : query.OrderByDescending(c => c.CreatedAt),
                    _ => query.OrderBy(c => c.Id)
                };
                _logger.LogInformation("Applied sorting by {SortBy} {Direction}", sortBy, ascending ? "ascending" : "descending");
            }

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} items after pagination", items.Count);

            return (items, totalCount, totalPages);
        }

        public async Task<Company?> GetByIdWithDetailsAsync(int id, bool includeCustomFields = false, bool includeContacts = false)
        {
            var query = _context.Companies.AsQueryable();

            if (includeCustomFields)
                query = query.Include(c => c.CustomFieldValues);

            if (includeContacts)
                query = query.Include(c => c.Contacts);

            return await query.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null)
        {
            var query = _context.Companies.AsQueryable();
            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }
            return !await query.AnyAsync(c => c.Name.ToLower() == name.ToLower());
        }

        public async Task<IEnumerable<Company>> GetByCustomFieldValueAsync(int customFieldId, string value)
        {
            return await _context.Companies
                .Include(c => c.CustomFieldValues)
                .Where(c => c.CustomFieldValues.Any(cfv => cfv.CustomFieldId == customFieldId && cfv.Value == value))
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(string? searchTerm = null)
        {
            var query = _context.Companies.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(c => EF.Functions.ILike(c.Name, $"%{searchTerm}%"));

            return await query.CountAsync();
        }

        public async Task<bool> HasRelatedDataAsync(int id)
        {
            var company = await _context.Companies
                .Include(c => c.Contacts)
                .Include(c => c.CustomFieldValues)
                .FirstOrDefaultAsync(c => c.Id == id);

            return company != null && (company.Contacts.Any() || company.CustomFieldValues.Any());
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
                return false;

            company.IsDeleted = true;
            company.DeletedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Company>> GetRecentlyModifiedAsync(int count = 10)
        {
            return await _context.Companies
                .OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public override async Task<bool> UpdateAsync(Company entity)
        {
            try
            {
                _context.Companies.Update(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override async Task<bool> DeleteAsync(Company entity)
        {
            try
            {
                _context.Companies.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
