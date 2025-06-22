using QatarPaymentTest.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QatarPaymentTest.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<(IEnumerable<CompanyDto> Items, int TotalCount, int TotalPages)> GetAllAsync(
            int page = 1,
            int pageSize = 10,
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true);

        Task<CompanyDto?> GetByIdAsync(int id);
        Task<CompanyDto> CreateAsync(CompanyDto companyDto);
        Task<bool> UpdateAsync(int id, CompanyDto companyDto);
        Task<bool> DeleteAsync(int id);

        Task<IEnumerable<CompanyDto>> GetByCustomFieldAsync(int customFieldId, string value);
        Task<bool> ValidateCompanyDataAsync(CompanyDto companyDto);
        Task<IEnumerable<CompanyDto>> GetRecentlyModifiedAsync(int count = 10);
        Task<bool> RestoreAsync(int id);
    }
}
