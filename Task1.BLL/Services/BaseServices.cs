﻿using AutoMapper;
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
	public abstract class BaseServices<TEntity, TDetailDto, TCreateDto, TUpdateDto> where TEntity : class
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

		public async Task<PaginatedList<TDetailDto>> GetAllAsync(string? search, int page)
		{
			try
			{
				var queryBuilder = CreateQueryBuilder(search);
				var queryOptions = queryBuilder.Build();

				var repo = _unitOfWork.GetRepo<TEntity>();
				var entities = await repo.GetAllAsync(queryOptions);

				var results = _mapper.Map<List<TDetailDto>>(entities);
				var pageResults = PaginatedList<TDetailDto>.Create(results, page, PAGE_SIZE);

				return pageResults;
			}
			catch (Exception)
			{
				return null;
				throw;
			}
			
		}

		public async Task<TDetailDto> GetByIdAsync(QueryBuilder<TEntity> queryBuilder)
		{
			try
			{
				var repo = _unitOfWork.GetRepo<TEntity>();

				var queryOptions = queryBuilder.Build();

				var entity = await repo.GetSingleAsync(queryOptions);

				return _mapper.Map<TDetailDto>(entity);
			}
			catch(Exception)
			{
				return default(TDetailDto);
				throw;
			}
		}

		public async Task<TDetailDto> CreateAsync(TCreateDto createDto)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var repo = _unitOfWork.GetRepo<TEntity>();
				var entity = _mapper.Map<TEntity>(createDto);
				var createResult = await repo.CreateAsync(entity);
				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				return _mapper.Map<TDetailDto>(createResult);
			}
			catch (Exception)
			{
				await _unitOfWork.RollBackAsync();
				return default(TDetailDto);
				throw;
			}
		}

		public async Task<TDetailDto> UpdateAsync(QueryBuilder<TEntity> queryBuilder, TUpdateDto updateDto)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var repo = _unitOfWork.GetRepo<TEntity>();
				var entity = await repo.GetSingleAsync(queryBuilder.Build());

				if (entity == null)
				{
					throw new Exception($"{typeof(TEntity).Name} not found");
				}

				var updatedEntity = _mapper.Map(updateDto, entity);
				await repo.UpdateAsync(updatedEntity);
				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				return _mapper.Map<TDetailDto>(updatedEntity);
			}
			catch (Exception)
			{
				await _unitOfWork.RollBackAsync();
				return default(TDetailDto);
				throw;
			}
		}

		public async Task<bool> DeleteAsync(QueryBuilder<TEntity> queryBuilder)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var repo = _unitOfWork.GetRepo<TEntity>();
				var entity = await repo.GetSingleAsync(queryBuilder.Build());

				if (entity == null)
				{
					throw new Exception($"{typeof(TEntity).Name} not found");
				}

				var navigationProperties = typeof(TEntity).GetProperties()
					.Where(p => typeof(IEnumerable<object>).IsAssignableFrom(p.PropertyType) && p.PropertyType != typeof(string));

				foreach (var property in navigationProperties)
				{
					var value = property.GetValue(entity) as IEnumerable<object>;

					if (value != null && value.Any()) 
					{
						throw new Exception($"Cannot delete {typeof(TEntity).Name} because {property.Name} contains related entities.");
					}
				}

				await repo.DeleteAsync(entity);
				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				return true;
			}
			catch (Exception)
			{
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}

	}

}
