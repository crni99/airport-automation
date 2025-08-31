using AirportAutomation.Api.Interfaces;
using AirportAutomation.Application.Dtos.PlaneTicket;
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

		[Fact]
		public async Task GetPlaneTicketsForPrice_ReturnsBadRequest_WhenMinPriceAndMaxPriceAreMissing()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;

			// Act
			var result = await _controller.GetPlaneTicketsForPrice(cancellationToken, null, null, page, pageSize);

			// Assert
			Assert.NotNull(result);
		}

		[Fact]
		[Trait("Category", "GetPlaneTicketsByPrice")]
		public async Task GetPlaneTicketsByPrice_InvalidPaginationParameters_ReturnsBadRequest()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int minPrice = 100;
			int maxPrice = 200;
			int invalidPage = -1;
			int invalidPageSize = 0;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid pagination parameters.");

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(invalidPage, invalidPageSize, It.IsAny<int>()))
				.Returns((false, 0, expectedBadRequestResult));

			// Act
			var result = await _controller.GetPlaneTicketsForPrice(cancellationToken, minPrice, maxPrice, invalidPage, invalidPageSize);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetPlaneTicketsByPrice")]
		public async Task GetPlaneTicketsByPrice_PlaneTicketsNotFound_ReturnsNotFound()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int minPrice = 100;
			int maxPrice = 200;
			int validPage = 1;
			int validPageSize = 10;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(validPage, validPageSize, It.IsAny<int>()))
				.Returns((true, validPageSize, null));
			_planeTicketServiceMock
				.Setup(service => service.GetPlaneTicketsForPrice(cancellationToken, validPage, validPageSize, minPrice, maxPrice))
				.ReturnsAsync(new List<PlaneTicketEntity>());

			// Act
			var result = await _controller.GetPlaneTicketsForPrice(cancellationToken, minPrice, maxPrice, validPage, validPageSize);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetPlaneTicketsByPrice")]
		public async Task GetPlaneTicketsByPrice_ReturnsPagedListOfPlaneTickets_WhenPlaneTicketsFound()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int minPrice = 100;
			int maxPrice = 200;
			int validPage = 1;
			int validPageSize = 10;
			var planeTicketEntities = new List<PlaneTicketEntity> { planeTicketEntity };
			var planeTicketDtos = new List<PlaneTicketDto> { planeTicketDto };
			var totalItems = 1;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(validPage, validPageSize, It.IsAny<int>()))
				.Returns((true, validPageSize, null));
			_planeTicketServiceMock
				.Setup(service => service.GetPlaneTicketsForPrice(cancellationToken, validPage, validPageSize, minPrice, maxPrice))
				.ReturnsAsync(planeTicketEntities);
			_planeTicketServiceMock
				.Setup(service => service.PlaneTicketsCount(cancellationToken, minPrice, maxPrice))
				.ReturnsAsync(totalItems);
			_mapperMock
				.Setup(m => m.Map<IEnumerable<PlaneTicketDto>>(planeTicketEntities))
				.Returns(planeTicketDtos);

			// Act
			var result = await _controller.GetPlaneTicketsForPrice(cancellationToken, minPrice, maxPrice, validPage, validPageSize);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PagedResponse<PlaneTicketDto>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var response = Assert.IsType<PagedResponse<PlaneTicketDto>>(okResult.Value);
			Assert.Equal(validPage, response.PageNumber);
			Assert.Equal(validPageSize, response.PageSize);
			Assert.Equal(totalItems, response.TotalCount);
			Assert.Equal(planeTicketDtos, response.Data);
		}

		[Fact]
		[Trait("Category", "PostPlaneTicket")]
		public async Task PostPlaneTicket_ReturnsCreatedAtActionResult_WhenPlaneTicketIsCreatedSuccessfully()
		{
			// Arrange
			var planeTicketCreateDto = new PlaneTicketCreateDto();
			var planeTicketEntity = new PlaneTicketEntity { Id = 1 };
			var planeTicketDto = new PlaneTicketDto { Id = 1 };

			// Set up the mapper to return the expected values
			_mapperMock.Setup(m => m.Map<PlaneTicketEntity>(planeTicketCreateDto)).Returns(planeTicketEntity);
			_mapperMock.Setup(m => m.Map<PlaneTicketDto>(planeTicketEntity)).Returns(planeTicketDto);

			// Adjust service setup to return the planeTicketEntity wrapped in a Task
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

	}
}