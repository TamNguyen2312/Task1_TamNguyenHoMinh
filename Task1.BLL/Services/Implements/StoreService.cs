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
        public async Task<ResponseAPIDTO<IEnumerable<StoreViewDTO>>> GetAllStoreAsync(GetStoresDTO getStoresDTO, int page)
        {

            var stores = await unitOfWork.GetRepo<Store>().GetAllAsync(null,
                                                                        r => r.OrderBy(q => q.StorName),
                                                                        false);
            stores = stores.AsQueryable();

            if(getStoresDTO.StorId != null)
            {
                stores = stores.Where(x => x.StorId.Equals(getStoresDTO.StorId));
            }

            if(getStoresDTO.StorName != null)
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

            if(getStoresDTO.Zip != null)
            {
                stores = stores.Where(x => x.Zip.IndexOf(getStoresDTO.Zip, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            var results = mapper.Map<List<StoreViewDTO>>(stores);

            var pageResults = PaginatedList<StoreViewDTO>.Create(results, page, PAGE_SIZE);

            return new ResponseAPIDTO<IEnumerable<StoreViewDTO>>
            {
                Success = true,
                Message = "Here is the list of stores",
                Data = pageResults,
                Total = results.Count(),
            };
        }
    }
}
