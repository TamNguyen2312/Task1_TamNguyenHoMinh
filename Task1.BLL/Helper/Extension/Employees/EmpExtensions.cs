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
    }
}
