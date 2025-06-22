using AutoMapper;
using QatarPaymentTest.Models.Dtos;
using QatarPaymentTest.Models.Entities;
using QatarPaymentTest.Models.Enum;
using QatarPaymentTest.Repositories.Interface;
using QatarPaymentTest.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace QatarPaymentTest.Services.Implementation
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ContactService> _logger;

        public ContactService(
            IContactRepository contactRepository,
            IMapper mapper,
            ILogger<ContactService> logger)
        {
            _contactRepository = contactRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<(IEnumerable<ContactDto> Items, int TotalCount, int TotalPages)> GetAllAsync(
            int page = 1,
            int pageSize = 10,
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true,
            bool includeInactive = false,
            bool includeDeleted = false,
            int? companyId = null)
        {
            try
            {
                var (contacts, totalCount, totalPages) = await _contactRepository.GetPagedAsync(
                    page,
                    pageSize,
                    searchTerm,
                    sortBy,
                    ascending,
                    companyId);

                var query = contacts.AsQueryable();

                if (!includeInactive)
                    query = query.Where(c => !c.IsInactive);
                if (!includeDeleted)
                    query = query.Where(c => !c.IsDeleted);

                var filteredContacts = query.ToList();
                var contactDtos = _mapper.Map<IEnumerable<ContactDto>>(filteredContacts);

                return (contactDtos, totalCount, totalPages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all contacts with pagination");
                throw;
            }
        }

        public async Task<ContactDto?> GetByIdAsync(int id, bool includeCompanies = true, bool includeCustomFields = true)
        {
            try
            {
                var query = _contactRepository.Query()
                    .Where(c => c.Id == id);

                if (includeCompanies)
                    query = query.Include(c => c.Companies);
                
                if (includeCustomFields)
                    query = query.Include(c => c.CustomFieldValues)
                                .ThenInclude(cfv => cfv.CustomField);

                var contact = await query.FirstOrDefaultAsync();
                return contact == null ? null : _mapper.Map<ContactDto>(contact);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting contact {Id}", id);
                throw;
            }
        }

        public async Task<ContactDto> CreateAsync(CreateContactDto dto)
        {
            try
            {
                if (!await ValidateContactDataAsync(dto))
                {
                    throw new InvalidOperationException("Contact validation failed");
                }

                var contact = _mapper.Map<Contact>(dto);
                contact.CreatedAt = DateTime.UtcNow;

                // Load companies
                if (dto.CompanyIds?.Any() == true)
                {
                    var companies = await _contactRepository.QueryRelated<Company>()
                        .Where(c => dto.CompanyIds.Contains(c.Id))
                        .ToListAsync();
                    
                    if (companies.Count != dto.CompanyIds.Count)
                    {
                        throw new InvalidOperationException("One or more company IDs are invalid");
                    }

                    foreach (var company in companies)
                    {
                        contact.Companies.Add(company);
                    }
                }

                // Handle custom field values
                if (dto.CustomFieldValues?.Any() == true)
                {
                    var customFields = await _contactRepository.QueryRelated<CustomField>()
                        .Where(cf => dto.CustomFieldValues.Keys.Contains(cf.Id))
                        .ToListAsync();

                    if (customFields.Count != dto.CustomFieldValues.Count)
                    {
                        throw new InvalidOperationException("One or more custom field IDs are invalid");
                    }

                    foreach (var field in customFields)
                    {
                        if (dto.CustomFieldValues.TryGetValue(field.Id, out var value))
                        {
                            if (!ValidateCustomFieldValue(field, value))
                            {
                                throw new InvalidOperationException($"Invalid value for custom field {field.Name}");
                            }

                            contact.CustomFieldValues.Add(new ContactCustomFieldValue
                            {
                                CustomFieldId = field.Id,
                                Value = value
                            });
                        }
                    }
                }

                await _contactRepository.AddAsync(contact);
                return _mapper.Map<ContactDto>(contact);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating contact");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(int id, UpdateContactDto dto)
        {
            try
            {
                if (!await ValidateContactDataAsync(dto, id))
                {
                    throw new InvalidOperationException("Contact validation failed");
                }

                var contact = await _contactRepository.Query()
                    .Include(c => c.Companies)
                    .Include(c => c.CustomFieldValues)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (contact == null)
                    return false;

                _mapper.Map(dto, contact);
                contact.UpdatedAt = DateTime.UtcNow;

                // Update companies
                contact.Companies.Clear();
                if (dto.CompanyIds?.Any() == true)
                {
                    var companies = await _contactRepository.QueryRelated<Company>()
                        .Where(c => dto.CompanyIds.Contains(c.Id))
                        .ToListAsync();
                    
                    if (companies.Count != dto.CompanyIds.Count)
                    {
                        throw new InvalidOperationException("One or more company IDs are invalid");
                    }

                    foreach (var company in companies)
                    {
                        contact.Companies.Add(company);
                    }
                }

                // Update custom field values
                contact.CustomFieldValues.Clear();
                if (dto.CustomFieldValues?.Any() == true)
                {
                    var customFields = await _contactRepository.QueryRelated<CustomField>()
                        .Where(cf => dto.CustomFieldValues.Keys.Contains(cf.Id))
                        .ToListAsync();

                    if (customFields.Count != dto.CustomFieldValues.Count)
                    {
                        throw new InvalidOperationException("One or more custom field IDs are invalid");
                    }

                    foreach (var field in customFields)
                    {
                        if (dto.CustomFieldValues.TryGetValue(field.Id, out var value))
                        {
                            if (!ValidateCustomFieldValue(field, value))
                            {
                                throw new InvalidOperationException($"Invalid value for custom field {field.Name}");
                            }

                            contact.CustomFieldValues.Add(new ContactCustomFieldValue
                            {
                                CustomFieldId = field.Id,
                                Value = value
                            });
                        }
                    }
                }

                await _contactRepository.UpdateAsync(contact);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating contact {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var contact = await _contactRepository.GetByIdAsync(id);
                if (contact == null)
                    return false;

                if (contact.IsPrimary)
                {
                    throw new InvalidOperationException("Cannot delete primary contact");
                }

                contact.IsDeleted = true;
                contact.DeletedAt = DateTime.UtcNow;
                await _contactRepository.UpdateAsync(contact);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting contact {Id}", id);
                throw;
            }
        }

        public async Task<(IEnumerable<ContactDto> Items, int TotalCount, int TotalPages)> GetByCompanyAsync(
            int companyId,
            int page = 1,
            int pageSize = 10,
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true)
        {
            try
            {
                var (contacts, totalCount, totalPages) = await _contactRepository.GetPagedAsync(
                    page,
                    pageSize,
                    searchTerm,
                    sortBy,
                    ascending,
                    companyId);

                var contactDtos = _mapper.Map<IEnumerable<ContactDto>>(contacts);
                return (contactDtos, totalCount, totalPages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting contacts for company {CompanyId}", companyId);
                throw;
            }
        }

        public async Task<ContactStatisticsDto> GetStatisticsAsync()
        {
            try
            {
                var query = _contactRepository.Query();
                
                var statistics = await query
                    .GroupBy(_ => 1)
                    .Select(g => new
                    {
                        TotalContacts = g.Count(),
                        ActiveContacts = g.Count(c => !c.IsInactive && !c.IsDeleted),
                        InactiveContacts = g.Count(c => c.IsInactive),
                        PrimaryContacts = g.Count(c => c.IsPrimary),
                        DeletedContacts = g.Count(c => c.IsDeleted),
                        ContactsWithCompany = g.Count(c => c.Companies.Any()),
                        ContactsWithoutCompany = g.Count(c => !c.Companies.Any()),
                        LastContactCreated = (DateTime?)g.Select(c => c.CreatedAt).Max(),
                        LastContactUpdated = (DateTime?)g.Select(c => c.UpdatedAt).Max()
                    })
                    .FirstOrDefaultAsync();

                if (statistics == null)
                {
                    statistics = new
                    {
                        TotalContacts = 0,
                        ActiveContacts = 0,
                        InactiveContacts = 0,
                        PrimaryContacts = 0,
                        DeletedContacts = 0,
                        ContactsWithCompany = 0,
                        ContactsWithoutCompany = 0,
                        LastContactCreated = (DateTime?)null,
                        LastContactUpdated = (DateTime?)null
                    };
                }

                var contactsByCompany = await query
                    .SelectMany(c => c.Companies.DefaultIfEmpty(), (c, comp) => new { CompanyName = comp == null ? "No Company" : comp.Name })
                    .GroupBy(x => x.CompanyName)
                    .Select(g => new { CompanyName = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.CompanyName, x => x.Count);

                var contactsByStatus = await query
                    .GroupBy(c => GetContactStatus(c))
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Status, x => x.Count);

                return new ContactStatisticsDto
                {
                    TotalContacts = statistics.TotalContacts,
                    ActiveContacts = statistics.ActiveContacts,
                    InactiveContacts = statistics.InactiveContacts,
                    PrimaryContacts = statistics.PrimaryContacts,
                    DeletedContacts = statistics.DeletedContacts,
                    ContactsWithCompany = statistics.ContactsWithCompany,
                    ContactsWithoutCompany = statistics.ContactsWithoutCompany,
                    LastContactCreated = statistics.LastContactCreated,
                    LastContactUpdated = statistics.LastContactUpdated,
                    ContactsByCompany = contactsByCompany,
                    ContactsByStatus = contactsByStatus
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting contact statistics");
                throw;
            }
        }

        private static string GetContactStatus(Contact contact)
        {
            if (contact.IsDeleted) return "Deleted";
            if (contact.IsInactive) return "Inactive";
            if (contact.IsPrimary) return "Primary";
            return "Active";
        }

        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                return await _contactRepository.ExistsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking contact existence {Id}", id);
                throw;
            }
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
        {
            try
            {
                return await _contactRepository.IsEmailUniqueAsync(email, excludeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email uniqueness");
                throw;
            }
        }

        public async Task<(IEnumerable<ContactDto> Items, int TotalCount, int TotalPages)> SearchAsync(
            string? searchTerm,
            Dictionary<string, object>? customFieldFilters,
            int page = 1,
            int pageSize = 10,
            string? sortBy = null,
            bool ascending = true)
        {
            try
            {
                var (contacts, totalCount) = await _contactRepository.SearchAsync(
                    searchTerm,
                    customFieldFilters,
                    page,
                    pageSize);

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                var contactDtos = _mapper.Map<IEnumerable<ContactDto>>(contacts);

                return (contactDtos, totalCount, totalPages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching contacts");
                throw;
            }
        }

        public async Task<bool> RestoreAsync(int id)
        {
            try
            {
                var contact = await _contactRepository.GetByIdAsync(id);
                if (contact == null || !contact.IsDeleted)
                    return false;

                contact.IsDeleted = false;
                contact.DeletedAt = null;
                await _contactRepository.UpdateAsync(contact);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring contact {Id}", id);
                throw;
            }
        }

        public async Task<bool> ValidateContactDataAsync(CreateContactDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || 
                string.IsNullOrWhiteSpace(dto.FirstName) || 
                string.IsNullOrWhiteSpace(dto.LastName))
                return false;

            if (!IsValidEmail(dto.Email))
                return false;

            return await IsEmailUniqueAsync(dto.Email);
        }

        public async Task<bool> ValidateContactDataAsync(UpdateContactDto dto, int contactId)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || 
                string.IsNullOrWhiteSpace(dto.FirstName) || 
                string.IsNullOrWhiteSpace(dto.LastName))
                return false;

            if (!IsValidEmail(dto.Email))
                return false;

            return await IsEmailUniqueAsync(dto.Email, contactId);
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private static bool ValidateCustomFieldValue(CustomField field, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return !field.IsRequired;

            return field.FieldType switch
            {
                CustomFieldType.Text => true,
                CustomFieldType.Number => decimal.TryParse(value, out _),
                CustomFieldType.Date => DateTime.TryParse(value, out _),
                CustomFieldType.Boolean => bool.TryParse(value, out _),
                CustomFieldType.Email => IsValidEmail(value),
                CustomFieldType.Phone => Regex.IsMatch(value, @"^\+?[\d\s-()]+$"),
                _ => false
            };
        }
    }
}
