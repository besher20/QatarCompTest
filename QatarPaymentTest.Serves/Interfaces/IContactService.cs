using QatarPaymentTest.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QatarPaymentTest.Services.Interfaces
{
    public interface IContactService
    {
        Task<IEnumerable<ContactDto>> GetAllAsync();
        Task<ContactDto?> GetByIdAsync(int id);
        Task<ContactDto> CreateAsync(ContactDto contactDto);
        Task<bool> UpdateAsync(int id, ContactDto contactDto);
        Task<bool> DeleteAsync(int id);

    }
}
