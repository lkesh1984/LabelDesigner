using LabelDesigner.Services.Interface;
using LabelDesigner.Shared.Request;
using Microsoft.AspNetCore.Mvc;

namespace LabelDesignerWebApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestPdfController : ControllerBase
    {
        private readonly ISvgConverterService _svgConverterService;
        private readonly ILogger _logger;
        public TestPdfController(ISvgConverterService svgConverterService, ILogger<TestPdfController> logger)
        {
            _svgConverterService = svgConverterService;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult GeneratePdf([FromBody] GenerateBulkPdfRequest request)
        {
            _logger.LogInformation($"PDF generation request for template: {request.TemplateId}");
            byte[] pdfContent = _svgConverterService.GeneratePdf(request);

            _logger.LogInformation($"PDF generated for template: {request.TemplateId}");
            return File(pdfContent, "application/pdf", $"Test.pdf");
        }
    }
}
