using LabelDesigner.Services.Interface;
using LabelDesigner.Shared.Request;
using Microsoft.AspNetCore.Mvc;

namespace LabelDesigner.PublicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PdfController : ControllerBase
    {
        private readonly ISvgConverterService _svgConverterService;
        public PdfController(ISvgConverterService svgConverterService)
        {
            _svgConverterService = svgConverterService;
        }

        [HttpPost]
        public IActionResult GeneratePdf([FromBody] GenerateBulkPdfRequest request)
        {
            byte[] pdfContent = _svgConverterService.GeneratePdf(request);
            return File(pdfContent, "application/pdf", $"{request.TemplateId}.pdf");
        }
    }
}
