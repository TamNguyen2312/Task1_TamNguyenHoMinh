using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1.BLL.Helper.Validation.StoreValid;
using Task1.DAL.Entities;

namespace Task1.BLL.DTOs.StoreDTOs
{
	public class StoreDetailDTO
	{
		public StoreDetailDTO()
		{
			Sales = new List<Sale>();
		}
		public string StorId { get; set; } = null!;

		[Display(Name = "Name")]
		[StringLength(40, ErrorMessage = "Store Name cannot be longer than 40")]
		public string? StorName { get; set; }

		[Display(Name = "Address")]
		[StringLength(40, ErrorMessage = "Store Address cannot be longer than 40")]
		public string? StorAddress { get; set; }

		[Display(Name = "City")]
		[StringLength(20, ErrorMessage = "City cannot be longer than 20")]
		public string? City { get; set; }

		[Display(Name = "State")]
		[StringLength(2, ErrorMessage = "State cannot be longer than 2")]
		public string? State { get; set; }

		[Display(Name = "Zip")]
		[StringLength(5, ErrorMessage = "Zip cannot be longer than 5")]
		[ZipValid(ErrorMessage = "Invalid type specified")]
		public string? Zip { get; set; }

		public ICollection<Sale> Sales { get; set; }
	}
}
