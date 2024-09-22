using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1.BLL.DTOs.Response;
using Task1.BLL.DTOs.StoreDTOs;

namespace Task1.BLL.Services.Interfaces
{
    public interface IStoreService
    {
        Task<ResponseApiDTO> GetAllStoreAsync(string? search, int page = 1);
        Task<ResponseApiDTO> GetStoreByIdAsync(string id);
        Task<ResponseApiDTO> CreateStoreAsync(StoreCreateRequestDTO storeRequest);
        Task<ResponseApiDTO> UpdateStoreAsync(string id, StoreUpdateRequestDTO storeRequest);
        Task<ResponseApiDTO> DeletetoreAsync(string id);
    }
}
