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

        [HttpGet]
        [Route("GetTitleById/{id}")]
        public async Task<IActionResult> GetTitleByIdAsync(string id)
        {
            var response = await titleService.GetTitleByIdAsync(id);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost]
        [Route("CreateTitle")]
        public async Task<IActionResult> CreateTitleAsync(TitleCreateRequestDTO titleCreateRequest)
        {
            var response = await titleService.CreateTitleAsync(titleCreateRequest);
            return StatusCode((int)response.StatusCode,response);
        }

        [HttpPut]
        [Route("UpdateTitle/{id}")]
        public async Task<IActionResult> UpdateTitleAsync(string id, [FromBody]TitleUpdateRequestDTO titleUpdateRequest)
        {
            var response = await titleService.UpdateTitleAsync(id, titleUpdateRequest);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpDelete]
        [Route("DeleteTitle/{id}")]
        public async Task<IActionResult> DeleteTitleAsync(string id)
        {
            var response = await titleService.DeleteTitleAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
