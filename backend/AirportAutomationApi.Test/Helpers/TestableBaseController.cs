using AirportAutomation.Api.Controllers;
using AirportAutomation.Api.Interfaces;
using AirportAutomation.Application.Dtos.Response;
using AirportAutomation.Core.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AirportAutomationApi.Test.Helpers
{
	public class TestableBaseController : BaseController
	{
		public Task<ActionResult<TDto>> TestGetByIdAsync<TEntity, TDto>(
			ICacheService cacheService, IInputValidationService inputValidation,
			IMapper mapper, ILogger logger, int id, string cacheKey, Func<Task<TEntity?>> fetchItem)
			where TEntity : class where TDto : class
			=> GetByIdAsync<TEntity, TDto>(cacheService, inputValidation, mapper, logger, id, cacheKey, fetchItem);

		public Task<ActionResult<PagedResponse<TDto>>> TestGetPagedAsync<TEntity, TDto>(
			ICacheService cacheService, IPaginationValidationService paginationValidation,
			IMapper mapper, ILogger logger, int page, int pageSize, int maxPageSize,
			string cacheKey, Func<Task<IList<TEntity>?>> fetchItems, Func<Task<int>> fetchCount)
			=> GetPagedAsync<TEntity, TDto>(cacheService, paginationValidation, mapper, logger, page, pageSize, maxPageSize, cacheKey, fetchItems, fetchCount);
	}
}
