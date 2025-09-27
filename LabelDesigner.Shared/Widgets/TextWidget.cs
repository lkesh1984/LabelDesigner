using System.ComponentModel.DataAnnotations;

namespace LabelDesigner.Shared.Widgets
{
    public class TextWidget : Widget
    {
        [Required, StringLength(200, ErrorMessage = "Text cannot exceed 200 characters")]
        public string Text { get; set; } = "Label";

        [Range(6, 100, ErrorMessage = "Font size must be between 6 and 100")]
        public int FontSize { get; set; } = 12;

        [StringLength(50)]
        public string FontFamily { get; set; } = "Arial";
        public string TextAnchor { get; set; } = "middle";
        public string DominantBaseline { get; set; } = "middle";

        public override WidgetType Type => WidgetType.Text;
    }
}
