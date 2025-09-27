namespace LabelDesigner.Shared.Widgets
{
    public class CanvasSettings
    {
        public double ViewWidth { get; set; } = 200;
        public double ViewHeight { get; set; } = 100;

        public string ViewBox => $"0 0 {ViewWidth} {ViewHeight}";
    }
}
