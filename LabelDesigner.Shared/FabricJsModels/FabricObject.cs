using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Text.Json.Serialization;

namespace LabelDesigner.Shared.FabricJsModels
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(CircleObject), "circle")]
    [JsonDerivedType(typeof(RectangleObject), "rectangle")]
    [JsonDerivedType(typeof(RectangleObject), "line")]
    [JsonDerivedType(typeof(RectangleObject), "textbox")]
    [JsonDerivedType(typeof(RectangleObject), "group")]
    [JsonDerivedType(typeof(RectangleObject), "image")]
    [JsonDerivedType(typeof(RectangleObject), "barcode")]
    public class FabricObject
    {
        [Required(ErrorMessage = "Id is required")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Type is required")]
        [JsonPropertyName("type")]
        public string Type { get; set; }
        public string CustomType { get; set; }
        public string Version { get; set; }
        public string OriginX { get; set; }
        public string OriginY { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }

        [Range(1, 5000, ErrorMessage = "Width must be between 1 and 5000")]
        public double Width { get; set; }

        [Range(1, 5000, ErrorMessage = "Height must be between 1 and 5000")]
        public double Height { get; set; }

        [RegularExpression(@"^#([0-9A-Fa-f]{6})$", ErrorMessage = "Invalid HEX color")]
        public string Fill { get; set; }
        public string Stroke { get; set; }
        public double StrokeWidth { get; set; }
        public List<double>? StrokeDashArray { get; set; }
        public string StrokeLineCap { get; set; }
        public double StrokeDashOffset { get; set; }
        public string StrokeLineJoin { get; set; }
        public bool StrokeUniform { get; set; }
        public int StrokeMiterLimit { get; set; }
        public double ScaleX { get; set; }
        public double ScaleY { get; set; }
        public double Angle { get; set; }
        public bool FlipX { get; set; }
        public bool FlipY { get; set; }
        public double Opacity { get; set; }
        public object? Shadow { get; set; }
        public bool Visible { get; set; }

        [RegularExpression(@"^#([0-9A-Fa-f]{6})$", ErrorMessage = "Invalid HEX color")]
        public string BackgroundColor { get; set; }
        public string FillRule { get; set; }
        public string PaintFirst { get; set; }
        public string GlobalCompositeOperation { get; set; }
        public double SkewX { get; set; }
        public double SkewY { get; set; }
    }
}
