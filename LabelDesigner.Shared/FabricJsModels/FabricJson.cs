namespace LabelDesigner.Shared.FabricJsModels
{
    public class FabricJson
    {
        public string Version { get; set; }
        public List<FabricObject> Objects { get; set; }
        public string Background { get; set; }
        public int idCounters { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}
