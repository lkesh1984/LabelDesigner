using LabelDesigner.Data.Interfaces;
using LabelDesigner.Shared.Widgets;
using Microsoft.AspNetCore.Mvc;

namespace LabelDesignerWebApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TemplatesController : ControllerBase
    {
        private readonly ITemplateRepository _repo;
        private readonly ILogger<TemplatesController> _logger;

        public TemplatesController(ITemplateRepository repo, ILogger<TemplatesController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<TemplateNew>> GetTemplates()
        {
            _logger.LogInformation("Fetching all templates");
            var templates = await _repo.GetTemplatesAsync();
            _logger.LogInformation("Fetched {Count} templates", templates?.Count() ?? 0);
            return templates;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TemplateNew>> GetTemplate(Guid id)
        {
            _logger.LogInformation("Fetching template with ID {TemplateId}", id);
            var template = await _repo.GetTemplateAsync(id);

            if (template == null)
            {
                _logger.LogWarning("Template with ID {TemplateId} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Template with ID {TemplateId} retrieved successfully", id);
            return template;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTemplate([FromBody] TemplateNew templateObject)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // returns validation errors
            }
            templateObject.SysId = Guid.NewGuid().ToString();

            _logger.LogInformation("Creating new template with ID {TemplateId}", templateObject.SysId);

            try
            {
                await _repo.CreateTemplateAsync(templateObject);
                _logger.LogInformation("Template with ID {TemplateId} created successfully", templateObject.SysId);
                return Ok();
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error creating template with ID {TemplateId}", templateObject.SysId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTemplate(Guid id, [FromBody] TemplateNew template)
        {
            template.SysId = id.ToString();
            _logger.LogInformation("Updating template with ID {TemplateId}", id);

            try
            {
                var updated = await _repo.UpdateTemplateAsync(template);
                if (!updated)
                {
                    _logger.LogWarning("Template with ID {TemplateId} not found for update", id);
                    return NotFound();
                }

                _logger.LogInformation("Template with ID {TemplateId} updated successfully", id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating template with ID {TemplateId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplate(Guid id)
        {
            _logger.LogInformation("Deleting template with ID {TemplateId}", id);

            try
            {
                var deleted = await _repo.DeleteTemplateAsync(id);
                if (!deleted)
                {
                    _logger.LogWarning("Template with ID {TemplateId} not found for deletion", id);
                    return NotFound();
                }

                _logger.LogInformation("Template with ID {TemplateId} deleted successfully", id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting template with ID {TemplateId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("cloneTemplate/{templateSysId}")]
        public async Task<IActionResult> CloneTemplate(Guid templateSysId)
        {
            _logger.LogInformation("Cloning Template with Id: " + templateSysId);
            try
            {
                var cloned = await _repo.CloneTemplateAsync(templateSysId);
                if (!cloned)
                {
                    _logger.LogWarning("Template with ID {TemplateId} " + templateSysId + " is not cloned");
                    return NotFound();
                }

                _logger.LogInformation("Template with ID {TemplateId} " + templateSysId + "  clones successfully");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cloning template with ID {TemplateId}", templateSysId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}