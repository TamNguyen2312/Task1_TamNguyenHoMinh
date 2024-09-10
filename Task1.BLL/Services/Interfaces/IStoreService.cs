using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1.BLL.DTOs.Response;
using Task1.BLL.DTOs.Store;

namespace Task1.BLL.Services.Interfaces
{
    public interface IStoreService
    {
        Task<ResponseApiDTO> GetAllStoreAsync(GetStoresDTO getStoresDTO, int page = 1);
        Task<ResponseApiDTO> GetStoreByIdAsync(string id);
    }
}
