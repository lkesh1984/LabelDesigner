using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LabelDesigner.Shared.FabricJsModels
{
    /// <summary>
    /// Json converter for Fabricjs objects.
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    public class FabricObjectConverter  : JsonConverter
    {
        const string TypeString = "Type";
        const string TypeString1 = "type";
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(FabricObject);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);

            string type1 = jo[TypeString1]?.ToString()?.ToLower();
            string type = jo[TypeString]?.ToString()?.ToLower();
            string customType = jo["CustomType"]?.ToString()?.ToLower();

            // Determine the type based on the "type" property
            if (type1 == null && type == null)
            {
                return null;
            }

            FabricObject fabricObject;
            switch (type1)
            {
                case FabricJsShapeType.TextBox:
                    fabricObject = new TextBoxObject();
                    break;
                case FabricJsShapeType.Circle:
                    fabricObject = new CircleObject();
                    break;
                case FabricJsShapeType.Line:
                    fabricObject = new LineObject();
                    break;
                case FabricJsShapeType.Image:
                    if (customType == FabricJsShapeType.Barcode)
                    {
                        fabricObject = new BarcodeObject();
                    }
                    else
                    {
                        fabricObject = new ImageObject();
                    }
                    break;
                case FabricJsShapeType.Polygon:
                    fabricObject = new PolygonObject();
                    break;
                case FabricJsShapeType.Rectangle:
                    fabricObject = new RectangleObject();
                    break;
                case FabricJsShapeType.Group:
                    if (customType == FabricJsShapeType.Barcode)
                    {
                        fabricObject = new BarcodeObject();
                    }
                    else
                    {
                        fabricObject = new GroupObject();
                    }
                    break;
                case FabricJsShapeType.Barcode:
                    fabricObject = new BarcodeObject();
                    break;
                default:
                    fabricObject = new FabricObject();
                    break;
            }

            serializer.Populate(jo.CreateReader(), fabricObject);
            return fabricObject;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is FabricObject fabricObject)
            {
                // Start writing object
                writer.WriteStartObject();

                // Always write type first
                writer.WritePropertyName("type");
                writer.WriteValue(fabricObject.Type);

                // Always write Id
                writer.WritePropertyName("id");
                writer.WriteValue(fabricObject.Id);

                // Common FabricObject properties
                writer.WritePropertyName("version");
                writer.WriteValue(fabricObject.Version);

                writer.WritePropertyName("left");
                writer.WriteValue(fabricObject.Left);

                writer.WritePropertyName("top");
                writer.WriteValue(fabricObject.Top);

                writer.WritePropertyName("width");
                writer.WriteValue(fabricObject.Width);

                writer.WritePropertyName("height");
                writer.WriteValue(fabricObject.Height);

                writer.WritePropertyName("fill");
                writer.WriteValue(fabricObject.Fill);

                writer.WritePropertyName("stroke");
                writer.WriteValue(fabricObject.Stroke);

                writer.WritePropertyName("strokeWidth");
                writer.WriteValue(fabricObject.StrokeWidth);

                writer.WritePropertyName("opacity");
                writer.WriteValue(fabricObject.Opacity);

                writer.WritePropertyName("angle");
                writer.WriteValue(fabricObject.Angle);

                writer.WritePropertyName("scaleX");
                writer.WriteValue(fabricObject.ScaleX);

                writer.WritePropertyName("scaleY");
                writer.WriteValue(fabricObject.ScaleY);

                writer.WritePropertyName("flipX");
                writer.WriteValue(fabricObject.FlipX);

                writer.WritePropertyName("flipY");
                writer.WriteValue(fabricObject.FlipY);

                writer.WritePropertyName("visible");
                writer.WriteValue(fabricObject.Visible);

                writer.WritePropertyName("backgroundColor");
                writer.WriteValue(fabricObject.BackgroundColor);

                // Additional properties per subclass
                switch (fabricObject.Type)
                {
                    case FabricJsShapeType.TextBox:
                        var textBox = fabricObject as TextBoxObject;
                        writer.WritePropertyName("text");
                        writer.WriteValue(textBox.Text);
                        writer.WritePropertyName("fontFamily");
                        writer.WriteValue(textBox.FontFamily);
                        writer.WritePropertyName("fontSize");
                        writer.WriteValue(textBox.FontSize);
                        writer.WritePropertyName("textAlign");
                        writer.WriteValue(textBox.TextAlign);
                        writer.WritePropertyName("fontStyle");
                        writer.WriteValue(textBox.FontStyle);
                        writer.WritePropertyName("underline");
                        writer.WriteValue(textBox.Underline);
                        writer.WritePropertyName("linethrough");
                        writer.WriteValue(textBox.Linethrough);
                        break;

                    case FabricJsShapeType.Circle:
                        var circle = fabricObject as CircleObject;
                        writer.WritePropertyName("radius");
                        writer.WriteValue(circle.Radius);
                        writer.WritePropertyName("startAngle");
                        writer.WriteValue(circle.StartAngle);
                        writer.WritePropertyName("endAngle");
                        writer.WriteValue(circle.EndAngle);
                        break;

                    case FabricJsShapeType.Rectangle:
                        var rect = fabricObject as RectangleObject;
                        writer.WritePropertyName("rx");
                        writer.WriteValue(rect.Rx);
                        writer.WritePropertyName("ry");
                        writer.WriteValue(rect.Ry);
                        break;

                    case FabricJsShapeType.Polygon:
                        var polygon = fabricObject as PolygonObject;
                        writer.WritePropertyName("points");
                        serializer.Serialize(writer, polygon.Points);
                        break;

                    case FabricJsShapeType.Line:
                        var line = fabricObject as LineObject;
                        writer.WritePropertyName("x1");
                        writer.WriteValue(line.X1);
                        writer.WritePropertyName("y1");
                        writer.WriteValue(line.Y1);
                        writer.WritePropertyName("x2");
                        writer.WriteValue(line.X2);
                        writer.WritePropertyName("y2");
                        writer.WriteValue(line.Y2);
                        break;

                    case FabricJsShapeType.Image:
                        var image = fabricObject as ImageObject;
                        writer.WritePropertyName("src");
                        writer.WriteValue(image.Src);
                        writer.WritePropertyName("cropX");
                        writer.WriteValue(image.CropX);
                        writer.WritePropertyName("cropY");
                        writer.WriteValue(image.CropY);
                        writer.WritePropertyName("filters");
                        serializer.Serialize(writer, image.Filters);
                        break;

                    case FabricJsShapeType.Group:
                        var group = fabricObject as GroupObject;
                        writer.WritePropertyName("objects");
                        serializer.Serialize(writer, group.Objects);
                        break;
                }

                writer.WriteEndObject();
            }
            else
            {
                throw new JsonSerializationException($"Unexpected type: {value.GetType()}");
            }
        }
    }
}
