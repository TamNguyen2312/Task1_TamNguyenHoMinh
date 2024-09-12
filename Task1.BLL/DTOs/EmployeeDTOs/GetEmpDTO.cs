using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1.BLL.DTOs.EmployeeDTOs
{
    public class GetEmpDTO
    {
        public string? EmpId { get; set; }

        public string? Fname { get; set; }

        public string? Minit { get; set; }

        public string? Lname { get; set; }

        public short? JobId { get; set; }

        public byte? FromJobLvl { get; set; }
        public byte? ToJobLvl { get; set; }

        public string? PubId { get; set; }

        public DateTime? HireDate { get; set; }
    }
}
