using AirportAutomation.Api.Controllers;
using AirportAutomation.Api.Interfaces;
using AirportAutomation.Application.Dtos.Airline;
using AirportAutomation.Application.Dtos.Response;
using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Enums;
using AirportAutomation.Core.Interfaces.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace AirportAutomationApi.Test.Controllers
{
	public class AirlinesControllerTests
	{
		private readonly AirlinesController _controller;
		private readonly Mock<IAirlineService> _airlineServiceMock;
		private readonly Mock<IPaginationValidationService> _paginationValidationServiceMock;
		private readonly Mock<IInputValidationService> _inputValidationServiceMock;
		private readonly Mock<IUtilityService> _utilityServiceMock;
		private readonly Mock<IExportService> _exportServiceMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<ILogger<AirlinesController>> _loggerMock;
		private readonly Mock<IConfiguration> _configurationMock;

		private readonly AirlineEntity airlineEntity = new()
		{
			Id = 1,
			Name = "Air Serbia"
		};

		private readonly AirlineDto airlineDto = new()
		{
			Id = 1,
			Name = "Air Serbia"
		};

		public AirlinesControllerTests()
		{
			_airlineServiceMock = new Mock<IAirlineService>();
			_paginationValidationServiceMock = new Mock<IPaginationValidationService>();
			_inputValidationServiceMock = new Mock<IInputValidationService>();
			_utilityServiceMock = new Mock<IUtilityService>();
			_exportServiceMock = new Mock<IExportService>();
			_mapperMock = new Mock<IMapper>();
			_loggerMock = new Mock<ILogger<AirlinesController>>();
			_configurationMock = new Mock<IConfiguration>();
			var configBuilder = new ConfigurationBuilder();
			configBuilder.AddInMemoryCollection(new Dictionary<string, string>
			{
				{"pageSettings:maxPageSize", "10"}
			});
			_configurationMock.Setup(x => x.GetSection(It.IsAny<string>()))
				.Returns(configBuilder.Build().GetSection(""));

			_controller = new AirlinesController(
				_airlineServiceMock.Object,
				_paginationValidationServiceMock.Object,
				_inputValidationServiceMock.Object,
				_utilityServiceMock.Object,
				_exportServiceMock.Object,
				_mapperMock.Object,
				_loggerMock.Object,
				_configurationMock.Object
			);
		}

		#region Constructor Tests
		/// <summary>
		/// This test ensures that the `AirlinesController` constructor throws an `ArgumentNullException`
		/// if a `null` value is passed in for the `IAirlineService`. This prevents the application from failing
		/// unexpectedly later on if a crucial dependency is missing.
		/// </summary>
		[Trait("Category", "Constructor Tests")]
		[Fact]
		public void Constructor_ThrowsArgumentNullException_WhenAirlineServiceIsNull()
		{
			// Arrange
			var config = _configurationMock.Object;
			var pagination = _paginationValidationServiceMock.Object;
			var input = _inputValidationServiceMock.Object;
			var utility = _utilityServiceMock.Object;
			var export = _exportServiceMock.Object;
			var mapper = _mapperMock.Object;
			var logger = _loggerMock.Object;

			// Act & Assert
			Assert.Throws<ArgumentNullException>(() => new AirlinesController(
				null,
				pagination,
				input,
				utility,
				export,
				mapper,
				logger,
				config));
		}

		/// <summary>
		/// This test checks that the constructor throws an `ArgumentNullException` when the
		/// `IPaginationValidationService` dependency is `null`.
		/// </summary>
		[Fact]
		[Trait("Category", "Constructor Tests")]
		public void Constructor_ThrowsArgumentNullException_WhenPaginationValidationServiceIsNull()
		{
			// Arrange
			var airline = _airlineServiceMock.Object;
			var config = _configurationMock.Object;
			var input = _inputValidationServiceMock.Object;
			var utility = _utilityServiceMock.Object;
			var export = _exportServiceMock.Object;
			var mapper = _mapperMock.Object;
			var logger = _loggerMock.Object;

			// Act & Assert
			Assert.Throws<ArgumentNullException>(() => new AirlinesController(
				airline,
				null,
				input,
				utility,
				export,
				mapper,
				logger,
				config));
		}

		/// <summary>
		/// This test confirms that the constructor throws an `ArgumentNullException` if the
		/// `IInputValidationService` is `null`, ensuring proper validation services are always provided.
		/// </summary>
		[Fact]
		[Trait("Category", "Constructor Tests")]
		public void Constructor_ThrowsArgumentNullException_WhenInputValidationServiceIsNull()
		{
			// Arrange
			var airline = _airlineServiceMock.Object;
			var config = _configurationMock.Object;
			var pagination = _paginationValidationServiceMock.Object;
			var utility = _utilityServiceMock.Object;
			var export = _exportServiceMock.Object;
			var mapper = _mapperMock.Object;
			var logger = _loggerMock.Object;

			// Act & Assert
			Assert.Throws<ArgumentNullException>(() => new AirlinesController(
				airline,
				pagination,
				null,
				utility,
				export,
				mapper,
				logger,
				config));
		}

		/// <summary>
		/// This test validates that the constructor throws an `ArgumentNullException` if the
		/// `IUtilityService` is `null`.
		/// </summary>
		[Fact]
		[Trait("Category", "Constructor Tests")]
		public void Constructor_ThrowsArgumentNullException_WhenUtilityServiceIsNull()
		{
			// Arrange
			var airline = _airlineServiceMock.Object;
			var config = _configurationMock.Object;
			var pagination = _paginationValidationServiceMock.Object;
			var input = _inputValidationServiceMock.Object;
			var export = _exportServiceMock.Object;
			var mapper = _mapperMock.Object;
			var logger = _loggerMock.Object;

			// Act & Assert
			Assert.Throws<ArgumentNullException>(() => new AirlinesController(
				airline,
				pagination,
				input,
				null,
				export,
				mapper,
				logger,
				config));
		}

		/// <summary>
		/// This test verifies that the constructor throws an `ArgumentNullException` when a `null`
		/// `IExportService` is passed to it.
		/// </summary>
		[Fact]
		[Trait("Category", "Constructor Tests")]
		public void Constructor_ThrowsArgumentNullException_WhenExportServiceIsNull()
		{
			// Arrange
			var airline = _airlineServiceMock.Object;
			var config = _configurationMock.Object;
			var pagination = _paginationValidationServiceMock.Object;
			var input = _inputValidationServiceMock.Object;
			var utility = _utilityServiceMock.Object;
			var mapper = _mapperMock.Object;
			var logger = _loggerMock.Object;

			// Act & Assert
			Assert.Throws<ArgumentNullException>(() => new AirlinesController(
				airline,
				pagination,
				input,
				utility,
				null,
				mapper,
				logger,
				config));
		}

		/// <summary>
		/// This test ensures the constructor throws an `ArgumentNullException` if the required `IMapper`
		/// dependency is `null`.
		/// </summary>
		[Fact]
		[Trait("Category", "Constructor Tests")]
		public void Constructor_ThrowsArgumentNullException_WhenMapperIsNull()
		{
			// Arrange
			var airline = _airlineServiceMock.Object;
			var config = _configurationMock.Object;
			var pagination = _paginationValidationServiceMock.Object;
			var input = _inputValidationServiceMock.Object;
			var utility = _utilityServiceMock.Object;
			var export = _exportServiceMock.Object;
			var logger = _loggerMock.Object;

			// Act & Assert
			Assert.Throws<ArgumentNullException>(() => new AirlinesController(
				airline,
				pagination,
				input,
				utility,
				export,
				null,
				logger,
				config));
		}

		/// <summary>
		/// This test confirms the constructor throws an `ArgumentNullException` if the `ILogger`
		/// dependency is `null`, which is important for maintaining a functional logging system.
		/// </summary>
		[Fact]
		[Trait("Category", "Constructor Tests")]
		public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
		{
			// Arrange
			var airline = _airlineServiceMock.Object;
			var config = _configurationMock.Object;
			var pagination = _paginationValidationServiceMock.Object;
			var input = _inputValidationServiceMock.Object;
			var utility = _utilityServiceMock.Object;
			var export = _exportServiceMock.Object;
			var mapper = _mapperMock.Object;

			// Act & Assert
			Assert.Throws<ArgumentNullException>(() => new AirlinesController(
				airline,
				pagination,
				input,
				utility,
				export,
				mapper,
				null,
				config));
		}

		/// <summary>
		/// This is a "happy path" test. It verifies that when all dependencies are correctly provided, the
		/// `AirlinesController` is successfully instantiated without throwing any exceptions. This test confirms that
		/// the constructor works as expected under normal circumstances.
		/// </summary>
		[Fact]
		[Trait("Category", "Constructor Tests")]
		public void Constructor_DoesNotThrowException_WhenAllDependenciesAreValid()
		{
			// Arrange
			var airline = _airlineServiceMock.Object;
			var config = _configurationMock.Object;
			var pagination = _paginationValidationServiceMock.Object;
			var input = _inputValidationServiceMock.Object;
			var utility = _utilityServiceMock.Object;
			var export = _exportServiceMock.Object;
			var mapper = _mapperMock.Object;
			var logger = _loggerMock.Object;

			// Act & Assert
			var exception = Record.Exception(() => new AirlinesController(
				airline,
				pagination,
				input,
				utility,
				export,
				mapper,
				logger,
				config));
			Assert.Null(exception);
		}
		#endregion

		#region GetAirlines
		/// <summary>
		/// This test validates that the `GetAirlines` method returns a `400 BadRequest`
		/// when provided with invalid pagination parameters, such as a negative page number or a zero page size.
		/// </summary>
		[Fact]
		[Trait("Category", "GetAirlines")]
		public async Task GetAirlines_InvalidPaginationParameters_ReturnsBadRequest()
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
			var result = await _controller.GetAirlines(cancellationToken, invalidPage, invalidPageSize);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
		}

		/// <summary>
		/// This test verifies that the `GetAirlines` method returns a `204 No Content`
		/// response when the airline service returns an empty list of airlines.
		/// </summary>
		[Fact]
		[Trait("Category", "GetAirlines")]
		public async Task GetAirlines_ReturnsNoContent_WhenNoAirlinesFound()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_airlineServiceMock.Setup(service => service.GetAirlines(cancellationToken, It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(new List<AirlineEntity>());

			// Act
			var result = await _controller.GetAirlines(cancellationToken, page, pageSize);

			// Assert
			Assert.IsType<NoContentResult>(result.Result);
		}

		/// <summary>
		/// This test ensures that the `GetAirlines` method returns a `204 No Content`
		/// response when the airline service returns a `null` list of airlines.
		/// </summary>
		[Fact]
		[Trait("Category", "GetAirlines")]
		public async Task GetAirlines_ReturnsNoContent_WhenAirlinesIsNull()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_airlineServiceMock.Setup(service => service.GetAirlines(cancellationToken, It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync((List<AirlineEntity>)null); // Simulate null return

			// Act
			var result = await _controller.GetAirlines(cancellationToken, page, pageSize);

			// Assert
			Assert.IsType<NoContentResult>(result.Result);
		}

		/// <summary>
		/// This test checks that the `GetAirlines` method correctly propagates an exception
		/// thrown by the airline service. It confirms the method does not handle the exception,
		/// allowing it to bubble up as expected.
		/// </summary>
		[Fact]
		[Trait("Category", "GetAirlines")]
		public async Task GetAirlines_ReturnsInternalServerError_WhenExceptionThrown()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_airlineServiceMock.Setup(service => service.GetAirlines(cancellationToken, It.IsAny<int>(), It.IsAny<int>()))
				.ThrowsAsync(new Exception("Simulated exception"));

			// Act & Assert
			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetAirlines(cancellationToken, page, pageSize));
		}

		/// <summary>
		/// This is a happy-path test that verifies the `GetAirlines` method returns a
		/// `200 OK` response with the correct paginated data, page number, page size, and total count.
		/// </summary>
		[Fact]
		[Trait("Category", "GetAirlines")]
		public async Task GetAirlines_ReturnsOk_WithPaginatedAirlines()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;
			var airlines = new List<AirlineEntity>
		{
			new AirlineEntity { Id = 1, Name = "Airline 1" },
			new AirlineEntity { Id = 2, Name = "Airline 2" }
		};
			var totalItems = 2;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_airlineServiceMock
				.Setup(service => service.GetAirlines(cancellationToken, page, pageSize))
				.ReturnsAsync(airlines);
			_airlineServiceMock
				.Setup(service => service.AirlinesCount(cancellationToken, null))
				.ReturnsAsync(totalItems);

			var expectedData = new List<AirlineDto>
		{
			new AirlineDto { Id = 1, Name = "Airline 1" },
			new AirlineDto { Id = 2, Name = "Airline 2" }
		};
			_mapperMock
				.Setup(m => m.Map<IEnumerable<AirlineDto>>(It.IsAny<IEnumerable<AirlineEntity>>()))
				.Returns(expectedData);

			// Act
			var result = await _controller.GetAirlines(cancellationToken, page, pageSize);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PagedResponse<AirlineDto>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var pagedResponse = Assert.IsType<PagedResponse<AirlineDto>>(okResult.Value);
			Assert.Equal(page, pagedResponse.PageNumber);
			Assert.Equal(pageSize, pagedResponse.PageSize);
			Assert.Equal(totalItems, pagedResponse.TotalCount);
			Assert.Equal(expectedData, pagedResponse.Data);
		}

		/// <summary>
		/// This test ensures that the `GetAirlines` method returns the correct subset of data
		/// for a specified page number and page size, confirming the pagination logic is working as intended.
		/// </summary>
		[Fact]
		[Trait("Category", "GetAirlines")]
		public async Task GetAirlines_ReturnsCorrectPageData()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 2;
			int pageSize = 5;
			var allAirlines = new List<AirlineEntity>
		{
			new AirlineEntity { Id = 1, Name = "Airline 1" },
			new AirlineEntity { Id = 2, Name = "Airline 2" },
			new AirlineEntity { Id = 3, Name = "Airline 3" },
			new AirlineEntity { Id = 4, Name = "Airline 4" },
			new AirlineEntity { Id = 5, Name = "Airline 5" },
			new AirlineEntity { Id = 6, Name = "Airline 6" },
			new AirlineEntity { Id = 7, Name = "Airline 7" },
			new AirlineEntity { Id = 8, Name = "Airline 8" },
			new AirlineEntity { Id = 9, Name = "Airline 9" },
			new AirlineEntity { Id = 10, Name = "Airline 10" }
		};
			var pagedAirlines = allAirlines.Skip((page - 1) * pageSize).Take(pageSize).ToList();

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_airlineServiceMock
				.Setup(service => service.GetAirlines(cancellationToken, page, pageSize))
				.ReturnsAsync(pagedAirlines);
			_airlineServiceMock
				.Setup(service => service.AirlinesCount(cancellationToken, null))
				.ReturnsAsync(allAirlines.Count);

			var expectedData = new List<AirlineDto>
		{
			new AirlineDto { Id = 6, Name = "Airline 6" },
			new AirlineDto { Id = 7, Name = "Airline 7" },
			new AirlineDto { Id = 8, Name = "Airline 8" },
			new AirlineDto { Id = 9, Name = "Airline 9" },
			new AirlineDto { Id = 10, Name = "Airline 10" }
		};
			_mapperMock
				.Setup(m => m.Map<IEnumerable<AirlineDto>>(It.IsAny<IEnumerable<AirlineEntity>>()))
				.Returns(expectedData);

			// Act
			var result = await _controller.GetAirlines(cancellationToken, page, pageSize);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PagedResponse<AirlineDto>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var pagedResponse = Assert.IsType<PagedResponse<AirlineDto>>(okResult.Value);
			Assert.Equal(page, pagedResponse.PageNumber);
			Assert.Equal(pageSize, pagedResponse.PageSize);
			Assert.Equal(allAirlines.Count, pagedResponse.TotalCount);
			Assert.Equal(expectedData, pagedResponse.Data);
		}
		#endregion

		#region GetAirline
		/// <summary>
		/// This test verifies that the `GetAirline` method returns a `400 BadRequest`
		/// response when a negative or invalid ID is provided.
		/// </summary>
		[Fact]
		[Trait("Category", "GetAirline")]
		public async Task GetAirline_InvalidId_ReturnsBadRequest()
		{
			// Arrange
			int invalidId = -1;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid input. The ID must be a non-negative integer.");

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(invalidId))
				.Returns(false);

			// Act
			var result = await _controller.GetAirline(invalidId);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
			var badRequestResult = result.Result as BadRequestObjectResult;
			Assert.Equal(expectedBadRequestResult.Value, badRequestResult.Value);
		}

		/// <summary>
		/// This test verifies that the `GetAirline` method returns a `404 Not Found`
		/// response when a valid ID is provided, but no airline with that ID exists in the database.
		/// </summary>
		[Fact]
		[Trait("Category", "GetAirline")]
		public async Task GetAirline_AirlineNotFound_ReturnsNotFound()
		{
			// Arrange
			int validId = 1;

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(validId))
				.Returns(true);
			_airlineServiceMock
				.Setup(service => service.AirlineExists(validId))
				.ReturnsAsync(false);

			// Act
			var result = await _controller.GetAirline(validId);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		/// <summary>
		/// This is a happy-path test that confirms the `GetAirline` method returns a `200 OK`
		/// response with the correct `AirlineDto` when a valid ID is provided and the airline exists.
		/// </summary>
		[Fact]
		[Trait("Category", "GetAirline")]
		public async Task GetAirline_ReturnsAirlineDto_WhenAirlineExists()
		{
			// Arrange
			int validId = 1;

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(validId))
				.Returns(true);
			_airlineServiceMock
				.Setup(service => service.AirlineExists(validId))
				.ReturnsAsync(true);
			_airlineServiceMock
				.Setup(service => service.GetAirline(validId))
				.ReturnsAsync(airlineEntity);
			_mapperMock
				.Setup(m => m.Map<AirlineDto>(airlineEntity))
				.Returns(airlineDto);

			// Act
			var result = await _controller.GetAirline(validId);

			// Assert
			var actionResult = Assert.IsType<ActionResult<AirlineDto>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var returnedAirlineDto = Assert.IsType<AirlineDto>(okResult.Value);
			Assert.Equal(airlineDto, returnedAirlineDto);
		}
		#endregion

		#region GetAirlinesByName
		/// <summary>
		/// This test verifies that the `GetAirlinesByName` method returns a `400 BadRequest`
		/// response when provided with an invalid or empty name string.
		/// </summary>
		[Fact]
		[Trait("Category", "GetAirlinesByName")]
		public async Task GetAirlinesByName_InvalidName_ReturnsBadRequest()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			string invalidName = string.Empty;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid input. The name must be a valid non-empty string.");

			_inputValidationServiceMock
				.Setup(x => x.IsValidString(invalidName))
				.Returns(false);

			// Act
			var result = await _controller.GetAirlinesByName(cancellationToken, invalidName);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
			var badRequestResult = result.Result as BadRequestObjectResult;
			Assert.Equal(expectedBadRequestResult.Value, badRequestResult.Value);
		}

		/// <summary>
		/// This test ensures that the `GetAirlinesByName` method returns a `400 BadRequest`
		/// when the provided pagination parameters are invalid (e.g., negative page number or zero page size).
		/// </summary>
		[Fact]
		[Trait("Category", "GetAirlinesByName")]
		public async Task GetAirlinesByName_InvalidPaginationParameters_ReturnsBadRequest()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			string validName = "ValidName";
			int invalidPage = -1;
			int invalidPageSize = 0;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid pagination parameters.");

			_inputValidationServiceMock
				.Setup(x => x.IsValidString(validName))
				.Returns(true);
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(invalidPage, invalidPageSize, It.IsAny<int>()))
				.Returns((false, 0, expectedBadRequestResult));

			// Act
			var result = await _controller.GetAirlinesByName(cancellationToken, validName, invalidPage, invalidPageSize);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
		}

		/// <summary>
		/// This test checks that the `GetAirlinesByName` method returns a `404 Not Found`
		/// response when the airline service returns a `null` list of airlines for the given name.
		/// </summary>
		[Fact]
		[Trait("Category", "GetAirlinesByName")]
		public async Task GetAirlinesByName_ReturnsNotFound_WhenAirlinesIsNull()
		{
			// Arrange
			var name = "Nonexistent Airline";
			_inputValidationServiceMock.Setup(s => s.IsValidString(name)).Returns(true);
			_paginationValidationServiceMock.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_airlineServiceMock.Setup(s => s.GetAirlinesByName(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>(), name))
				.ReturnsAsync((List<AirlineEntity>)null);

			// Act
			var result = await _controller.GetAirlinesByName(CancellationToken.None, name);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		/// <summary>
		/// This test verifies that the `GetAirlinesByName` method returns a `404 Not Found`
		/// when the airline service returns an empty list for the provided name.
		/// </summary>
		[Fact]
		[Trait("Category", "GetAirlinesByName")]
		public async Task GetAirlinesByName_ReturnsNotFound_WhenAirlinesAreEmpty()
		{
			// Arrange
			var name = "Nonexistent Airline";
			_inputValidationServiceMock.Setup(s => s.IsValidString(name)).Returns(true);
			_paginationValidationServiceMock.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_airlineServiceMock.Setup(s => s.GetAirlinesByName(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>(), name))
				.ReturnsAsync(new List<AirlineEntity>());

			// Act
			var result = await _controller.GetAirlinesByName(CancellationToken.None, name);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		/// <summary>
		/// This is a happy-path test that confirms the `GetAirlinesByName` method returns a `200 OK`
		/// response with the correct paginated data, page number, page size, and total count when airlines are found.
		/// </summary>
		[Fact]
		[Trait("Category", "GetAirlinesByName")]
		public async Task GetAirlinesByName_ReturnsPagedListOfAirlines_WhenAirlinesFound()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			string validName = "ValidName";
			int validPage = 1;
			int validPageSize = 10;
			var airlineEntities = new List<AirlineEntity> { new AirlineEntity() };
			var airlineDtos = new List<AirlineDto> { new AirlineDto() };
			var totalItems = 1;

			_inputValidationServiceMock
				.Setup(x => x.IsValidString(validName))
				.Returns(true);
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(validPage, validPageSize, It.IsAny<int>()))
				.Returns((true, validPageSize, null));
			_airlineServiceMock
				.Setup(service => service.GetAirlinesByName(cancellationToken, validPage, validPageSize, validName))
				.ReturnsAsync(airlineEntities);
			_airlineServiceMock
				.Setup(service => service.AirlinesCount(cancellationToken, validName))
				.ReturnsAsync(totalItems);
			_mapperMock
				.Setup(m => m.Map<IEnumerable<AirlineDto>>(airlineEntities))
				.Returns(airlineDtos);

			// Act
			var result = await _controller.GetAirlinesByName(cancellationToken, validName, validPage, validPageSize);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PagedResponse<AirlineDto>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var response = Assert.IsType<PagedResponse<AirlineDto>>(okResult.Value);
			Assert.Equal(validPage, response.PageNumber);
			Assert.Equal(validPageSize, response.PageSize);
			Assert.Equal(totalItems, response.TotalCount);
			Assert.Equal(airlineDtos, response.Data);
		}
		#endregion

		#region PostAirline
		/// <summary>
		/// This is a happy-path test that verifies the `PostAirline` method returns a `201 CreatedAtActionResult`
		/// when an airline is successfully created. It also checks that the returned object, action name, and route values are correct.
		/// </summary>
		[Fact]
		[Trait("Category", "PostAirline")]
		public async Task PostAirline_ReturnsCreatedAtActionResult_WhenAirlineIsCreatedSuccessfully()
		{
			// Arrange
			var airlineCreateDto = new AirlineCreateDto();
			var airlineEntity = new AirlineEntity { Id = 1 };
			var airlineDto = new AirlineDto { Id = 1 };

			// Set up the mapper to return the expected values
			_mapperMock.Setup(m => m.Map<AirlineEntity>(airlineCreateDto)).Returns(airlineEntity);
			_mapperMock.Setup(m => m.Map<AirlineDto>(airlineEntity)).Returns(airlineDto);

			// Adjust service setup to return the airlineEntity wrapped in a Task
			_airlineServiceMock.Setup(service => service.PostAirline(airlineEntity))
							   .ReturnsAsync(airlineEntity);

			// Act
			var result = await _controller.PostAirline(airlineCreateDto);

			// Assert
			var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
			var returnedValue = Assert.IsType<AirlineDto>(actionResult.Value);
			Assert.Equal(airlineDto.Id, returnedValue.Id);
			Assert.Equal("GetAirline", actionResult.ActionName);
			Assert.Equal(1, actionResult.RouteValues["id"]);
		}

		/// <summary>
		/// This test ensures that the `PostAirline` method correctly propagates an exception
		/// thrown by the airline service during the creation process.
		/// </summary>
		[Fact]
		[Trait("Category", "PostAirline")]
		public async Task PostAirline_ThrowsException_WhenServiceFails()
		{
			// Arrange
			var airlineCreateDto = new AirlineCreateDto();
			var airlineEntity = new AirlineEntity();
			_mapperMock.Setup(m => m.Map<AirlineEntity>(airlineCreateDto)).Returns(airlineEntity);

			// Set up the service to throw an exception
			_airlineServiceMock.Setup(service => service.PostAirline(airlineEntity))
							   .ThrowsAsync(new Exception("Simulated exception"));

			// Act & Assert
			await Assert.ThrowsAsync<Exception>(async () => await _controller.PostAirline(airlineCreateDto));
		}
		#endregion

		#region PutAirline
		/// <summary>
		/// This is a happy-path test that verifies the `PutAirline` method returns a `204 No Content`
		/// response when an airline is successfully updated.
		/// </summary>
		[Fact]
		[Trait("Category", "PutAirline")]
		public async Task PutAirline_ReturnsNoContent_WhenUpdateIsSuccessful()
		{
			// Arrange
			int id = 1;
			var airlineDto = new AirlineDto { Id = id };
			var airlineEntity = new AirlineEntity { Id = id };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_mapperMock.Setup(m => m.Map<AirlineEntity>(airlineDto)).Returns(airlineEntity);
			_airlineServiceMock.Setup(service => service.AirlineExists(id)).ReturnsAsync(true);
			_airlineServiceMock.Setup(service => service.PutAirline(airlineEntity)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.PutAirline(id, airlineDto);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		/// <summary>
		/// This test ensures the `PutAirline` method returns a `400 BadRequest` response
		/// when the provided ID is invalid (e.g., negative).
		/// </summary>
		[Fact]
		[Trait("Category", "PutAirline")]
		public async Task PutAirline_ReturnsBadRequest_WhenIdIsInvalid()
		{
			// Arrange
			int invalidId = -1;
			var airlineDto = new AirlineDto { Id = invalidId };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(invalidId)).Returns(false);

			// Act
			var result = await _controller.PutAirline(invalidId, airlineDto);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Invalid input. The ID must be a non-negative integer.", badRequestResult.Value);
		}

		/// <summary>
		/// This test validates that the `PutAirline` method returns a `400 BadRequest` response
		/// when the ID in the DTO body does not match the ID in the URL.
		/// </summary>
		[Fact]
		[Trait("Category", "PutAirline")]
		public async Task PutAirline_ReturnsBadRequest_WhenIdInDtoDoesNotMatchIdInUrl()
		{
			// Arrange
			int id = 1;
			var airlineDto = new AirlineDto { Id = 2 };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);

			// Act
			var result = await _controller.PutAirline(id, airlineDto);

			// Assert
			Assert.IsType<BadRequestResult>(result);
		}

		/// <summary>
		/// This test verifies that the `PutAirline` method returns a `404 Not Found`
		/// response when an airline with the given ID does not exist.
		/// </summary>
		[Fact]
		[Trait("Category", "PutAirline")]
		public async Task PutAirline_ReturnsNotFound_WhenAirlineDoesNotExist()
		{
			// Arrange
			int id = 1;
			var airlineDto = new AirlineDto { Id = id };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_airlineServiceMock.Setup(service => service.AirlineExists(id)).ReturnsAsync(false);

			// Act
			var result = await _controller.PutAirline(id, airlineDto);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}
		#endregion

		#region PatchAirline
		/// <summary>
		/// This happy-path test verifies that the `PatchAirline` method returns a `200 OK`
		/// response when a partial update is successful.
		/// </summary>
		[Fact]
		[Trait("Category", "PatchAirline")]
		public async Task PatchAirline_ReturnsOk_WhenUpdateIsSuccessful()
		{
			// Arrange
			int id = 1;
			var airlineDocument = new JsonPatchDocument();
			var updatedAirline = new AirlineEntity { Id = id };
			var airlineDto = new AirlineDto { Id = id };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_airlineServiceMock.Setup(service => service.AirlineExists(id)).ReturnsAsync(true);
			_airlineServiceMock.Setup(service => service.PatchAirline(id, airlineDocument)).ReturnsAsync(updatedAirline);
			_mapperMock.Setup(m => m.Map<AirlineDto>(updatedAirline)).Returns(airlineDto);

			// Act
			var result = await _controller.PatchAirline(id, airlineDocument);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(airlineDto, okResult.Value);
		}

		/// <summary>
		/// This test ensures the `PatchAirline` method returns a `400 BadRequest` response
		/// when the provided ID is invalid (e.g., negative).
		/// </summary>
		[Fact]
		[Trait("Category", "PatchAirline")]
		public async Task PatchAirline_ReturnsBadRequest_WhenIdIsInvalid()
		{
			// Arrange
			int invalidId = -1;
			var airlineDocument = new JsonPatchDocument();

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(invalidId)).Returns(false);

			// Act
			var result = await _controller.PatchAirline(invalidId, airlineDocument);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Invalid input. The ID must be a non-negative integer.", badRequestResult.Value);
		}

		/// <summary>
		/// This test verifies that the `PatchAirline` method returns a `404 Not Found`
		/// response when an airline with the given ID does not exist.
		/// </summary>
		[Fact]
		[Trait("Category", "PatchAirline")]
		public async Task PatchAirline_ReturnsNotFound_WhenAirlineDoesNotExist()
		{
			// Arrange
			int id = 1;
			var airlineDocument = new JsonPatchDocument();

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_airlineServiceMock.Setup(service => service.AirlineExists(id)).ReturnsAsync(false);

			// Act
			var result = await _controller.PatchAirline(id, airlineDocument);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}
		#endregion

		#region DeleteAirline
		/// <summary>
		/// This is a happy-path test that verifies the `DeleteAirline` method returns a `204 No Content`
		/// response when an airline is successfully deleted.
		/// </summary>
		[Fact]
		[Trait("Category", "DeleteAirline")]
		public async Task DeleteAirline_ReturnsNoContent_WhenDeletionIsSuccessful()
		{
			// Arrange
			int id = 1;
			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_airlineServiceMock.Setup(service => service.AirlineExists(id)).ReturnsAsync(true);
			_airlineServiceMock.Setup(service => service.DeleteAirline(id)).ReturnsAsync(true);

			// Act
			var result = await _controller.DeleteAirline(id);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		/// <summary>
		/// This test ensures that the `DeleteAirline` method returns a `400 BadRequest`
		/// when the provided ID is invalid (e.g., negative).
		/// </summary>
		[Fact]
		[Trait("Category", "DeleteAirline")]
		public async Task DeleteAirline_ReturnsBadRequest_WhenIdIsInvalid()
		{
			// Arrange
			int invalidId = -1;
			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(invalidId)).Returns(false);

			// Act
			var result = await _controller.DeleteAirline(invalidId);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Invalid input. The ID must be a non-negative integer.", badRequestResult.Value);
		}

		/// <summary>
		/// This test verifies that the `DeleteAirline` method returns a `404 Not Found`
		/// response when an airline with the given ID does not exist.
		/// </summary>
		[Fact]
		[Trait("Category", "DeleteAirline")]
		public async Task DeleteAirline_ReturnsNotFound_WhenAirlineDoesNotExist()
		{
			// Arrange
			int id = 1;
			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_airlineServiceMock.Setup(service => service.AirlineExists(id)).ReturnsAsync(false);

			// Act
			var result = await _controller.DeleteAirline(id);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		/// <summary>
		/// This test checks that the `DeleteAirline` method returns a `409 Conflict`
		/// response when the airline cannot be deleted due to being referenced by other entities.
		/// </summary>
		[Fact]
		[Trait("Category", "DeleteAirline")]
		public async Task DeleteAirline_ReturnsConflict_WhenAirlineCannotBeDeleted()
		{
			// Arrange
			int id = 1;
			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_airlineServiceMock.Setup(service => service.AirlineExists(id)).ReturnsAsync(true);
			_airlineServiceMock.Setup(service => service.DeleteAirline(id)).ReturnsAsync(false);

			// Act
			var result = await _controller.DeleteAirline(id);

			// Assert
			var conflictResult = Assert.IsType<ConflictObjectResult>(result);
			Assert.Equal("Airline cannot be deleted because it is being referenced by other entities.", conflictResult.Value);
		}
		#endregion

		#region ExportToPdf
		/// <summary>
		/// This is a happy-path test that verifies the `ExportToPdf` method returns a file (`FileContentResult`)
		/// with the correct content and headers when the `getAll` parameter is set to `true`.
		/// </summary>
		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_ReturnsFile_WhenGetAllIsTrue()
		{
			// Arrange
			var mockAirlines = new List<AirlineEntity> { new AirlineEntity { Id = 1, Name = "Test Airline" } };
			var mockPdfBytes = new byte[] { 1, 2, 3 };
			_airlineServiceMock.Setup(s => s.GetAllAirlines(It.IsAny<CancellationToken>()))
				.ReturnsAsync(mockAirlines);
			_exportServiceMock.Setup(s => s.ExportToPDF("Airlines", mockAirlines))
				.Returns(mockPdfBytes);
			_utilityServiceMock.Setup(s => s.GenerateUniqueFileName("Airlines", FileExtension.Pdf))
				.Returns("Airlines-test.pdf");

			// Act
			var result = await _controller.ExportToPdf(CancellationToken.None, getAll: true);

			// Assert
			var fileResult = Assert.IsType<FileContentResult>(result);
			Assert.Equal("Airlines-test.pdf", fileResult.FileDownloadName);
			Assert.Equal("application/pdf", fileResult.ContentType);
			Assert.Equal(mockPdfBytes, fileResult.FileContents);
		}

		/// <summary>
		/// This test verifies that the `ExportToPdf` method returns a file with the correct content
		/// and headers when a valid name is provided and the pagination is successful.
		/// </summary>
		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_ReturnsFile_WhenNameIsValid()
		{
			// Arrange
			var mockAirlines = new List<AirlineEntity> { new AirlineEntity { Id = 1, Name = "Test Airline" } };
			var mockPdfBytes = new byte[] { 1, 2, 3 };
			_paginationValidationServiceMock.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_inputValidationServiceMock.Setup(s => s.IsValidString(It.IsAny<string>()))
				.Returns(true);
			_airlineServiceMock.Setup(s => s.GetAirlinesByName(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
				.ReturnsAsync(mockAirlines);
			_exportServiceMock.Setup(s => s.ExportToPDF("Airlines", mockAirlines))
				.Returns(mockPdfBytes);
			_utilityServiceMock.Setup(s => s.GenerateUniqueFileName("Airlines", FileExtension.Pdf))
				.Returns("Airlines-test.pdf");

			// Act
			var result = await _controller.ExportToPdf(CancellationToken.None, name: "Test");

			// Assert
			var fileResult = Assert.IsType<FileContentResult>(result);
			Assert.Equal("Airlines-test.pdf", fileResult.FileDownloadName);
		}

		/// <summary>
		/// This test confirms that the `ExportToPdf` method returns a file when the `name` parameter is invalid,
		/// as it should fall back to getting all airlines without a name filter.
		/// </summary>
		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_ReturnsFile_WhenNameIsInvalid()
		{
			// Arrange
			var mockAirlines = new List<AirlineEntity> { new AirlineEntity { Id = 1, Name = "Test Airline" } };
			var mockPdfBytes = new byte[] { 1, 2, 3 };
			_paginationValidationServiceMock.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_inputValidationServiceMock.Setup(s => s.IsValidString(It.IsAny<string>()))
				.Returns(false);
			_airlineServiceMock.Setup(s => s.GetAirlines(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(mockAirlines);
			_exportServiceMock.Setup(s => s.ExportToPDF("Airlines", mockAirlines))
				.Returns(mockPdfBytes);
			_utilityServiceMock.Setup(s => s.GenerateUniqueFileName("Airlines", FileExtension.Pdf))
				.Returns("Airlines-test.pdf");

			// Act
			var result = await _controller.ExportToPdf(CancellationToken.None, name: null);

			// Assert
			var fileResult = Assert.IsType<FileContentResult>(result);
			Assert.Equal("Airlines-test.pdf", fileResult.FileDownloadName);
		}

		/// <summary>
		/// This test ensures the `ExportToPdf` method returns a `204 No Content` response
		/// when the service returns an empty list of airlines to be exported.
		/// </summary>
		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_ReturnsNoContent_WhenAirlinesAreEmpty()
		{
			// Arrange
			_paginationValidationServiceMock.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_inputValidationServiceMock.Setup(s => s.IsValidString(It.IsAny<string>()))
				.Returns(false);
			_airlineServiceMock.Setup(s => s.GetAirlines(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(new List<AirlineEntity>());

			// Act
			var result = await _controller.ExportToPdf(CancellationToken.None);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		/// <summary>
		/// This test checks that the `ExportToPdf` method returns a `204 No Content` response
		/// when the service returns a `null` list of airlines to be exported.
		/// </summary>
		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_ReturnsNoContent_WhenAirlinesIsNull()
		{
			// Arrange
			_paginationValidationServiceMock.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_inputValidationServiceMock.Setup(s => s.IsValidString(It.IsAny<string>()))
				.Returns(false);
			_airlineServiceMock.Setup(s => s.GetAirlines(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync((List<AirlineEntity>)null);

			// Act
			var result = await _controller.ExportToPdf(CancellationToken.None);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		/// <summary>
		/// This test verifies the `ExportToPdf` method returns a `400 BadRequest` response
		/// when the provided page number is invalid.
		/// </summary>
		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_ReturnsBadRequest_WhenPageIsInvalid()
		{
			// Arrange
			_paginationValidationServiceMock.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((false, 0, new BadRequestObjectResult("Invalid page number.")));

			// Act
			var result = await _controller.ExportToPdf(CancellationToken.None, page: 0);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Invalid page number.", badRequestResult.Value);
		}

		/// <summary>
		/// This test verifies the `ExportToPdf` method returns a `400 BadRequest` response
		/// when the provided page size is invalid.
		/// </summary>
		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_ReturnsBadRequest_WhenPageSizeIsInvalid()
		{
			// Arrange
			int maxPageSize = 10;
			_paginationValidationServiceMock.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((false, 0, new BadRequestObjectResult($"Invalid page size. It should be between 1 and {maxPageSize}.")));

			// Act
			var result = await _controller.ExportToPdf(CancellationToken.None, pageSize: 0);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal($"Invalid page size. It should be between 1 and {maxPageSize}.", badRequestResult.Value);
		}

		/// <summary>
		/// This test ensures the `ExportToPdf` method returns a `500 Internal Server Error`
		/// when the PDF generation service returns a `null` byte array.
		/// </summary>
		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_ReturnsInternalServerError_WhenPdfIsNull()
		{
			// Arrange
			var mockAirlines = new List<AirlineEntity> { new AirlineEntity { Id = 1, Name = "Test Airline" } };
			_paginationValidationServiceMock.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_inputValidationServiceMock.Setup(s => s.IsValidString(It.IsAny<string>()))
				.Returns(false);
			_airlineServiceMock.Setup(s => s.GetAirlines(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(mockAirlines);
			_exportServiceMock.Setup(s => s.ExportToPDF("Airlines", mockAirlines))
				.Returns((byte[])null);

			// Act
			var result = await _controller.ExportToPdf(CancellationToken.None);

			// Assert
			var statusCodeResult = Assert.IsType<ObjectResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
			Assert.Equal("Failed to generate PDF file.", statusCodeResult.Value);
		}
		#endregion

		#region ExportToExcel
		/// <summary>
		/// This is a happy-path test that verifies the `ExportToExcel` method returns a file (`FileContentResult`)
		/// with the correct content and headers when the `getAll` parameter is set to `true`.
		/// </summary>
		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_ReturnsFile_WhenGetAllIsTrue()
		{
			// Arrange
			var mockAirlines = new List<AirlineEntity> { new AirlineEntity { Id = 1, Name = "Test Airline" } };
			var mockPdfBytes = new byte[] { 1, 2, 3 };
			_airlineServiceMock.Setup(s => s.GetAllAirlines(It.IsAny<CancellationToken>()))
				.ReturnsAsync(mockAirlines);
			_exportServiceMock.Setup(s => s.ExportToExcel("Airlines", mockAirlines))
				.Returns(mockPdfBytes);
			_utilityServiceMock.Setup(s => s.GenerateUniqueFileName("Airlines", FileExtension.Xlsx))
				.Returns("Airlines-test.xlsx");

			// Act
			var result = await _controller.ExportToExcel(CancellationToken.None, getAll: true);

			// Assert
			var fileResult = Assert.IsType<FileContentResult>(result);
			Assert.Equal("Airlines-test.xlsx", fileResult.FileDownloadName);
			Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileResult.ContentType);
			Assert.Equal(mockPdfBytes, fileResult.FileContents);
		}

		/// <summary>
		/// This test verifies that the `ExportToExcel` method returns a file with the correct content
		/// and headers when a valid name is provided and the pagination is successful.
		/// </summary>
		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_ReturnsFile_WhenNameIsValid()
		{
			// Arrange
			var mockAirlines = new List<AirlineEntity> { new AirlineEntity { Id = 1, Name = "Test Airline" } };
			var mockPdfBytes = new byte[] { 1, 2, 3 };
			_paginationValidationServiceMock.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_inputValidationServiceMock.Setup(s => s.IsValidString(It.IsAny<string>()))
				.Returns(true);
			_airlineServiceMock.Setup(s => s.GetAirlinesByName(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
				.ReturnsAsync(mockAirlines);
			_exportServiceMock.Setup(s => s.ExportToExcel("Airlines", mockAirlines))
				.Returns(mockPdfBytes);
			_utilityServiceMock.Setup(s => s.GenerateUniqueFileName("Airlines", FileExtension.Xlsx))
				.Returns("Airlines-test.xlsx");

			// Act
			var result = await _controller.ExportToExcel(CancellationToken.None, name: "Test");

			// Assert
			var fileResult = Assert.IsType<FileContentResult>(result);
			Assert.Equal("Airlines-test.xlsx", fileResult.FileDownloadName);
		}

		/// <summary>
		/// This test confirms that the `ExportToExcel` method returns a file when the `name` parameter is invalid,
		/// as it should fall back to getting all airlines without a name filter.
		/// </summary>
		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_ReturnsFile_WhenNameIsInvalid()
		{
			// Arrange
			var mockAirlines = new List<AirlineEntity> { new AirlineEntity { Id = 1, Name = "Test Airline" } };
			var mockPdfBytes = new byte[] { 1, 2, 3 };
			_paginationValidationServiceMock.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_inputValidationServiceMock.Setup(s => s.IsValidString(It.IsAny<string>()))
				.Returns(false);
			_airlineServiceMock.Setup(s => s.GetAirlines(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(mockAirlines);
			_exportServiceMock.Setup(s => s.ExportToExcel("Airlines", mockAirlines))
				.Returns(mockPdfBytes);
			_utilityServiceMock.Setup(s => s.GenerateUniqueFileName("Airlines", FileExtension.Xlsx))
				.Returns("Airlines-test.xlsx");

			// Act
			var result = await _controller.ExportToExcel(CancellationToken.None, name: null);

			// Assert
			var fileResult = Assert.IsType<FileContentResult>(result);
			Assert.Equal("Airlines-test.xlsx", fileResult.FileDownloadName);
		}

		/// <summary>
		/// This test ensures the `ExportToExcel` method returns a `204 No Content` response
		/// when the service returns an empty list of airlines to be exported.
		/// </summary>
		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_ReturnsNoContent_WhenAirlinesAreEmpty()
		{
			// Arrange
			_paginationValidationServiceMock.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_inputValidationServiceMock.Setup(s => s.IsValidString(It.IsAny<string>()))
				.Returns(false);
			_airlineServiceMock.Setup(s => s.GetAirlines(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(new List<AirlineEntity>());

			// Act
			var result = await _controller.ExportToExcel(CancellationToken.None);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		/// <summary>
		/// This test checks that the `ExportToExcel` method returns a `204 No Content` response
		/// when the service returns a `null` list of airlines to be exported.
		/// </summary>
		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_ReturnsNoContent_WhenAirlinesIsNull()
		{
			// Arrange
			_paginationValidationServiceMock.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_inputValidationServiceMock.Setup(s => s.IsValidString(It.IsAny<string>()))
				.Returns(false);
			_airlineServiceMock.Setup(s => s.GetAirlines(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync((List<AirlineEntity>)null);

			// Act
			var result = await _controller.ExportToExcel(CancellationToken.None);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		/// <summary>
		/// This test verifies the `ExportToExcel` method returns a `400 BadRequest` response
		/// when the provided page number is invalid.
		/// </summary>
		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_ReturnsBadRequest_WhenPageIsInvalid()
		{
			// Arrange
			_paginationValidationServiceMock.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((false, 0, new BadRequestObjectResult("Invalid page number.")));

			// Act
			var result = await _controller.ExportToExcel(CancellationToken.None, page: 0);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Invalid page number.", badRequestResult.Value);
		}

		/// <summary>
		/// This test verifies the `ExportToExcel` method returns a `400 BadRequest` response
		/// when the provided page size is invalid.
		/// </summary>
		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_ReturnsBadRequest_WhenPageSizeIsInvalid()
		{
			// Arrange
			int maxPageSize = 10;
			_paginationValidationServiceMock.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((false, 0, new BadRequestObjectResult($"Invalid page size. It should be between 1 and {maxPageSize}.")));

			// Act
			var result = await _controller.ExportToExcel(CancellationToken.None, pageSize: 0);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal($"Invalid page size. It should be between 1 and {maxPageSize}.", badRequestResult.Value);
		}

		/// <summary>
		/// This test ensures the `ExportToExcel` method returns a `500 Internal Server Error`
		/// when the Excel generation service returns a `null` byte array.
		/// </summary>
		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_ReturnsInternalServerError_WhenExcelIsNull()
		{
			// Arrange
			var mockAirlines = new List<AirlineEntity> { new AirlineEntity { Id = 1, Name = "Test Airline" } };
			_paginationValidationServiceMock.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_inputValidationServiceMock.Setup(s => s.IsValidString(It.IsAny<string>()))
				.Returns(false);
			_airlineServiceMock.Setup(s => s.GetAirlines(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(mockAirlines);
			_exportServiceMock.Setup(s => s.ExportToExcel("Airlines", mockAirlines))
				.Returns((byte[])null);

			// Act
			var result = await _controller.ExportToExcel(CancellationToken.None);

			// Assert
			var statusCodeResult = Assert.IsType<ObjectResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
			Assert.Equal("Failed to generate Excel file.", statusCodeResult.Value);
		}
		#endregion

	}
}