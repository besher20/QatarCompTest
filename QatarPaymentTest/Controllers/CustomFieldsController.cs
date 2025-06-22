using Microsoft.AspNetCore.Mvc;
using QatarPaymentTest.Models.Dtos;
using QatarPaymentTest.Services.Interfaces;

namespace QatarPaymentTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomFieldsController : ControllerBase
    {
        private readonly ICustomFieldService _customFieldService;
        private readonly ILogger<CustomFieldsController> _logger;

        public CustomFieldsController(ICustomFieldService customFieldService, ILogger<CustomFieldsController> logger)
        {
            _customFieldService = customFieldService;
            _logger = logger;
        }

        /// <summary>
        /// Get all custom fields with optional filtering
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CustomFieldDto>>), 200)]
        public async Task<IActionResult> GetCustomFields(
            [FromQuery] string? entityType = null,
            [FromQuery] bool includeDeleted = false)
        {
            try
            {
                _logger.LogInformation("Getting custom fields with entityType {EntityType}, includeDeleted {IncludeDeleted}", 
                    entityType ?? "all", includeDeleted);

                var fields = await _customFieldService.GetAllAsync(entityType, includeDeleted);

                return Ok(new ApiResponse<IEnumerable<CustomFieldDto>>
                {
                    Success = true,
                    Data = fields,
                    Message = "Custom fields retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting custom fields");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving custom fields"
                });
            }
        }

        /// <summary>
        /// Get a custom field by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CustomFieldDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCustomField(int id)
        {
            try
            {
                _logger.LogInformation("Getting custom field with ID {Id}", id);

                var field = await _customFieldService.GetByIdAsync(id);
                if (field == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = $"Custom field with ID {id} not found"
                    });
                }

                return Ok(new ApiResponse<CustomFieldDto>
                {
                    Success = true,
                    Data = field,
                    Message = "Custom field retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting custom field {Id}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the custom field"
                });
            }
        }

        /// <summary>
        /// Create a new custom field
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CustomFieldDto>), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateCustomField([FromBody] CreateCustomFieldDto fieldDto)
        {
            try
            {
                _logger.LogInformation("Creating new custom field {FieldName}", fieldDto.Name);

                if (!await _customFieldService.ValidateCustomFieldDataAsync(fieldDto))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid custom field data or name already exists"
                    });
                }

                var createdField = await _customFieldService.CreateAsync(fieldDto);

                return CreatedAtAction(
                    nameof(GetCustomField),
                    new { id = createdField.Id },
                    new ApiResponse<CustomFieldDto>
                    {
                        Success = true,
                        Data = createdField,
                        Message = "Custom field created successfully"
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating custom field");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while creating the custom field"
                });
            }
        }

        /// <summary>
        /// Update an existing custom field
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateCustomField(int id, [FromBody] UpdateCustomFieldDto fieldDto)
        {
            try
            {
                _logger.LogInformation("Updating custom field {Id}", id);

                if (!await _customFieldService.ExistsAsync(id))
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = $"Custom field with ID {id} not found"
                    });
                }

                if (!await _customFieldService.ValidateCustomFieldDataAsync(fieldDto))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid custom field data or name already exists"
                    });
                }

                var result = await _customFieldService.UpdateAsync(id, fieldDto);
                if (!result)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = $"Custom field with ID {id} not found"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Custom field updated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating custom field {Id}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while updating the custom field"
                });
            }
        }

        /// <summary>
        /// Delete a custom field if it's not in use
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteCustomField(int id)
        {
            try
            {
                _logger.LogInformation("Deleting custom field {Id}", id);

                if (await _customFieldService.IsFieldInUseAsync(id))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Cannot delete custom field as it is currently in use"
                    });
                }

                var result = await _customFieldService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = $"Custom field with ID {id} not found"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Custom field deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting custom field {Id}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while deleting the custom field"
                });
            }
        }

        /// <summary>
        /// Get custom fields by type
        /// </summary>
        [HttpGet("by-type/{entityType}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CustomFieldDto>>), 200)]
        public async Task<IActionResult> GetCustomFieldsByType(string entityType)
        {
            try
            {
                _logger.LogInformation("Getting custom fields for type {EntityType}", entityType);

                var fields = await _customFieldService.GetByTypeAsync(entityType);

                return Ok(new ApiResponse<IEnumerable<CustomFieldDto>>
                {
                    Success = true,
                    Data = fields,
                    Message = "Custom fields retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting custom fields for type {EntityType}", entityType);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving custom fields"
                });
            }
        }

        /// <summary>
        /// Get usage statistics for a custom field
        /// </summary>
        [HttpGet("{id}/usage")]
        [ProducesResponseType(typeof(ApiResponse<CustomFieldUsageDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCustomFieldUsage(int id)
        {
            try
            {
                _logger.LogInformation("Getting usage statistics for custom field {Id}", id);

                var usage = await _customFieldService.GetFieldUsageAsync(id);

                return Ok(new ApiResponse<CustomFieldUsageDto>
                {
                    Success = true,
                    Data = usage,
                    Message = "Custom field usage statistics retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting usage statistics for custom field {Id}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving custom field usage statistics"
                });
            }
        }
    }
}
