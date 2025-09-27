using LabelDesigner.Shared.Request;

namespace LabelDesigner.Services.Interface
{
    public interface ISvgConverterService
    {
        byte[] GeneratePdf(GenerateBulkPdfRequest request);
    }
}
