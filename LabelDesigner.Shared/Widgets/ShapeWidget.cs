namespace LabelDesigner.Shared.Widgets
{
    public abstract class ShapeWidget : Widget
    {
        protected ShapeWidget(double width, double height, double x, double y)
        {
            Width = width;
            Height = height;
            X = x;
            Y = y;
        }

        // Appearance
        public string BorderColor { get; set; } = "#000000";
        public double BorderThickness { get; set; } = 1;

        public string StrokeColor { get; set; } = "black";

        // Child widgets inside this shape
        public List<Widget> Children { get; set; } = new();
    }
}
