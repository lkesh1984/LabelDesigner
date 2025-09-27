namespace LabelDesigner.Shared.Widgets
{
    public class CircleWidget : ShapeWidget
    {
        public CircleWidget(double width, double height, double x, double y) : base(width, height, x, y)
        {

        }

        public override WidgetType Type { get => WidgetType.Circle; }
    }
}
