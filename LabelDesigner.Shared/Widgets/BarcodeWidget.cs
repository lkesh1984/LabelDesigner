using System.ComponentModel.DataAnnotations;

namespace LabelDesigner.Shared.Widgets
{
    public class BarcodeWidget : ShapeWidget
    {
        public BarcodeWidget(double width, double height, double x, double y) : base(width, height, x, y)
        {
        }

        [Required(ErrorMessage = "Barcode data is required")]
        [StringLength(500, ErrorMessage = "Barcode data too long")]
        public string Data { get; set; } = "";

        [Required]
        public BarcodeType BarcodeType { get; set; }

        [Range(1, 10, ErrorMessage = "Module width must be between 1 and 10")]
        public double ModuleWidth { get; set; } = 2;

        [Range(10, 500, ErrorMessage = "Height must be between 10 and 500")]
        public double BarcodeHeight { get; set; } = 50;

        public override WidgetType Type => WidgetType.Barcode;
    }
}
