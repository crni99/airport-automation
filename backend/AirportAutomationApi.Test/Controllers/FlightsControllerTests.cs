using AirportAutomation.Api.Interfaces;
using AirportAutomation.Application.Dtos.Flight;
using AirportAutomation.Application.Dtos.Response;
using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IServices;
using AirportAutomation.Api.Controllers;
using AirportAutomation.Api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace AirportAutomationApi.Test.Controllers
{

	public class FlightsControllerTests
	{
		private readonly FlightsController _controller;
		private readonly Mock<IFlightService> _flightServiceMock;
		private readonly Mock<IPaginationValidationService> _paginationValidationServiceMock;
		private readonly Mock<IInputValidationService> _inputValidationServiceMock;
		private readonly Mock<IUtilityService> _utilityServiceMock;
		private readonly Mock<IExportService> _exportServiceMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<ILogger<FlightsController>> _loggerMock;
		private readonly Mock<IConfiguration> _configurationMock;

		private readonly FlightEntity flightEntity = new()
		{
			Id = 1,
			DepartureDate = new DateOnly(2023, 09, 20),
			DepartureTime = new TimeOnly(09, 51, 00),
			AirlineId = 1,
			DestinationId = 1,
			PilotId = 1
		};

		private readonly FlightDto flightDto = new()
		{
			Id = 1,
			DepartureDate = new DateOnly(2023, 09, 20),
			DepartureTime = new TimeOnly(09, 51, 00),
			AirlineId = 1,
			DestinationId = 1,
			PilotId = 1
		};

		public FlightsControllerTests()
		{
			_flightServiceMock = new Mock<IFlightService>();
			_paginationValidationServiceMock = new Mock<IPaginationValidationService>();
			_inputValidationServiceMock = new Mock<IInputValidationService>();
			_utilityServiceMock = new Mock<IUtilityService>();
			_exportServiceMock = new Mock<IExportService>();
			_mapperMock = new Mock<IMapper>();
			_loggerMock = new Mock<ILogger<FlightsController>>();
			_configurationMock = new Mock<IConfiguration>();
			var configBuilder = new ConfigurationBuilder();
			configBuilder.AddInMemoryCollection(new Dictionary<string, string>
			{
				{"pageSettings:maxPageSize", "10"}
			});
			_configurationMock.Setup(x => x.GetSection(It.IsAny<string>()))
				.Returns(configBuilder.Build().GetSection(""));

			_controller = new FlightsController(
				_flightServiceMock.Object,
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
		[Trait("Category", "GetFlights")]
		public async Task GetFlights_InvalidPaginationParameters_ReturnsBadRequest()
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
			var result = await _controller.GetFlights(cancellationToken, invalidPage, invalidPageSize);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetFlights")]
		public async Task GetFlights_ReturnsNoContent_WhenNoFlightsFound()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_flightServiceMock.Setup(service => service.GetFlights(cancellationToken, It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(new List<FlightEntity>());

			// Act
			var result = await _controller.GetFlights(cancellationToken, page, pageSize);

			// Assert
			Assert.IsType<NoContentResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetFlights")]
		public async Task GetFlights_ReturnsInternalServerError_WhenExceptionThrown()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_flightServiceMock.Setup(service => service.GetFlights(cancellationToken, It.IsAny<int>(), It.IsAny<int>()))
				.ThrowsAsync(new Exception("Simulated exception"));

			// Act & Assert
			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetFlights(cancellationToken, page, pageSize));
		}

		[Fact]
		[Trait("Category", "GetFlights")]
		public async Task GetFlights_ReturnsOk_WithPaginatedFlights()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;
			var flights = new List<FlightEntity>
			{
				new FlightEntity { /* Initialize properties */ },
				new FlightEntity { /* Initialize properties */ }
			};
			var totalItems = 2;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_flightServiceMock
				.Setup(service => service.GetFlights(cancellationToken, page, pageSize))
				.ReturnsAsync(flights);
			_flightServiceMock
				.Setup(service => service.FlightsCount(cancellationToken, null, null))
				.ReturnsAsync(totalItems);

			var expectedData = new List<FlightDto>
			{
				new FlightDto { /* Initialize properties */ },
				new FlightDto { /* Initialize properties */ }
			};
			_mapperMock
				.Setup(m => m.Map<IEnumerable<FlightDto>>(It.IsAny<IEnumerable<FlightEntity>>()))
				.Returns(expectedData);

			// Act
			var result = await _controller.GetFlights(cancellationToken, page, pageSize);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PagedResponse<FlightDto>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var pagedResponse = Assert.IsType<PagedResponse<FlightDto>>(okResult.Value);
			Assert.Equal(page, pagedResponse.PageNumber);
			Assert.Equal(pageSize, pagedResponse.PageSize);
			Assert.Equal(totalItems, pagedResponse.TotalCount);
			Assert.Equal(expectedData, pagedResponse.Data);
		}

		[Fact]
		[Trait("Category", "GetFlights")]
		public async Task GetFlights_ReturnsCorrectPageData()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 2;
			int pageSize = 5;
			var allFlights = new List<FlightEntity>
			{
				new FlightEntity { /* Initialize properties */ },
				new FlightEntity { /* Initialize properties */ },
				new FlightEntity { /* Initialize properties */ },
				new FlightEntity { /* Initialize properties */ },
				new FlightEntity { /* Initialize properties */ },
				new FlightEntity { /* Initialize properties */ },
				new FlightEntity { /* Initialize properties */ },
				new FlightEntity { /* Initialize properties */ },
				new FlightEntity { /* Initialize properties */ },
				new FlightEntity { /* Initialize properties */ }
			};
			var pagedFlights = allFlights.Skip((page - 1) * pageSize).Take(pageSize).ToList();

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_flightServiceMock
				.Setup(service => service.GetFlights(cancellationToken, page, pageSize))
				.ReturnsAsync(pagedFlights);
			_flightServiceMock
				.Setup(service => service.FlightsCount(cancellationToken, null, null))
				.ReturnsAsync(allFlights.Count);

			var expectedData = new List<FlightDto>
			{
				new FlightDto { /* Initialize properties */ },
				new FlightDto { /* Initialize properties */ },
				new FlightDto { /* Initialize properties */ },
				new FlightDto { /* Initialize properties */ },
				new FlightDto { /* Initialize properties */ }
			};
			_mapperMock
				.Setup(m => m.Map<IEnumerable<FlightDto>>(It.IsAny<IEnumerable<FlightEntity>>()))
				.Returns(expectedData);

			// Act
			var result = await _controller.GetFlights(cancellationToken, page, pageSize);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PagedResponse<FlightDto>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var pagedResponse = Assert.IsType<PagedResponse<FlightDto>>(okResult.Value);
			Assert.Equal(page, pagedResponse.PageNumber);
			Assert.Equal(pageSize, pagedResponse.PageSize);
			Assert.Equal(allFlights.Count, pagedResponse.TotalCount);
			Assert.Equal(expectedData, pagedResponse.Data);
		}

		[Fact]
		[Trait("Category", "GetFlight")]
		public async Task GetFlight_InvalidId_ReturnsBadRequest()
		{
			// Arrange
			int invalidId = -1;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid input. The ID must be a non-negative integer.");

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(invalidId))
				.Returns(false);

			// Act
			var result = await _controller.GetFlight(invalidId);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
			var badRequestResult = result.Result as BadRequestObjectResult;
			Assert.Equal(expectedBadRequestResult.Value, badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "GetFlight")]
		public async Task GetFlight_FlightNotFound_ReturnsNotFound()
		{
			// Arrange
			int validId = 1;

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(validId))
				.Returns(true);
			_flightServiceMock
				.Setup(service => service.FlightExists(validId))
				.ReturnsAsync(false);

			// Act
			var result = await _controller.GetFlight(validId);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetFlight")]
		public async Task GetFlight_ReturnsFlightDto_WhenFlightExists()
		{
			// Arrange
			int validId = 1;

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(validId))
				.Returns(true);
			_flightServiceMock
				.Setup(service => service.FlightExists(validId))
				.ReturnsAsync(true);
			_flightServiceMock
				.Setup(service => service.GetFlight(validId))
				.ReturnsAsync(flightEntity);
			_mapperMock
				.Setup(m => m.Map<FlightDto>(flightEntity))
				.Returns(flightDto);

			// Act
			var result = await _controller.GetFlight(validId);

			// Assert
			var actionResult = Assert.IsType<ActionResult<FlightDto>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var returnedFlightDto = Assert.IsType<FlightDto>(okResult.Value);
			Assert.Equal(flightDto, returnedFlightDto);
		}

		[Fact]
		[Trait("Category", "GetFlightsBetweenDates")]
		public async Task GetFlightsBetweenDates_NoDatesProvided_ReturnsBadRequest()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			DateOnly? startDate = null;
			DateOnly? endDate = null;
			var expectedErrorMessage = "Both start date and end date are missing in the request.";

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, new OkResult()));

			// Act
			var result = await _controller.GetFlightsBetweenDates(cancellationToken, startDate, endDate);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal(expectedErrorMessage, badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "GetFlightsBetweenDates")]
		public async Task GetFlightsBetweenDates_InvalidStartDateProvided_ReturnsBadRequest()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var startDate = new DateOnly(1000, 1, 1);
			var endDate = new DateOnly(2024, 12, 1);
			var expectedErrorMessage = "Invalid input. The start and end dates must be valid dates.";

			// Mocks
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, new OkResult()));
			_inputValidationServiceMock
				.Setup(x => x.IsValidDateOnly(startDate))
				.Returns(false); // invalid start date
			_inputValidationServiceMock
				.Setup(x => x.IsValidDateOnly(endDate))
				.Returns(true); // valid end date

			// Act
			var result = await _controller.GetFlightsBetweenDates(cancellationToken, startDate, endDate);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal(expectedErrorMessage, badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "GetFlightsBetweenDates")]
		public async Task GetFlightsBetweenDates_InvalidEndDateProvided_ReturnsBadRequest()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var startDate = new DateOnly(2024, 1, 1);
			var endDate = new DateOnly(1000, 12, 1);
			var expectedErrorMessage = "Invalid input. The start and end dates must be valid dates.";

			// Mocks
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, new OkResult()));
			_inputValidationServiceMock
				.Setup(x => x.IsValidDateOnly(startDate))
				.Returns(true); // valid start date
			_inputValidationServiceMock
				.Setup(x => x.IsValidDateOnly(endDate))
				.Returns(false); // invalid end date

			// Act
			var result = await _controller.GetFlightsBetweenDates(cancellationToken, startDate, endDate);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal(expectedErrorMessage, badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "GetFlightsBetweenDates")]
		public async Task GetFlightsBetweenDates_InvalidPaginationParameters_ReturnsBadRequest()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			DateOnly startDate = new DateOnly();
			int invalidPage = -1;
			int invalidPageSize = 0;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid pagination parameters.");

			_inputValidationServiceMock
				.Setup(x => x.IsValidDateOnly(startDate))
				.Returns(true);
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(invalidPage, invalidPageSize, It.IsAny<int>()))
				.Returns((false, 0, expectedBadRequestResult));

			// Act
			var result = await _controller.GetFlightsBetweenDates(cancellationToken, startDate, null, invalidPage, invalidPageSize);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetFlightsBetweenDates")]
		public async Task GetFlightsBetweenDates_ValidDates_NoFlightsFound_ReturnsNotFound()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var startDate = new DateOnly(2024, 1, 1);
			var endDate = new DateOnly(2024, 1, 31);

			// Mocks
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, new OkResult()));
			_inputValidationServiceMock
				.Setup(x => x.IsValidDateOnly(startDate))
				.Returns(true);
			_inputValidationServiceMock
				.Setup(x => x.IsValidDateOnly(endDate))
				.Returns(true);
			_flightServiceMock
				.Setup(x => x.GetFlightsBetweenDates(cancellationToken, It.IsAny<int>(), It.IsAny<int>(), startDate, endDate))
				.ReturnsAsync(new List<FlightEntity>());
			_flightServiceMock
				.Setup(x => x.FlightsCount(cancellationToken, startDate, endDate))
				.ReturnsAsync(0);

			// Act
			var result = await _controller.GetFlightsBetweenDates(cancellationToken, startDate, endDate);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
			var flights = await _flightServiceMock.Object.GetFlightsBetweenDates(cancellationToken, 1, 10, startDate, endDate);
			Assert.Empty(flights);
		}

		[Fact]
		[Trait("Category", "GetFlightsBetweenDates")]
		public async Task GetFlightsBetweenDates_ReturnsPagedListOfFlights_WhenFlightsFound()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			DateOnly startDate = new DateOnly();
			DateOnly endDate = new DateOnly();
			int validPage = 1;
			int validPageSize = 10;
			var flightEntities = new List<FlightEntity> { flightEntity };
			var flightDtos = new List<FlightDto> { flightDto };
			var totalItems = 1;

			_inputValidationServiceMock
				.Setup(x => x.IsValidDateOnly(startDate))
				.Returns(true);
			_inputValidationServiceMock
				.Setup(x => x.IsValidDateOnly(endDate))
				.Returns(true);
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(validPage, validPageSize, It.IsAny<int>()))
				.Returns((true, validPageSize, null));
			_flightServiceMock
				.Setup(service => service.GetFlightsBetweenDates(cancellationToken, validPage, validPageSize, startDate, endDate))
				.ReturnsAsync(flightEntities);
			_flightServiceMock
				.Setup(service => service.FlightsCount(cancellationToken, startDate, endDate))
				.ReturnsAsync(totalItems);
			_mapperMock
				.Setup(m => m.Map<IEnumerable<FlightDto>>(flightEntities))
				.Returns(flightDtos);

			// Act
			var result = await _controller.GetFlightsBetweenDates(cancellationToken, startDate, endDate, validPage, validPageSize);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PagedResponse<FlightDto>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var response = Assert.IsType<PagedResponse<FlightDto>>(okResult.Value);
			Assert.Equal(validPage, response.PageNumber);
			Assert.Equal(validPageSize, response.PageSize);
			Assert.Equal(totalItems, response.TotalCount);
			Assert.Equal(flightDtos, response.Data);
		}

		[Fact]
		[Trait("Category", "PostFlight")]
		public async Task PostFlight_ReturnsCreatedAtActionResult_WhenFlightIsCreatedSuccessfully()
		{
			// Arrange
			var flightCreateDto = new FlightCreateDto();
			var flightEntity = new FlightEntity { Id = 1 };
			var flightDto = new FlightDto { Id = 1 };

			// Set up the mapper to return the expected values
			_mapperMock.Setup(m => m.Map<FlightEntity>(flightCreateDto)).Returns(flightEntity);
			_mapperMock.Setup(m => m.Map<FlightDto>(flightEntity)).Returns(flightDto);

			// Adjust service setup to return the flightEntity wrapped in a Task
			_flightServiceMock.Setup(service => service.PostFlight(flightEntity))
							   .ReturnsAsync(flightEntity);

			// Act
			var result = await _controller.PostFlight(flightCreateDto);

			// Assert
			var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
			var returnedValue = Assert.IsType<FlightDto>(actionResult.Value);
			Assert.Equal(flightDto.Id, returnedValue.Id);
			Assert.Equal("GetFlight", actionResult.ActionName);
			Assert.Equal(1, actionResult.RouteValues["id"]);
		}

		[Fact]
		[Trait("Category", "PostFlight")]
		public async Task PostFlight_ThrowsException_WhenServiceFails()
		{
			// Arrange
			var flightCreateDto = new FlightCreateDto();
			var flightEntity = new FlightEntity();
			_mapperMock.Setup(m => m.Map<FlightEntity>(flightCreateDto)).Returns(flightEntity);

			// Set up the service to throw an exception
			_flightServiceMock.Setup(service => service.PostFlight(flightEntity))
							   .ThrowsAsync(new Exception("Simulated exception"));

			// Act & Assert
			await Assert.ThrowsAsync<Exception>(async () => await _controller.PostFlight(flightCreateDto));
		}

		[Fact]
		[Trait("Category", "PutFlight")]
		public async Task PutFlight_ReturnsNoContent_WhenUpdateIsSuccessful()
		{
			// Arrange
			int id = 1;
			var flightUpdateDto = new FlightUpdateDto { Id = id };
			var flightEntity = new FlightEntity { Id = id };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_mapperMock.Setup(m => m.Map<FlightEntity>(flightDto)).Returns(flightEntity);
			_flightServiceMock.Setup(service => service.FlightExists(id)).ReturnsAsync(true);
			_flightServiceMock.Setup(service => service.PutFlight(flightEntity)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.PutFlight(id, flightUpdateDto);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "PutFlight")]
		public async Task PutFlight_ReturnsBadRequest_WhenIdIsInvalid()
		{
			// Arrange
			int invalidId = -1;
			var flightUpdateDto = new FlightUpdateDto { Id = invalidId };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(invalidId)).Returns(false);

			// Act
			var result = await _controller.PutFlight(invalidId, flightUpdateDto);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Invalid input. The ID must be a non-negative integer.", badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "PutFlight")]
		public async Task PutFlight_ReturnsBadRequest_WhenIdInDtoDoesNotMatchIdInUrl()
		{
			// Arrange
			int id = 1;
			var flightUpdateDto = new FlightUpdateDto { Id = 2 };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);

			// Act
			var result = await _controller.PutFlight(id, flightUpdateDto);

			// Assert
			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		[Trait("Category", "PutFlight")]
		public async Task PutFlight_ReturnsNotFound_WhenFlightDoesNotExist()
		{
			// Arrange
			int id = 1;
			var flightUpdateDto = new FlightUpdateDto { Id = id };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_flightServiceMock.Setup(service => service.FlightExists(id)).ReturnsAsync(false);

			// Act
			var result = await _controller.PutFlight(id, flightUpdateDto);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "PatchFlight")]
		public async Task PatchFlight_ReturnsOk_WhenUpdateIsSuccessful()
		{
			// Arrange
			int id = 1;
			var flightDocument = new JsonPatchDocument();
			var updatedFlight = new FlightEntity { Id = id };
			var flightDto = new FlightDto { Id = id };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_flightServiceMock.Setup(service => service.FlightExists(id)).ReturnsAsync(true);
			_flightServiceMock.Setup(service => service.PatchFlight(id, flightDocument)).ReturnsAsync(updatedFlight);
			_mapperMock.Setup(m => m.Map<FlightDto>(updatedFlight)).Returns(flightDto);

			// Act
			var result = await _controller.PatchFlight(id, flightDocument);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(flightDto, okResult.Value);
		}

		[Fact]
		[Trait("Category", "PatchFlight")]
		public async Task PatchFlight_ReturnsBadRequest_WhenIdIsInvalid()
		{
			// Arrange
			int invalidId = -1;
			var flightDocument = new JsonPatchDocument();

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(invalidId)).Returns(false);

			// Act
			var result = await _controller.PatchFlight(invalidId, flightDocument);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Invalid input. The ID must be a non-negative integer.", badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "PatchFlight")]
		public async Task PatchFlight_ReturnsNotFound_WhenFlightDoesNotExist()
		{
			// Arrange
			int id = 1;
			var flightDocument = new JsonPatchDocument();

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_flightServiceMock.Setup(service => service.FlightExists(id)).ReturnsAsync(false);

			// Act
			var result = await _controller.PatchFlight(id, flightDocument);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "DeleteFlight")]
		public async Task DeleteFlight_ReturnsNoContent_WhenDeletionIsSuccessful()
		{
			// Arrange
			int id = 1;
			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_flightServiceMock.Setup(service => service.FlightExists(id)).ReturnsAsync(true);
			_flightServiceMock.Setup(service => service.DeleteFlight(id)).ReturnsAsync(true);

			// Act
			var result = await _controller.DeleteFlight(id);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "DeleteFlight")]
		public async Task DeleteFlight_ReturnsBadRequest_WhenIdIsInvalid()
		{
			// Arrange
			int invalidId = -1;
			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(invalidId)).Returns(false);

			// Act
			var result = await _controller.DeleteFlight(invalidId);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Invalid input. The ID must be a non-negative integer.", badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "DeleteFlight")]
		public async Task DeleteFlight_ReturnsNotFound_WhenFlightDoesNotExist()
		{
			// Arrange
			int id = 1;
			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_flightServiceMock.Setup(service => service.FlightExists(id)).ReturnsAsync(false);

			// Act
			var result = await _controller.DeleteFlight(id);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "DeleteFlight")]
		public async Task DeleteFlight_ReturnsConflict_WhenFlightCannotBeDeleted()
		{
			// Arrange
			int id = 1;
			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_flightServiceMock.Setup(service => service.FlightExists(id)).ReturnsAsync(true);
			_flightServiceMock.Setup(service => service.DeleteFlight(id)).ReturnsAsync(false);

			// Act
			var result = await _controller.DeleteFlight(id);

			// Assert
			var conflictResult = Assert.IsType<ConflictObjectResult>(result);
			Assert.Equal("Flight cannot be deleted because it is being referenced by other entities.", conflictResult.Value);
		}

	}
}