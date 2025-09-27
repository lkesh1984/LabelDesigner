using LabelDesigner.Data.Interfaces;
using LabelDesigner.Services.Interface;
using LabelDesigner.Shared;
using LabelDesigner.Shared.FabricJsModels;
using LabelDesigner.Shared.Request;
using Newtonsoft.Json;
using System.Text;
using LabelDesigner.Shared.Widgets;

namespace LabelDesigner.Services.Implementations
{
    public class SvgConverterService : ISvgConverterService
    {
        private IBarcodeService _barcodeService = null;
        private IPdfConverterService _pdfConverterService;
        private ITemplateRepository _templateRepository;

        public SvgConverterService(IBarcodeService barcodeService, IPdfConverterService pdfConverter, ITemplateRepository templateRepository)
        {
            _barcodeService = barcodeService;
            _pdfConverterService = pdfConverter;
            _templateRepository = templateRepository;
        }

        public byte[] GeneratePdf(GenerateBulkPdfRequest request)
        {
            int width = request.Width == 0 ? 800 : request.Width;
            int height = request.Height == 0 ? 500 : request.Height; ;

            var template = _templateRepository.GetTemplateAsync(Guid.Parse(request.TemplateId)).Result;
            //var template = new TemplateNew
            //{
            //    CanvasSettings = "",
            //    FabricJson = File.ReadAllText("C:/Git/Hackathon/LabelDesigner/Inputjson.txt")
            //};

            var fabricData = JsonConvert.DeserializeObject<FabricJson>(template.FabricJson, new FabricObjectConverter());

            StringBuilder svgString = new StringBuilder(@"<!DOCTYPE html>
                                                            <html lang=""en"">
                                                            <head>
                                                              <meta charset=""UTF-8"">
                                                              <title>SVG Example</title>
                                                              <style>
                                                                html, body {
                                                                  height: 100%;
                                                                  margin: 0;
                                                                  display: flex;
                                                                  justify-content: center; /* horizontal centering */
                                                                  align-items: center;     /* vertical centering */
                                                                  background-color: #f0f0f0; /* optional */
                                                                }

                                                                svg {
                                                                  max-width: 90%;  /* optional: keep it responsive */
                                                                  max-height: 90%;
                                                                }
                                                              </style>
                                                            </head>
                                                            <body>");

            // convert to SVG
            foreach (var record in request.Records)
            {
                svgString.Append(ConvertToSvg(record, width, height, fabricData));
            }
            svgString.AppendLine("</body></html>");

            // path where file should be saved
            string filePath = Constants.SvgFilePath;

            // write UTF-8 without BOM
            File.WriteAllText(filePath, svgString.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            return _pdfConverterService.ConvertSvgToPdf(filePath, Constants.PdfFilePath);

            //return sb.ToString();
        }

        private StringBuilder ConvertToSvg(GeneratePdfRequest request, int width, int height, FabricJson? fabricData)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<svg xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" version=\"1.1\" width=\"{width}\" height=\"{height}\" viewBox=\"0 0 {fabricData.Width} {fabricData.Height}\">");

            // Background
            if (!string.IsNullOrEmpty(fabricData.Background))
            {
                sb.AppendLine($"  <rect width=\"100%\" height=\"100%\" fill=\"{fabricData.Background}\" />");
            }

            // Objects
            foreach (var obj in fabricData.Objects)
            {
                sb.AppendLine(ConvertObject(obj, request.TemplateWidgets));
            }

            sb.AppendLine("</svg>");
            return sb;
        }

        private string ConvertObject(FabricObject obj, List<TemplateWidget> templateWidgets = null)
        {
            var widgetValue = string.Empty;
            if (obj.Type != FabricJsShapeType.Group)
                widgetValue = templateWidgets.Where(x => x.WidgetId == obj.Id).Select(x => x.WidgetValue).FirstOrDefault();

            switch (obj.Type)
            {
                case FabricJsShapeType.TextBox:
                    return ConvertTextbox(obj as TextBoxObject, widgetValue);
                    break;

                case FabricJsShapeType.Circle:
                    return ConvertCircle(obj as CircleObject);
                    break;

                case FabricJsShapeType.Line:
                    return ConvertLine(obj as LineObject);

                case FabricJsShapeType.Rectangle:
                    return ConvertRect(obj as RectangleObject);
                    break;
                case FabricJsShapeType.Polygon:
                    return ConvertPolygon(obj as PolygonObject);
                    break;

                case FabricJsShapeType.Group:
                    if (obj.CustomType == FabricJsShapeType.Barcode)
                    {
                        return ConvertBarcode(obj as BarcodeObject, widgetValue);
                    }
                    return ConvertGroup(obj as GroupObject, templateWidgets);
                    break;
                case FabricJsShapeType.Image:
                    if (obj.CustomType == FabricJsShapeType.Barcode)
                    {
                        return ConvertBarcode(obj as BarcodeObject, widgetValue);
                    }
                    return ConvertImage(obj as ImageObject);
                    break;
                case FabricJsShapeType.Barcode:
                    return ConvertBarcode(obj as BarcodeObject, widgetValue);
                    break;

                default:
                    return $"  <!-- Unsupported object type: {obj.Type} -->";
            }
        }

        private string ConvertImage(ImageObject obj)
        {
            var transform = $"translate({obj.Left},{obj.Top}) rotate({obj.Angle}) scale({obj.ScaleX},{obj.ScaleY}) skewX({obj.SkewX}) skewY({obj.SkewY})";

            return $"  <image x=\"0\" y=\"0\" width=\"{obj.Width}\" height=\"{obj.Height}\" " +
                   $"xlink:href=\"{obj.Src}\" transform=\"{transform}\" />";
        }

        private string ConvertBarcode(BarcodeObject obj, string widgetValue)
        {
            string barcodeSvg = _barcodeService.CreateBarcodeBase64(widgetValue, obj);

            return barcodeSvg;
        }

        private string ConvertGroup(GroupObject group, List<TemplateWidget> templateWidgets)
        {
            var transform = $"translate({group.Left},{group.Top}) rotate({group.Angle}) scale({group.ScaleX},{group.ScaleY}) skewX({group.SkewX}) skewY({group.SkewY})";

            var sb = new StringBuilder();
            sb.AppendLine($"  <g transform=\"{transform}\">");

            if (group.Objects != null)
            {
                foreach (var child in group.Objects)
                {
                    sb.AppendLine(ConvertObject(child, templateWidgets));
                }
            }

            sb.AppendLine("  </g>");
            return sb.ToString();
        }

        
        private string ConvertTextbox(TextBoxObject obj, string widgetValue)
        {
            var textValue = obj.IsDynamic ? widgetValue : obj.Text;
            var sb = new StringBuilder();

            // Apply Fabric-like transform
            double tx = obj.Left + obj.Width / 2.0;
            double ty = obj.Top + obj.Height / 2.0;
            var transform = $"matrix({obj.ScaleX:0.##} 0 0 {obj.ScaleY:0.##} {tx:0.##} {ty:0.##})";

            sb.AppendLine($"<g transform=\"{transform}\">");

            // Background rect (can be uncommented later)
            //if (!string.IsNullOrEmpty(obj.BackgroundColor) && obj.BackgroundColor != "transparent")
            //{
            //    double rectX = -obj.Width / 2.0;
            //    double rectY = -obj.Height / 2.0;
            //    sb.AppendLine(
            //        $"  <rect fill=\"{obj.BackgroundColor}\" x=\"{rectX:0.##}\" y=\"{rectY:0.##}\" width=\"{obj.Width:0.##}\" height=\"{obj.Height:0.##}\" />");
            //}

            // Text + multi-line tspans
            var textDecoration = obj.Linethrough ? "line-through" : "";
            sb.AppendLine($"  <text font-family=\"{obj.FontFamily}\" font-size=\"{obj.FontSize}\" " +
                          $"font-weight=\"{obj.FontWeight}\" font-style=\"{obj.FontStyle}\" fill=\"{obj.Fill}\" text-decoration=\"{textDecoration}\">");

            // Split lines
            var lines = textValue.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            double startX = -obj.Width / 2.0;
            double startY = -obj.Height / 2.0 + obj.FontSize; // top + baseline
            double lineDy = obj.FontSize * obj.LineHeight;

            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0)
                    sb.AppendLine($"    <tspan x=\"{startX:0.##}\" y=\"{startY:0.##}\">{System.Security.SecurityElement.Escape(lines[i])}</tspan>");
                else
                    sb.AppendLine($"    <tspan x=\"{startX:0.##}\" dy=\"{lineDy:0.##}\">{System.Security.SecurityElement.Escape(lines[i])}</tspan>");
            }

            sb.AppendLine("  </text>");
            sb.AppendLine("</g>");
            return sb.ToString();
        }


        private string ConvertCircle(CircleObject obj)
        {
            string fill = string.IsNullOrEmpty(obj.Fill) ? "none" : obj.Fill;
            string stroke = string.IsNullOrEmpty(obj.Stroke) ? "none" : obj.Stroke;
            double strokeWidth = obj.StrokeWidth <= 0 ? 1 : obj.StrokeWidth;

            var transform = $"translate({obj.Left},{obj.Top}) rotate({obj.Angle}) scale({obj.ScaleX},{obj.ScaleY}) skewX({obj.SkewX}) skewY({obj.SkewY})";

            if (obj.StartAngle == 0 && obj.EndAngle >= 360)
            {
                return $"  <circle cx=\"{obj.Radius}\" cy=\"{obj.Radius}\" r=\"{obj.Radius}\" " +
                       $"fill=\"{fill}\" stroke=\"{stroke}\" stroke-width=\"{strokeWidth}\" " +
                       $"transform=\"{transform}\" />";
            }
            else
            {
                double startRad = obj.StartAngle * Math.PI / 180.0;
                double endRad = obj.EndAngle * Math.PI / 180.0;

                double x1 = obj.Radius + obj.Radius * Math.Cos(startRad);
                double y1 = obj.Radius + obj.Radius * Math.Sin(startRad);
                double x2 = obj.Radius + obj.Radius * Math.Cos(endRad);
                double y2 = obj.Radius + obj.Radius * Math.Sin(endRad);

                int largeArc = (obj.EndAngle - obj.StartAngle) > 180 ? 1 : 0;

                return $"  <path d=\"M {obj.Radius},{obj.Radius} L {x1},{y1} A {obj.Radius},{obj.Radius} 0 {largeArc},1 {x2},{y2} Z\" " +
                       $"fill=\"{fill}\" stroke=\"{stroke}\" stroke-width=\"{strokeWidth}\" transform=\"{transform}\" />";
            }
        }

        public string ConvertLine(LineObject obj)
        {
            // Create the transform matrix from Fabric's props
            // Fabric uses angle in degrees
            double angleRad = obj.Angle * Math.PI / 180.0;

            double cos = Math.Cos(angleRad);
            double sin = Math.Sin(angleRad);

            // Fabric matrix (a b c d e f)
            double a = cos * obj.ScaleX;
            double b = sin * obj.ScaleX;
            double c = -sin * obj.ScaleY;
            double d = cos * obj.ScaleY;

            // Compute center, not left/top
            double centerX = obj.Left + (obj.Width / 2.0);
            double centerY = obj.Top + (obj.Height / 2.0);

            string transform = string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                "matrix({0} {1} {2} {3} {4} {5})",
                a, b, c, d, centerX, centerY
            );

            // Build <g> wrapper with transform and Id
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"<g transform=\"{transform}\" id=\"{obj.Id}\">");

            // Build <line>
            sb.Append("  <line ");
            sb.Append($"x1=\"{obj.X1}\" y1=\"{obj.Y1}\" x2=\"{obj.X2}\" y2=\"{obj.Y2}\" ");
            sb.Append("style=\"");
            sb.Append($"stroke:{obj.Stroke};");
            sb.Append($"stroke-width:{obj.StrokeWidth};");
            sb.Append("stroke-dasharray:none;");
            sb.Append($"stroke-linecap:{obj.StrokeLineCap};");
            sb.Append($"stroke-dashoffset:{obj.StrokeDashOffset};");
            sb.Append($"stroke-linejoin:{obj.StrokeLineJoin};");
            sb.Append($"stroke-miterlimit:{obj.StrokeMiterLimit};");
            sb.Append($"fill:{obj.Fill};");
            sb.Append($"fill-rule:{obj.FillRule};");
            sb.Append($"opacity:{obj.Opacity};");
            sb.Append("\" />\n");

            // Close group
            sb.Append("</g>");

            return sb.ToString();
        }

        private string ConvertLine1(LineObject obj)
        {
            double x1 = obj.Left + obj.X1;
            double y1 = obj.Top + obj.Y1;
            double x2 = obj.Left + obj.X2;
            double y2 = obj.Top + obj.Y2;

            var transform = $"translate({obj.Left},{obj.Top}) rotate({obj.Angle}) scale({obj.ScaleX},{obj.ScaleY}) skewX({obj.SkewX}) skewY({obj.SkewY})";

            return $"  <line x1=\"{x1}\" y1=\"{obj.Y1}\" x2=\"{x2}\" y2=\"{obj.Y2}\" " +
                   $"stroke=\"{obj.Stroke}\" stroke-width=\"{obj.StrokeWidth}\" stroke-linecap=\"{obj.StrokeLineCap}\" " +
                   $"transform=\"{transform}\" />";
        }

        private static string ConvertRect(RectangleObject obj)
        {
            string fill = string.IsNullOrEmpty(obj.Fill) ? "none" : obj.Fill;
            string stroke = string.IsNullOrEmpty(obj.Stroke) ? "none" : obj.Stroke;
            double strokeWidth = obj.StrokeWidth <= 0 ? 1 : obj.StrokeWidth;

            var transform = $"translate({obj.Left},{obj.Top}) rotate({obj.Angle}) scale({obj.ScaleX},{obj.ScaleY}) skewX({obj.SkewX}) skewY({obj.SkewY})";

            return $"  <rect x=\"0\" y=\"0\" width=\"{obj.Width}\" height=\"{obj.Height}\" " +
                   $"fill=\"{fill}\" stroke=\"{stroke}\" stroke-width=\"{strokeWidth}\" " +
                   $"rx=\"{obj.Rx}\" ry=\"{obj.Ry}\" transform=\"{transform}\" />";
        }

        private static string ConvertPolygon(PolygonObject obj)
        {
            var points = new List<string>();
            foreach (var p in obj.Points)
            {
                points.Add($"{p.X + obj.Left},{p.Y + obj.Top}");
            }

            string fill = string.IsNullOrEmpty(obj.Fill) ? "none" : obj.Fill;
            string stroke = string.IsNullOrEmpty(obj.Stroke) ? "none" : obj.Stroke;
            double strokeWidth = obj.StrokeWidth <= 0 ? 1 : obj.StrokeWidth;

            return $"  <polygon points=\"{string.Join(" ", points)}\" fill=\"{fill}\" stroke=\"{stroke}\" stroke-width=\"{strokeWidth}\" />";

        }

        private string MapTextAlign(string align)
        {
            switch (align)
            {
                case "left":
                    return "start";
                case "center":
                    return "middle";
                case "right":
                    return "end";
                default:
                    return "start";
            }
        }
    }
}
