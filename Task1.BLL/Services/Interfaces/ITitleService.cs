using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1.BLL.DTOs.Response;
using Task1.BLL.DTOs.TitleDTOs;

namespace Task1.BLL.Services.Interfaces
{
    public interface ITitleService
    {
        public Task<ResponseApiDTO> GetAllTitleAsync(GetTitleDTO getTitleDTO, int page);
    }
}
