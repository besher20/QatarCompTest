using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QatarPaymentTest.Models.Dtos;
using QatarPaymentTest.Models.Entities;
using QatarPaymentTest.Repositories.Interface;
using QatarPaymentTest.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QatarPaymentTest.Services.Implementation
{
    public class CustomFieldService : ICustomFieldService
    {
        private readonly IGenericRepository<CustomField> _customFieldRepository;
        private readonly IMapper _mapper;

        public CustomFieldService(IGenericRepository<CustomField> customFieldRepository, IMapper mapper)
        {
            _customFieldRepository = customFieldRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CustomFieldDto>> GetAllAsync(string? entityType = null, bool includeDeleted = false)
        {
            var query = _customFieldRepository.Query();
            
            if (!includeDeleted)
                query = query.Where(cf => !cf.IsDeleted);
                
            if (!string.IsNullOrWhiteSpace(entityType))
                query = query.Where(cf => cf.EntityType == entityType);

            var fields = await query.ToListAsync();
            return _mapper.Map<IEnumerable<CustomFieldDto>>(fields);
        }

        public async Task<CustomFieldDto?> GetByIdAsync(int id)
        {
            var field = await _customFieldRepository.GetByIdAsync(id);
            return field == null ? null : _mapper.Map<CustomFieldDto>(field);
        }

        public async Task<CustomFieldDto> CreateAsync(CreateCustomFieldDto dto)
        {
            var field = _mapper.Map<CustomField>(dto);
            field.CreatedAt = DateTime.UtcNow;
            
            await _customFieldRepository.AddAsync(field);
            return _mapper.Map<CustomFieldDto>(field);
        }

        public async Task<bool> UpdateAsync(int id, UpdateCustomFieldDto dto)
        {
            var existingField = await _customFieldRepository.GetByIdAsync(id);
            if (existingField == null)
                return false;

            _mapper.Map(dto, existingField);
            existingField.UpdatedAt = DateTime.UtcNow;

            await _customFieldRepository.UpdateAsync(existingField);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var field = await _customFieldRepository.GetByIdAsync(id);
            if (field == null)
                return false;

            if (await IsFieldInUseAsync(id))
            {
                field.IsDeleted = true;
                field.DeletedAt = DateTime.UtcNow;
                await _customFieldRepository.UpdateAsync(field);
                return true;
            }

            await _customFieldRepository.DeleteAsync(field);
            return true;
        }

        public async Task<bool> ValidateCustomFieldDataAsync(CreateCustomFieldDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.EntityType))
                return false;

            var query = _customFieldRepository.Query()
                .Where(cf => cf.Name.ToLower() == dto.Name.ToLower() && 
                           cf.EntityType == dto.EntityType);

            return !await query.AnyAsync();
        }

        public async Task<bool> ValidateCustomFieldDataAsync(UpdateCustomFieldDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.EntityType))
                return false;

            var query = _customFieldRepository.Query()
                .Where(cf => cf.Name.ToLower() == dto.Name.ToLower() && 
                           cf.EntityType == dto.EntityType);

            return !await query.AnyAsync();
        }

        public async Task<bool> IsFieldInUseAsync(int id)
        {
            var field = await _customFieldRepository.GetByIdAsync(id);
            if (field == null)
                return false;

            return await _customFieldRepository.Query()
                .Where(cf => cf.Id == id)
                .AnyAsync(cf => cf.CompanyValues.Any() || cf.ContactValues.Any());
        }

        public async Task<IEnumerable<CustomFieldDto>> GetByTypeAsync(string entityType)
        {
            var fields = await _customFieldRepository.Query()
                .Where(cf => cf.EntityType == entityType && !cf.IsDeleted)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CustomFieldDto>>(fields);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _customFieldRepository.ExistsAsync(id);
        }

        public async Task<CustomFieldUsageDto> GetFieldUsageAsync(int id)
        {
            var field = await _customFieldRepository.Query()
                .Include(cf => cf.CompanyValues)
                .Include(cf => cf.ContactValues)
                .FirstOrDefaultAsync(cf => cf.Id == id);

            if (field == null)
                throw new InvalidOperationException("Custom field not found");

            var allValues = field.EntityType == "Company" 
                ? field.CompanyValues.Select(cv => cv.Value)
                : field.ContactValues.Select(cv => cv.Value);

            var valueDistribution = allValues
                .Where(v => v != null)
                .GroupBy(v => v!)
                .ToDictionary(g => g.Key, g => g.Count());

            return new CustomFieldUsageDto
            {
                CustomFieldId = field.Id,
                Name = field.Name,
                EntityType = field.EntityType,
                TotalUsageCount = allValues.Count(),
                ValueDistribution = valueDistribution,
                LastUsed = field.UpdatedAt ?? field.CreatedAt
            };
        }
    }
}
