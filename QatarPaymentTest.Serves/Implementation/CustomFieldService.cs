using AutoMapper;
using QatarPaymentTest.Models.Dtos;
using QatarPaymentTest.Models.Entities;
using QatarPaymentTest.Repositories.Interface;
using QatarPaymentTest.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QatarPaymentTest.Services.Implementation
{
    public class CustomFieldService
: ICustomFieldService
    {
        private readonly IGenericRepository<CustomField> _customFieldRepository;
        private readonly IMapper _mapper;

        public CustomFieldService(IGenericRepository<CustomField> customFieldRepository, IMapper mapper)
        {
            _customFieldRepository = customFieldRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CustomFieldDto>> GetAllAsync()
        {
            var fields = await _customFieldRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CustomFieldDto>>(fields);
        }

        public async Task<CustomFieldDto?> GetByIdAsync(int id)
        {
            var field = await _customFieldRepository.GetByIdAsync(id);
            return field == null ? null : _mapper.Map<CustomFieldDto>(field);
        }

        public async Task<CustomFieldDto> CreateAsync(CreateCustomFieldDto dto)
        {
            try
            {
                var field = _mapper.Map<CustomField>(dto);
                await _customFieldRepository.AddAsync(field);
                 return _mapper.Map<CustomFieldDto>(field);

            }
            catch (Exception)
            {

                throw;
            }
          
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var field = await _customFieldRepository.GetByIdAsync(id);
            if (field == null) return false;

            await _customFieldRepository.DeleteAsync(field);
            return true;
        }
    }

}
