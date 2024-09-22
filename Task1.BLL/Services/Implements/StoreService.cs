using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Task1.BLL.DTOs.Response;
using Task1.BLL.DTOs.StoreDTOs;
using Task1.BLL.Helper.Extension.Stores;
using Task1.BLL.Helper.Paging;
using Task1.BLL.Services.Interfaces;
using Task1.DAL.Entities;
using Task1.DAL.Repositories;

namespace Task1.BLL.Services.Implements
{
    public class StoreService : IStoreService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public static int PAGE_SIZE { get; set; } = 2;

        public StoreService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<ResponseApiDTO> GetAllStoreAsync(GetStoresDTO getStoresDTO, int page)
        {
            try
            {
                var stores = await unitOfWork.GetRepo<Store>().GetAllAsync(null,
                                                                        r => r.OrderBy(q => q.StorName),
                                                                        false);

                stores = stores.ApplyFilters(getStoresDTO);

                var results = mapper.Map<List<StoreViewDTO>>(stores);

                var pageResults = PaginatedList<StoreViewDTO>.Create(results, page, PAGE_SIZE);

                if (pageResults.IsNullOrEmpty())
                {
                    return new ResponseApiDTO
                    {
                        IsSuccess = true,
                        ErrorMessage = new List<string> { $"Store not found" },
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

        public async Task<ResponseApiDTO> GetStoreByIdAsync(string id)
        {
            try
            {
                var store = await unitOfWork.GetRepo<Store>().GetSingle(x => x.StorId.Equals(id), null, false);

                if (store == null)
                {
                    return new ResponseApiDTO
                    {
                        IsSuccess = false,
                        ErrorMessage = new List<string> { $"Store not found" },
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
                        Result = mapper.Map<StoreViewDTO>(store)
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

        public async Task<ResponseApiDTO> CreateStoreAsync(StoreCreateRequestDTO storeRequest)
        {
            await unitOfWork.BeginTransactionAsync();
            try
            {
                var storeRepo = unitOfWork.GetRepo<Store>();

                string storeId;
                do
                {
                    storeId = StoreExtensions.AutoGenerateStoreId();
                } while ((await storeRepo.GetSingle(s => s.StorId.Equals(storeId))) != null);

                var store = mapper.Map<Store>(storeRequest);
                store.StorId = storeId;
                 
                var createResult = await unitOfWork.GetRepo<Store>().CreateAsync(store);
                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitTransactionAsync();

                if(createResult == null)
                {
                    return new ResponseApiDTO
                    {
                        IsSuccess = false,
                        ErrorMessage = new List<string> { "Created store failed" },
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
                        Result = mapper.Map<StoreViewDTO>(createResult)
                    };
                }
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


        public async Task<ResponseApiDTO> UpdateStoreAsync(string id, StoreUpdateRequestDTO storeRequest)
        {
            try
            {
                using var transaction = unitOfWork.BeginTransactionAsync();

                var storeRepo = unitOfWork.GetRepo<Store>();

                var store = await storeRepo.GetSingle(x => x.StorId.Equals(id), null, false);

                if (store == null)
                {
                    return new ResponseApiDTO
                    {
                        IsSuccess = false,
                        ErrorMessage = new List<string> { "Store cannot found" },
                        StatusCode = HttpStatusCode.NotFound,
                        Result = null
                    };
                }

                var storeUpdate = mapper.Map(storeRequest, store);

                await storeRepo.UpdateAsync(storeUpdate);

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

        public async Task<ResponseApiDTO> DeletetoreAsync(string id)
        {
            try
            {
                using var transaction = unitOfWork.BeginTransactionAsync();

                var storeRepo = unitOfWork.GetRepo<Store>();

                var store = await storeRepo.GetSingle(x => x.StorId.Equals(id), null, false, r => r.Sales);

                if (store == null)
                {
                    return new ResponseApiDTO
                    {
                        IsSuccess = false,
                        ErrorMessage = new List<string> { "Store cannot found" },
                        StatusCode = HttpStatusCode.NotFound,
                        Result = null
                    };
                }

                if(store.Sales.Any())
                {
                    return new ResponseApiDTO
                    {
                        IsSuccess = false,
                        ErrorMessage = new List<string> { "Cannot delete store that have any Sales" },
                        StatusCode = HttpStatusCode.BadRequest,
                        Result = null
                    };
                }

                await storeRepo.DeleteAsync(store);

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
