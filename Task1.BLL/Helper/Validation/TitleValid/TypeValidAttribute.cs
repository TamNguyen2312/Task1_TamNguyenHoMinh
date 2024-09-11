using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1.BLL.Helper.Validation.TitleValid
{
    public class TypeValidAttribute : ValidationAttribute
    {
        private readonly Type _enumType;

        public TypeValidAttribute(Type enumTitleType)
        {
            _enumType = enumTitleType;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string strValue && Enum.IsDefined(_enumType, strValue))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult($"The field {validationContext.DisplayName} must be one of the following values: {string.Join(", ", Enum.GetNames(_enumType))}.");
        }
    }
}
