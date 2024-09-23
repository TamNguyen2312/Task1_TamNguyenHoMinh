using AutoMapper;
using Task1.BLL.DTOs.StoreDTOs;
using Task1.BLL.Helper.Extension.Stores;
using Task1.BLL.Services.Interfaces;
using Task1.DAL.Entities;
using Task1.DAL.Repositories;
using Task1.Util.Queries;

namespace Task1.BLL.Services.Implements
{
	public class StoreService : BaseServices<Store, StoreDetailDTO, StoreCreateRequestDTO, StoreUpdateRequestDTO>, IStoreService
	{
		private readonly IUnitOfWork unitOfWork;
		private readonly IMapper mapper;

		public new static int PAGE_SIZE { get; set; } = 2;

		public StoreService(IUnitOfWork unitOfWork, IMapper mapper) 
				: base(unitOfWork, mapper)
		{
			this.unitOfWork = unitOfWork;
			this.mapper = mapper;
		}

		protected override QueryBuilder<Store> CreateQueryBuilder(string? search = null)
		{
			return base.CreateQueryBuilder(search)
					   .WithInclude(s => s.Sales)
					   .WithOrderBy(x => x.OrderBy(r => r.StorName)); 
		}

		public async Task<StoreListViewDTO> GetAllStoreAsync(string? search, int page)
		{
			try
			{
				var response = await base.GetAllAsync(search, page);
				return new StoreListViewDTO
				{
					StoreDetailDTOs = response,
					CurrentPage = page,
					TotalPages = response.TotalPages,
					TotalItems = response.TotalItems
				};
			}
			catch (Exception)
			{
				return null;
				throw;
			}
		}


		public async Task<StoreDetailDTO> GetStoreByIdAsync(string id)
		{
			try
			{
				var queryBuilder = base.CreateQueryBuilder()
						.WithInclude(s => s.Sales)
						.WithPredicate(s => s.StorId.Equals(id));
				var response = await base.GetByIdAsync(queryBuilder);
				return response;
			}
			catch (Exception)
			{
				return null;
				throw;
			}
		}

		public async Task<StoreDetailDTO> CreateStoreAsync(StoreCreateRequestDTO storeRequest)
		{

			try
			{
				await unitOfWork.BeginTransactionAsync();
				var storeRepo = unitOfWork.GetRepo<Store>();

				string storeId;
				do
				{
					storeId = StoreExtensions.AutoGenerateStoreId();
				} while ((await storeRepo.GetSingleAsync(base.CreateQueryBuilder().WithPredicate(x => x.StorId.Equals(storeId)).Build())) != null);

				var store = mapper.Map<Store>(storeRequest);
				store.StorId = storeId;

				var createResult = await storeRepo.CreateAsync(store);
				await unitOfWork.SaveChangesAsync();
				await unitOfWork.CommitTransactionAsync();

				return _mapper.Map<StoreDetailDTO>(createResult);
			}
			catch (Exception)
			{
				await unitOfWork.RollBackAsync();
				return null;
				throw;
			}
		}


		public async Task<StoreDetailDTO> UpdateStoreAsync(string id, StoreUpdateRequestDTO storeRequest)
		{

			try
			{
				var queryBuilder = base.CreateQueryBuilder().WithPredicate(s => s.StorId.Equals(id)).WithTracking(true);
				var response = await base.UpdateAsync(queryBuilder, storeRequest);
				return response;
			}
			catch (Exception)
			{
				return null;
				throw;
			}
		}

		public async Task<bool> DeletetoreAsync(string id)
		{

			try
			{
				var queryBuilder = base.CreateQueryBuilder().WithPredicate(s => s.StorId.Equals(id)).WithInclude(s => s.Sales);
				var response = await base.DeleteAsync(queryBuilder);
				if(!response) return false;
				return true;
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
