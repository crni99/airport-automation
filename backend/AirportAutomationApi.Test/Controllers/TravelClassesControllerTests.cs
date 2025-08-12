using AirportAutomation.Api.Interfaces;
using AirportAutomation.Application.Dtos.Response;
using AirportAutomation.Application.Dtos.TravelClass;
using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IServices;
using AirportАutomation.Api.Controllers;
using AirportАutomation.Api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace AirportAutomationApi.Test.Controllers
{
	public class TravelClassesControllerTests
	{
		private readonly TravelClassesController _controller;
		private readonly Mock<ITravelClassService> _travelClassServiceMock;
		private readonly Mock<IPaginationValidationService> _paginationValidationServiceMock;
		private readonly Mock<IInputValidationService> _inputValidationServiceMock;
		private readonly Mock<IUtilityService> _utilityServiceMock;
		private readonly Mock<IExportService> _exportServiceMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<ILogger<TravelClassesController>> _loggerMock;
		private readonly Mock<IConfiguration> _configurationMock;

		private readonly TravelClassEntity travelClassEntity = new()
		{
			Id = 1,
			Type = "Business"
		};

		private readonly TravelClassDto travelClassDto = new()
		{
			Id = 2,
			Type = "First"
		};

		public TravelClassesControllerTests()
		{
			_travelClassServiceMock = new Mock<ITravelClassService>();
			_paginationValidationServiceMock = new Mock<IPaginationValidationService>();
			_inputValidationServiceMock = new Mock<IInputValidationService>();
			_utilityServiceMock = new Mock<IUtilityService>();
			_exportServiceMock = new Mock<IExportService>();
			_mapperMock = new Mock<IMapper>();
			_loggerMock = new Mock<ILogger<TravelClassesController>>();
			_configurationMock = new Mock<IConfiguration>();
			var configBuilder = new ConfigurationBuilder();
			configBuilder.AddInMemoryCollection(new Dictionary<string, string>
			{
				{"pageSettings:maxPageSize", "10"}
			});
			_configurationMock.Setup(x => x.GetSection(It.IsAny<string>()))
				.Returns(configBuilder.Build().GetSection(""));

			_controller = new TravelClassesController(
				_travelClassServiceMock.Object,
				_paginationValidationServiceMock.Object,
				_inputValidationServiceMock.Object,
				_utilityServiceMock.Object,
				_exportServiceMock.Object,
				_mapperMock.Object,
				_loggerMock.Object,
				_configurationMock.Object
			);
		}

		[Fact]
		[Trait("Category", "GetTravelClasses")]
		public async Task GetTravelClasses_InvalidPaginationParameters_ReturnsBadRequest()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int invalidPage = -1;
			int invalidPageSize = 0;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid pagination parameters.");

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(invalidPage, invalidPageSize, It.IsAny<int>()))
				.Returns((false, 0, expectedBadRequestResult));

			// Act
			var result = await _controller.GetTravelClasses(cancellationToken, invalidPage, invalidPageSize);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetTravelClasses")]
		public async Task GetTravelClasses_ReturnsNoContent_WhenNoTravelClassesFound()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_travelClassServiceMock.Setup(service => service.GetTravelClasses(cancellationToken, It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(new List<TravelClassEntity>());

			// Act
			var result = await _controller.GetTravelClasses(cancellationToken, page, pageSize);

			// Assert
			Assert.IsType<NoContentResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetTravelClasses")]
		public async Task GetTravelClasses_ReturnsInternalServerError_WhenExceptionThrown()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_travelClassServiceMock.Setup(service => service.GetTravelClasses(cancellationToken, It.IsAny<int>(), It.IsAny<int>()))
				.ThrowsAsync(new Exception("Simulated exception"));

			// Act & Assert
			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetTravelClasses(cancellationToken, page, pageSize));
		}

		[Fact]
		[Trait("Category", "GetTravelClasses")]
		public async Task GetTravelClasses_ReturnsOk_WithPaginatedTravelClasses()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;
			var travelClasses = new List<TravelClassEntity>
			{
				new TravelClassEntity { /* Initialize properties */ },
				new TravelClassEntity { /* Initialize properties */ }
			};
			var totalItems = 2;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_travelClassServiceMock
				.Setup(service => service.GetTravelClasses(cancellationToken, page, pageSize))
				.ReturnsAsync(travelClasses);
			_travelClassServiceMock
				.Setup(service => service.TravelClassesCount(cancellationToken))
				.ReturnsAsync(totalItems);

			var expectedData = new List<TravelClassDto>
			{
				new TravelClassDto { /* Initialize properties */ },
				new TravelClassDto { /* Initialize properties */ }
			};
			_mapperMock
				.Setup(m => m.Map<IEnumerable<TravelClassDto>>(It.IsAny<IEnumerable<TravelClassEntity>>()))
				.Returns(expectedData);

			// Act
			var result = await _controller.GetTravelClasses(cancellationToken, page, pageSize);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PagedResponse<TravelClassDto>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var pagedResponse = Assert.IsType<PagedResponse<TravelClassDto>>(okResult.Value);
			Assert.Equal(page, pagedResponse.PageNumber);
			Assert.Equal(pageSize, pagedResponse.PageSize);
			Assert.Equal(totalItems, pagedResponse.TotalCount);
			Assert.Equal(expectedData, pagedResponse.Data);
		}

		[Fact]
		[Trait("Category", "GetTravelClasses")]
		public async Task GetTravelClasses_ReturnsCorrectPageData()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 2;
			int pageSize = 5;
			var allTravelClasses = new List<TravelClassEntity>
			{
				new TravelClassEntity { /* Initialize properties */ },
				new TravelClassEntity { /* Initialize properties */ },
				new TravelClassEntity { /* Initialize properties */ },
				new TravelClassEntity { /* Initialize properties */ },
				new TravelClassEntity { /* Initialize properties */ },
				new TravelClassEntity { /* Initialize properties */ },
				new TravelClassEntity { /* Initialize properties */ },
				new TravelClassEntity { /* Initialize properties */ },
				new TravelClassEntity { /* Initialize properties */ },
				new TravelClassEntity { /* Initialize properties */ }
			};
			var pagedTravelClasses = allTravelClasses.Skip((page - 1) * pageSize).Take(pageSize).ToList();

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_travelClassServiceMock
				.Setup(service => service.GetTravelClasses(cancellationToken, page, pageSize))
				.ReturnsAsync(pagedTravelClasses);
			_travelClassServiceMock
				.Setup(service => service.TravelClassesCount(cancellationToken))
				.ReturnsAsync(allTravelClasses.Count);

			var expectedData = new List<TravelClassDto>
			{
				new TravelClassDto { /* Initialize properties */ },
				new TravelClassDto { /* Initialize properties */ },
				new TravelClassDto { /* Initialize properties */ },
				new TravelClassDto { /* Initialize properties */ },
				new TravelClassDto { /* Initialize properties */ }
			};
			_mapperMock
				.Setup(m => m.Map<IEnumerable<TravelClassDto>>(It.IsAny<IEnumerable<TravelClassEntity>>()))
				.Returns(expectedData);

			// Act
			var result = await _controller.GetTravelClasses(cancellationToken, page, pageSize);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PagedResponse<TravelClassDto>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var pagedResponse = Assert.IsType<PagedResponse<TravelClassDto>>(okResult.Value);
			Assert.Equal(page, pagedResponse.PageNumber);
			Assert.Equal(pageSize, pagedResponse.PageSize);
			Assert.Equal(allTravelClasses.Count, pagedResponse.TotalCount);
			Assert.Equal(expectedData, pagedResponse.Data);
		}

		[Fact]
		[Trait("Category", "GetTravelClass")]
		public async Task GetTravelClass_InvalidId_ReturnsBadRequest()
		{
			// Arrange
			int invalidId = -1;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid input. The ID must be a non-negative integer.");

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(invalidId))
				.Returns(false);

			// Act
			var result = await _controller.GetTravelClass(invalidId);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
			var badRequestResult = result.Result as BadRequestObjectResult;
			Assert.Equal(expectedBadRequestResult.Value, badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "GetTravelClass")]
		public async Task GetTravelClass_TravelClassNotFound_ReturnsNotFound()
		{
			// Arrange
			int validId = 1;

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(validId))
				.Returns(true);
			_travelClassServiceMock
				.Setup(service => service.TravelClassExists(validId))
				.ReturnsAsync(false);

			// Act
			var result = await _controller.GetTravelClass(validId);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetTravelClass")]
		public async Task GetTravelClass_ReturnsTravelClassDto_WhenTravelClassExists()
		{
			// Arrange
			int validId = 1;

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(validId))
				.Returns(true);
			_travelClassServiceMock
				.Setup(service => service.TravelClassExists(validId))
				.ReturnsAsync(true);
			_travelClassServiceMock
				.Setup(service => service.GetTravelClass(validId))
				.ReturnsAsync(travelClassEntity);
			_mapperMock
				.Setup(m => m.Map<TravelClassDto>(travelClassEntity))
				.Returns(travelClassDto);

			// Act
			var result = await _controller.GetTravelClass(validId);

			// Assert
			var actionResult = Assert.IsType<ActionResult<TravelClassDto>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var returnedTravelClassDto = Assert.IsType<TravelClassDto>(okResult.Value);
			Assert.Equal(travelClassDto, returnedTravelClassDto);
		}

	}
}