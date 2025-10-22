using AirportAutomation.Api.Controllers;
using AirportAutomation.Api.Interfaces;
using AirportAutomation.Application.Dtos.PlaneTicket;
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

	public class PlaneTicketsControllerTests
	{
		private readonly PlaneTicketsController _controller;
		private readonly Mock<IPlaneTicketService> _planeTicketServiceMock;
		private readonly Mock<IPaginationValidationService> _paginationValidationServiceMock;
		private readonly Mock<IInputValidationService> _inputValidationServiceMock;
		private readonly Mock<IUtilityService> _utilityServiceMock;
		private readonly Mock<IExportService> _exportServiceMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<ILogger<PlaneTicketsController>> _loggerMock;
		private readonly Mock<IConfiguration> _configurationMock;

		private readonly PlaneTicketEntity planeTicketEntity = new()
		{
			Id = 1,
			Price = 200,
			PurchaseDate = new DateOnly(2023, 09, 20),
			SeatNumber = 1,
			PassengerId = 1,
			TravelClassId = 1,
			FlightId = 1
		};

		private readonly PlaneTicketDto planeTicketDto = new()
		{
			Id = 2,
			Price = 400,
			PurchaseDate = new DateOnly(2023, 09, 20),
			SeatNumber = 2,
			PassengerId = 2,
			TravelClassId = 2,
			FlightId = 2
		};

		public PlaneTicketsControllerTests()
		{
			_planeTicketServiceMock = new Mock<IPlaneTicketService>();
			_paginationValidationServiceMock = new Mock<IPaginationValidationService>();
			_inputValidationServiceMock = new Mock<IInputValidationService>();
			_utilityServiceMock = new Mock<IUtilityService>();
			_exportServiceMock = new Mock<IExportService>();
			_mapperMock = new Mock<IMapper>();
			_loggerMock = new Mock<ILogger<PlaneTicketsController>>();
			_configurationMock = new Mock<IConfiguration>();
			var configBuilder = new ConfigurationBuilder();
			configBuilder.AddInMemoryCollection(new Dictionary<string, string>
			{
				{"pageSettings:maxPageSize", "10"}
			});
			_configurationMock.Setup(x => x.GetSection(It.IsAny<string>()))
				.Returns(configBuilder.Build().GetSection(""));

			_controller = new PlaneTicketsController(
				_planeTicketServiceMock.Object,
				_paginationValidationServiceMock.Object,
				_inputValidationServiceMock.Object,
				_utilityServiceMock.Object,
				_exportServiceMock.Object,
				_mapperMock.Object,
				_loggerMock.Object,
				_configurationMock.Object
			);
		}

		[Theory]
		[Trait("Category", "Constructor")]
		[InlineData("planeTicketService")]
		[InlineData("paginationValidationService")]
		[InlineData("inputValidationService")]
		[InlineData("utilityService")]
		[InlineData("exportService")]
		[InlineData("mapper")]
		[InlineData("logger")]
		public void Constructor_WhenServiceIsNull_ThrowsArgumentNullException(string serviceName)
		{
			// Arrange
			var planeTicketServiceMock = new Mock<IPlaneTicketService>();
			var paginationValidationServiceMock = new Mock<IPaginationValidationService>();
			var inputValidationServiceMock = new Mock<IInputValidationService>();
			var utilityServiceMock = new Mock<IUtilityService>();
			var exportServiceMock = new Mock<IExportService>();
			var mapperMock = new Mock<IMapper>();
			var loggerMock = new Mock<ILogger<PlaneTicketsController>>();
			var configurationMock = new Mock<IConfiguration>();

			// Set up mocks to return null based on the test case
			IPlaneTicketService planeTicketService = serviceName == "planeTicketService" ? null : planeTicketServiceMock.Object;
			IPaginationValidationService paginationValidationService = serviceName == "paginationValidationService" ? null : paginationValidationServiceMock.Object;
			IInputValidationService inputValidationService = serviceName == "inputValidationService" ? null : inputValidationServiceMock.Object;
			IUtilityService utilityService = serviceName == "utilityService" ? null : utilityServiceMock.Object;
			IExportService exportService = serviceName == "exportService" ? null : exportServiceMock.Object;
			IMapper mapper = serviceName == "mapper" ? null : mapperMock.Object;
			ILogger<PlaneTicketsController> logger = serviceName == "logger" ? null : loggerMock.Object;

			// Act & Assert
			var exception = Record.Exception(() => new PlaneTicketsController(
				planeTicketService,
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

		#region GetPlaneTickets

		[Fact]
		[Trait("Category", "GetPlaneTickets")]
		public async Task GetPlaneTickets_InvalidPaginationParameters_ReturnsBadRequest()
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
			var result = await _controller.GetPlaneTickets(cancellationToken, invalidPage, invalidPageSize);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetPlaneTickets")]
		public async Task GetPlaneTickets_ReturnsNoContent_WhenNoPlaneTicketsFound()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_planeTicketServiceMock.Setup(service => service.GetPlaneTickets(cancellationToken, It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(new List<PlaneTicketEntity>());

			// Act
			var result = await _controller.GetPlaneTickets(cancellationToken, page, pageSize);

			// Assert
			Assert.IsType<NoContentResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetPlaneTickets")]
		public async Task GetPlaneTickets_ReturnsNoContent_WhenPlaneTicketsListIsNull()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_planeTicketServiceMock.Setup(service => service.GetPlaneTickets(cancellationToken, It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync((List<PlaneTicketEntity>)null);

			// Act
			var result = await _controller.GetPlaneTickets(cancellationToken, page, pageSize);

			// Assert
			Assert.IsType<NoContentResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetPlaneTickets")]
		public async Task GetPlaneTickets_ReturnsInternalServerError_WhenExceptionThrown()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_planeTicketServiceMock.Setup(service => service.GetPlaneTickets(cancellationToken, It.IsAny<int>(), It.IsAny<int>()))
				.ThrowsAsync(new Exception("Simulated exception"));

			// Act & Assert
			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetPlaneTickets(cancellationToken, page, pageSize));
		}

		[Fact]
		[Trait("Category", "GetPlaneTickets")]
		public async Task GetPlaneTickets_ReturnsOk_WithPaginatedPlaneTickets()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;
			var planeTickets = new List<PlaneTicketEntity>
			{
				new PlaneTicketEntity { /* Initialize properties */ },
				new PlaneTicketEntity { /* Initialize properties */ }
			};
			var totalItems = 2;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_planeTicketServiceMock
				.Setup(service => service.GetPlaneTickets(cancellationToken, page, pageSize))
				.ReturnsAsync(planeTickets);
			_planeTicketServiceMock
				.Setup(service => service.PlaneTicketsCount(cancellationToken, null, null))
				.ReturnsAsync(totalItems);

			var expectedData = new List<PlaneTicketDto>
			{
				new PlaneTicketDto { /* Initialize properties */ },
				new PlaneTicketDto { /* Initialize properties */ }
			};
			_mapperMock
				.Setup(m => m.Map<IEnumerable<PlaneTicketDto>>(It.IsAny<IEnumerable<PlaneTicketEntity>>()))
				.Returns(expectedData);

			// Act
			var result = await _controller.GetPlaneTickets(cancellationToken, page, pageSize);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PagedResponse<PlaneTicketDto>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var pagedResponse = Assert.IsType<PagedResponse<PlaneTicketDto>>(okResult.Value);
			Assert.Equal(page, pagedResponse.PageNumber);
			Assert.Equal(pageSize, pagedResponse.PageSize);
			Assert.Equal(totalItems, pagedResponse.TotalCount);
			Assert.Equal(expectedData, pagedResponse.Data);
		}

		[Fact]
		[Trait("Category", "GetPlaneTickets")]
		public async Task GetPlaneTickets_ReturnsCorrectPageData()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 2;
			int pageSize = 5;
			var allPlaneTickets = new List<PlaneTicketEntity>
			{
				new PlaneTicketEntity { /* Initialize properties */ },
				new PlaneTicketEntity { /* Initialize properties */ },
				new PlaneTicketEntity { /* Initialize properties */ },
				new PlaneTicketEntity { /* Initialize properties */ },
				new PlaneTicketEntity { /* Initialize properties */ },
				new PlaneTicketEntity { /* Initialize properties */ },
				new PlaneTicketEntity { /* Initialize properties */ },
				new PlaneTicketEntity { /* Initialize properties */ },
				new PlaneTicketEntity { /* Initialize properties */ },
				new PlaneTicketEntity { /* Initialize properties */ }
			};
			var pagedPlaneTickets = allPlaneTickets.Skip((page - 1) * pageSize).Take(pageSize).ToList();

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_planeTicketServiceMock
				.Setup(service => service.GetPlaneTickets(cancellationToken, page, pageSize))
				.ReturnsAsync(pagedPlaneTickets);
			_planeTicketServiceMock
				.Setup(service => service.PlaneTicketsCount(cancellationToken, null, null))
				.ReturnsAsync(allPlaneTickets.Count);

			var expectedData = new List<PlaneTicketDto>
			{
				new PlaneTicketDto { /* Initialize properties */ },
				new PlaneTicketDto { /* Initialize properties */ },
				new PlaneTicketDto { /* Initialize properties */ },
				new PlaneTicketDto { /* Initialize properties */ },
				new PlaneTicketDto { /* Initialize properties */ }
			};
			_mapperMock
				.Setup(m => m.Map<IEnumerable<PlaneTicketDto>>(It.IsAny<IEnumerable<PlaneTicketEntity>>()))
				.Returns(expectedData);

			// Act
			var result = await _controller.GetPlaneTickets(cancellationToken, page, pageSize);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PagedResponse<PlaneTicketDto>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var pagedResponse = Assert.IsType<PagedResponse<PlaneTicketDto>>(okResult.Value);
			Assert.Equal(page, pagedResponse.PageNumber);
			Assert.Equal(pageSize, pagedResponse.PageSize);
			Assert.Equal(allPlaneTickets.Count, pagedResponse.TotalCount);
			Assert.Equal(expectedData, pagedResponse.Data);
		}

		#endregion

		#region GetPlaneTicket

		[Fact]
		[Trait("Category", "GetPlaneTicket")]
		public async Task GetPlaneTicket_InvalidId_ReturnsBadRequest()
		{
			// Arrange
			int invalidId = -1;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid input. The ID must be a non-negative integer.");

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(invalidId))
				.Returns(false);

			// Act
			var result = await _controller.GetPlaneTicket(invalidId);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
			var badRequestResult = result.Result as BadRequestObjectResult;
			Assert.Equal(expectedBadRequestResult.Value, badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "GetPlaneTicket")]
		public async Task GetPlaneTicket_PlaneTicketNotFound_ReturnsNotFound()
		{
			// Arrange
			int validId = 1;

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(validId))
				.Returns(true);
			_planeTicketServiceMock
				.Setup(service => service.PlaneTicketExists(validId))
				.ReturnsAsync(false);

			// Act
			var result = await _controller.GetPlaneTicket(validId);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetPlaneTicket")]
		public async Task GetPlaneTicket_ReturnsPlaneTicketDto_WhenPlaneTicketExists()
		{
			// Arrange
			int validId = 1;

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(validId))
				.Returns(true);
			_planeTicketServiceMock
				.Setup(service => service.PlaneTicketExists(validId))
				.ReturnsAsync(true);
			_planeTicketServiceMock
				.Setup(service => service.GetPlaneTicket(validId))
				.ReturnsAsync(planeTicketEntity);
			_mapperMock
				.Setup(m => m.Map<PlaneTicketDto>(planeTicketEntity))
				.Returns(planeTicketDto);

			// Act
			var result = await _controller.GetPlaneTicket(validId);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PlaneTicketDto>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var returnedPlaneTicketDto = Assert.IsType<PlaneTicketDto>(okResult.Value);
			Assert.Equal(planeTicketDto, returnedPlaneTicketDto);
		}

		#endregion

		#region SearchPlaneTickets

		[Fact]
		[Trait("Category", "SearchPlaneTickets")]
		public async Task SearchPlaneTickets_EmptyFilter_ReturnsBadRequest()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			// Assuming IsEmpty() is a static extension method, we don't need to mock it.
			var emptyFilter = new PlaneTicketSearchFilter();
			var expectedBadRequestResult = "At least one filter criterion must be provided.";

			// Act
			var result = await _controller.SearchPlaneTickets(cancellationToken, emptyFilter);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal(expectedBadRequestResult, badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "SearchPlaneTickets")]
		public async Task SearchPlaneTickets_InvalidPaginationParameters_ReturnsBadRequest()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var filter = new PlaneTicketSearchFilter { SeatNumber = 16 };
			int invalidPage = -1;
			int invalidPageSize = 0;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid pagination parameters.");

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(invalidPage, invalidPageSize, It.IsAny<int>()))
				.Returns((false, 0, expectedBadRequestResult));

			// Act
			var result = await _controller.SearchPlaneTickets(cancellationToken, filter, invalidPage, invalidPageSize);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "SearchPlaneTickets")]
		public async Task SearchPlaneTickets_PlaneTicketsNotFound_ReturnsNotFound()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var filter = new PlaneTicketSearchFilter { SeatNumber = 16 };
			int page = 1;
			int pageSize = 10;
			var emptyPlaneTicketsList = new List<PlaneTicketEntity>();

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(page, pageSize, It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_planeTicketServiceMock
				.Setup(service => service.SearchPlaneTickets(cancellationToken, page, pageSize, filter))
				.ReturnsAsync(emptyPlaneTicketsList);

			// Act
			var result = await _controller.SearchPlaneTickets(cancellationToken, filter, page, pageSize);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "SearchPlaneTickets")]
		public async Task SearchPlaneTickets_ReturnsNotFound_WhenPlaneTicketsListIsNull()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var filter = new PlaneTicketSearchFilter { SeatNumber = 16 };
			int page = 1;
			int pageSize = 10;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(page, pageSize, It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_planeTicketServiceMock
				.Setup(service => service.SearchPlaneTickets(cancellationToken, page, pageSize, filter))
				.ReturnsAsync((List<PlaneTicketEntity>)null);

			// Act
			var result = await _controller.SearchPlaneTickets(cancellationToken, filter, page, pageSize);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "SearchPlaneTickets")]
		public async Task SearchPlaneTickets_ReturnsOk_WithPaginatedPlaneTickets()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var filter = new PlaneTicketSearchFilter { SeatNumber = 16 };
			int page = 1;
			int pageSize = 10;
			var planeTicketEntities = new List<PlaneTicketEntity> { new PlaneTicketEntity(), new PlaneTicketEntity() };
			var planeTicketDtos = new List<PlaneTicketDto> { new PlaneTicketDto(), new PlaneTicketDto() };
			var totalItems = 2;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(page, pageSize, It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_planeTicketServiceMock
				.Setup(service => service.SearchPlaneTickets(cancellationToken, page, pageSize, filter))
				.ReturnsAsync(planeTicketEntities);
			_planeTicketServiceMock
				.Setup(service => service.PlaneTicketsCountFilter(cancellationToken, filter))
				.ReturnsAsync(totalItems);
			_mapperMock
				.Setup(m => m.Map<IEnumerable<PlaneTicketDto>>(planeTicketEntities))
				.Returns(planeTicketDtos);

			// Act
			var result = await _controller.SearchPlaneTickets(cancellationToken, filter, page, pageSize);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PagedResponse<PlaneTicketDto>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var response = Assert.IsType<PagedResponse<PlaneTicketDto>>(okResult.Value);
			Assert.Equal(page, response.PageNumber);
			Assert.Equal(pageSize, response.PageSize);
			Assert.Equal(totalItems, response.TotalCount);
			Assert.Equal(planeTicketDtos, response.Data);
		}

		#endregion

		#region PostPlaneTicket

		[Fact]
		[Trait("Category", "PostPlaneTicket")]
		public async Task PostPlaneTicket_ReturnsCreatedAtActionResult_WhenPlaneTicketIsCreatedSuccessfully()
		{
			// Arrange
			var planeTicketCreateDto = new PlaneTicketCreateDto();
			var planeTicketEntity = new PlaneTicketEntity { Id = 1 };
			var planeTicketDto = new PlaneTicketDto { Id = 1 };

			_mapperMock.Setup(m => m.Map<PlaneTicketEntity>(planeTicketCreateDto)).Returns(planeTicketEntity);
			_mapperMock.Setup(m => m.Map<PlaneTicketDto>(planeTicketEntity)).Returns(planeTicketDto);
			_planeTicketServiceMock.Setup(service => service.PostPlaneTicket(planeTicketEntity))
							   .ReturnsAsync(planeTicketEntity);

			// Act
			var result = await _controller.PostPlaneTicket(planeTicketCreateDto);

			// Assert
			var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
			var returnedValue = Assert.IsType<PlaneTicketDto>(actionResult.Value);
			Assert.Equal(planeTicketDto.Id, returnedValue.Id);
			Assert.Equal("GetPlaneTicket", actionResult.ActionName);
			Assert.Equal(1, actionResult.RouteValues["id"]);
		}

		[Fact]
		[Trait("Category", "PostPlaneTicket")]
		public async Task PostPlaneTicket_ThrowsException_WhenServiceFails()
		{
			// Arrange
			var planeTicketCreateDto = new PlaneTicketCreateDto();
			var planeTicketEntity = new PlaneTicketEntity();
			_mapperMock.Setup(m => m.Map<PlaneTicketEntity>(planeTicketCreateDto)).Returns(planeTicketEntity);

			// Set up the service to throw an exception
			_planeTicketServiceMock.Setup(service => service.PostPlaneTicket(planeTicketEntity))
							   .ThrowsAsync(new Exception("Simulated exception"));

			// Act & Assert
			await Assert.ThrowsAsync<Exception>(async () => await _controller.PostPlaneTicket(planeTicketCreateDto));
		}

		#endregion

		#region PutPlaneTicket

		[Fact]
		[Trait("Category", "PutPlaneTicket")]
		public async Task PutPlaneTicket_ReturnsNoContent_WhenUpdateIsSuccessful()
		{
			// Arrange
			int id = 1;
			var planeTicketUpdateDto = new PlaneTicketUpdateDto { Id = id };
			var planeTicketEntity = new PlaneTicketEntity { Id = id };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_mapperMock.Setup(m => m.Map<PlaneTicketEntity>(planeTicketUpdateDto)).Returns(planeTicketEntity);
			_planeTicketServiceMock.Setup(service => service.PlaneTicketExists(id)).ReturnsAsync(true);
			_planeTicketServiceMock.Setup(service => service.PutPlaneTicket(planeTicketEntity)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.PutPlaneTicket(id, planeTicketUpdateDto);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "PutPlaneTicket")]
		public async Task PutPlaneTicket_ReturnsBadRequest_WhenIdIsInvalid()
		{
			// Arrange
			int invalidId = -1;
			var planeTicketUpdateDto = new PlaneTicketUpdateDto { Id = invalidId };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(invalidId)).Returns(false);

			// Act
			var result = await _controller.PutPlaneTicket(invalidId, planeTicketUpdateDto);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Invalid input. The ID must be a non-negative integer.", badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "PutPlaneTicket")]
		public async Task PutPlaneTicket_ReturnsBadRequest_WhenIdInDtoDoesNotMatchIdInUrl()
		{
			// Arrange
			int id = 1;
			var planeTicketUpdateDto = new PlaneTicketUpdateDto { Id = 2 };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);

			// Act
			var result = await _controller.PutPlaneTicket(id, planeTicketUpdateDto);

			// Assert
			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		[Trait("Category", "PutPlaneTicket")]
		public async Task PutPlaneTicket_ReturnsNotFound_WhenPlaneTicketDoesNotExist()
		{
			// Arrange
			int id = 1;
			var planeTicketUpdateDto = new PlaneTicketUpdateDto { Id = id };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_planeTicketServiceMock.Setup(service => service.PlaneTicketExists(id)).ReturnsAsync(false);

			// Act
			var result = await _controller.PutPlaneTicket(id, planeTicketUpdateDto);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		#endregion

		#region PatchPlaneTicket

		[Fact]
		[Trait("Category", "PatchPlaneTicket")]
		public async Task PatchPlaneTicket_ReturnsOk_WhenUpdateIsSuccessful()
		{
			// Arrange
			int id = 1;
			var planeTicketDocument = new JsonPatchDocument();
			var updatedPlaneTicket = new PlaneTicketEntity { Id = id };
			var planeTicketDto = new PlaneTicketDto { Id = id };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_planeTicketServiceMock.Setup(service => service.PlaneTicketExists(id)).ReturnsAsync(true);
			_planeTicketServiceMock.Setup(service => service.PatchPlaneTicket(id, planeTicketDocument)).ReturnsAsync(updatedPlaneTicket);
			_mapperMock.Setup(m => m.Map<PlaneTicketDto>(updatedPlaneTicket)).Returns(planeTicketDto);

			// Act
			var result = await _controller.PatchPlaneTicket(id, planeTicketDocument);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(planeTicketDto, okResult.Value);
		}

		[Fact]
		[Trait("Category", "PatchPlaneTicket")]
		public async Task PatchPlaneTicket_ReturnsBadRequest_WhenIdIsInvalid()
		{
			// Arrange
			int invalidId = -1;
			var planeTicketDocument = new JsonPatchDocument();

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(invalidId)).Returns(false);

			// Act
			var result = await _controller.PatchPlaneTicket(invalidId, planeTicketDocument);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Invalid input. The ID must be a non-negative integer.", badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "PatchPlaneTicket")]
		public async Task PatchPlaneTicket_ReturnsNotFound_WhenPlaneTicketDoesNotExist()
		{
			// Arrange
			int id = 1;
			var planeTicketDocument = new JsonPatchDocument();

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_planeTicketServiceMock.Setup(service => service.PlaneTicketExists(id)).ReturnsAsync(false);

			// Act
			var result = await _controller.PatchPlaneTicket(id, planeTicketDocument);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		#endregion

		#region DeletePlaneTicket

		[Fact]
		[Trait("Category", "DeletePlaneTicket")]
		public async Task DeletePlaneTicket_ReturnsNoContent_WhenDeletionIsSuccessful()
		{
			// Arrange
			int id = 1;
			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_planeTicketServiceMock.Setup(service => service.PlaneTicketExists(id)).ReturnsAsync(true);
			_planeTicketServiceMock.Setup(service => service.DeletePlaneTicket(id)).ReturnsAsync(true);

			// Act
			var result = await _controller.DeletePlaneTicket(id);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "DeletePlaneTicket")]
		public async Task DeletePlaneTicket_ReturnsBadRequest_WhenIdIsInvalid()
		{
			// Arrange
			int invalidId = -1;
			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(invalidId)).Returns(false);

			// Act
			var result = await _controller.DeletePlaneTicket(invalidId);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Invalid input. The ID must be a non-negative integer.", badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "DeletePlaneTicket")]
		public async Task DeletePlaneTicket_ReturnsNotFound_WhenPlaneTicketDoesNotExist()
		{
			// Arrange
			int id = 1;
			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_planeTicketServiceMock.Setup(service => service.PlaneTicketExists(id)).ReturnsAsync(false);

			// Act
			var result = await _controller.DeletePlaneTicket(id);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "DeletePlaneTicket")]
		public async Task DeletePlaneTicket_ReturnsConflict_WhenPlaneTicketCannotBeDeleted()
		{
			// Arrange
			int id = 1;
			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_planeTicketServiceMock.Setup(service => service.PlaneTicketExists(id)).ReturnsAsync(true);
			_planeTicketServiceMock.Setup(service => service.DeletePlaneTicket(id)).ReturnsAsync(false);

			// Act
			var result = await _controller.DeletePlaneTicket(id);

			// Assert
			var conflictResult = Assert.IsType<ConflictResult>(result);
		}

		#endregion

		#region ExportToPdf

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_GetAllIsTrue_ReturnsFileContent()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var pilots = new List<PlaneTicketEntity> { new PlaneTicketEntity(), new PlaneTicketEntity() };
			var pdfBytes = new byte[] { 1, 2, 3, 4 };
			var fileName = "PlaneTickets.pdf";
			var getAll = true;

			_planeTicketServiceMock
				.Setup(s => s.GetAllPlaneTickets(cancellationToken))
				.ReturnsAsync(pilots);
			_exportServiceMock
				.Setup(s => s.ExportToPDF("Plane Tickets", pilots))
				.Returns(pdfBytes);
			_utilityServiceMock
				.Setup(s => s.GenerateUniqueFileName("PlaneTickets", FileExtension.Pdf))
				.Returns(fileName);

			// Act
			var result = await _controller.ExportToPdf(cancellationToken, null, getAll: getAll);

			// Assert
			var fileResult = Assert.IsType<FileContentResult>(result);
			Assert.Equal("application/pdf", fileResult.ContentType);
			Assert.Equal(fileName, fileResult.FileDownloadName);
			Assert.Equal(pdfBytes, fileResult.FileContents);
		}

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_EmptyFilter_ReturnsFileContent()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var filter = new PlaneTicketSearchFilter { SeatNumber = null };
			int page = 1;
			int pageSize = 10;
			var pilots = new List<PlaneTicketEntity> { new PlaneTicketEntity() };
			var pdfBytes = new byte[] { 1, 2, 3, 4 };
			var fileName = "PlaneTickets.pdf";

			_paginationValidationServiceMock
				.Setup(s => s.ValidatePaginationParameters(page, pageSize, It.IsAny<int>()))
				.Returns((true, pageSize, null));
			// Assuming IsEmpty is a static extension method, no mock setup is needed.
			_planeTicketServiceMock
				.Setup(s => s.GetPlaneTickets(cancellationToken, page, pageSize))
				.ReturnsAsync(pilots);
			_exportServiceMock
				.Setup(s => s.ExportToPDF("Plane Tickets", pilots))
				.Returns(pdfBytes);
			_utilityServiceMock
				.Setup(s => s.GenerateUniqueFileName("PlaneTickets", FileExtension.Pdf))
				.Returns(fileName);

			// Act
			var result = await _controller.ExportToPdf(cancellationToken, filter, page, pageSize);

			// Assert
			var fileResult = Assert.IsType<FileContentResult>(result);
			Assert.Equal("application/pdf", fileResult.ContentType);
			Assert.Equal(fileName, fileResult.FileDownloadName);
			Assert.Equal(pdfBytes, fileResult.FileContents);
		}

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_NonEmptyFilter_ReturnsFileContent()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var filter = new PlaneTicketSearchFilter { SeatNumber = 16 };
			int page = 1;
			int pageSize = 10;
			var pilots = new List<PlaneTicketEntity> { new PlaneTicketEntity() };
			var pdfBytes = new byte[] { 1, 2, 3, 4 };
			var fileName = "PlaneTickets.pdf";

			_paginationValidationServiceMock
				.Setup(s => s.ValidatePaginationParameters(page, pageSize, It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_planeTicketServiceMock
				.Setup(s => s.SearchPlaneTickets(cancellationToken, page, pageSize, filter))
				.ReturnsAsync(pilots);
			_exportServiceMock
				.Setup(s => s.ExportToPDF("Plane Tickets", pilots))
				.Returns(pdfBytes);
			_utilityServiceMock
				.Setup(s => s.GenerateUniqueFileName("PlaneTickets", FileExtension.Pdf))
				.Returns(fileName);

			// Act
			var result = await _controller.ExportToPdf(cancellationToken, filter, page, pageSize);

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
			var filter = new PlaneTicketSearchFilter { SeatNumber = 16 };
			int invalidPage = -1;
			int invalidPageSize = 0;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid pagination parameters.");

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(invalidPage, invalidPageSize, It.IsAny<int>()))
				.Returns((false, 0, expectedBadRequestResult));

			// Act
			var result = await _controller.ExportToPdf(cancellationToken, filter, invalidPage, invalidPageSize);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_NoPilotsFound_ReturnsNoContent()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var filter = new PlaneTicketSearchFilter();
			var pilots = new List<PlaneTicketEntity>();
			var getAll = true;

			_planeTicketServiceMock
				.Setup(s => s.GetAllPlaneTickets(cancellationToken))
				.ReturnsAsync(pilots);

			// Act
			var result = await _controller.ExportToPdf(cancellationToken, filter, getAll: getAll);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_PilotsListIsNull_ReturnsNoContent()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var filter = new PlaneTicketSearchFilter();
			var getAll = true;

			_planeTicketServiceMock
				.Setup(s => s.GetAllPlaneTickets(cancellationToken))
				.ReturnsAsync((IList<PlaneTicketEntity>)null);

			// Act
			var result = await _controller.ExportToPdf(cancellationToken, filter, getAll: getAll);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_PdfGenerationFails_ReturnsInternalServerError()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var pilots = new List<PlaneTicketEntity> { new PlaneTicketEntity() };
			var getAll = true;

			_planeTicketServiceMock
				.Setup(s => s.GetAllPlaneTickets(cancellationToken))
				.ReturnsAsync(pilots);
			_exportServiceMock
				.Setup(s => s.ExportToPDF("Plane Tickets", pilots))
				.Returns((byte[])null);

			// Act
			var result = await _controller.ExportToPdf(cancellationToken, null, getAll: getAll);

			// Assert
			var statusCodeResult = Assert.IsType<ObjectResult>(result);
			Assert.Equal(500, statusCodeResult.StatusCode);
			Assert.Equal("Failed to generate PDF file.", statusCodeResult.Value);
		}

		#endregion

		#region ExportToExcel

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_GetAllIsTrue_ReturnsFileContent()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var planeTickets = new List<PlaneTicketEntity> { new PlaneTicketEntity(), new PlaneTicketEntity() };
			var excelBytes = new byte[] { 1, 2, 3, 4 };
			var fileName = "PlaneTickets.xlsx";
			var getAll = true;

			_planeTicketServiceMock
				.Setup(s => s.GetAllPlaneTickets(cancellationToken))
				.ReturnsAsync(planeTickets);
			_exportServiceMock
				.Setup(s => s.ExportToExcel("PlaneTickets", planeTickets))
				.Returns(excelBytes);
			_utilityServiceMock
				.Setup(s => s.GenerateUniqueFileName("PlaneTickets", FileExtension.Xlsx))
				.Returns(fileName);

			// Act
			var result = await _controller.ExportToExcel(cancellationToken, null, getAll: getAll);

			// Assert
			var fileResult = Assert.IsType<FileContentResult>(result);
			Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileResult.ContentType);
			Assert.Equal(fileName, fileResult.FileDownloadName);
			Assert.Equal(excelBytes, fileResult.FileContents);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_EmptyFilter_ReturnsFileContent()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var filter = new PlaneTicketSearchFilter { SeatNumber = null };
			int page = 1;
			int pageSize = 10;
			var planeTickets = new List<PlaneTicketEntity> { new PlaneTicketEntity() };
			var excelBytes = new byte[] { 1, 2, 3, 4 };
			var fileName = "PlaneTickets.xlsx";

			_paginationValidationServiceMock
				.Setup(s => s.ValidatePaginationParameters(page, pageSize, It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_planeTicketServiceMock
				.Setup(s => s.GetPlaneTickets(cancellationToken, page, pageSize))
				.ReturnsAsync(planeTickets);
			_exportServiceMock
				.Setup(s => s.ExportToExcel("PlaneTickets", planeTickets))
				.Returns(excelBytes);
			_utilityServiceMock
				.Setup(s => s.GenerateUniqueFileName("PlaneTickets", FileExtension.Xlsx))
				.Returns(fileName);

			// Act
			var result = await _controller.ExportToExcel(cancellationToken, filter, page, pageSize);

			// Assert
			var fileResult = Assert.IsType<FileContentResult>(result);
			Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileResult.ContentType);
			Assert.Equal(fileName, fileResult.FileDownloadName);
			Assert.Equal(excelBytes, fileResult.FileContents);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_NonEmptyFilter_ReturnsFileContent()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var filter = new PlaneTicketSearchFilter { SeatNumber = 16 };
			int page = 1;
			int pageSize = 10;
			var planeTickets = new List<PlaneTicketEntity> { new PlaneTicketEntity() };
			var excelBytes = new byte[] { 1, 2, 3, 4 };
			var fileName = "PlaneTickets.xlsx";

			_paginationValidationServiceMock
				.Setup(s => s.ValidatePaginationParameters(page, pageSize, It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_planeTicketServiceMock
				.Setup(s => s.SearchPlaneTickets(cancellationToken, page, pageSize, filter))
				.ReturnsAsync(planeTickets);
			_exportServiceMock
				.Setup(s => s.ExportToExcel("PlaneTickets", planeTickets))
				.Returns(excelBytes);
			_utilityServiceMock
				.Setup(s => s.GenerateUniqueFileName("PlaneTickets", FileExtension.Xlsx))
				.Returns(fileName);

			// Act
			var result = await _controller.ExportToExcel(cancellationToken, filter, page, pageSize);

			// Assert
			var fileResult = Assert.IsType<FileContentResult>(result);
			Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileResult.ContentType);
			Assert.Equal(fileName, fileResult.FileDownloadName);
			Assert.Equal(excelBytes, fileResult.FileContents);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_InvalidPaginationParameters_ReturnsBadRequest()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var filter = new PlaneTicketSearchFilter { SeatNumber = 16 };
			int invalidPage = -1;
			int invalidPageSize = 0;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid pagination parameters.");

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(invalidPage, invalidPageSize, It.IsAny<int>()))
				.Returns((false, 0, expectedBadRequestResult));

			// Act
			var result = await _controller.ExportToExcel(cancellationToken, filter, invalidPage, invalidPageSize);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_NoPlaneTicketsFound_ReturnsNoContent()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var filter = new PlaneTicketSearchFilter();
			var planeTickets = new List<PlaneTicketEntity>();
			var getAll = true;

			_planeTicketServiceMock
				.Setup(s => s.GetAllPlaneTickets(cancellationToken))
				.ReturnsAsync(planeTickets);

			// Act
			var result = await _controller.ExportToExcel(cancellationToken, filter, getAll: getAll);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_PlaneTicketsListIsNull_ReturnsNoContent()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var filter = new PlaneTicketSearchFilter();
			var getAll = true;

			_planeTicketServiceMock
				.Setup(s => s.GetAllPlaneTickets(cancellationToken))
				.ReturnsAsync((IList<PlaneTicketEntity>)null);

			// Act
			var result = await _controller.ExportToExcel(cancellationToken, filter, getAll: getAll);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_ExcelGenerationFails_ReturnsInternalServerError()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var planeTickets = new List<PlaneTicketEntity> { new PlaneTicketEntity() };
			var getAll = true;

			_planeTicketServiceMock
				.Setup(s => s.GetAllPlaneTickets(cancellationToken))
				.ReturnsAsync(planeTickets);
			_exportServiceMock
				.Setup(s => s.ExportToExcel("PlaneTickets", planeTickets))
				.Returns((byte[])null);

			// Act
			var result = await _controller.ExportToExcel(cancellationToken, null, getAll: getAll);

			// Assert
			var statusCodeResult = Assert.IsType<ObjectResult>(result);
			Assert.Equal(500, statusCodeResult.StatusCode);
			Assert.Equal("Failed to generate Excel file.", statusCodeResult.Value);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_ExcelGenerationReturnsEmptyBytes_ReturnsInternalServerError()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			var planeTickets = new List<PlaneTicketEntity> { new PlaneTicketEntity() };
			var getAll = true;

			_planeTicketServiceMock
				.Setup(s => s.GetAllPlaneTickets(cancellationToken))
				.ReturnsAsync(planeTickets);
			_exportServiceMock
				.Setup(s => s.ExportToExcel("PlaneTickets", planeTickets))
				.Returns(Array.Empty<byte>());

			// Act
			var result = await _controller.ExportToExcel(cancellationToken, null, getAll: getAll);

			// Assert
			var statusCodeResult = Assert.IsType<ObjectResult>(result);
			Assert.Equal(500, statusCodeResult.StatusCode);
			Assert.Equal("Failed to generate Excel file.", statusCodeResult.Value);
		}

		#endregion

	}
}