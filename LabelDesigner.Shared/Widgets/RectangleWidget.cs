namespace LabelDesigner.Shared.Widgets
{
    public class RectangleWidget : ShapeWidget
    {
        public RectangleWidget(double width = 100, double height = 50, double x = 20, double y = 30) : base(width, height, x, y)
        {
        }

        public override WidgetType Type => WidgetType.Rectangle;
    }
}
