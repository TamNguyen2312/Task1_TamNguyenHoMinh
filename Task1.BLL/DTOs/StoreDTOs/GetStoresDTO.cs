using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1.BLL.DTOs.StoreDTOs
{
    public class GetStoresDTO
    {
        public string? StorId { get; set; } = null!;

        public string? StorName { get; set; }

        public string? StorAddress { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Zip { get; set; }
    }
}
