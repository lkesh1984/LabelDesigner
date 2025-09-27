namespace LabelDesigner.Shared.FabricJsModels
{
    public class BarcodeObject : FabricObject
    {
        public string Src { get; set; }
        public double CropX { get; set; }
        public double CropY { get; set; }
        public List<object> Filters { get; set; }
        public string BarcodeType { get; set; }
    }
}
