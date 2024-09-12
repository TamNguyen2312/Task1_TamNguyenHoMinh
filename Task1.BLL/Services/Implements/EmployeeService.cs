using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Task1.BLL.DTOs.EmployeeDTOs;
using Task1.BLL.DTOs.Response;
using Task1.BLL.DTOs.StoreDTOs;
using Task1.BLL.Helper.Extension.Employees;
using Task1.BLL.Helper.Paging;
using Task1.BLL.Services.Interfaces;
using Task1.DAL.Entities;
using Task1.DAL.Repositories;

namespace Task1.BLL.Services.Implements
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public static int PAGE_SIZE { get; set; } = 7;

        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<ResponseApiDTO> GetAllEmployeeAsync(GetEmpDTO getEmpDTO, int page)
        {
            try
            {
                var emps = await unitOfWork.GetRepo<Employee>().GetAllAsync(null, x => x.OrderBy(r => r.Fname), false);

                emps = emps.ApplyFilter(getEmpDTO);

                var results = mapper.Map<List<EmpViewDTO>>(emps);

                var pageResults = PaginatedList<EmpViewDTO>.Create(results, page, PAGE_SIZE);

                if (pageResults.IsNullOrEmpty())
                {
                    return new ResponseApiDTO
                    {
                        IsSuccess = true,
                        ErrorMessage = new List<string> { $"Employee not found" },
                        StatusCode = HttpStatusCode.OK,
                        Result = null
                    };
                }

                return new ResponseApiDTO
                {

                    IsSuccess = true,
                    ErrorMessage = null,
                    StatusCode = HttpStatusCode.OK,
                    Result = pageResults
                };
            }
            catch (Exception ex)
            {
                return new ResponseApiDTO
                {
                    IsSuccess = false,
                    ErrorMessage = new List<string> { $"Errors occur", ex.Message.ToString() },
                    StatusCode = HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<ResponseApiDTO> GetEmployeeByIdAsync(string id)
        {
            try
            {
                var emp = await unitOfWork.GetRepo<Employee>().GetSingle(x => x.EmpId.Equals(id), null, false);

                if (emp == null)
                {
                    return new ResponseApiDTO
                    {
                        IsSuccess = false,
                        ErrorMessage = new List<string> { $"Employee not found" },
                        StatusCode = HttpStatusCode.NotFound,
                        Result = null
                    };
                }
                else
                {
                    return new ResponseApiDTO
                    {
                        IsSuccess = true,
                        ErrorMessage = null,
                        StatusCode = HttpStatusCode.OK,
                        Result = mapper.Map<EmpViewDTO>(emp)
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseApiDTO
                {
                    IsSuccess = false,
                    ErrorMessage = new List<string> { $"Errors occur", ex.Message.ToString() },
                    StatusCode = HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
