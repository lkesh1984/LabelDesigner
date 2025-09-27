using IronBarCode;
using LabelDesigner.Data.Interfaces;
using LabelDesigner.Services.Interface;
using LabelDesigner.Shared;
using LabelDesigner.Shared.FabricJsModels;
using LabelDesigner.Shared.Request;
using LabelDesigner.Shared.Widgets;
using System.Text;
using System.Text.Json;

namespace LabelDesigner.Services.Implementations
{
    /// <summary>
    /// Service to generate barcode
    /// </summary>
    public class BarcodeService : IBarcodeService
    {
        private readonly ITemplateRepository _templateRepository;
        public BarcodeService(ITemplateRepository templateRepository)
        {
            _templateRepository = templateRepository;
        }
        /// <summary>
        /// Method to generate barcode
        /// </summary>
        /// <param name="generateBarcodeRequest"></param>
        /// <returns></returns>
        public async Task<string> GenerateBarcode(GeneratePdfRequest generateBarcodeRequest)
        {
            //step 1: get the template from the database
            //var templates = await _templateRepository.GetTemplatesAsync(Guid.Parse(generateBarcodeRequest.TemplateId));
            var templates = await _templateRepository.GetTemplatesAsync(Guid.NewGuid());
            var template = templates.FirstOrDefault();

            //var template = JsonSerializer.Deserialize<Template>("{\r\n  \"sysId\": \"c5a13fc4-1d8f-4c39-8c17-44ac5b72d17d\",\r\n  \"name\": \"Product Label Template\",\r\n  \"createdOn\": \"2025-09-15T10:00:00Z\",\r\n  \"createdBy\": 1,\r\n  \"modifiedOn\": \"2025-09-15T12:00:00Z\",\r\n  \"modifiedBy\": 2,\r\n  \"canvasSettings\": {\r\n    \"width\": 800,\r\n    \"height\": 600,\r\n    \"backgroundColor\": \"#FFFFFF\"\r\n  },\r\n  \"widgets\": [\r\n    {\r\n      \"id\": 1,\r\n      \"type\": \"Rectangle\",\r\n      \"x\": 50,\r\n      \"y\": 40,\r\n      \"width\": 300,\r\n      \"height\": 200,\r\n      \"color\": \"#FF0000\",\r\n      \"fillColor\": \"#FFCCCC\",\r\n      \"mode\": \"ReadOnly\",\r\n      \"borderColor\": \"#000000\",\r\n      \"borderThickness\": 2,\r\n      \"strokeColor\": \"black\",\r\n      \"strokeWidth\": 0.05\r\n    },\r\n    {\r\n      \"id\": 4,\r\n      \"type\": \"Barcode\",\r\n      \"x\": 100,\r\n      \"y\": 300,\r\n      \"width\": 250,\r\n      \"height\": 100,\r\n      \"color\": \"#000000\",\r\n      \"fillColor\": \"none\",\r\n      \"mode\": \"Input\",\r\n      \"borderColor\": \"#000000\",\r\n      \"borderThickness\": 1,\r\n      \"strokeColor\": \"black\",\r\n      \"strokeWidth\": 0.02,\r\n      \"data\": \"\"\r\n    },\r\n    {\r\n      \"id\": 5,\r\n      \"type\": \"Text\",\r\n      \"x\": 150,\r\n      \"y\": 450,\r\n      \"width\": 200,\r\n      \"height\": 50,\r\n      \"color\": \"#0000FF\",\r\n      \"fillColor\": \"none\",\r\n      \"mode\": \"Input\",\r\n      \"strokeColor\": \"black\",\r\n      \"strokeWidth\": 0.02,\r\n      \"text\": \"\",\r\n      \"fontSize\": 16,\r\n      \"fontFamily\": \"Arial\"\r\n    }\r\n  ]\r\n}");

            var stringBuilder = new StringBuilder();
            //stringBuilder.AppendLine("<!DOCTYPE html>");
            //stringBuilder.AppendLine("<html>");
            //stringBuilder.AppendLine("<head>");
            //stringBuilder.AppendLine("<title>IronBarcode Example with SVG</title>");
            //stringBuilder.AppendLine("</head>");
            //stringBuilder.AppendLine("<body>");

            if (template != null)
            {
                stringBuilder.AppendLine("<svg xmlns='http://www.w3.org/2000/svg'>");

                foreach (var requestWidget in generateBarcodeRequest.TemplateWidgets)
                {
                    //SetWidgetValue(requestWidget, stringBuilder, template);
                }

                stringBuilder.AppendLine("</svg>");
            }

            //stringBuilder.AppendLine("</body>");
            //stringBuilder.AppendLine("</html>");
            return stringBuilder.ToString();
        }

        #region private methods

        /// <summary>
        /// Method to get barcode format with barcode input type
        /// </summary>
        /// <param name="barcodeType"></param>
        /// <returns></returns>
        private BarcodeWriterEncoding? GetBarcodeFormat(BarcodeType barcodeType)
        {
            switch (barcodeType)
            {
                case BarcodeType.Code128:
                    return BarcodeWriterEncoding.Code128;
                case BarcodeType.QRCode:
                    return BarcodeWriterEncoding.QRCode;
                case BarcodeType.DataMatrix:
                    return BarcodeWriterEncoding.DataMatrix;
                case BarcodeType.PDF417:
                    return BarcodeWriterEncoding.PDF417;
                case BarcodeType.EAN8:
                    return BarcodeWriterEncoding.EAN8;
                case BarcodeType.EAN13:
                    return BarcodeWriterEncoding.EAN13;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Method to check if the value is valid for barcode type used
        /// </summary>
        /// <param name="barcodeType"></param>
        /// <param name="barcodeValue"></param>
        /// <returns></returns>
        private bool IsValidBarcodeValue(BarcodeType barcodeType, string barcodeValue)
        {
            switch (barcodeType)
            {
                case BarcodeType.Code128:
                    return barcodeValue.Length > 0;
                case BarcodeType.QRCode:
                case BarcodeType.DataMatrix:
                case BarcodeType.PDF417:
                    return !string.IsNullOrEmpty(barcodeValue);
                case BarcodeType.EAN8:
                    return barcodeValue.All(char.IsDigit) && barcodeValue.Length == 7;
                case BarcodeType.EAN13:
                    return barcodeValue.All(char.IsDigit) && barcodeValue.Length == 12;
            }

            return false;
        }
        /// <summary>
        /// Method to set widget value
        /// </summary>
        /// <param name="templateWidget"></param>
        /// <param name="stringBuilder"></param>
        /// <param name="template"></param>
        private void SetWidgetValue(TemplateWidget templateWidget, StringBuilder stringBuilder, Template template)
        {
            //var widgetSettings = template.Widgets.Where(x => x.Id == templateWidget.WidgetId).FirstOrDefault();

            //switch ((WidgetType)templateWidget.WidgetType)
            //{
            //    case WidgetType.Rectangle:
            //        stringBuilder.AppendLine($"<rect id='{templateWidget.WidgetId}' x='{widgetSettings.X}' y='{widgetSettings.Y}' width='{widgetSettings.Width}' height='{widgetSettings.Height}' fill='none' stroke='black'/>");
            //        break;
            //    case WidgetType.Text:
            //        stringBuilder.AppendLine($"<text id='{templateWidget.WidgetId}' x='{widgetSettings.X}' y='{widgetSettings.Y + 12}' font-size='12'>{System.Security.SecurityElement.Escape(templateWidget.WidgetValue)}</text>");
            //        break;
            //    case WidgetType.Barcode:
            //        CreateBarcode(templateWidget, widgetSettings as BarcodeWidget, stringBuilder);
            //        break;
            //}
        }

        /// <summary>
        /// Method to create barcode and embed it as base64 in the image tag
        /// </summary>
        /// <param name="templateWidget"></param>
        /// <param name="widgetSettings"></param>
        /// <param name="stringBuilder"></param>
        private void CreateBarcode(TemplateWidget templateWidget, BarcodeWidget widgetSettings, StringBuilder stringBuilder)
        {
            string content = templateWidget.WidgetValue;

            //if (IsValidBarcodeValue((BarcodeType)templateWidget.WidgetType, templateWidget.WidgetValue) && GetBarcodeFormat((BarcodeType)templateWidget.WidgetType) != null)
            //{
            //    var barcodeWriter = BarcodeWriter.CreateBarcode(templateWidget.WidgetValue
            //                                                , BarcodeEncoding.EAN8
            //                                                , (int)widgetSettings.Width, (int)widgetSettings.Height);

            //    string pngPath = Path.Combine("C:\\projects", "barcode.png");
            //    barcodeWriter.SaveAsPng(pngPath);
            //    string base64Image = Convert.ToBase64String(barcodeWriter.ToPngBinaryData());

            //    stringBuilder.AppendLine($"<img src=\"data:image/png;base64,{base64Image}\" alt=\"Barcode\" />");
            //    stringBuilder.AppendLine($"<p>Content: {templateWidget.WidgetValue} </p>");
            //}
            //else
            //{
            //    Console.WriteLine("Unable to create barcode as the widget value is not compatible with the barcode: " + templateWidget.WidgetId);
            //}
        }

        /// <summary>
        /// Method to create barcode and embed it as base64 in the image tag
        /// </summary>
        /// <param name="templateWidget"></param>
        /// <param name="widgetSettings"></param>
        /// <param name="stringBuilder"></param>
        public string CreateBarcodeBase64(string widgetValue, BarcodeObject barcodeObject)
        {
            License.LicenseKey = Constants.BarcodeLicenseKey;

            StringBuilder stringBuilder = new StringBuilder();
            string content = widgetValue;

            if (IsValidBarcodeValue((BarcodeType)Enum.Parse(typeof(BarcodeType), barcodeObject.BarcodeType),
                    widgetValue) &&
                GetBarcodeFormat((BarcodeType)Enum.Parse(typeof(BarcodeType), barcodeObject.BarcodeType)) != null)
            {
                var barcodeWriter = BarcodeWriter.CreateBarcode(widgetValue
                    , GetBarcodeFormat((BarcodeType)Enum.Parse(typeof(BarcodeType), barcodeObject.BarcodeType)).Value
                    , (int)barcodeObject.Width, (int)barcodeObject.Height);

                barcodeWriter.SaveAsPng(Constants.BarcodeImageFilePath);
                string base64Image = Convert.ToBase64String(barcodeWriter.ToPngBinaryData());

                var transform =
                    $"translate({barcodeObject.Left},{barcodeObject.Top}) rotate({barcodeObject.Angle}) scale({barcodeObject.ScaleX},{barcodeObject.ScaleY}) skewX({barcodeObject.SkewX}) skewY({barcodeObject.SkewY})";

                stringBuilder.AppendLine(
                    $"  <image x=\"0\" y=\"0\" width=\"{barcodeObject.Width}\" height=\"{barcodeObject.Height}\" " +
                    $"xlink:href=\"data:image/png;base64,{base64Image}\" transform=\"{transform}\" />");

                // need to check this code for placing the barcode value under the barcode widget.
                //if (barcodeObject.BarcodeType != BarcodeType.QRCode.ToString())
                //{
                //    stringBuilder.AppendLine(
                //        $"<text x=\"{barcodeObject.Left}\" y=\"{barcodeObject.Top + barcodeObject.Height + 13}\" width=\"{barcodeObject.Width}\" height=\"{barcodeObject.Height}\">{widgetValue} </text>");
                //}
            }
            else
            {
                Console.WriteLine("Unable to create barcode as the widget value is not compatible with the barcode: ");
                throw new Exception(
                    "Unable to create barcode as the widget value is not compatible with the barcode: ");
            }

            return stringBuilder.ToString();
        }
    }

    #endregion
}
