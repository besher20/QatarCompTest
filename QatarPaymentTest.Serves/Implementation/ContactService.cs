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
    public class ContactService: IContactService
    {
        private readonly IContactRepository _contactRepository;
        private readonly IMapper _mapper;

        public ContactService(IContactRepository contactRepository, IMapper mapper)
        {
            _contactRepository = contactRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ContactDto>> GetAllAsync()
        {
            var contacts = await _contactRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ContactDto>>(contacts);
        }

        public async Task<ContactDto?> GetByIdAsync(int id)
        {
            var contact = await _contactRepository.GetByIdAsync(id);
            return contact == null ? null : _mapper.Map<ContactDto>(contact);
        }

        public async Task<ContactDto> CreateAsync(ContactDto contactDto)
        {
            var contact = _mapper.Map<Contact>(contactDto);
            await _contactRepository.AddAsync(contact);
            return _mapper.Map<ContactDto>(contact);
        }

        public async Task<bool> UpdateAsync(int id, ContactDto contactDto)
        {
            var contact = await _contactRepository.GetByIdAsync(id);
            if (contact == null) return false;

            _mapper.Map(contactDto, contact);
            await _contactRepository.UpdateAsync(contact);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var contact = await _contactRepository.GetByIdAsync(id);
            if (contact == null) return false;

            await _contactRepository.DeleteAsync(contact);
            return true;
        }
    }

}
