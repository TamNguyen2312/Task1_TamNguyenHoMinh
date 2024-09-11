using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Task1.BLL.DTOs.Response;
using Task1.BLL.DTOs.Store;
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

                if (getStoresDTO.StorId != null)
                {
                    stores = stores.Where(x => x.StorId.Equals(getStoresDTO.StorId));
                }

                if (getStoresDTO.StorName != null)
                {
                    stores = stores.Where(x => x.StorName.IndexOf(getStoresDTO.StorName, StringComparison.OrdinalIgnoreCase) >= 0);
                }

                if (getStoresDTO.StorAddress != null)
                {
                    stores = stores.Where(x => x.StorAddress.IndexOf(getStoresDTO.StorAddress, StringComparison.OrdinalIgnoreCase) >= 0);
                }

                if (getStoresDTO.City != null)
                {
                    stores = stores.Where(x => x.City.IndexOf(getStoresDTO.City, StringComparison.OrdinalIgnoreCase) >= 0);
                }

                if (getStoresDTO.State != null)
                {
                    stores = stores.Where(x => x.State.IndexOf(getStoresDTO.State, StringComparison.OrdinalIgnoreCase) >= 0);
                }

                if (getStoresDTO.Zip != null)
                {
                    stores = stores.Where(x => x.Zip.IndexOf(getStoresDTO.Zip, StringComparison.OrdinalIgnoreCase) >= 0);
                }

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
            finally
            {
                unitOfWork.Dispose();
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
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<ResponseApiDTO> CreateStoreAsync(StoreCreateRequestDTO storeRequest)
        {
            try
            {
                using var transaction = unitOfWork.BeginTransactionAsync();

                var storeRepo = unitOfWork.GetRepo<Store>();

                string storeId;
                do
                {
                    storeId = AutoGenerateStoreId();
                } while (await storeRepo.GetSingle(s => s.StorId == storeId) != null);

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
                        Result = createResult
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


        private string AutoGenerateStoreId()
        {
            Random random = new Random();
            int number = random.Next(1000, 10000); // Tạo số ngẫu nhiên từ 1000 đến 9999
            return number.ToString();
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
