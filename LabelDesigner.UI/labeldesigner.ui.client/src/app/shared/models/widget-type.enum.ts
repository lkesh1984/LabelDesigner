export enum WidgetType
{
  None,
  Text,       // Text elements usually on top or labels
  Line,       // Basic lines
  Rectangle,  // Rectangular shapes
  Circle,     // Circle shapes
  Ellipse,    // Elliptical shapes
  Oval,       // Another form of ellipse, similar use
  Triangle,   // Polygon shape
  Icon,       // Icon graphics (can be any composed SVG)
  Barcode,    // Specialized visual element (barcode)
  Image,       // Raster or embedded images, typically on bottom layer or background
  Custom       // Raster or embedded images, typically on bottom layer or background
}
