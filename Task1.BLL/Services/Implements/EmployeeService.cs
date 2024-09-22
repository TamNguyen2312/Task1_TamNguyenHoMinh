using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using Task1.BLL.DTOs.EmployeeDTOs;
using Task1.BLL.DTOs.Response;
using Task1.BLL.Helper.Extension.Employees;
using Task1.BLL.Helper.Paging;
using Task1.BLL.Services.Interfaces;
using Task1.DAL.Entities;
using Task1.DAL.Repositories;
using Task1.Util.Filters;
using Task1.Util.Queries;

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
        public async Task<ResponseApiDTO> GetAllEmployeeAsync(string? search, int page)
        {
            try
            {
				var empRepo = unitOfWork.GetRepo<Employee>();

                var queryBuilder = new QueryBuilder<Employee>()
                                    .WithOrderBy(x => x.OrderBy(r => r.Fname))
                                    .WithTracking(false)
                                    .WithInclude(x => x.Pub, r => r.Job);

				if (!string.IsNullOrEmpty(search))
				{
					var predicate = FilterHelper.BuildSearchExpression<Employee>(search);
					queryBuilder.WithPredicate(predicate);
				}

                var queryOptions = queryBuilder.Build();

				var emps = await unitOfWork.GetRepo<Employee>().GetAllAsync(queryOptions);

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
        }

        public async Task<ResponseApiDTO> GetEmployeeByIdAsync(string id)
        {
            try
            {
				var empRepo = unitOfWork.GetRepo<Employee>();

				var queryOptions = new QueryBuilder<Employee>()
								   .WithPredicate(x => x.EmpId.Equals(id))
								   .WithTracking(false)
								   .Build();

				var emp = await empRepo.GetSingleAsync(queryOptions);

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
        }

        public async Task<ResponseApiDTO> CreateEmployeeAsync(EmpCreateRequestDTO empCreateRequest)
        {
			
			try
            {
				await unitOfWork.BeginTransactionAsync();
				var empRepo = unitOfWork.GetRepo<Employee>();

				var existedJob = await unitOfWork.GetRepo<Job>().GetSingleAsync(new QueryBuilder<Job>()
                                                                                .WithPredicate(x => x.JobId == (empCreateRequest.JobId))
                                                                                .WithTracking(false)
                                                                                .Build());
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

                var existedPublisher = await unitOfWork.GetRepo<Publisher>().GetSingleAsync(new QueryBuilder<Publisher>()
																				.WithPredicate(x => x.PubId.Equals(empCreateRequest.PubId))
																				.WithTracking(false)
																				.Build());
                if (existedPublisher == null)
                {
                    empCreateRequest.PubId = "9952";
                }

                string empId;
                do
                {
                    empId = EmpExtensions.AutoGenerateEmpId(empCreateRequest);
                } while (await empRepo.GetSingleAsync(new QueryBuilder<Employee>()
														.WithPredicate(x => x.EmpId.Equals(empId))
													    .WithTracking(false)
													    .Build()) != null);

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
				await unitOfWork.BeginTransactionAsync();

				var empRepo = unitOfWork.GetRepo<Employee>();

                var existedJob = await unitOfWork.GetRepo<Job>().GetSingleAsync(new QueryBuilder<Job>()
														.WithPredicate(x => x.JobId == (empUpdateRequest.JobId))
														.WithTracking(false)
														.Build());
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

				var existedPublisher = await unitOfWork.GetRepo<Publisher>().GetSingleAsync(new QueryBuilder<Publisher>()
																				 .WithPredicate(x => x.PubId.Equals(empUpdateRequest.PubId))
																				 .WithTracking(false)
																				 .Build());
				if (existedPublisher == null)
                {
                    empUpdateRequest.PubId = "9952";
                }

                var emp = await empRepo.GetSingleAsync(new QueryBuilder<Employee>()
														.WithPredicate(x => x.EmpId.Equals(id))
														.WithTracking(false)
														.Build());

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
				await unitOfWork.BeginTransactionAsync();

				var empRepo = unitOfWork.GetRepo<Employee>();
                var queryOptions = new QueryBuilder<Employee>()
                                        .WithPredicate(x => x.EmpId.Equals(id))
                                        .WithTracking(false)
                                        .WithInclude(x => x.Job, x => x.Pub)
                                        .Build();

                var emp = await empRepo.GetSingleAsync(queryOptions);

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
