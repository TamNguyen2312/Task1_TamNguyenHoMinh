using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1.BLL.DTOs.EmployeeDTOs;
using Task1.DAL.Entities;

namespace Task1.BLL.Helper.Extension.Employees
{
    public static class EmpExtensions
    {
        public static IEnumerable<Employee> ApplyFilter(this IEnumerable<Employee> employees, GetEmpDTO filters)
        {
            return employees.Where(emp =>
                (string.IsNullOrEmpty(filters.EmpId) || emp.EmpId.Equals(filters.EmpId, StringComparison.OrdinalIgnoreCase)) &&

                (string.IsNullOrEmpty(filters.Fname) || emp.Fname.IndexOf(filters.Fname, StringComparison.OrdinalIgnoreCase) >= 0) &&

                (string.IsNullOrEmpty(filters.Minit) || emp.Minit.Equals(filters.Minit, StringComparison.OrdinalIgnoreCase)) &&

                (string.IsNullOrEmpty(filters.Lname) || emp.Lname.IndexOf(filters.Lname, StringComparison.OrdinalIgnoreCase) >= 0) &&

                (!filters.JobId.HasValue || emp.JobId == filters.JobId) &&

                (!filters.FromJobLvl.HasValue || emp.JobLvl >= filters.FromJobLvl) &&

                (!filters.ToJobLvl.HasValue || emp.JobLvl <= filters.ToJobLvl) &&

                (string.IsNullOrEmpty(filters.PubId) || emp.PubId.Equals(filters.PubId, StringComparison.OrdinalIgnoreCase)) &&

                (!filters.HireDate.HasValue || emp.HireDate.Date == filters.HireDate.Value.Date)
            );
        }

        public static string AutoGenerateEmpId(EmpCreateRequestDTO empCreateRequestDTO)
        {
            // Lấy ký tự đầu của Fname và ký tự đầu của Lname
            char firstChar = char.ToUpper(empCreateRequestDTO.Fname[0]);
            char thirdChar = char.ToUpper(empCreateRequestDTO.Lname[0]);

            // Lấy ký tự đầu của Minit, nếu null hoặc empty thì gán là '-'
            char secondChar = !string.IsNullOrEmpty(empCreateRequestDTO.Minit) ? char.ToUpper(empCreateRequestDTO.Minit[0]) : '-';

            // Ký tự thứ tư là một chữ số từ 1 đến 9
            Random random = new Random();
            int fourthDigit = random.Next(1, 10);

            // Bốn ký tự tiếp theo là các chữ số (0-9)
            int nextFourDigits = random.Next(1000, 10000);

            // Lấy ký tự cuối cùng theo Gender
            string genderSuffix = empCreateRequestDTO.Gender == EmpGender.Male.ToString() ? "M" : "F";

            // Kết hợp thành chuỗi EmpId
            return $"{firstChar}{secondChar}{thirdChar}{fourthDigit}{nextFourDigits}{genderSuffix}";
        }
    }
}
