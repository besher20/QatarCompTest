using Microsoft.AspNetCore.Mvc;
using QatarPaymentTest.Models.Dtos;
using QatarPaymentTest.Services.Interfaces;

namespace QatarPaymentTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _contactService;
        private readonly ILogger<ContactsController> _logger;

        public ContactsController(IContactService contactService, ILogger<ContactsController> logger)
        {
            _contactService = contactService;
            _logger = logger;
        }

        /// <summary>
        /// Get all contacts with pagination and filtering
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<ContactDto>>), 200)]
        public async Task<IActionResult> GetContacts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool ascending = true,
            [FromQuery] bool includeInactive = false,
            [FromQuery] bool includeDeleted = false,
            [FromQuery] int? companyId = null)
        {
            try
            {
                _logger.LogInformation(
                    "Getting contacts with page {Page}, pageSize {PageSize}, searchTerm {SearchTerm}, " +
                    "sortBy {SortBy}, ascending {Ascending}, includeInactive {IncludeInactive}, " +
                    "includeDeleted {IncludeDeleted}, companyId {CompanyId}",
                    page, pageSize, searchTerm, sortBy, ascending, includeInactive, includeDeleted, companyId);

                var (items, totalCount, totalPages) = await _contactService.GetAllAsync(
                    page, pageSize, searchTerm, sortBy, ascending, includeInactive, includeDeleted, companyId);

                return Ok(new ApiResponse<PagedResult<ContactDto>>
                {
                    Success = true,
                    Data = new PagedResult<ContactDto>
                    {
                        Items = items,
                        TotalCount = totalCount,
                        TotalPages = totalPages,
                        CurrentPage = page,
                        PageSize = pageSize
                    },
                    Message = "Contacts retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting contacts");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving contacts"
                });
            }
        }

        /// <summary>
        /// Search contacts with custom field filters
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<ContactDto>>), 200)]
        public async Task<IActionResult> SearchContacts(
            [FromQuery] string? searchTerm = null,
            [FromQuery] Dictionary<string, object>? customFieldFilters = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool ascending = true)
        {
            try
            {
                _logger.LogInformation(
                    "Searching contacts with searchTerm {SearchTerm}, customFieldFilters {CustomFieldFilters}, " +
                    "page {Page}, pageSize {PageSize}, sortBy {SortBy}, ascending {Ascending}",
                    searchTerm, customFieldFilters, page, pageSize, sortBy, ascending);

                var (items, totalCount, totalPages) = await _contactService.SearchAsync(
                    searchTerm, customFieldFilters, page, pageSize, sortBy, ascending);

                return Ok(new ApiResponse<PagedResult<ContactDto>>
                {
                    Success = true,
                    Data = new PagedResult<ContactDto>
                    {
                        Items = items,
                        TotalCount = totalCount,
                        TotalPages = totalPages,
                        CurrentPage = page,
                        PageSize = pageSize
                    },
                    Message = "Contacts retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching contacts");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while searching contacts"
                });
            }
        }

        /// <summary>
        /// Get a contact by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ContactDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetContact(
            int id,
            [FromQuery] bool includeCompanies = true,
            [FromQuery] bool includeCustomFields = true)
        {
            try
            {
                _logger.LogInformation(
                    "Getting contact with ID {Id}, includeCompanies {IncludeCompanies}, includeCustomFields {IncludeCustomFields}",
                    id, includeCompanies, includeCustomFields);

                var contact = await _contactService.GetByIdAsync(id, includeCompanies, includeCustomFields);
                if (contact == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = $"Contact with ID {id} not found"
                    });
                }

                return Ok(new ApiResponse<ContactDto>
                {
                    Success = true,
                    Data = contact,
                    Message = "Contact retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting contact {Id}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the contact"
                });
            }
        }

        /// <summary>
        /// Create a new contact
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ContactDto>), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateContact([FromBody] CreateContactDto contactDto)
        {
            try
            {
                _logger.LogInformation("Creating new contact");

                if (!await _contactService.ValidateContactDataAsync(contactDto))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid contact data"
                    });
                }

                var createdContact = await _contactService.CreateAsync(contactDto);

                return CreatedAtAction(
                    nameof(GetContact),
                    new { id = createdContact.Id },
                    new ApiResponse<ContactDto>
                    {
                        Success = true,
                        Data = createdContact,
                        Message = "Contact created successfully"
                    });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error while creating contact");
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating contact");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while creating the contact"
                });
            }
        }

        /// <summary>
        /// Update an existing contact
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateContact(int id, [FromBody] UpdateContactDto contactDto)
        {
            try
            {
                _logger.LogInformation("Updating contact {Id}", id);

                if (!await _contactService.ExistsAsync(id))
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = $"Contact with ID {id} not found"
                    });
                }

                if (!await _contactService.ValidateContactDataAsync(contactDto, id))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid contact data"
                    });
                }

                var result = await _contactService.UpdateAsync(id, contactDto);
                if (!result)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = $"Contact with ID {id} not found"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Contact updated successfully"
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error while updating contact {Id}", id);
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating contact {Id}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while updating the contact"
                });
            }
        }

        /// <summary>
        /// Delete a contact
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteContact(int id)
        {
            try
            {
                _logger.LogInformation("Deleting contact {Id}", id);

                var result = await _contactService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = $"Contact with ID {id} not found"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Contact deleted successfully"
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Cannot delete contact {Id}: {Message}", id, ex.Message);
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting contact {Id}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while deleting the contact"
                });
            }
        }

        /// <summary>
        /// Restore a deleted contact
        /// </summary>
        [HttpPost("{id}/restore")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RestoreContact(int id)
        {
            try
            {
                _logger.LogInformation("Restoring contact {Id}", id);

                var result = await _contactService.RestoreAsync(id);
                if (!result)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = $"Contact with ID {id} not found or is not deleted"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Contact restored successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring contact {Id}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while restoring the contact"
                });
            }
        }

        /// <summary>
        /// Get contacts by company ID with pagination
        /// </summary>
        [HttpGet("by-company/{companyId}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<ContactDto>>), 200)]
        public async Task<IActionResult> GetContactsByCompany(
            int companyId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool ascending = true)
        {
            try
            {
                _logger.LogInformation(
                    "Getting contacts for company {CompanyId} with page {Page}, pageSize {PageSize}, " +
                    "searchTerm {SearchTerm}, sortBy {SortBy}, ascending {Ascending}",
                    companyId, page, pageSize, searchTerm, sortBy, ascending);

                var (items, totalCount, totalPages) = await _contactService.GetByCompanyAsync(
                    companyId, page, pageSize, searchTerm, sortBy, ascending);

                return Ok(new ApiResponse<PagedResult<ContactDto>>
                {
                    Success = true,
                    Data = new PagedResult<ContactDto>
                    {
                        Items = items,
                        TotalCount = totalCount,
                        TotalPages = totalPages,
                        CurrentPage = page,
                        PageSize = pageSize
                    },
                    Message = "Contacts retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting contacts for company {CompanyId}", companyId);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving contacts"
                });
            }
        }

        /// <summary>
        /// Get contact statistics
        /// </summary>
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(ApiResponse<ContactStatisticsDto>), 200)]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                _logger.LogInformation("Getting contact statistics");

                var statistics = await _contactService.GetStatisticsAsync();

                return Ok(new ApiResponse<ContactStatisticsDto>
                {
                    Success = true,
                    Data = statistics,
                    Message = "Contact statistics retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting contact statistics");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving contact statistics"
                });
            }
        }
    }
}
