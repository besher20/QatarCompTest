using Microsoft.AspNetCore.Mvc;
using QatarPaymentTest.Models.Dtos;
using QatarPaymentTest.Services.Interfaces;

namespace CustomerManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomFieldsController : ControllerBase
    {
        private readonly ICustomFieldService _customFieldService;

        public CustomFieldsController(ICustomFieldService customFieldService)
        {
            _customFieldService = customFieldService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var fields = await _customFieldService.GetAllAsync();
            return Ok(fields);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var field = await _customFieldService.GetByIdAsync(id);
            if (field == null)
                return NotFound();

            return Ok(field);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomFieldDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _customFieldService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _customFieldService.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
