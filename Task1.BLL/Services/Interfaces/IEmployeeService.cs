using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1.BLL.DTOs.EmployeeDTOs;
using Task1.BLL.DTOs.Response;

namespace Task1.BLL.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<ResponseApiDTO> GetAllEmployeeAsync(GetEmpDTO getEmpDTO, int page);
        Task<ResponseApiDTO> GetEmployeeByIdAsync(string id);
    }
}
