using AirportAutomation.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AirportAutomation.Api.Services
{
	public class PaginationValidationService : IPaginationValidationService
	{
		private readonly ILogger<PaginationValidationService> _logger;

		public PaginationValidationService(ILogger<PaginationValidationService> logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public (bool isValid, int correctedPageSize, ActionResult result) ValidatePaginationParameters(int page, int pageSize, int maxPageSize)
		{
			if (page < 1)
			{
				_logger.LogInformation("Invalid page number: Page number should be a positive integer.");
				return (false, 0, new BadRequestObjectResult("Invalid page number."));
			}

			if (pageSize < 1)
			{
				_logger.LogInformation("Invalid page size: Page size should be a positive integer between 1 and {MaxPageSize}.", maxPageSize);
				return (false, 0, new BadRequestObjectResult($"Invalid page size. It should be between 1 and {maxPageSize}."));
			}

			if (pageSize > maxPageSize)
			{
				pageSize = maxPageSize;
			}

			return (true, pageSize, null);
		}
	}
}
