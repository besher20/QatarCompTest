using Microsoft.AspNetCore.Mvc;
using QatarPaymentTest.Models.Dtos;
using QatarPaymentTest.Services.Interfaces;

namespace QatarPaymentTest.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompaniesController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var companies = await _companyService.GetAllAsync();
            return Ok(companies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var company = await _companyService.GetByIdAsync(id);
            if (company == null)
                return NotFound();

            return Ok(company);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CompanyDto companyDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _companyService.CreateAsync(companyDto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CompanyDto companyDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _companyService.UpdateAsync(id, companyDto);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _companyService.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }

}
