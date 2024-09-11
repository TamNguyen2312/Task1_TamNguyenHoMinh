using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Task1.BLL.Helper.Validation.Store
{
    public class ZipValidAttribute : ValidationAttribute
    {
        private readonly string _pattern = @"^\d{5}(-\d{4})?$"; // Mẫu ZIP cho định dạng 12345 hoặc 12345-6789

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Nếu giá trị null, không cần kiểm tra
            }

            string zipCode = value.ToString();

            if (Regex.IsMatch(zipCode, _pattern))
            {
                return ValidationResult.Success; // Mã ZIP hợp lệ
            }
            else
            {
                return new ValidationResult("Zip code is invalid");
            }
        }
    }
}
