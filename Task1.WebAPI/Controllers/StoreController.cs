using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Task1.BLL.DTOs.Response;
using Task1.BLL.DTOs.Store;
using Task1.BLL.Services.Interfaces;
using Task1.DAL.Entities;

namespace Task1.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService storeService;

        public StoreController(IStoreService storeService)
        {
            this.storeService = storeService;
        }

        [HttpGet]
        [Route("GetAllStores")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllStoreAsync([FromQuery] GetStoresDTO getStoresDTO, int page = 1)
        {
            var response = await storeService.GetAllStoreAsync(getStoresDTO, page);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet/*("{id:int}", Name ="GetStoreById")*/]
        [Route("GetStoreById/{id}")]
        public async Task<IActionResult> GetStoreByIdAsync([FromRoute] string id)
        {
            var response = await storeService.GetStoreByIdAsync(id);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost]
        [Route("CreateStore")]
        public async Task<ActionResult<StoreCreateRequestDTO>> CreateStoreAsync(StoreCreateRequestDTO storeRequest)
        {
            var response = await storeService.CreateStoreAsync(storeRequest);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut]
        [Route("UpdateStore/{id}")]
        public async Task<IActionResult> UpdateStoreAsync(string id, [FromBody]StoreUpdateRequestDTO storeRequest)
        {
            var response = await storeService.UpdateStoreAsync(id, storeRequest);

            return StatusCode((int)response.StatusCode, response);
        }
    }
}
