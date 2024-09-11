using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using Task1.BLL.DTOs.Response;
using Task1.BLL.DTOs.TitleDTOs;
using Task1.BLL.Helper.Extension.Titles;
using Task1.BLL.Helper.Paging;
using Task1.BLL.Services.Interfaces;
using Task1.DAL.Entities;
using Task1.DAL.Repositories;

namespace Task1.BLL.Services.Implements
{
    public class TitleService : ITitleService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public static int PAGE_SIZE { get; set; } = 6;

        public TitleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<ResponseApiDTO> GetAllTitleAsync(GetTitleDTO getTitleDTO, int page)
        {
            try
            {
                var titleRepo = unitOfWork.GetRepo<Title>();

                var titles = await titleRepo.GetAllAsync(null, x => x.OrderBy(r => r.Title1), false);

                titles = titles.ApplyFilter(getTitleDTO);

                var results = mapper.Map<List<TitleViewDTO>>(titles);

                var pageResults = PaginatedList<TitleViewDTO>.Create(results, page, PAGE_SIZE);

                if (pageResults.IsNullOrEmpty())
                {
                    return new ResponseApiDTO
                    {
                        IsSuccess = true,
                        ErrorMessage = new List<string> { $"Title not found" },
                        StatusCode = HttpStatusCode.OK,
                        Result = null
                    };
                }

                return new ResponseApiDTO
                {

                    IsSuccess = true,
                    ErrorMessage = null,
                    StatusCode = HttpStatusCode.OK,
                    Result = pageResults
                };
            }
            catch (Exception ex)
            {
                return new ResponseApiDTO
                {
                    IsSuccess = false,
                    ErrorMessage = new List<string> { $"Errors occur", ex.Message.ToString() },
                    StatusCode = HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<ResponseApiDTO> GetTitleByIdAsync(string id)
        {
            try
            {
                var titleRepo = unitOfWork.GetRepo<Title>();

                var title = await titleRepo.GetSingle(x => x.TitleId.Equals(id), null, false);

                if (title == null)
                {
                    return new ResponseApiDTO
                    {
                        IsSuccess = false,
                        ErrorMessage = new List<string> { $"Title not found" },
                        StatusCode = HttpStatusCode.NotFound,
                        Result = null
                    };
                }
                else
                {
                    return new ResponseApiDTO
                    {
                        IsSuccess = true,
                        ErrorMessage = null,
                        StatusCode = HttpStatusCode.OK,
                        Result = mapper.Map<TitleViewDTO>(title)
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseApiDTO
                {
                    IsSuccess = false,
                    ErrorMessage = new List<string> { $"Errors occur", ex.Message.ToString() },
                    StatusCode = HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<ResponseApiDTO> CreateTitleAsync(TitleCreateRequestDTO titleCreateRequest)
        {
            try
            {
                using var transaction = unitOfWork.BeginTransactionAsync();

                var titleRepo = unitOfWork.GetRepo<Title>();

                var existedPubliser = await unitOfWork.GetRepo<Publisher>().GetSingle(x => x.PubId.Equals(titleCreateRequest.PubId));

                if(existedPubliser == null)
                {
                    return new ResponseApiDTO
                    {
                        IsSuccess = false,
                        ErrorMessage = new List<string> { "Publisher does not exist" },
                        StatusCode = HttpStatusCode.BadRequest,
                        Result = null
                    };
                }

                string titleId;
                do
                {
                    titleId = TitleExtensions.GenerateTitleId(titleCreateRequest.Type);
                } while (await titleRepo.GetSingle(s => s.TitleId == titleId) != null);

                var title = mapper.Map<Title>(titleCreateRequest);
                title.TitleId = titleId;

                var createResult = await unitOfWork.GetRepo<Title>().CreateAsync(title);
                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitTransactionAsync();

                if (createResult == null)
                {
                    return new ResponseApiDTO
                    {
                        IsSuccess = false,
                        ErrorMessage = new List<string> { "Created title failed" },
                        StatusCode = HttpStatusCode.BadRequest,
                        Result = null
                    };
                }
                else
                {
                    return new ResponseApiDTO
                    {
                        IsSuccess = true,
                        ErrorMessage = null,
                        StatusCode = HttpStatusCode.Created,
                        Result = mapper.Map<TitleViewDTO>(createResult)
                    };
                }
            }
            catch(Exception ex)
            {
                await unitOfWork.RollBackAsync();
                return new ResponseApiDTO
                {
                    IsSuccess = false,
                    ErrorMessage = new List<string> { "Errors occur", ex.Message.ToString() },
                    StatusCode = HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
        }
    }
}
