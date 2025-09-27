using LabelDesigner.Shared.FabricJsModels;
using LabelDesigner.Shared.Request;
using LabelDesigner.Shared.Widgets;

namespace LabelDesigner.Services.Interface
{
    public interface IBarcodeService
    {
        /// <summary>
        /// Method to generate barcode
        /// </summary>
        /// <param name="generateBarcodeRequest"></param>
        /// <returns></returns>
        Task<string> GenerateBarcode(GeneratePdfRequest generateBarcodeRequest);

        string CreateBarcodeBase64(string widgetValue, BarcodeObject barcodeObject);
    }
}
