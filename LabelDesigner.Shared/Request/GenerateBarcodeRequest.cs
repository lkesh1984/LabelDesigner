using System.ComponentModel.DataAnnotations;

namespace LabelDesigner.Shared.Request
{
    /// <summary>
    /// Request model to generate barcode
    /// </summary>
    public class GeneratePdfRequest
    {
        /// <summary>
        /// Gets or Sets template widget's values with input type
        /// </summary>
        [Required(ErrorMessage = "Template Widgets cannot be empty")]
        public List<TemplateWidget> TemplateWidgets { get; set;}
    }
}
