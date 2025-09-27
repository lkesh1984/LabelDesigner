using System.ComponentModel.DataAnnotations;

namespace LabelDesigner.Shared.FabricJsModels
{
    internal class FabricJsonValidator
    {
        public static IList<ValidationResult> IsValidJson(FabricObject fabricObject)
        {
            ValidationContext context = new ValidationContext(fabricObject, serviceProvider: null, items: null);
            IList<ValidationResult> validationResult = new List<ValidationResult>();
            Validator.TryValidateObject(fabricObject, context, validationResult, true);

            return validationResult;
        }
    }
}
