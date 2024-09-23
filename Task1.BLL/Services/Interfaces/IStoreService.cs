using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Task1.BLL.DTOs.Response;
using Task1.BLL.DTOs.StoreDTOs;
using Task1.DAL.Entities;

namespace Task1.BLL.Services.Interfaces
{
    public interface IStoreService
    {
        Task<StoreListViewDTO> GetAllStoreAsync(string? search, int page = 1);
        Task<StoreDetailDTO> GetStoreByIdAsync(string id);
        Task<StoreDetailDTO> CreateStoreAsync(StoreCreateRequestDTO storeRequest);
        Task<StoreDetailDTO> UpdateStoreAsync(string id, StoreUpdateRequestDTO storeRequest);
        Task<bool> DeletetoreAsync(string id);
    }
}
