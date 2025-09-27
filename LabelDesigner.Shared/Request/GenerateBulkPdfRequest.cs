using System.ComponentModel.DataAnnotations;

namespace LabelDesigner.Shared.Request
{
    public class GenerateBulkPdfRequest
    {
        /// <summary>
        /// Gets or Sets Template Id
        /// </summary>
        [Required(ErrorMessage = "TemplateId must be provided.")]
        public string TemplateId { get; set; }

        public int Height { get; set; } = 500;
        public int Width { get; set; } = 800;

        /// <summary>
        /// Gets or Sets template widget's values with input type.
        /// </summary>
        [Required(ErrorMessage = "Template Widgets cannot be empty")]
        public List<GeneratePdfRequest> Records { get; set; }
    }
}
