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
        Task<ResponseApiDTO> GetAllEmployeeAsync(string? search, int page);
        Task<ResponseApiDTO> GetEmployeeByIdAsync(string id);
        Task<ResponseApiDTO> CreateEmployeeAsync(EmpCreateRequestDTO empCreateRequest);
        Task<ResponseApiDTO> UpdateEmployeeAsync(string id, EmpUpdateRequestDTO empUpdateRequest);
        Task<ResponseApiDTO> DeleteEmployeeAsync(string id);
    }
}
