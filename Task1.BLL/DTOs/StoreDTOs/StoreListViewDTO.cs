using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1.BLL.DTOs.StoreDTOs
{
	public class StoreListViewDTO
	{
		public StoreListViewDTO()
		{
			StoreDetailDTOs = new List<StoreDetailDTO>();
		}
		public IEnumerable<StoreDetailDTO> StoreDetailDTOs { get; set; }
		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }

		public int TotalItems {  get; set; }
	}
}
