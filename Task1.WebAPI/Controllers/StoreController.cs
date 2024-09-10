using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task1.BLL.DTOs.Response;
using Task1.BLL.DTOs.Store;
using Task1.BLL.Services.Interfaces;

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
        public async Task<IActionResult> GetAllStoreAsync([FromQuery]GetStoresDTO getStoresDTO, int page = 1)
        {
            var result = await storeService.GetAllStoreAsync(getStoresDTO, page);

            return Ok(result);
        }
    }
}
