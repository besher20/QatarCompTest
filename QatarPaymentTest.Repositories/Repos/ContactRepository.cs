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
        private readonly ApplicationDbContext _context;

        public ContactRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<TEntity> QueryRelated<TEntity>() where TEntity : class
        {
            return _context.Set<TEntity>();
        }

        public async Task<(IEnumerable<Contact> Items, int TotalCount, int TotalPages)> GetPagedAsync(
            int page,
            int pageSize,
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true,
            int? companyId = null)
        {
            var query = _context.Contacts
                .Include(c => c.Companies)
                .Include(c => c.CustomFieldValues)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(c =>
                    c.FirstName.ToLower().Contains(searchTerm) ||
                    c.LastName.ToLower().Contains(searchTerm) ||
                    c.Email.ToLower().Contains(searchTerm));
            }

            if (companyId.HasValue)
            {
                query = query.Where(c => c.Companies.Any(comp => comp.Id == companyId));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query = sortBy.ToLower() switch
                {
                    "firstname" => ascending ? query.OrderBy(c => c.FirstName) : query.OrderByDescending(c => c.FirstName),
                    "lastname" => ascending ? query.OrderBy(c => c.LastName) : query.OrderByDescending(c => c.LastName),
                    "email" => ascending ? query.OrderBy(c => c.Email) : query.OrderByDescending(c => c.Email),
                    "createdat" => ascending ? query.OrderBy(c => c.CreatedAt) : query.OrderByDescending(c => c.CreatedAt),
                    _ => query.OrderBy(c => c.Id)
                };
            }

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount, totalPages);
        }

        public async Task<Contact?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Contacts
                .Include(c => c.Companies)
                .Include(c => c.CustomFieldValues)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Contact>> GetByCompanyIdAsync(int companyId)
        {
            return await _context.Contacts
                .Include(c => c.Companies)
                .Include(c => c.CustomFieldValues)
                .Where(c => c.Companies.Any(comp => comp.Id == companyId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Contact>> GetPrimaryContactsAsync()
        {
            return await _context.Contacts
                .Include(c => c.Companies)
                .Include(c => c.CustomFieldValues)
                .Where(c => c.IsPrimary)
                .ToListAsync();
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
        {
            var query = _context.Contacts.AsQueryable();
            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }
            return !await query.AnyAsync(c => c.Email.ToLower() == email.ToLower());
        }

        public override async Task<IEnumerable<Contact>> GetAllAsync()
        {
            return await _context.Contacts
                .Include(c => c.Companies)
                .Include(c => c.CustomFieldValues)
                .ToListAsync();
        }

        public override async Task<bool> DeleteAsync(Contact contact)
        {
            if (contact == null) return false;

            contact.IsDeleted = true;
            contact.DeletedAt = DateTime.UtcNow;
            _context.Contacts.Update(contact);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            var query = _context.Contacts.AsQueryable();
            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }
            return await query.AnyAsync(c => 
                (c.FirstName + " " + c.LastName).ToLower() == name.ToLower());
        }

        public async Task<IEnumerable<Contact>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(c => c.Companies)
                .Include(c => c.CustomFieldValues)
                    .ThenInclude(cfv => cfv.CustomField)
                .ToListAsync();
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
