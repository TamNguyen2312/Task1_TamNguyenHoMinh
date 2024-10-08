﻿using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using Task1.BLL.DTOs.EmployeeDTOs;
using Task1.BLL.DTOs.Response;
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

        public async Task<ResponseApiDTO> CreateEmployeeAsync(EmpCreateRequestDTO empCreateRequest)
        {
            try
            {
                using var transaction = unitOfWork.BeginTransactionAsync();

                var empRepo = unitOfWork.GetRepo<Employee>();

                var existedJob = await unitOfWork.GetRepo<Job>().GetSingle(x => x.JobId == empCreateRequest.JobId, null, false);
                if(existedJob == null)
                {
                    empCreateRequest.JobId = 1;
                    empCreateRequest.JobLvl = 10;
                }
                else
                {
                    if(empCreateRequest.JobLvl < existedJob.MinLvl || empCreateRequest.JobLvl > existedJob.MaxLvl)
                    {
                        return new ResponseApiDTO
                        {
                            IsSuccess = true,
                            ErrorMessage = new List<string> { $"Job Id {empCreateRequest.JobId} has the range of Job level from {existedJob.MinLvl} to {existedJob.MaxLvl}" },
                            StatusCode = HttpStatusCode.BadRequest,
                            Result = null
                        };
                    }
                }

                var existedPublisher = await unitOfWork.GetRepo<Publisher>().GetSingle(x => x.PubId.Equals(empCreateRequest.PubId), null, false);
                if (existedPublisher == null)
                {
                    empCreateRequest.PubId = "9952";
                }

                string empId;
                do
                {
                    empId = EmpExtensions.AutoGenerateEmpId(empCreateRequest);
                } while (await empRepo.GetSingle(s => s.EmpId == empId) != null);

                var emp = mapper.Map<Employee>(empCreateRequest);
                emp.EmpId = empId;
                emp.Minit = emp.Minit?.ToUpper();


                var createResult = await unitOfWork.GetRepo<Employee>().CreateAsync(emp);
                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitTransactionAsync();

                if (createResult == null)
                {
                    return new ResponseApiDTO
                    {
                        IsSuccess = false,
                        ErrorMessage = new List<string> { "Created employee failed" },
                        StatusCode = HttpStatusCode.BadRequest,
                        Result = null
                    };
                }
                else
                {
                    return new ResponseApiDTO
                    {
                        IsSuccess = true,
                        ErrorMessage = null,
                        StatusCode = HttpStatusCode.Created,
                        Result = mapper.Map<EmpViewDTO>(createResult)
                    };
                }
            }
            catch(Exception ex)
            {
                await unitOfWork.RollBackAsync();
                return new ResponseApiDTO
                {
                    IsSuccess = false,
                    ErrorMessage = new List<string> { "Errors occur", ex.Message.ToString() },
                    StatusCode = HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
        }

        public async Task<ResponseApiDTO> UpdateEmployeeAsync(string id, EmpUpdateRequestDTO empUpdateRequest)
        {
            try
            {
                using var transaction = unitOfWork.BeginTransactionAsync();

                var empRepo = unitOfWork.GetRepo<Employee>();

                var existedJob = await unitOfWork.GetRepo<Job>().GetSingle(x => x.JobId == empUpdateRequest.JobId, null, false);
                if (existedJob == null)
                {
                    empUpdateRequest.JobId = 1;
                    empUpdateRequest.JobLvl = 10;
                }
                else
                {
                    if (empUpdateRequest.JobLvl < existedJob.MinLvl || empUpdateRequest.JobLvl > existedJob.MaxLvl)
                    {
                        return new ResponseApiDTO
                        {
                            IsSuccess = true,
                            ErrorMessage = new List<string> { $"Job Id {empUpdateRequest.JobId} has the range of Job level from {existedJob.MinLvl} to {existedJob.MaxLvl}" },
                            StatusCode = HttpStatusCode.BadRequest,
                            Result = null
                        };
                    }
                }

                var existedPublisher = await unitOfWork.GetRepo<Publisher>().GetSingle(x => x.PubId.Equals(empUpdateRequest.PubId), null, false);
                if (existedPublisher == null)
                {
                    empUpdateRequest.PubId = "9952";
                }

                var emp = await empRepo.GetSingle(x => x.EmpId.Equals(id), null, false);

                if (emp == null)
                {
                    return new ResponseApiDTO
                    {
                        IsSuccess = false,
                        ErrorMessage = new List<string> { "Employee cannot found" },
                        StatusCode = HttpStatusCode.NotFound,
                        Result = null
                    };
                }

                var empUpdate = mapper.Map(empUpdateRequest, emp);
                empUpdate.Minit = empUpdate.Minit?.ToUpper();

                await empRepo.UpdateAsync(empUpdate);

                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitTransactionAsync();

                return new ResponseApiDTO
                {
                    IsSuccess = true,
                    ErrorMessage = null,
                    StatusCode = HttpStatusCode.NoContent,
                    Result = null
                };
            }
            catch (Exception ex)
            {
                await unitOfWork.RollBackAsync();
                return new ResponseApiDTO
                {
                    IsSuccess = false,
                    ErrorMessage = new List<string> { "Errors occur", ex.Message.ToString() },
                    StatusCode = HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
        }

        public async Task<ResponseApiDTO> DeleteEmployeeAsync(string id)
        {
            try
            {
                using var transaction = unitOfWork.BeginTransactionAsync();

                var empRepo = unitOfWork.GetRepo<Employee>();

                var emp = await empRepo.GetSingle(x => x.EmpId.Equals(id), null, false, r => r.Job, r => r.Pub);

                if (emp == null)
                {
                    return new ResponseApiDTO
                    {
                        IsSuccess = false,
                        ErrorMessage = new List<string> { "Employee cannot found" },
                        StatusCode = HttpStatusCode.NotFound,
                        Result = null
                    };
                }

                await empRepo.DeleteAsync(emp);

                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitTransactionAsync();

                return new ResponseApiDTO
                {
                    IsSuccess = true,
                    ErrorMessage = null,
                    StatusCode = HttpStatusCode.NoContent,
                    Result = null
                };
            }
            catch (Exception ex)
            {
                await unitOfWork.RollBackAsync();
                return new ResponseApiDTO
                {
                    IsSuccess = false,
                    ErrorMessage = new List<string> { "Errors occur", ex.Message.ToString() },
                    StatusCode = HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
        }
    }
}
