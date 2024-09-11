using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task1.BLL.DTOs.TitleDTOs;
using Task1.BLL.Services.Interfaces;

namespace Task1.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitleController : ControllerBase
    {
        private readonly ITitleService titleService;

        public TitleController(ITitleService titleService)
        {
            this.titleService = titleService;
        }

        [HttpGet]
        [Route("GetAllTitles")]
        public async Task<IActionResult> GetAllTitleAsync([FromQuery]GetTitleDTO getTitleDTO, int page = 1)
        {
            var response = await titleService.GetAllTitleAsync(getTitleDTO, page);

            return StatusCode((int)response.StatusCode, response);
        }
    }
}
