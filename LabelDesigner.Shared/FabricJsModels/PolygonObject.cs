namespace LabelDesigner.Shared.FabricJsModels
{
    public class PolygonObject : FabricObject
    {
        public List<Point> Points { get; set; }
    }

    public class Point
    {
        public double X { get; set; } 
        public double Y { get; set; }
    }
}
