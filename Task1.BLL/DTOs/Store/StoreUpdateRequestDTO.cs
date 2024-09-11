using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1.BLL.Helper.Validation.Store;

namespace Task1.BLL.DTOs.Store
{
    public class StoreUpdateRequestDTO
    {
        [StringLength(40, ErrorMessage = "Store Name cannot be longer than 40")]
        public string? StorName { get; set; }

        [StringLength(40, ErrorMessage = "Store Address cannot be longer than 40")]
        public string? StorAddress { get; set; }

        [StringLength(20, ErrorMessage = "City cannot be longer than 20")]
        public string? City { get; set; }

        [StringLength(2, ErrorMessage = "State cannot be longer than 2")]
        public string? State { get; set; }

        [StringLength(5, ErrorMessage = "Zip cannot be longer than 5")]
        [ZipValid]
        public string? Zip { get; set; }
    }
}
