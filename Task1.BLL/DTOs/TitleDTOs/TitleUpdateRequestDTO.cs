using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1.BLL.Helper.Extension.Titles;
using Task1.BLL.Helper.Validation.TitleValid;

namespace Task1.BLL.DTOs.TitleDTOs
{
    public class TitleUpdateRequestDTO
    {
        [Required(ErrorMessage = "Title1 is required")]
        [StringLength(80, ErrorMessage = "Title1 cannot be longer than 80 characters")]
        public string Title1 { get; set; } = null!;

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Price must be a positive value")]
        public decimal? Price { get; set; }

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Advance must be a positive value")]
        public decimal? Advance { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Royalty must be a non-negative integer")]
        public int? Royalty { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "YtdSales must be a non-negative integer")]
        public int? YtdSales { get; set; }

        [StringLength(200, ErrorMessage = "Notes cannot be longer than 200 characters")]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "Pubdate is required")]
        public DateTime Pubdate { get; set; }
    }
}
