using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1.BLL.DTOs.StoreDTOs;
using Task1.DAL.Entities;

namespace Task1.BLL.Helper.Extension.Stores
{
    public static class StoreExtensions
    {
        public static IEnumerable<Store> ApplyFilters(this IEnumerable<Store> stores, GetStoresDTO filters)
        {
            return stores.Where(x =>
                (filters.StorId == null || x.StorId.Equals(filters.StorId)) &&
                (filters.StorName == null || x.StorName.IndexOf(filters.StorName, StringComparison.OrdinalIgnoreCase) >= 0) &&
                (filters.StorAddress == null || x.StorAddress.IndexOf(filters.StorAddress, StringComparison.OrdinalIgnoreCase) >= 0) &&
                (filters.City == null || x.City.IndexOf(filters.City, StringComparison.OrdinalIgnoreCase) >= 0) &&
                (filters.State == null || x.State.IndexOf(filters.State, StringComparison.OrdinalIgnoreCase) >= 0) &&
                (filters.Zip == null || x.Zip.IndexOf(filters.Zip, StringComparison.OrdinalIgnoreCase) >= 0)
            );
        }

        public static string AutoGenerateStoreId()
        {
            Random random = new Random();
            int number = random.Next(1000, 10000); 
            return number.ToString();
        }
    }
}
