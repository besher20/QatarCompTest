using AutoMapper;
using QatarPaymentTest.Models.Dtos;
using QatarPaymentTest.Models.Entities;
using QatarPaymentTest.Repositories.Interface;
using QatarPaymentTest.Repositories.Repos;
using QatarPaymentTest.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QatarPaymentTest.Services.Implementation
{
    public class CompanyService: ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public CompanyService(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CompanyDto>> GetAllAsync()
        {
            var companies = await _companyRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CompanyDto>>(companies);
        }

        public async Task<CompanyDto?> GetByIdAsync(int id)
        {
            var company = await _companyRepository.GetByIdAsync(id);
            return company == null ? null : _mapper.Map<CompanyDto>(company);
        }

        public async Task<CompanyDto> CreateAsync(CompanyDto companyDto)
        {
            var company = _mapper.Map<Company>(companyDto);
            await _companyRepository.AddAsync(company);
            return _mapper.Map<CompanyDto>(company);
        }

        public async Task<bool> UpdateAsync(int id, CompanyDto companyDto)
        {
            var company = await _companyRepository.GetByIdAsync(id);
            if (company == null) return false;

            _mapper.Map(companyDto, company);
            await _companyRepository.UpdateAsync(company);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var company = await _companyRepository.GetByIdAsync(id);
            if (company == null) return false;

            await _companyRepository.DeleteAsync(company);
            return true;
        }
    }

}
