using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1.BLL.Helper.Validation.EmpValid
{
    public class EmpGenderValidAttribute : ValidationAttribute
    {
        private readonly Type _enumType;

        public EmpGenderValidAttribute(Type empGender)
        {
            if (!empGender.IsEnum)
            {
                throw new ArgumentException("Type must be an enum");
            }
            _enumType = empGender;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || !Enum.IsDefined(_enumType, value))
            {
                return new ValidationResult($"The field {validationContext.DisplayName} must be one of the following values: {string.Join(", ", Enum.GetNames(_enumType))}.");
            }
            return ValidationResult.Success;
        }
    }
}
