namespace LabelDesigner.Shared.FabricJsModels
{
    public class ImageObject : FabricObject
    {
        public string Src { get; set; }
        public double CropX { get; set; }
        public double CropY { get; set; }
        public List<object> Filters { get; set; }
    }
}
