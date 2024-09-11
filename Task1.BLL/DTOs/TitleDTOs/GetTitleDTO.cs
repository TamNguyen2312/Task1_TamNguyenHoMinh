using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1.BLL.DTOs.TitleDTOs
{
    public class GetTitleDTO
    {
        public string? TitleId { get; set; } = null!;

        public string? Title1 { get; set; } = null!;

        public string? Type { get; set; } = null!;

        public string? PubId { get; set; }

        public decimal? FromPrice { get; set; }
        public decimal? ToPrice { get; set; }

        public decimal? FromAdvance { get; set; }
        public decimal? ToAdvance { get; set; }

        public int? FromRoyalty { get; set; }
        public int? ToRoyalty { get; set; }

        public int? FromYtdSales { get; set; }
        public int? ToYtdSales { get; set; }

        public string? Notes { get; set; }

        public DateTime? FromPubdate { get; set; }
        public DateTime? ToPubdate { get; set; }
    }
}
