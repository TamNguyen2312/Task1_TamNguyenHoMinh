using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Task1.BLL.DTOs.Response;
using Task1.BLL.Helper.Paging;
using Task1.DAL.Repositories;
using Task1.Util.Filters;
using Task1.Util.Queries;

namespace Task1.BLL.Services
{
	public abstract class BaseServices<TEntity, TViewDto, TCreateDto, TUpdateDto> where TEntity : class
	{
		protected readonly IUnitOfWork _unitOfWork;
		protected readonly IMapper _mapper;
		public static int PAGE_SIZE { get; set; } = 10;
		public BaseServices(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		protected virtual QueryBuilder<TEntity> CreateQueryBuilder(string? search = null)
		{
			var queryBuilder = new QueryBuilder<TEntity>()
								.WithTracking(false);

			if (!string.IsNullOrEmpty(search))
			{
				var predicate = FilterHelper.BuildSearchExpression<TEntity>(search);
				queryBuilder.WithPredicate(predicate);
			}

			return queryBuilder;
		}

		public async Task<ResponseApiDTO> GetAllAsync(string? search, int page)
		{
			var queryBuilder = CreateQueryBuilder(search)
								.WithOrderBy(x => x.OrderBy(e => e.ToString()));  // Default order

			var queryOptions = queryBuilder.Build();
			var repo = _unitOfWork.GetRepo<TEntity>();
			var entities = await repo.GetAllAsync(queryOptions);

			var results = _mapper.Map<List<TViewDto>>(entities);
			var pageResults = PaginatedList<TViewDto>.Create(results, page, PAGE_SIZE);

			if (pageResults.IsNullOrEmpty())
			{
				return new ResponseApiDTO
				{
					IsSuccess = true,
					ErrorMessage = new List<string> { $"{typeof(TEntity).Name} not found" },
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

		public async Task<ResponseApiDTO> GetByIdAsync(string id)
		{
			var queryBuilder = CreateQueryBuilder()
								.WithPredicate(x => x.Equals(id));

			var repo = _unitOfWork.GetRepo<TEntity>();

			var queryOptions = queryBuilder.Build();

			var entity = await repo.GetSingleAsync(queryOptions);
			if (entity == null)
			{
				return new ResponseApiDTO
				{
					IsSuccess = false,
					ErrorMessage = new List<string> { $"{typeof(TEntity).Name} not found" },
					StatusCode = HttpStatusCode.NotFound,
					Result = null
				};
			}

			return new ResponseApiDTO
			{
				IsSuccess = true,
				ErrorMessage = null,
				StatusCode = HttpStatusCode.OK,
				Result = _mapper.Map<TViewDto>(entity)
			};
		}

		public async Task<ResponseApiDTO> CreateAsync(TCreateDto createDto)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var repo = _unitOfWork.GetRepo<TEntity>();
				var entity = _mapper.Map<TEntity>(createDto);
				var createResult = await repo.CreateAsync(entity);
				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				if (createResult == null)
				{
					return new ResponseApiDTO
					{
						IsSuccess = false,
						ErrorMessage = new List<string> { $"Created {typeof(TEntity).Name} failed" },
						StatusCode = HttpStatusCode.BadRequest,
						Result = null
					};
				}

				return new ResponseApiDTO
				{
					IsSuccess = true,
					ErrorMessage = null,
					StatusCode = HttpStatusCode.Created,
					Result = _mapper.Map<TViewDto>(createResult)
				};
			}
			catch (Exception ex)
			{
				await _unitOfWork.RollBackAsync();
				return new ResponseApiDTO
				{
					IsSuccess = false,
					ErrorMessage = new List<string> { "Errors occur", ex.Message.ToString() },
					StatusCode = HttpStatusCode.InternalServerError,
					Result = null
				};
			}
		}

		public async Task<ResponseApiDTO> UpdateAsync(string id, TUpdateDto updateDto)
		{
			var repo = _unitOfWork.GetRepo<TEntity>();
			var entity = await repo.GetSingleAsync(CreateQueryBuilder()
												   .WithPredicate(x => x.Equals(id))
												   .WithTracking(true)
												   .Build());

			if (entity == null)
			{
				return new ResponseApiDTO
				{
					IsSuccess = false,
					ErrorMessage = new List<string> { $"{typeof(TEntity).Name} not found" },
					StatusCode = HttpStatusCode.NotFound,
					Result = null
				};
			}

			var updatedEntity = _mapper.Map(updateDto, entity);
			await repo.UpdateAsync(updatedEntity);
			await _unitOfWork.SaveChangesAsync();

			return new ResponseApiDTO
			{
				IsSuccess = true,
				ErrorMessage = null,
				StatusCode = HttpStatusCode.NoContent,
				Result = null
			};
		}

		public async Task<ResponseApiDTO> DeleteAsync(string id)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var repo = _unitOfWork.GetRepo<TEntity>();
				var entity = await repo.GetSingleAsync(CreateQueryBuilder()
												   .WithPredicate(x => x.Equals(id))
												   .WithTracking(true)
												   .Build());

				if (entity == null)
				{
					return new ResponseApiDTO
					{
						IsSuccess = false,
						ErrorMessage = new List<string> { $"{typeof(TEntity).Name} not found" },
						StatusCode = HttpStatusCode.NotFound,
						Result = null
					};
				}

				await repo.DeleteAsync(entity);
				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

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
				await _unitOfWork.RollBackAsync();
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
