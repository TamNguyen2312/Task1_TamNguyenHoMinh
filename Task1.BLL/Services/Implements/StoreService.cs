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
using Task1.Util.Filters;
using Task1.Util.Queries;

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
		public async Task<ResponseApiDTO> GetAllStoreAsync(string? search, int page)
		{
			try
			{
				var storeRepo = unitOfWork.GetRepo<Store>();

				var queryBuilder = new QueryBuilder<Store>()
									.WithOrderBy(x => x.OrderBy(r => r.StorName))
									.WithTracking(false) 
									.WithInclude(r => r.Sales); 

				if (!string.IsNullOrEmpty(search))
				{
					var predicate = FilterHelper.BuildSearchExpression<Store>(search);
					queryBuilder.WithPredicate(predicate);
				}

				var queryOptions = queryBuilder.Build();

				var stores = await storeRepo.GetAllAsync(queryOptions);

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
					ErrorMessage = new List<string> { $"Errors occur", ex.Message },
					StatusCode = HttpStatusCode.InternalServerError,
					Result = null
				};
			}
		}


		public async Task<ResponseApiDTO> GetStoreByIdAsync(string id)
		{
			try
			{
				var storeRepo = unitOfWork.GetRepo<Store>();
				var queryOptions = new QueryBuilder<Store>()
								   .WithPredicate(x => x.StorId.Equals(id))
								   .WithTracking(false)
								   .WithInclude(r => r.Sales)
								   .Build();
				var store = await storeRepo.GetSingleAsync(queryOptions);

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

			try
			{
				await unitOfWork.BeginTransactionAsync();
				var storeRepo = unitOfWork.GetRepo<Store>();

				string storeId;
				do
				{
					storeId = StoreExtensions.AutoGenerateStoreId();
				} while ((await storeRepo.GetSingleAsync(new QueryBuilder<Store>().WithPredicate(x => x.StorId.Equals(storeId)).Build())) != null);

				var store = mapper.Map<Store>(storeRequest);
				store.StorId = storeId;

				var createResult = await storeRepo.CreateAsync(store);
				await unitOfWork.SaveChangesAsync();
				await unitOfWork.CommitTransactionAsync();

				if (createResult == null)
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
				await unitOfWork.BeginTransactionAsync();

				var storeRepo = unitOfWork.GetRepo<Store>();

				var queryOptions = new QueryBuilder<Store>()
					.WithPredicate(x => x.StorId.Equals(id))
					.WithTracking(true)
					.Build();

				var store = await storeRepo.GetSingleAsync(queryOptions);

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
				await unitOfWork.BeginTransactionAsync();
				var storeRepo = unitOfWork.GetRepo<Store>();

				var queryOptions = new QueryBuilder<Store>()
					.WithPredicate(x => x.StorId.Equals(id))
					.WithTracking(true)
					.WithInclude(x => x.Sales)
					.Build();

				var store = await storeRepo.GetSingleAsync(queryOptions);

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

				if (store.Sales.Any())
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
