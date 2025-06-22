using Microsoft.AspNetCore.Mvc;
using QatarPaymentTest.Models.Dtos;
using QatarPaymentTest.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace QatarPaymentTest.Controllers
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
        public async Task<IActionResult> GetAll([FromQuery] string? entityType = null, [FromQuery] bool includeDeleted = false)
        {
            try
            {
                var fields = await _customFieldService.GetAllAsync(entityType, includeDeleted);
                return Ok(new ApiResponse<object> { Success = true, Data = fields });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = "An error occurred while fetching custom fields" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var field = await _customFieldService.GetByIdAsync(id);
                if (field == null)
                    return NotFound(new ApiResponse<object> { Success = false, Message = "Custom field not found" });

                return Ok(new ApiResponse<object> { Success = true, Data = field });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = "An error occurred while fetching the custom field" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomFieldDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Invalid data provided", Errors = ModelState });

            try
            {
                if (!await ValidateCustomFieldDto(dto))
                    return BadRequest(new ApiResponse<object> { Success = false, Message = "Invalid custom field configuration" });

                if (!await _customFieldService.ValidateCustomFieldDataAsync(new CustomFieldDto { Name = dto.Name, EntityType = dto.EntityType }))
                    return BadRequest(new ApiResponse<object> { Success = false, Message = "A custom field with this name already exists for this entity type" });

                var createdField = await _customFieldService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = createdField.Id }, 
                    new ApiResponse<object> { Success = true, Data = createdField });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = "An error occurred while creating the custom field" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomFieldDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Invalid data provided", Errors = ModelState });

            try
            {
                if (!await _customFieldService.ExistsAsync(id))
                    return NotFound(new ApiResponse<object> { Success = false, Message = "Custom field not found" });

                if (!await ValidateCustomFieldDto(dto))
                    return BadRequest(new ApiResponse<object> { Success = false, Message = "Invalid custom field configuration" });

                if (!await _customFieldService.ValidateCustomFieldDataAsync(new CustomFieldDto { Id = id, Name = dto.Name, EntityType = dto.EntityType }))
                    return BadRequest(new ApiResponse<object> { Success = false, Message = "A custom field with this name already exists for this entity type" });

                var success = await _customFieldService.UpdateAsync(id, dto);
                if (!success)
                    return NotFound(new ApiResponse<object> { Success = false, Message = "Custom field not found" });

                return Ok(new ApiResponse<object> { Success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = "An error occurred while updating the custom field" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (!await _customFieldService.ExistsAsync(id))
                    return NotFound(new ApiResponse<object> { Success = false, Message = "Custom field not found" });

                if (await _customFieldService.IsFieldInUseAsync(id))
                    return BadRequest(new ApiResponse<object> { Success = false, Message = "Cannot delete a custom field that is in use" });

                var success = await _customFieldService.DeleteAsync(id);
                if (!success)
                    return NotFound(new ApiResponse<object> { Success = false, Message = "Custom field not found" });

                return Ok(new ApiResponse<object> { Success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = "An error occurred while deleting the custom field" });
            }
        }

        [HttpGet("type/{entityType}")]
        public async Task<IActionResult> GetByType(string entityType)
        {
            try
            {
                var fields = await _customFieldService.GetByTypeAsync(entityType);
                return Ok(new ApiResponse<object> { Success = true, Data = fields });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = "An error occurred while fetching custom fields" });
            }
        }

        [HttpGet("{id}/usage")]
        public async Task<IActionResult> GetFieldUsage(int id)
        {
            try
            {
                if (!await _customFieldService.ExistsAsync(id))
                    return NotFound(new ApiResponse<object> { Success = false, Message = "Custom field not found" });

                var usage = await _customFieldService.GetFieldUsageAsync(id);
                return Ok(new ApiResponse<object> { Success = true, Data = usage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = "An error occurred while fetching field usage" });
            }
        }

        private async Task<bool> ValidateCustomFieldDto(CreateCustomFieldDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.EntityType))
                return false;

            // Validate based on field type
            switch (dto.FieldType)
            {
                case Models.Enum.CustomFieldType.Text:
                case Models.Enum.CustomFieldType.MultilineText:
                    if (dto.MinLength.HasValue && dto.MaxLength.HasValue && dto.MinLength > dto.MaxLength)
                        return false;
                    break;

                case Models.Enum.CustomFieldType.Number:
                case Models.Enum.CustomFieldType.Decimal:
                    if (dto.MinValue.HasValue && dto.MaxValue.HasValue && dto.MinValue > dto.MaxValue)
                        return false;
                    break;

                case Models.Enum.CustomFieldType.Select:
                case Models.Enum.CustomFieldType.MultiSelect:
                    if (dto.AllowedValues == null || dto.AllowedValues.Count == 0)
                        return false;
                    break;

                case Models.Enum.CustomFieldType.Regex:
                    if (string.IsNullOrWhiteSpace(dto.ValidationRegex))
                        return false;
                    try
                    {
                        var regex = new System.Text.RegularExpressions.Regex(dto.ValidationRegex);
                    }
                    catch
                    {
                        return false;
                    }
                    break;
            }

            return true;
        }
    }
} 