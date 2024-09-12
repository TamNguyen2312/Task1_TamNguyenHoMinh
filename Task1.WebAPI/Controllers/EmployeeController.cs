using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task1.BLL.DTOs.EmployeeDTOs;
using Task1.BLL.Services.Interfaces;

namespace Task1.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        [HttpGet]
        [Route("GetAllEmployees")]
        public async Task<IActionResult> GetAllEmployeesAsync([FromQuery] GetEmpDTO getEmpDTO, [FromQuery] int page = 1)
        {
            var response = await employeeService.GetAllEmployeeAsync(getEmpDTO, page);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet]
        [Route("GetEmployeeById/{id}")]
        public async Task<IActionResult> GetEmployeeByIdAsync(string id)
        {
            var response = await employeeService.GetEmployeeByIdAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost]
        [Route("CreateEmployee")]
        public async Task<IActionResult> CreateEmployeeAsync([FromBody] EmpCreateRequestDTO empCreateRequest)
        {
            var response = await employeeService.CreateEmployeeAsync(empCreateRequest);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut]
        [Route("UpdateEmployee/{id}")]
        public async Task<IActionResult> UpdateEmployeeAsync(string id, EmpUpdateRequestDTO empUpdateRequest)
        {
            var response = await employeeService.UpdateEmployeeAsync(id, empUpdateRequest);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpDelete]
        [Route("DeleteEmployee/{id}")]
        public async Task<IActionResult> DeleteEmployeeAsync(string id)
        {
            var response = await employeeService.DeleteEmployeeAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
