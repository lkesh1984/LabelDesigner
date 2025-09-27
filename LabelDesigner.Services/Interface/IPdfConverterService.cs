namespace LabelDesigner.Services.Interface
{
    public interface IPdfConverterService
    {
        byte[] ConvertSvgToPdf(string svgFilePath, string outputPdfPath);
    }
}
