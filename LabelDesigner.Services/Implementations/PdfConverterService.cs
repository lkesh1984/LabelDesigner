using LabelDesigner.Services.Interface;
using Winnovative;

namespace LabelDesigner.Services.Implementations
{
    public class PdfConverterService : IPdfConverterService
    {
        public byte[] ConvertSvgToPdf(string svgFilePath, string outputPdfPath)
        {
            // Initialize the converter with default options
            HtmlToPdfConverter converter = new HtmlToPdfConverter();

            // Set options via properties
            converter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
            converter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Portrait;

            // Convert SVG/HTML file to PDF as byte[]
            byte[] pdfBytes = converter.ConvertHtmlFile(svgFilePath);

            // Save PDF to file
            //File.WriteAllBytes(outputPdfPath, pdfBytes);
            return pdfBytes;
        }
    }
}
