using System.ComponentModel.DataAnnotations;

namespace LabelDesigner.Shared.FabricJsModels
{
    public class CircleObject : FabricObject
    {
        [Range(0.1, double.MaxValue, ErrorMessage = "Radius must be > 0.")]
        public double Radius { get; set; }

        [Range(0, 360, ErrorMessage = "StartAngle can be from 0 to 360")]
        public double StartAngle { get; set; }

        [Range(0, 360, ErrorMessage = "EndAngle can be from 0 to 360")]
        public double EndAngle { get; set; }
    }
}
