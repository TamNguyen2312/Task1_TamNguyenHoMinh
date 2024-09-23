using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Task1.BLL.DTOs.Response;
using Task1.BLL.DTOs.StoreDTOs;
using Task1.BLL.Services.Interfaces;
using Task1.DAL.Entities;

namespace Task1.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : BaseAPIController
    {
        private readonly IStoreService storeService;

        public StoreController(IStoreService storeService)
        {
            this.storeService = storeService;
        }

        [HttpGet]
        [Route("GetAllStores")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllStoreAsync([FromQuery] string? search, int page = 1)
        {
            try
            {
                var response = await storeService.GetAllStoreAsync(search, page);
                if(response == null)
                {
                    return GetError();
                }

                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                return Error(ex.Message.ToString());
            }
        }

        [HttpGet/*("{id:int}", Name ="GetStoreById")*/]
        [Route("GetStoreById/{id}")]
        public async Task<IActionResult> GetStoreByIdAsync([FromRoute] string id)
        {
            try
            {
				var response = await storeService.GetStoreByIdAsync(id);
                if(response == null)
                {
                    return GetNotFound($"Cannot find store with Id: {id}");
                }
                return GetSuccess(response);
			}
            catch(Exception ex)
            {
                return Error(ex.Message.ToString());
            }   
        }

        [HttpPost]
        [Route("CreateStore")]
        public async Task<IActionResult> CreateStoreAsync(StoreCreateRequestDTO storeRequest)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return ModelInvalid();
                }

                var response = await storeService.CreateStoreAsync(storeRequest);

                if(response == null)
                {
                    return SaveError();
                }

                return SaveSuccess(response);
            }
            catch (Exception ex)
            {
                return Error(ex.Message.ToString());
            }
        }

        [HttpPut]
        [Route("UpdateStore/{id}")]
        public async Task<IActionResult> UpdateStoreAsync(string id, [FromBody]StoreUpdateRequestDTO storeRequest)
        {
			try
			{
				if (!ModelState.IsValid)
				{
					return ModelInvalid();
				}

				var response = await storeService.UpdateStoreAsync(id, storeRequest);

				if (response == null)
				{
					return SaveError();
				}

				return SaveSuccess(response);
			}
			catch (Exception ex)
			{
				return Error(ex.Message.ToString());
			}
		}

        [HttpDelete]
        [Route("DeleteStore/{id}")]
        public async Task<IActionResult> DeleteStoreAsync(string id)
        {
			try
			{
				if (!ModelState.IsValid)
				{
					return ModelInvalid();
				}

				var response = await storeService.DeletetoreAsync(id);
                
                if(!response)
                {
                    return Error("The process has meet any erros. Please try again afer a few minutes");
                }

				return Success(response, "Deleted Succesfully");
			}
			catch (Exception ex)
			{
				return Error(ex.Message.ToString());
			}
		}
    }
}
