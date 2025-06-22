using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QatarPaymentTest.Models.Dtos;
using QatarPaymentTest.Services.Interfaces;
using System.Net.Mime;

namespace QatarPaymentTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly ILogger<CompaniesController> _logger;

        public CompaniesController(ICompanyService companyService, ILogger<CompaniesController> logger)
        {
            _companyService = companyService;
            _logger = logger;
        }

        /// <summary>
        /// Get paginated list of companies with optional filtering and sorting
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> GetCompanies(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool ascending = true)
        {
            try
            {
                _logger.LogInformation("Getting companies with page {Page}, size {PageSize}, search {SearchTerm}", 
                    page, pageSize, searchTerm ?? "none");

                var result = await _companyService.GetAllAsync(page, pageSize, searchTerm, sortBy, ascending);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Data = new
                    {
                        Items = result.Items,
                        TotalCount = result.TotalCount,
                        TotalPages = result.TotalPages
                    },
                    Message = "Companies retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting companies");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving companies"
                });
            }
        }

        /// <summary>
        /// Get a company by ID with all related data
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CompanyDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCompany(int id)
        {
            try
            {
                _logger.LogInformation("Getting company with ID {Id}", id);

                var company = await _companyService.GetByIdAsync(id);
                if (company == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = $"Company with ID {id} not found"
                    });
                }

                return Ok(new ApiResponse<CompanyDto>
                {
                    Success = true,
                    Data = company,
                    Message = "Company retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company {Id}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the company"
                });
            }
        }

        /// <summary>
        /// Create a new company
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CompanyDto>), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyDto companyDto)
        {
            try
            {
                _logger.LogInformation("Creating new company {CompanyName}", companyDto.Name);

                if (!await _companyService.ValidateCompanyDataAsync(companyDto))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid company data or company name already exists"
                    });
                }

                var createdCompany = await _companyService.CreateAsync(companyDto);

                return CreatedAtAction(
                    nameof(GetCompany),
                    new { id = createdCompany.Id },
                    new ApiResponse<CompanyDto>
                    {
                        Success = true,
                        Data = createdCompany,
                        Message = "Company created successfully"
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating company");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while creating the company"
                });
            }
        }

        /// <summary>
        /// Update an existing company
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateCompany(int id, [FromBody] CompanyDto companyDto)
        {
            try
            {
                _logger.LogInformation("Updating company {Id}", id);

                if (id != companyDto.Id)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "ID mismatch"
                    });
                }

                if (!await _companyService.ValidateCompanyDataAsync(companyDto))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid company data or company name already exists"
                    });
                }

                var result = await _companyService.UpdateAsync(id, companyDto);
                if (!result)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = $"Company with ID {id} not found"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Company updated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating company {Id}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while updating the company"
                });
            }
        }

        /// <summary>
        /// Delete a company (soft delete if it has related data)
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            try
            {
                _logger.LogInformation("Deleting company {Id}", id);

                var result = await _companyService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = $"Company with ID {id} not found"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Company deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting company {Id}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while deleting the company"
                });
            }
        }

        /// <summary>
        /// Get companies by custom field value
        /// </summary>
        [HttpGet("by-custom-field/{customFieldId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CompanyDto>>), 200)]
        public async Task<IActionResult> GetCompaniesByCustomField(int customFieldId, [FromQuery] string value)
        {
            try
            {
                _logger.LogInformation("Getting companies by custom field {FieldId} with value {Value}", 
                    customFieldId, value);

                var companies = await _companyService.GetByCustomFieldAsync(customFieldId, value);

                return Ok(new ApiResponse<IEnumerable<CompanyDto>>
                {
                    Success = true,
                    Data = companies,
                    Message = "Companies retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting companies by custom field");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving companies"
                });
            }
        }

        /// <summary>
        /// Get recently modified companies
        /// </summary>
        [HttpGet("recently-modified")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CompanyDto>>), 200)]
        public async Task<IActionResult> GetRecentlyModified([FromQuery] int count = 10)
        {
            try
            {
                _logger.LogInformation("Getting {Count} recently modified companies", count);

                var companies = await _companyService.GetRecentlyModifiedAsync(count);

                return Ok(new ApiResponse<IEnumerable<CompanyDto>>
                {
                    Success = true,
                    Data = companies,
                    Message = "Recently modified companies retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recently modified companies");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving recently modified companies"
                });
            }
        }

        /// <summary>
        /// Restore a soft-deleted company
        /// </summary>
        [HttpPost("{id}/restore")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RestoreCompany(int id)
        {
            try
            {
                _logger.LogInformation("Restoring company {Id}", id);

                var result = await _companyService.RestoreAsync(id);
                if (!result)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = $"Company with ID {id} not found or is not deleted"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Company restored successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring company {Id}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while restoring the company"
                });
            }
        }
    }
}
