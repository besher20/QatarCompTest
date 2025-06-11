using Microsoft.AspNetCore.Mvc;

using QatarPaymentTest.Models.Dtos;
using QatarPaymentTest.Services.Interfaces;

namespace CustomerManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactsController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var contacts = await _contactService.GetAllAsync();
            return Ok(contacts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var contact = await _contactService.GetByIdAsync(id);
            if (contact == null)
                return NotFound();

            return Ok(contact);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ContactDto contactDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _contactService.CreateAsync(contactDto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ContactDto contactDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _contactService.UpdateAsync(id, contactDto);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _contactService.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
