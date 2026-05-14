using AirportAutomation.Api.Interfaces;
using AirportAutomation.Application.Dtos.Response;
using AirportAutomation.Core.Enums;
using AirportAutomation.Core.Interfaces;
using AirportAutomation.Core.Interfaces.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AirportAutomation.Api.Controllers
{
	/// <summary>
	/// Serves as a base class for API controllers, providing common functionality and routing.
	/// </summary>
	/// <remarks>
	/// This class is intended to be inherited by other controllers in the API.
	/// </remarks>
	[ApiController]
	[Route("api/v{version:apiVersion}/[controller]")]
	public abstract class BaseController : ControllerBase
	{
		protected async Task<ActionResult<PagedResponse<TDto>>> GetPagedAsync<TEntity, TDto>(
			ICacheService cacheService,
			IPaginationValidationService paginationValidation,
			IMapper mapper,
			ILogger logger,
			int page,
			int pageSize,
			int maxPageSize,
			string cacheKey,
			Func<Task<IList<TEntity>?>> fetchItems,
			Func<Task<int>> fetchCount)
		{
			var (isValid, correctedPageSize, result) =
				paginationValidation.ValidatePaginationParameters(page, pageSize, maxPageSize);
			if (!isValid) return result;

			var response = await cacheService.GetOrCreateAsync<PagedResponse<TDto>>(cacheKey, async () =>
			{
				var items = await fetchItems();
				if (items is null || !items.Any())
				{
					logger.LogInformation("{Dto} not found.", typeof(TDto).Name);
					return null;
				}
				var totalItems = await fetchCount();
				var data = mapper.Map<IEnumerable<TDto>>(items);
				return new PagedResponse<TDto>(data, page, correctedPageSize, totalItems);
			});

			return response is null ? NoContent() : Ok(response);
		}

		protected async Task<ActionResult<TDto>> GetByIdAsync<TEntity, TDto>(
			ICacheService cacheService,
			IInputValidationService inputValidation,
			IMapper mapper,
			ILogger logger,
			int id,
			string cacheKey,
			Func<Task<TEntity?>> fetchItem)
			where TEntity : class
			where TDto : class
		{
			if (!inputValidation.IsNonNegativeInt(id))
			{
				logger.LogInformation("Invalid input. The ID {Id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}

			var dto = await cacheService.GetOrCreateAsync<TDto>(cacheKey, async () =>
			{
				var item = await fetchItem();
				if (item is null)
				{
					logger.LogInformation("{Dto} with id {Id} not found.", typeof(TDto).Name, id);
					return null;
				}
				return mapper.Map<TDto>(item);
			});

			return dto is null ? NotFound() : Ok(dto);
		}

		protected async Task<ActionResult> ExportAsync<TEntity>(
			string entityName,
			FileExtension fileType,
			IExportService exportService,
			IPaginationValidationService paginationValidation,
			IInputValidationService inputValidation,
			ILogger logger,
			int page,
			int pageSize,
			int maxPageSize,
			bool getAll,
			Func<Task<IList<TEntity>>> fetchAll,
			Func<int, int, Task<IList<TEntity>>> fetchPaged,
			Func<int, int, Task<IList<TEntity>>>? fetchSearch = null)
		{
			IList<TEntity> items;

			if (getAll)
			{
				items = await fetchAll();
			}
			else
			{
				var (isValid, correctedPageSize, result) =
					paginationValidation.ValidatePaginationParameters(page, pageSize, maxPageSize);
				if (!isValid) return result;

				items = fetchSearch is not null
					? await fetchSearch(page, correctedPageSize)
					: await fetchPaged(page, correctedPageSize);
			}

			if (items is null || !items.Any())
			{
				logger.LogInformation("No {Entity} found for export.", entityName);
				return NoContent();
			}

			return fileType switch
			{
				FileExtension.Pdf => ExportPdf(entityName, items, exportService, logger),
				FileExtension.Xlsx => ExportExcel(entityName, items, exportService, logger),
				_ => StatusCode(500, "Unsupported file type.")
			};
		}

		private ActionResult ExportPdf<TEntity>(
			string entityName,
			IList<TEntity> items,
			IExportService exportService,
			ILogger logger)
		{
			var pdf = exportService.ExportToPDF(entityName, items);
			if (pdf is null)
			{
				logger.LogError("PDF generation failed for {Entity}.", entityName);
				return StatusCode(500, "Failed to generate PDF file.");
			}
			var fileName = $"{entityName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";
			return File(pdf, "application/pdf", fileName);
		}

		private ActionResult ExportExcel<TEntity>(
			string entityName,
			IList<TEntity> items,
			IExportService exportService,
			ILogger logger)
		{
			var excel = exportService.ExportToExcel(entityName, items);
			if (excel is null || excel.Length == 0)
			{
				logger.LogError("Excel generation failed for {Entity}.", entityName);
				return StatusCode(500, "Failed to generate Excel file.");
			}
			var fileName = $"{entityName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
			return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
		}

	}
}
