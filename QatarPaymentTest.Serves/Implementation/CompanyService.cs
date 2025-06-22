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
using Microsoft.Extensions.Logging;

namespace QatarPaymentTest.Services.Implementation
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CompanyService> _logger;

        public CompanyService(ICompanyRepository companyRepository, IMapper mapper, ILogger<CompanyService> logger)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<(IEnumerable<CompanyDto> Items, int TotalCount, int TotalPages)> GetAllAsync(
            int page = 1,
            int pageSize = 10,
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true)
        {
            _logger.LogInformation("Getting companies from repository with parameters: page={Page}, pageSize={PageSize}, searchTerm={SearchTerm}", 
                page, pageSize, searchTerm ?? "none");

            var (companies, totalCount, totalPages) = await _companyRepository.GetPagedAsync(
                page,
                pageSize,
                searchTerm,
                sortBy,
                ascending,
                includeCustomFields: true,
                includeContacts: true);

            _logger.LogInformation("Retrieved {Count} companies from repository, TotalCount={TotalCount}, TotalPages={TotalPages}", 
                companies?.Count() ?? 0, totalCount, totalPages);

            var companyDtos = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            
            _logger.LogInformation("Mapped {Count} companies to DTOs", companyDtos?.Count() ?? 0);
            
            return (companyDtos.ToList(), totalCount, totalPages);
        }

        public async Task<CompanyDto?> GetByIdAsync(int id)
        {
            var company = await _companyRepository.GetByIdWithDetailsAsync(id, includeCustomFields: true, includeContacts: true);
            return company == null ? null : _mapper.Map<CompanyDto>(company);
        }

        public async Task<CompanyDto> CreateAsync(CompanyDto companyDto)
        {
            if (!await ValidateCompanyDataAsync(companyDto))
                throw new InvalidOperationException("Company validation failed");

            var company = _mapper.Map<Company>(companyDto);
            company.CreatedAt = DateTime.UtcNow;

            await _companyRepository.AddAsync(company);
            return _mapper.Map<CompanyDto>(company);
        }

        public async Task<bool> UpdateAsync(int id, CompanyDto companyDto)
        {
            if (!await ValidateCompanyDataAsync(companyDto))
                throw new InvalidOperationException("Company validation failed");

            var existingCompany = await _companyRepository.GetByIdWithDetailsAsync(id);
            if (existingCompany == null)
                return false;

            _mapper.Map(companyDto, existingCompany);
            existingCompany.UpdatedAt = DateTime.UtcNow;

            await _companyRepository.UpdateAsync(existingCompany);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var company = await _companyRepository.GetByIdWithDetailsAsync(id);
            if (company == null)
                return false;

            if (await _companyRepository.HasRelatedDataAsync(id))
            {
                company.IsDeleted = true;
                company.DeletedAt = DateTime.UtcNow;
                await _companyRepository.UpdateAsync(company);
                return true;
            }

            return await _companyRepository.DeleteAsync(company);
        }

        public async Task<IEnumerable<CompanyDto>> GetByCustomFieldAsync(int customFieldId, string value)
        {
            var companies = await _companyRepository.GetByCustomFieldValueAsync(customFieldId, value);
            return _mapper.Map<IEnumerable<CompanyDto>>(companies);
        }

        public async Task<bool> ValidateCompanyDataAsync(CompanyDto companyDto)
        {
            if (string.IsNullOrWhiteSpace(companyDto.Name))
                return false;

            return await _companyRepository.IsNameUniqueAsync(companyDto.Name, companyDto.Id);
        }

        public async Task<IEnumerable<CompanyDto>> GetRecentlyModifiedAsync(int count = 10)
        {
            var companies = await _companyRepository.GetRecentlyModifiedAsync(count);
            return _mapper.Map<IEnumerable<CompanyDto>>(companies);
        }

        public async Task<bool> RestoreAsync(int id)
        {
            var company = await _companyRepository.GetByIdWithDetailsAsync(id);
            if (company == null || !company.IsDeleted)
                return false;

            company.IsDeleted = false;
            company.DeletedAt = null;
            company.UpdatedAt = DateTime.UtcNow;

            await _companyRepository.UpdateAsync(company);
            return true;
        }
    }
}
