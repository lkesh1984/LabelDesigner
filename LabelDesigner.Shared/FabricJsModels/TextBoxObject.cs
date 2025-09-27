using System.ComponentModel.DataAnnotations;

namespace LabelDesigner.Shared.FabricJsModels
{
    public class TextBoxObject : FabricObject
    {
        public bool IsDynamic { get; set; } = false;
        public string FontFamily { get; set; } = "Arial";
        public string FontWeight { get; set; }

        [Range(1, 200)]
        public double FontSize { get; set; }

        public string Text { get; set; }
        public bool Underline { get; set; }
        public bool Overline { get; set; }
        public bool Linethrough { get; set; }

        [Required]
        [RegularExpression("left|center|right|justify", ErrorMessage = "Invalid text alignment.")]
        public string TextAlign { get; set; }

        [Required]
        [RegularExpression("normal|italic|oblique", ErrorMessage = "Invalid font style.")]
        public string FontStyle { get; set; }
        public double LineHeight { get; set; }
        public string TextBackgroundColor { get; set; }
        public int CharSpacing { get; set; }
        public List<object> Styles { get; set; }
        public string Direction { get; set; }
        public object Path { get; set; }
        public int PathStartOffset { get; set; }
        public string PathSide { get; set; }
        public string PathAlign { get; set; }
        public int MinWidth { get; set; }
        public bool SplitByGrapheme { get; set; }

    }
}
