namespace LabelDesigner.Shared.Widgets
{
    public class Template
    {
        public Guid SysId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int ModifiedBy { get; set; }
        public IList<Widget> Widgets { get; set; }
        public CanvasSettings CanvasSettings { get; set; }
    }
}
