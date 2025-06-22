using QatarPaymentTest.Models.Dtos;
using QatarPaymentTest.Models.Enum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QatarPaymentTest.Services.Interfaces
{
    public interface ICustomFieldService
    {
        Task<IEnumerable<CustomFieldDto>> GetAllAsync(string? entityType = null, bool includeDeleted = false);
        Task<CustomFieldDto?> GetByIdAsync(int id);
        Task<CustomFieldDto> CreateAsync(CreateCustomFieldDto dto);
        Task<bool> UpdateAsync(int id, UpdateCustomFieldDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ValidateCustomFieldDataAsync(CreateCustomFieldDto dto);
        Task<bool> ValidateCustomFieldDataAsync(UpdateCustomFieldDto dto);
        Task<bool> IsFieldInUseAsync(int id);
        Task<IEnumerable<CustomFieldDto>> GetByTypeAsync(string entityType);
        Task<bool> ExistsAsync(int id);
        Task<CustomFieldUsageDto> GetFieldUsageAsync(int id);
    }
}
