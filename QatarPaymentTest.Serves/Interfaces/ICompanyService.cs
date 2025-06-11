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
        Task<IEnumerable<CompanyDto>> GetAllAsync();
        Task<CompanyDto?> GetByIdAsync(int id);
        Task<CompanyDto> CreateAsync(CompanyDto companyDto);
        Task<bool> UpdateAsync(int id, CompanyDto companyDto);
        Task<bool> DeleteAsync(int id);

    }
}
