using System.ComponentModel.DataAnnotations;

namespace LabelDesigner.Shared.Widgets
{
    public abstract class Widget
    {
        public int Id { get; set; }

        [Required]
        public abstract WidgetType Type { get; }

        [Range(0, double.MaxValue, ErrorMessage = "X must be >= 0")]
        public double X { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Y must be >= 0")]
        public double Y { get; set; }

        [Range(1, 5000, ErrorMessage = "Width must be between 1 and 5000")]
        public double Width { get; set; }

        [Range(1, 5000, ErrorMessage = "Height must be between 1 and 5000")]
        public double Height { get; set; }

        [RegularExpression(@"^#([0-9A-Fa-f]{6})$", ErrorMessage = "Invalid HEX color")]
        public string Color { get; set; } = "#000000";

        [Range(0, 360, ErrorMessage = "Rotation must be between 0 and 360")]
        public double Rotation { get; set; } = 0;

        public bool FlipHorizontal { get; set; } = false;
        public bool FlipVertical { get; set; } = false;

        public WidgetMode Mode { get; set; } = WidgetMode.ReadOnly;
        public string FillColor { get; set; } = "none";
        public double StrokeWidth { get; set; } = .02;
        public string StrokeWidthFormatted { get { return $"{StrokeWidth}vw"; } }
    }

}
