using Microsoft.AspNetCore.Mvc;
using QatarPaymentTest.Models.Dtos;
using QatarPaymentTest.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace QatarPaymentTest.Controllers
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
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false, [FromQuery] bool includeDeleted = false)
        {
            try
            {
                var contacts = await _contactService.GetAllAsync(includeInactive, includeDeleted);
                return Ok(new ApiResponse<object> { Success = true, Data = contacts });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = "An error occurred while fetching contacts" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var contact = await _contactService.GetByIdAsync(id);
                if (contact == null)
                    return NotFound(new ApiResponse<object> { Success = false, Message = "Contact not found" });

                return Ok(new ApiResponse<object> { Success = true, Data = contact });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = "An error occurred while fetching the contact" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateContactDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Invalid data provided" });

            try
            {
                var contact = await _contactService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = contact.Id }, 
                    new ApiResponse<object> { Success = true, Data = contact });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = "An error occurred while creating the contact" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateContactDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Invalid data provided" });

            try
            {
                var success = await _contactService.UpdateAsync(id, dto);
                if (!success)
                    return NotFound(new ApiResponse<object> { Success = false, Message = "Contact not found" });

                return Ok(new ApiResponse<object> { Success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = "An error occurred while updating the contact" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _contactService.DeleteAsync(id);
                if (!success)
                    return NotFound(new ApiResponse<object> { Success = false, Message = "Contact not found" });

                return Ok(new ApiResponse<object> { Success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = "An error occurred while deleting the contact" });
            }
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var statistics = await _contactService.GetStatisticsAsync();
                return Ok(new ApiResponse<object> { Success = true, Data = statistics });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = "An error occurred while fetching contact statistics" });
            }
        }

        [HttpGet("company/{companyId}")]
        public async Task<IActionResult> GetByCompany(int companyId)
        {
            try
            {
                var contacts = await _contactService.GetByCompanyAsync(companyId);
                return Ok(new ApiResponse<object> { Success = true, Data = contacts });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = "An error occurred while fetching contacts by company" });
            }
        }
    }
} 