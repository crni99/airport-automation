using AirportAutomation.Api.Controllers;
using AirportAutomation.Api.Interfaces;
using AirportAutomation.Application.Dtos.Passenger;
using AirportAutomation.Application.Dtos.Response;
using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Enums;
using AirportAutomation.Core.Filters;
using AirportAutomation.Core.Interfaces.IServices;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace AirportAutomationApi.Test.Controllers
{
	public class PassengersControllerTests
	{
		private readonly PassengersController _controller;
		private readonly Mock<IPassengerService> _passengerServiceMock;
		private readonly Mock<IPaginationValidationService> _paginationValidationServiceMock;
		private readonly Mock<IInputValidationService> _inputValidationServiceMock;
		private readonly Mock<IUtilityService> _utilityServiceMock;
		private readonly Mock<IExportService> _exportServiceMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<ILogger<PassengersController>> _loggerMock;
		private readonly Mock<IConfiguration> _configurationMock;
		private readonly List<PassengerEntity> _samplePassengers;
		private readonly byte[] _sampleExcelBytes;

		private readonly PassengerEntity passengerEntity = new()
		{
			Id = 1,
			FirstName = "Ognjen",
			LastName = "Andjelic",
			UPRN = "1234567890123",
			Passport = "1234567",
			Address = "DD 10",
			Phone = "064"
		};

		private readonly PassengerDto passengerDto = new()
		{
			Id = 1,
			FirstName = "Ognjen",
			LastName = "Andjelic",
			UPRN = "1234567890123",
			Passport = "1234567",
			Address = "DD 10",
			Phone = "064"
		};
		public PassengersControllerTests()
		{
			_passengerServiceMock = new Mock<IPassengerService>();
			_paginationValidationServiceMock = new Mock<IPaginationValidationService>();
			_inputValidationServiceMock = new Mock<IInputValidationService>();
			_utilityServiceMock = new Mock<IUtilityService>();
			_exportServiceMock = new Mock<IExportService>();
			_mapperMock = new Mock<IMapper>();
			_loggerMock = new Mock<ILogger<PassengersController>>();
			_configurationMock = new Mock<IConfiguration>();
			var configBuilder = new ConfigurationBuilder();
			configBuilder.AddInMemoryCollection(new Dictionary<string, string>
			{
				{"pageSettings:maxPageSize", "10"}
			});
			_configurationMock.Setup(x => x.GetSection(It.IsAny<string>()))
				.Returns(configBuilder.Build().GetSection(""));

			_controller = new PassengersController(
				_passengerServiceMock.Object,
				_paginationValidationServiceMock.Object,
				_inputValidationServiceMock.Object,
				_utilityServiceMock.Object,
				_exportServiceMock.Object,
				_mapperMock.Object,
				_loggerMock.Object,
				_configurationMock.Object
			);
			_samplePassengers = new List<PassengerEntity> { new PassengerEntity { Id = 1 } };
			_sampleExcelBytes = new byte[] { 0x50, 0x4B, 0x03, 0x04 };
		}

		[Theory]
		[Trait("Category", "Constructor")]
		[InlineData("passengerService")]
		[InlineData("paginationValidationService")]
		[InlineData("inputValidationService")]
		[InlineData("utilityService")]
		[InlineData("exportService")]
		[InlineData("mapper")]
		[InlineData("logger")]
		public void Constructor_WhenServiceIsNull_ThrowsArgumentNullException(string serviceName)
		{
			// Arrange
			var passengerServiceMock = new Mock<IPassengerService>();
			var paginationValidationServiceMock = new Mock<IPaginationValidationService>();
			var inputValidationServiceMock = new Mock<IInputValidationService>();
			var utilityServiceMock = new Mock<IUtilityService>();
			var exportServiceMock = new Mock<IExportService>();
			var mapperMock = new Mock<IMapper>();
			var loggerMock = new Mock<ILogger<PassengersController>>();
			var configurationMock = new Mock<IConfiguration>();

			// Set up mocks to return null based on the test case
			IPassengerService passengerService = serviceName == "passengerService" ? null : passengerServiceMock.Object;
			IPaginationValidationService paginationValidationService = serviceName == "paginationValidationService" ? null : paginationValidationServiceMock.Object;
			IInputValidationService inputValidationService = serviceName == "inputValidationService" ? null : inputValidationServiceMock.Object;
			IUtilityService utilityService = serviceName == "utilityService" ? null : utilityServiceMock.Object;
			IExportService exportService = serviceName == "exportService" ? null : exportServiceMock.Object;
			IMapper mapper = serviceName == "mapper" ? null : mapperMock.Object;
			ILogger<PassengersController> logger = serviceName == "logger" ? null : loggerMock.Object;

			// Act & Assert
			var exception = Record.Exception(() => new PassengersController(
				passengerService,
				paginationValidationService,
				inputValidationService,
				utilityService,
				exportService,
				mapper,
				logger,
				configurationMock.Object
			));

			// Assert
			Assert.NotNull(exception);
			Assert.IsType<ArgumentNullException>(exception);
			Assert.Contains(serviceName, exception.Message);
		}

		#region GetPassengers

		[Fact]
		[Trait("Category", "GetPassengers")]
		public async Task GetPassengers_InvalidPaginationParameters_ReturnsBadRequest()
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
			var result = await _controller.GetPassengers(cancellationToken, invalidPage, invalidPageSize);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetPassengers")]
		public async Task GetPassengers_ReturnsNoContent_WhenNoPassengersFound()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_passengerServiceMock.Setup(service => service.GetPassengers(cancellationToken, It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(new List<PassengerEntity>());

			// Act
			var result = await _controller.GetPassengers(cancellationToken, page, pageSize);

			// Assert
			Assert.IsType<NoContentResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetPassengers")]
		public async Task GetPassengers_ReturnsNoContent_WhenServiceReturnsNull()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_passengerServiceMock
				.Setup(service => service.GetPassengers(cancellationToken, It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync((List<PassengerEntity>)null);

			// Act
			var result = await _controller.GetPassengers(cancellationToken, page, pageSize);

			// Assert
			Assert.IsType<NoContentResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetPassengers")]
		public async Task GetPassengers_ReturnsInternalServerError_WhenExceptionThrown()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_passengerServiceMock.Setup(service => service.GetPassengers(cancellationToken, It.IsAny<int>(), It.IsAny<int>()))
				.ThrowsAsync(new Exception("Simulated exception"));

			// Act & Assert
			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetPassengers(cancellationToken, page, pageSize));
		}

		[Fact]
		[Trait("Category", "GetPassengers")]
		public async Task GetPassengers_ReturnsOk_WithPaginatedPassengers()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;
			var passengers = new List<PassengerEntity>
			{
				new PassengerEntity { /* Initialize properties */ },
				new PassengerEntity { /* Initialize properties */ }
			};
			var totalItems = 2;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_passengerServiceMock
				.Setup(service => service.GetPassengers(cancellationToken, page, pageSize))
				.ReturnsAsync(passengers);
			_passengerServiceMock
				.Setup(service => service.PassengersCount(cancellationToken, null, null))
				.ReturnsAsync(totalItems);

			var expectedData = new List<PassengerDto>
			{
				new PassengerDto { /* Initialize properties */ },
				new PassengerDto { /* Initialize properties */ }
			};
			_mapperMock
				.Setup(m => m.Map<IEnumerable<PassengerDto>>(It.IsAny<IEnumerable<PassengerEntity>>()))
				.Returns(expectedData);

			// Act
			var result = await _controller.GetPassengers(cancellationToken, page, pageSize);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PagedResponse<PassengerDto>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var pagedResponse = Assert.IsType<PagedResponse<PassengerDto>>(okResult.Value);
			Assert.Equal(page, pagedResponse.PageNumber);
			Assert.Equal(pageSize, pagedResponse.PageSize);
			Assert.Equal(totalItems, pagedResponse.TotalCount);
			Assert.Equal(expectedData, pagedResponse.Data);
		}

		[Fact]
		[Trait("Category", "GetPassengers")]
		public async Task GetPassengers_ReturnsCorrectPageData()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 2;
			int pageSize = 5;
			var allPassengers = new List<PassengerEntity>
			{
				new PassengerEntity { /* Initialize properties */ },
				new PassengerEntity { /* Initialize properties */ },
				new PassengerEntity { /* Initialize properties */ },
				new PassengerEntity { /* Initialize properties */ },
				new PassengerEntity { /* Initialize properties */ },
				new PassengerEntity { /* Initialize properties */ },
				new PassengerEntity { /* Initialize properties */ },
				new PassengerEntity { /* Initialize properties */ },
				new PassengerEntity { /* Initialize properties */ },
				new PassengerEntity { /* Initialize properties */ }
			};
			var pagedPassengers = allPassengers.Skip((page - 1) * pageSize).Take(pageSize).ToList();

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_passengerServiceMock
				.Setup(service => service.GetPassengers(cancellationToken, page, pageSize))
				.ReturnsAsync(pagedPassengers);
			_passengerServiceMock
				.Setup(service => service.PassengersCount(cancellationToken, null, null))
				.ReturnsAsync(allPassengers.Count);

			var expectedData = new List<PassengerDto>
			{
				new PassengerDto { /* Initialize properties */ },
				new PassengerDto { /* Initialize properties */ },
				new PassengerDto { /* Initialize properties */ },
				new PassengerDto { /* Initialize properties */ },
				new PassengerDto { /* Initialize properties */ }
			};
			_mapperMock
				.Setup(m => m.Map<IEnumerable<PassengerDto>>(It.IsAny<IEnumerable<PassengerEntity>>()))
				.Returns(expectedData);

			// Act
			var result = await _controller.GetPassengers(cancellationToken, page, pageSize);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PagedResponse<PassengerDto>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var pagedResponse = Assert.IsType<PagedResponse<PassengerDto>>(okResult.Value);
			Assert.Equal(page, pagedResponse.PageNumber);
			Assert.Equal(pageSize, pagedResponse.PageSize);
			Assert.Equal(allPassengers.Count, pagedResponse.TotalCount);
			Assert.Equal(expectedData, pagedResponse.Data);
		}

		#endregion

		#region GetPassenger

		[Fact]
		[Trait("Category", "GetPassenger")]
		public async Task GetPassenger_InvalidId_ReturnsBadRequest()
		{
			// Arrange
			int invalidId = -1;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid input. The ID must be a non-negative integer.");

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(invalidId))
				.Returns(false);

			// Act
			var result = await _controller.GetPassenger(invalidId);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
			var badRequestResult = result.Result as BadRequestObjectResult;
			Assert.Equal(expectedBadRequestResult.Value, badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "GetPassenger")]
		public async Task GetPassenger_PassengerNotFound_ReturnsNotFound()
		{
			// Arrange
			int validId = 1;

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(validId))
				.Returns(true);
			_passengerServiceMock
				.Setup(service => service.PassengerExists(validId))
				.ReturnsAsync(false);

			// Act
			var result = await _controller.GetPassenger(validId);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetPassenger")]
		public async Task GetPassenger_ReturnsPassengerDto_WhenPassengerExists()
		{
			// Arrange
			int validId = 1;

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(validId))
				.Returns(true);
			_passengerServiceMock
				.Setup(service => service.PassengerExists(validId))
				.ReturnsAsync(true);
			_passengerServiceMock
				.Setup(service => service.GetPassenger(validId))
				.ReturnsAsync(passengerEntity);
			_mapperMock
				.Setup(m => m.Map<PassengerDto>(passengerEntity))
				.Returns(passengerDto);

			// Act
			var result = await _controller.GetPassenger(validId);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PassengerDto>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var returnedPassengerDto = Assert.IsType<PassengerDto>(okResult.Value);
			Assert.Equal(passengerDto, returnedPassengerDto);
		}

		#endregion

		#region SearchPassengers

		[Fact]
		[Trait("Category", "SearchPassengers")]
		public async Task SearchPassengers_ValidFilterWithResults_ReturnsOkWithPagedResponse()
		{
			// Arrange
			var filter = new PassengerSearchFilter { FirstName = "John" };
			var passengers = new List<PassengerEntity> { new PassengerEntity { Id = 1, FirstName = "John" } };
			var passengerDtos = new List<PassengerDto> { new PassengerDto { Id = 1, FirstName = "John" } };
			var totalItems = 1;

			_paginationValidationServiceMock
				.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_passengerServiceMock
				.Setup(s => s.SearchPassengers(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<PassengerSearchFilter>()))
				.ReturnsAsync(passengers);
			_passengerServiceMock
				.Setup(s => s.PassengersCountFilter(It.IsAny<CancellationToken>(), It.IsAny<PassengerSearchFilter>()))
				.ReturnsAsync(totalItems);
			_mapperMock
				.Setup(m => m.Map<IEnumerable<PassengerDto>>(It.IsAny<IEnumerable<PassengerEntity>>()))
				.Returns(passengerDtos);

			// Act
			var result = await _controller.SearchPassengers(new CancellationToken(), filter, 1, 10);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var pagedResponse = Assert.IsType<PagedResponse<PassengerDto>>(okResult.Value);
			Assert.NotEmpty(pagedResponse.Data);
			Assert.Equal(totalItems, pagedResponse.TotalCount);
		}

		[Fact]
		[Trait("Category", "SearchPassengers")]
		public async Task SearchPassengers_ValidFilterWithNoResults_ReturnsNotFound()
		{
			// Arrange
			var filter = new PassengerSearchFilter { Phone = "2025" };
			_paginationValidationServiceMock
				.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_passengerServiceMock
				.Setup(s => s.SearchPassengers(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<PassengerSearchFilter>()))
				.ReturnsAsync(new List<PassengerEntity>());

			// Act
			var result = await _controller.SearchPassengers(new CancellationToken(), filter, 1, 10);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "SearchPassengers")]
		public async Task SearchPassengers_EmptyFilter_ReturnsBadRequest()
		{
			// Arrange
			var filter = new PassengerSearchFilter();

			// Act
			var result = await _controller.SearchPassengers(new CancellationToken(), filter, 1, 10);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal("At least one filter criterion must be provided.", badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "SearchPassengers")]
		public async Task SearchPassengers_InvalidPagination_ReturnsBadRequestFromValidationService()
		{
			// Arrange
			var filter = new PassengerSearchFilter { FirstName = "John" };
			var badRequestResult = new BadRequestObjectResult("Invalid pagination parameters.");
			_paginationValidationServiceMock
				.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((false, 0, badRequestResult));

			// Act
			var result = await _controller.SearchPassengers(new CancellationToken(), filter, -1, 0);

			// Assert
			Assert.Same(badRequestResult, result.Result);
		}

		[Fact]
		[Trait("Category", "SearchPassengers")]
		public async Task SearchPassengers_PageSizeExceedsMax_CorrectsPageSize()
		{
			// Arrange
			var filter = new PassengerSearchFilter { FirstName = "John" };
			var passengers = new List<PassengerEntity> { new PassengerEntity { Id = 1, FirstName = "John" } };
			var passengerDtos = new List<PassengerDto> { new PassengerDto { Id = 1, FirstName = "John" } };
			var totalItems = 1;

			_paginationValidationServiceMock
				.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 50, null));
			_passengerServiceMock
				.Setup(s => s.SearchPassengers(It.IsAny<CancellationToken>(), It.IsAny<int>(), 50, It.IsAny<PassengerSearchFilter>()))
				.ReturnsAsync(passengers);
			_passengerServiceMock
				.Setup(s => s.PassengersCountFilter(It.IsAny<CancellationToken>(), It.IsAny<PassengerSearchFilter>()))
				.ReturnsAsync(totalItems);
			_mapperMock
				.Setup(m => m.Map<IEnumerable<PassengerDto>>(It.IsAny<IEnumerable<PassengerEntity>>()))
				.Returns(passengerDtos);

			// Act
			var result = await _controller.SearchPassengers(new CancellationToken(), filter, 1, 100);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var pagedResponse = Assert.IsType<PagedResponse<PassengerDto>>(okResult.Value);
			Assert.Equal(50, pagedResponse.PageSize);
		}

		[Fact]
		[Trait("Category", "SearchPassengers")]
		public async Task SearchPassengers_PassengerServiceReturnsNull_ReturnsNotFound()
		{
			// Arrange
			var filter = new PassengerSearchFilter { FirstName = "John" };
			_paginationValidationServiceMock
				.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_passengerServiceMock
				.Setup(s => s.SearchPassengers(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<PassengerSearchFilter>()))
				.ReturnsAsync((List<PassengerEntity>)null);

			// Act
			var result = await _controller.SearchPassengers(new CancellationToken(), filter, 1, 10);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		#endregion

		#region PostPassenger

		[Fact]
		[Trait("Category", "PostPassenger")]
		public async Task PostPassenger_ReturnsCreatedAtActionResult_WhenPassengerIsCreatedSuccessfully()
		{
			// Arrange
			var passengerCreateDto = new PassengerCreateDto();
			var passengerEntity = new PassengerEntity { Id = 1 };
			var passengerDto = new PassengerDto { Id = 1 };

			_mapperMock.Setup(m => m.Map<PassengerEntity>(passengerCreateDto)).Returns(passengerEntity);
			_mapperMock.Setup(m => m.Map<PassengerDto>(passengerEntity)).Returns(passengerDto);
			_passengerServiceMock.Setup(service => service.PostPassenger(passengerEntity))
							   .ReturnsAsync(passengerEntity);

			// Act
			var result = await _controller.PostPassenger(passengerCreateDto);

			// Assert
			var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
			var returnedValue = Assert.IsType<PassengerDto>(actionResult.Value);
			Assert.Equal(passengerDto.Id, returnedValue.Id);
			Assert.Equal("GetPassenger", actionResult.ActionName);
			Assert.Equal(1, actionResult.RouteValues["id"]);
		}

		[Fact]
		[Trait("Category", "PostPassenger")]
		public async Task PostPassenger_ThrowsException_WhenServiceFails()
		{
			// Arrange
			var passengerCreateDto = new PassengerCreateDto();
			var passengerEntity = new PassengerEntity();
			_mapperMock.Setup(m => m.Map<PassengerEntity>(passengerCreateDto)).Returns(passengerEntity);

			// Set up the service to throw an exception
			_passengerServiceMock.Setup(service => service.PostPassenger(passengerEntity))
							   .ThrowsAsync(new Exception("Simulated exception"));

			// Act & Assert
			await Assert.ThrowsAsync<Exception>(async () => await _controller.PostPassenger(passengerCreateDto));
		}

		[Fact]
		[Trait("Category", "PostPassenger")]
		public async Task PostPassenger_ReturnsConflict_WhenPassengerExistsByUPRN()
		{
			// Arrange
			var passengerCreateDto = new PassengerCreateDto { UPRN = "12345", Passport = "ABC" };

			_passengerServiceMock.Setup(service => service.ExistsByUPRN(passengerCreateDto.UPRN))
							   .ReturnsAsync(true);
			_passengerServiceMock.Setup(service => service.ExistsByPassport(passengerCreateDto.Passport))
							   .ReturnsAsync(false);
			_passengerServiceMock.Setup(service => service.PostPassenger(It.IsAny<PassengerEntity>()))
							   .Verifiable();

			// Act
			var result = await _controller.PostPassenger(passengerCreateDto);

			// Assert
			Assert.IsType<ConflictObjectResult>(result.Result);
			_passengerServiceMock.Verify(service => service.PostPassenger(It.IsAny<PassengerEntity>()), Times.Never);
		}

		[Fact]
		[Trait("Category", "PostPassenger")]
		public async Task PostPassenger_ReturnsConflict_WhenPassengerExistsByPassport()
		{
			// Arrange
			var passengerCreateDto = new PassengerCreateDto { UPRN = "12345", Passport = "ABC" };

			_passengerServiceMock.Setup(service => service.ExistsByUPRN(passengerCreateDto.UPRN))
							   .ReturnsAsync(false);
			_passengerServiceMock.Setup(service => service.ExistsByPassport(passengerCreateDto.Passport))
							   .ReturnsAsync(true);
			_passengerServiceMock.Setup(service => service.PostPassenger(It.IsAny<PassengerEntity>()))
							   .Verifiable();

			// Act
			var result = await _controller.PostPassenger(passengerCreateDto);

			// Assert
			Assert.IsType<ConflictObjectResult>(result.Result);
			_passengerServiceMock.Verify(service => service.PostPassenger(It.IsAny<PassengerEntity>()), Times.Never);
		}

		#endregion

		#region PutPassenger

		[Fact]
		[Trait("Category", "PutPassenger")]
		public async Task PutPassenger_ReturnsNoContent_WhenUpdateIsSuccessful()
		{
			// Arrange
			int id = 1;
			var passengerDto = new PassengerDto { Id = id };
			var passengerEntity = new PassengerEntity { Id = id };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_mapperMock.Setup(m => m.Map<PassengerEntity>(passengerDto)).Returns(passengerEntity);
			_passengerServiceMock.Setup(service => service.PassengerExists(id)).ReturnsAsync(true);
			_passengerServiceMock.Setup(service => service.PutPassenger(passengerEntity)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.PutPassenger(id, passengerDto);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "PutPassenger")]
		public async Task PutPassenger_ReturnsBadRequest_WhenIdIsInvalid()
		{
			// Arrange
			int invalidId = -1;
			var passengerDto = new PassengerDto { Id = invalidId };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(invalidId)).Returns(false);

			// Act
			var result = await _controller.PutPassenger(invalidId, passengerDto);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Invalid input. The ID must be a non-negative integer.", badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "PutPassenger")]
		public async Task PutPassenger_ReturnsBadRequest_WhenIdInDtoDoesNotMatchIdInUrl()
		{
			// Arrange
			int id = 1;
			var passengerDto = new PassengerDto { Id = 2 };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);

			// Act
			var result = await _controller.PutPassenger(id, passengerDto);

			// Assert
			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		[Trait("Category", "PutPassenger")]
		public async Task PutPassenger_ReturnsNotFound_WhenPassengerDoesNotExist()
		{
			// Arrange
			int id = 1;
			var passengerDto = new PassengerDto { Id = id };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_passengerServiceMock.Setup(service => service.PassengerExists(id)).ReturnsAsync(false);

			// Act
			var result = await _controller.PutPassenger(id, passengerDto);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		#endregion

		#region PatchPassenger

		[Fact]
		[Trait("Category", "PatchPassenger")]
		public async Task PatchPassenger_ReturnsOk_WhenUpdateIsSuccessful()
		{
			// Arrange
			int id = 1;
			var passengerDocument = new JsonPatchDocument();
			var updatedPassenger = new PassengerEntity { Id = id };
			var passengerDto = new PassengerDto { Id = id };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_passengerServiceMock.Setup(service => service.PassengerExists(id)).ReturnsAsync(true);
			_passengerServiceMock.Setup(service => service.PatchPassenger(id, passengerDocument)).ReturnsAsync(updatedPassenger);
			_mapperMock.Setup(m => m.Map<PassengerDto>(updatedPassenger)).Returns(passengerDto);

			// Act
			var result = await _controller.PatchPassenger(id, passengerDocument);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(passengerDto, okResult.Value);
		}

		[Fact]
		[Trait("Category", "PatchPassenger")]
		public async Task PatchPassenger_ReturnsBadRequest_WhenIdIsInvalid()
		{
			// Arrange
			int invalidId = -1;
			var passengerDocument = new JsonPatchDocument();

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(invalidId)).Returns(false);

			// Act
			var result = await _controller.PatchPassenger(invalidId, passengerDocument);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Invalid input. The ID must be a non-negative integer.", badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "PatchPassenger")]
		public async Task PatchPassenger_ReturnsNotFound_WhenPassengerDoesNotExist()
		{
			// Arrange
			int id = 1;
			var passengerDocument = new JsonPatchDocument();

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_passengerServiceMock.Setup(service => service.PassengerExists(id)).ReturnsAsync(false);

			// Act
			var result = await _controller.PatchPassenger(id, passengerDocument);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		#endregion

		#region DeletePassenger

		[Fact]
		[Trait("Category", "DeletePassenger")]
		public async Task DeletePassenger_ReturnsNoContent_WhenDeletionIsSuccessful()
		{
			// Arrange
			int id = 1;
			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_passengerServiceMock.Setup(service => service.PassengerExists(id)).ReturnsAsync(true);
			_passengerServiceMock.Setup(service => service.DeletePassenger(id)).ReturnsAsync(true);

			// Act
			var result = await _controller.DeletePassenger(id);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "DeletePassenger")]
		public async Task DeletePassenger_ReturnsBadRequest_WhenIdIsInvalid()
		{
			// Arrange
			int invalidId = -1;
			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(invalidId)).Returns(false);

			// Act
			var result = await _controller.DeletePassenger(invalidId);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Invalid input. The ID must be a non-negative integer.", badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "DeletePassenger")]
		public async Task DeletePassenger_ReturnsNotFound_WhenPassengerDoesNotExist()
		{
			// Arrange
			int id = 1;
			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_passengerServiceMock.Setup(service => service.PassengerExists(id)).ReturnsAsync(false);

			// Act
			var result = await _controller.DeletePassenger(id);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "DeletePassenger")]
		public async Task DeletePassenger_ReturnsConflict_WhenPassengerCannotBeDeleted()
		{
			// Arrange
			int id = 1;
			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_passengerServiceMock.Setup(service => service.PassengerExists(id)).ReturnsAsync(true);
			_passengerServiceMock.Setup(service => service.DeletePassenger(id)).ReturnsAsync(false);

			// Act
			var result = await _controller.DeletePassenger(id);

			// Assert
			var conflictResult = Assert.IsType<ConflictObjectResult>(result);
			Assert.Equal("Passenger cannot be deleted because it is being referenced by other entities.", conflictResult.Value);
		}

		#endregion

		#region ExportToPdf

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_GetAllPassengers_ReturnsFileResult()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var passengers = new List<PassengerEntity> { new PassengerEntity() };
			var pdfBytes = new byte[] { 1, 2, 3 };
			var fileName = "Passengers_test.pdf";

			_passengerServiceMock.Setup(x => x.GetAllPassengers(cancellationToken)).ReturnsAsync(passengers);
			_exportServiceMock.Setup(x => x.ExportToPDF("Passengers", passengers)).Returns(pdfBytes);
			_utilityServiceMock.Setup(x => x.GenerateUniqueFileName("Passengers", FileExtension.Pdf)).Returns(fileName);

			// Act
			var result = await _controller.ExportToPdf(cancellationToken, null, It.IsAny<int>(), It.IsAny<int>(), getAll: true);

			// Assert
			var fileResult = Assert.IsType<FileContentResult>(result);
			Assert.Equal("application/pdf", fileResult.ContentType);
			Assert.Equal(fileName, fileResult.FileDownloadName);
			Assert.Equal(pdfBytes, fileResult.FileContents);
		}

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_ValidFilter_ReturnsFileResult()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var filter = new PassengerSearchFilter { FirstName = "John" };
			var passengers = new List<PassengerEntity> { new PassengerEntity() };
			var pdfBytes = new byte[] { 1, 2, 3 };
			var fileName = "Passengers_test.pdf";

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_passengerServiceMock
				.Setup(x => x.SearchPassengers(cancellationToken, It.IsAny<int>(), It.IsAny<int>(), filter))
				.ReturnsAsync(passengers);
			_exportServiceMock.Setup(x => x.ExportToPDF("Passengers", passengers)).Returns(pdfBytes);
			_utilityServiceMock.Setup(x => x.GenerateUniqueFileName("Passengers", FileExtension.Pdf)).Returns(fileName);

			// Act
			var result = await _controller.ExportToPdf(cancellationToken, filter);

			// Assert
			var fileResult = Assert.IsType<FileContentResult>(result);
			Assert.Equal("application/pdf", fileResult.ContentType);
			Assert.Equal(fileName, fileResult.FileDownloadName);
			Assert.Equal(pdfBytes, fileResult.FileContents);
		}

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_NoFilter_ReturnsFileResult()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var passengers = new List<PassengerEntity> { new PassengerEntity() };
			var pdfBytes = new byte[] { 1, 2, 3 };
			var fileName = "Passengers_test.pdf";

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_passengerServiceMock
				.Setup(x => x.GetPassengers(cancellationToken, It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(passengers);
			_exportServiceMock.Setup(x => x.ExportToPDF("Passengers", passengers)).Returns(pdfBytes);
			_utilityServiceMock.Setup(x => x.GenerateUniqueFileName("Passengers", FileExtension.Pdf)).Returns(fileName);

			// Act
			var result = await _controller.ExportToPdf(cancellationToken, null, It.IsAny<int>(), It.IsAny<int>(), false);

			// Assert
			var fileResult = Assert.IsType<FileContentResult>(result);
			Assert.Equal("application/pdf", fileResult.ContentType);
			Assert.Equal(fileName, fileResult.FileDownloadName);
			Assert.Equal(pdfBytes, fileResult.FileContents);
		}

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_InvalidPaginationParameters_ReturnsBadRequest()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid pagination parameters.");
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((false, 0, expectedBadRequestResult));

			// Act
			var result = await _controller.ExportToPdf(cancellationToken, null, page: -1, pageSize: 0, false);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_GetAll_NoPassengersFound_ReturnsNoContent()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var passengers = new List<PassengerEntity>();

			_passengerServiceMock.Setup(x => x.GetAllPassengers(cancellationToken)).ReturnsAsync(passengers);

			// Act
			var result = await _controller.ExportToPdf(cancellationToken, null, It.IsAny<int>(), It.IsAny<int>(), getAll: true);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_GetAll_ServiceReturnsNull_ReturnsNoContent()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			_passengerServiceMock.Setup(x => x.GetAllPassengers(cancellationToken)).ReturnsAsync((List<PassengerEntity>)null);

			// Act
			var result = await _controller.ExportToPdf(cancellationToken, null, It.IsAny<int>(), It.IsAny<int>(), getAll: true);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_WithFilter_NoPassengersFound_ReturnsNoContent()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var filter = new PassengerSearchFilter { FirstName = "NonExistent" };
			var passengers = new List<PassengerEntity>();

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_passengerServiceMock
				.Setup(x => x.SearchPassengers(cancellationToken, It.IsAny<int>(), It.IsAny<int>(), filter))
				.ReturnsAsync(passengers);

			// Act
			var result = await _controller.ExportToPdf(cancellationToken, filter);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_WithFilter_ServiceReturnsNull_ReturnsNoContent()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var filter = new PassengerSearchFilter { FirstName = "John" };
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_passengerServiceMock
				.Setup(x => x.SearchPassengers(cancellationToken, It.IsAny<int>(), It.IsAny<int>(), filter))
				.ReturnsAsync((List<PassengerEntity>)null);

			// Act
			var result = await _controller.ExportToPdf(cancellationToken, filter);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_NoFilter_NoPassengersFound_ReturnsNoContent()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var passengers = new List<PassengerEntity>();

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_passengerServiceMock.Setup(x => x.GetPassengers(cancellationToken, It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(passengers);

			// Act
			var result = await _controller.ExportToPdf(cancellationToken, null, It.IsAny<int>(), It.IsAny<int>(), false);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_NoFilter_ServiceReturnsNull_ReturnsNoContent()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_passengerServiceMock.Setup(x => x.GetPassengers(cancellationToken, It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((List<PassengerEntity>)null);

			// Act
			var result = await _controller.ExportToPdf(cancellationToken, null, It.IsAny<int>(), It.IsAny<int>(), false);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_PdfGenerationFails_ReturnsInternalServerError()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var passengers = new List<PassengerEntity> { new PassengerEntity() };

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_passengerServiceMock.Setup(x => x.GetPassengers(cancellationToken, It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(passengers);
			_exportServiceMock.Setup(x => x.ExportToPDF("Passengers", passengers)).Returns((byte[])null);

			// Act
			var result = await _controller.ExportToPdf(cancellationToken, null, It.IsAny<int>(), It.IsAny<int>(), false);

			// Assert
			var statusCodeResult = Assert.IsType<ObjectResult>(result);
			Assert.Equal(500, statusCodeResult.StatusCode);
			Assert.Equal("Failed to generate PDF file.", statusCodeResult.Value);
		}

		#endregion

		#region ExportToExcel

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_GetAll_ReturnsFileResult()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var fileName = "Passengers_test.xlsx";

			_passengerServiceMock.Setup(x => x.GetAllPassengers(cancellationToken)).ReturnsAsync(_samplePassengers);
			_exportServiceMock.Setup(x => x.ExportToExcel("Passengers", _samplePassengers)).Returns(_sampleExcelBytes);
			_utilityServiceMock.Setup(x => x.GenerateUniqueFileName("Passengers", FileExtension.Xlsx)).Returns(fileName);

			// Act
			var result = await _controller.ExportToExcel(cancellationToken, new PassengerSearchFilter(), getAll: true);

			// Assert
			var fileResult = Assert.IsType<FileContentResult>(result);
			Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileResult.ContentType);
			Assert.Equal(fileName, fileResult.FileDownloadName);
			Assert.Equal(_sampleExcelBytes, fileResult.FileContents);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_ValidFilter_ReturnsFileResult()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var filter = new PassengerSearchFilter { FirstName = "John" };
			var fileName = "Passengers_test.xlsx";

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_passengerServiceMock
				.Setup(x => x.SearchPassengers(cancellationToken, It.IsAny<int>(), It.IsAny<int>(), filter))
				.ReturnsAsync(_samplePassengers);
			_exportServiceMock.Setup(x => x.ExportToExcel("Passengers", _samplePassengers)).Returns(_sampleExcelBytes);
			_utilityServiceMock.Setup(x => x.GenerateUniqueFileName("Passengers", FileExtension.Xlsx)).Returns(fileName);

			// Act
			var result = await _controller.ExportToExcel(cancellationToken, filter);

			// Assert
			var fileResult = Assert.IsType<FileContentResult>(result);
			Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileResult.ContentType);
			Assert.Equal(fileName, fileResult.FileDownloadName);
			Assert.Equal(_sampleExcelBytes, fileResult.FileContents);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_NoFilter_ReturnsFileResult()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var filter = new PassengerSearchFilter();
			var fileName = "Passengers_test.xlsx";

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_passengerServiceMock
				.Setup(x => x.GetPassengers(cancellationToken, It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(_samplePassengers);
			_exportServiceMock.Setup(x => x.ExportToExcel("Passengers", _samplePassengers)).Returns(_sampleExcelBytes);
			_utilityServiceMock.Setup(x => x.GenerateUniqueFileName("Passengers", FileExtension.Xlsx)).Returns(fileName);

			// Act
			var result = await _controller.ExportToExcel(cancellationToken, filter);

			// Assert
			var fileResult = Assert.IsType<FileContentResult>(result);
			Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileResult.ContentType);
			Assert.Equal(fileName, fileResult.FileDownloadName);
			Assert.Equal(_sampleExcelBytes, fileResult.FileContents);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_InvalidPaginationParameters_ReturnsBadRequest()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid pagination parameters.");
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((false, 0, expectedBadRequestResult));

			// Act
			var result = await _controller.ExportToExcel(cancellationToken, new PassengerSearchFilter(), page: -1, pageSize: 0);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_GetAll_NoPassengersFound_ReturnsNoContent()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var passengers = new List<PassengerEntity>();

			_passengerServiceMock.Setup(x => x.GetAllPassengers(cancellationToken)).ReturnsAsync(passengers);

			// Act
			var result = await _controller.ExportToExcel(cancellationToken, new PassengerSearchFilter(), getAll: true);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_WithFilter_NoPassengersFound_ReturnsNoContent()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var filter = new PassengerSearchFilter { FirstName = "NonExistent" };
			var passengers = new List<PassengerEntity>();

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_passengerServiceMock
				.Setup(x => x.SearchPassengers(cancellationToken, It.IsAny<int>(), It.IsAny<int>(), filter))
				.ReturnsAsync(passengers);

			// Act
			var result = await _controller.ExportToExcel(cancellationToken, filter);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_NoFilter_NoPassengersFound_ReturnsNoContent()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var filter = new PassengerSearchFilter();
			var passengers = new List<PassengerEntity>();

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_passengerServiceMock.Setup(x => x.GetPassengers(cancellationToken, It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(passengers);

			// Act
			var result = await _controller.ExportToExcel(cancellationToken, filter);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_ServiceReturnsNull_ReturnsNoContent()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var filter = new PassengerSearchFilter();

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_passengerServiceMock.Setup(x => x.GetPassengers(cancellationToken, It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((List<PassengerEntity>)null);

			// Act
			var result = await _controller.ExportToExcel(cancellationToken, filter);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_GenerationFails_ReturnsInternalServerError()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var passengers = new List<PassengerEntity> { new PassengerEntity() };

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_passengerServiceMock.Setup(x => x.GetPassengers(cancellationToken, It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(passengers);
			_exportServiceMock.Setup(x => x.ExportToExcel("Passengers", passengers)).Returns((byte[])null);

			// Act
			var result = await _controller.ExportToExcel(cancellationToken, new PassengerSearchFilter());

			// Assert
			var statusCodeResult = Assert.IsType<ObjectResult>(result);
			Assert.Equal(500, statusCodeResult.StatusCode);
			Assert.Equal("Failed to generate Excel file.", statusCodeResult.Value);
		}

		#endregion

	}
}