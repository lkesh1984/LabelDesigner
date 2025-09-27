

using Winnovative;

namespace LabelDesigner.Services
{
    public class PdfData
    {
        public static string CreatePdfFromHtml()
        {
            // Sample HTML content
            string htmlContent = @"
            <html>
                <head>
                    <style>
                        body { font-family: Arial; }
                        h1 { color: darkblue; }
                        p { font-size: 14px; }
                    </style>
                </head>
                <body>
                    <h1>Hello from HTML!</h1>
<svg width=""100"" height=""100"">
  <circle cx=""50"" cy=""50"" r=""40"" stroke=""green"" stroke-width=""4"" fill=""yellow"" />
</svg>
                    <p>This PDF was generated from HTML using HtmlRenderer.PdfSharp.</p>
                </body>
            </html>";

            // Create the converter instance
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

            // OPTIONAL: Set license key (for trial or full version)
          //  htmlToPdfConverter.LicenseKey = "YOUR_LICENSE_KEY_HERE";

            // Set options (optional)
            htmlToPdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
            htmlToPdfConverter.PdfDocumentOptions.PdfCompressionLevel = PdfCompressionLevel.Normal;

            // Convert HTML string to PDF and save to file
          //  string htmlContent = "<html><body><h1>Hello from .NET 8!</h1></body></html>";
            byte[] pdfBuffer = htmlToPdfConverter.ConvertHtml(htmlContent, baseUrl: null);

            string outputPath = "output.pdf";
            // Save to file
            File.WriteAllBytes("output.pdf", pdfBuffer);

            Console.WriteLine("PDF created successfully.");
            var completepath = Path.GetFullPath(outputPath);
            return Path.GetFullPath(outputPath);
        }
    }
}