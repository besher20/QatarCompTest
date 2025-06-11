using QatarPaymentTest.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QatarPaymentTest.Services.Interfaces
{
    public interface ICustomFieldService
    {
        Task<IEnumerable<CustomFieldDto>> GetAllAsync();
        Task<CustomFieldDto?> GetByIdAsync(int id);
        Task<CustomFieldDto> CreateAsync(CreateCustomFieldDto dto);
        Task<bool> DeleteAsync(int id);

    }
}
