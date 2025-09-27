namespace LabelDesigner.Shared
{
    // Base widget types
    public enum WidgetType
    {
        Text,       // Text elements usually on top or labels
        Line,       // Basic lines
        Rectangle,  // Rectangular shapes
        Circle,     // Circle shapes
        Ellipse,    // Elliptical shapes
        Oval,       // Another form of ellipse, similar use
        Triangle,   // Polygon shape
        Icon,       // Icon graphics (can be any composed SVG)
        Barcode,    // Specialized visual element (barcode)
        Image       // Raster or embedded images, typically on bottom layer or background
    }

    // Specific shapes for ShapeWidget
    public enum ShapeType
    {
        Rectangle,
        Circle,
        Ellipse,
        Oval,
        Triangle
    }
    public enum BarcodeType
    {
        Code128,
        QRCode,
        EAN13,
        EAN8,
        Code39,
        Code93,
        PDF417,
        DataMatrix
    }
    public enum WidgetMode
    {
        ReadOnly,
        Input
    }

    public static class FabricJsShapeType
    {
        public const string TextBox = "textbox";
        public const string Circle = "circle";
        public const string Line = "line";
        public const string Image = "image";
        public const string Polygon = "polygon";
        public const string Rectangle = "rect";
        public const string Group = "group";
        public const string Barcode = "barcode";
    }
}
