using LabelDesigner.Shared.FabricJsModels;

namespace LabelDesigner.Shared.Widgets
{
    public class TemplateNew
    {
        public string SysId { get; set; }
        public string VersionId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int ModifiedBy { get; set; }
        public string FabricJson { get; set; }
        public string CanvasSettings { get; set; }
        public string TemplateSvg { get; set; }
    }
}
