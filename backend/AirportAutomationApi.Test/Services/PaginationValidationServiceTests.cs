using AirportАutomation.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AirportAutomationApi.Test.Services
{
	public class PaginationValidationServiceTests
	{
		private readonly PaginationValidationService service;
		private readonly Mock<ILogger<PaginationValidationService>> _loggerMock;

		public PaginationValidationServiceTests()
		{
			_loggerMock = new Mock<ILogger<PaginationValidationService>>();
			service = new PaginationValidationService(
				_loggerMock.Object
			);
		}

		[Fact]
		public void ValidatePaginationParameters_ValidPageAndPageSize_ShouldReturnValidResult()
		{
			int page = 1;
			int pageSize = 10;
			int maxPageSize = 100;

			var (isValid, correctedPageSize, result) = service.ValidatePaginationParameters(page, pageSize, maxPageSize);

			Assert.True(isValid);
			Assert.Equal(pageSize, correctedPageSize);
			Assert.Null(result);
		}

		[Fact]
		public void ValidatePaginationParameters_InvalidPage_ShouldReturnBadRequestResult()
		{
			int page = 0;
			int pageSize = 10;
			int maxPageSize = 100;

			var (isValid, correctedPageSize, result) = service.ValidatePaginationParameters(page, pageSize, maxPageSize);

			Assert.False(isValid);
			Assert.Equal(0, correctedPageSize);
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public void ValidatePaginationParameters_InvalidPageSize_ShouldReturnBadRequestResult()
		{
			int page = 1;
			int pageSize = 0;
			int maxPageSize = 100;

			var (isValid, correctedPageSize, result) = service.ValidatePaginationParameters(page, pageSize, maxPageSize);

			Assert.False(isValid);
			Assert.Equal(0, correctedPageSize);
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public void ValidatePaginationParameters_ExceedMaxPageSize_ShouldCorrectPageSizeAndReturnValidResult()
		{
			int page = 1;
			int pageSize = 200;
			int maxPageSize = 100;

			var (isValid, correctedPageSize, result) = service.ValidatePaginationParameters(page, pageSize, maxPageSize);

			Assert.True(isValid);
			Assert.Equal(maxPageSize, correctedPageSize);
			Assert.Null(result);
		}
	}
}

