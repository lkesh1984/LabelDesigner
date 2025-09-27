namespace LabelDesigner.Shared.Widgets
{
    public class LineWidget : ShapeWidget
    {
        public LineWidget(double width, double height, double x, double y) : base(width, height, x, y)
        {
        }

        public override WidgetType Type => WidgetType.Line;
    }
}
