using AirportAutomation.Api.Controllers;
using AirportAutomation.Api.Interfaces;
using AirportAutomation.Application.Dtos.Pilot;
using AirportAutomation.Application.Dtos.Response;
using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IServices;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace AirportAutomationApi.Test.Controllers
{
	public class PilotsControllerTests
	{
		private readonly PilotsController _controller;
		private readonly Mock<IPilotService> _pilotServiceMock;
		private readonly Mock<IPaginationValidationService> _paginationValidationServiceMock;
		private readonly Mock<IInputValidationService> _inputValidationServiceMock;
		private readonly Mock<IUtilityService> _utilityServiceMock;
		private readonly Mock<IExportService> _exportServiceMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<ILogger<PilotsController>> _loggerMock;
		private readonly Mock<IConfiguration> _configurationMock;

		private readonly PilotEntity pilotEntity = new()
		{
			Id = 1,
			FirstName = "Ognjen",
			LastName = "Andjelic",
			UPRN = "1234567890123",
			FlyingHours = 100
		};

		private readonly PilotDto pilotDto = new()
		{
			Id = 1,
			FirstName = "Ognjen",
			LastName = "Andjelic",
			UPRN = "1234567890123",
			FlyingHours = 100
		};
		public PilotsControllerTests()
		{
			_pilotServiceMock = new Mock<IPilotService>();
			_paginationValidationServiceMock = new Mock<IPaginationValidationService>();
			_inputValidationServiceMock = new Mock<IInputValidationService>();
			_utilityServiceMock = new Mock<IUtilityService>();
			_exportServiceMock = new Mock<IExportService>();
			_mapperMock = new Mock<IMapper>();
			_loggerMock = new Mock<ILogger<PilotsController>>();
			_configurationMock = new Mock<IConfiguration>();
			var configBuilder = new ConfigurationBuilder();
			configBuilder.AddInMemoryCollection(new Dictionary<string, string>
			{
				{"pageSettings:maxPageSize", "10"}
			});
			_configurationMock.Setup(x => x.GetSection(It.IsAny<string>()))
				.Returns(configBuilder.Build().GetSection(""));

			_controller = new PilotsController(
				_pilotServiceMock.Object,
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
		[InlineData("pilotService")]
		[InlineData("paginationValidationService")]
		[InlineData("inputValidationService")]
		[InlineData("utilityService")]
		[InlineData("exportService")]
		[InlineData("mapper")]
		[InlineData("logger")]
		public void Constructor_WhenServiceIsNull_ThrowsArgumentNullException(string serviceName)
		{
			// Arrange
			var pilotServiceMock = new Mock<IPilotService>();
			var paginationValidationServiceMock = new Mock<IPaginationValidationService>();
			var inputValidationServiceMock = new Mock<IInputValidationService>();
			var utilityServiceMock = new Mock<IUtilityService>();
			var exportServiceMock = new Mock<IExportService>();
			var mapperMock = new Mock<IMapper>();
			var loggerMock = new Mock<ILogger<PilotsController>>();
			var configurationMock = new Mock<IConfiguration>();

			// Set up mocks to return null based on the test case
			IPilotService pilotService = serviceName == "pilotService" ? null : pilotServiceMock.Object;
			IPaginationValidationService paginationValidationService = serviceName == "paginationValidationService" ? null : paginationValidationServiceMock.Object;
			IInputValidationService inputValidationService = serviceName == "inputValidationService" ? null : inputValidationServiceMock.Object;
			IUtilityService utilityService = serviceName == "utilityService" ? null : utilityServiceMock.Object;
			IExportService exportService = serviceName == "exportService" ? null : exportServiceMock.Object;
			IMapper mapper = serviceName == "mapper" ? null : mapperMock.Object;
			ILogger<PilotsController> logger = serviceName == "logger" ? null : loggerMock.Object;

			// Act & Assert
			var exception = Record.Exception(() => new PilotsController(
				pilotService,
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

		[Fact]
		[Trait("Category", "GetPilots")]
		public async Task GetPilots_InvalidPaginationParameters_ReturnsBadRequest()
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
			var result = await _controller.GetPilots(cancellationToken, invalidPage, invalidPageSize);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetPilots")]
		public async Task GetPilots_ReturnsNoContent_WhenNoPilotsFound()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_pilotServiceMock.Setup(service => service.GetPilots(cancellationToken, It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(new List<PilotEntity>());

			// Act
			var result = await _controller.GetPilots(cancellationToken, page, pageSize);

			// Assert
			Assert.IsType<NoContentResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetPilots")]
		public async Task GetPilots_ReturnsInternalServerError_WhenExceptionThrown()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_pilotServiceMock.Setup(service => service.GetPilots(cancellationToken, It.IsAny<int>(), It.IsAny<int>()))
				.ThrowsAsync(new Exception("Simulated exception"));

			// Act & Assert
			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetPilots(cancellationToken, page, pageSize));
		}

		[Fact]
		[Trait("Category", "GetPilots")]
		public async Task GetPilots_ReturnsOk_WithPaginatedPilots()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;
			var pilots = new List<PilotEntity>
			{
				new PilotEntity { /* Initialize properties */ },
				new PilotEntity { /* Initialize properties */ }
			};
			var totalItems = 2;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_pilotServiceMock
				.Setup(service => service.GetPilots(cancellationToken, page, pageSize))
				.ReturnsAsync(pilots);
			_pilotServiceMock
				.Setup(service => service.PilotsCount(cancellationToken, null, null))
				.ReturnsAsync(totalItems);

			var expectedData = new List<PilotDto>
			{
				new PilotDto { /* Initialize properties */ },
				new PilotDto { /* Initialize properties */ }
			};
			_mapperMock
				.Setup(m => m.Map<IEnumerable<PilotDto>>(It.IsAny<IEnumerable<PilotEntity>>()))
				.Returns(expectedData);

			// Act
			var result = await _controller.GetPilots(cancellationToken, page, pageSize);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PagedResponse<PilotDto>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var pagedResponse = Assert.IsType<PagedResponse<PilotDto>>(okResult.Value);
			Assert.Equal(page, pagedResponse.PageNumber);
			Assert.Equal(pageSize, pagedResponse.PageSize);
			Assert.Equal(totalItems, pagedResponse.TotalCount);
			Assert.Equal(expectedData, pagedResponse.Data);
		}

		[Fact]
		[Trait("Category", "GetPilots")]
		public async Task GetPilots_ReturnsCorrectPageData()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 2;
			int pageSize = 5;
			var allPilots = new List<PilotEntity>
			{
				new PilotEntity { /* Initialize properties */ },
				new PilotEntity { /* Initialize properties */ },
				new PilotEntity { /* Initialize properties */ },
				new PilotEntity { /* Initialize properties */ },
				new PilotEntity { /* Initialize properties */ },
				new PilotEntity { /* Initialize properties */ },
				new PilotEntity { /* Initialize properties */ },
				new PilotEntity { /* Initialize properties */ },
				new PilotEntity { /* Initialize properties */ },
				new PilotEntity { /* Initialize properties */ }
			};
			var pagedPilots = allPilots.Skip((page - 1) * pageSize).Take(pageSize).ToList();

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_pilotServiceMock
				.Setup(service => service.GetPilots(cancellationToken, page, pageSize))
				.ReturnsAsync(pagedPilots);
			_pilotServiceMock
				.Setup(service => service.PilotsCount(cancellationToken, null, null))
				.ReturnsAsync(allPilots.Count);

			var expectedData = new List<PilotDto>
			{
				new PilotDto { /* Initialize properties */ },
				new PilotDto { /* Initialize properties */ },
				new PilotDto { /* Initialize properties */ },
				new PilotDto { /* Initialize properties */ },
				new PilotDto { /* Initialize properties */ }
			};
			_mapperMock
				.Setup(m => m.Map<IEnumerable<PilotDto>>(It.IsAny<IEnumerable<PilotEntity>>()))
				.Returns(expectedData);

			// Act
			var result = await _controller.GetPilots(cancellationToken, page, pageSize);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PagedResponse<PilotDto>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var pagedResponse = Assert.IsType<PagedResponse<PilotDto>>(okResult.Value);
			Assert.Equal(page, pagedResponse.PageNumber);
			Assert.Equal(pageSize, pagedResponse.PageSize);
			Assert.Equal(allPilots.Count, pagedResponse.TotalCount);
			Assert.Equal(expectedData, pagedResponse.Data);
		}

		[Fact]
		[Trait("Category", "GetPilot")]
		public async Task GetPilot_InvalidId_ReturnsBadRequest()
		{
			// Arrange
			int invalidId = -1;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid input. The ID must be a non-negative integer.");

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(invalidId))
				.Returns(false);

			// Act
			var result = await _controller.GetPilot(invalidId);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
			var badRequestResult = result.Result as BadRequestObjectResult;
			Assert.Equal(expectedBadRequestResult.Value, badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "GetPilot")]
		public async Task GetPilot_PilotNotFound_ReturnsNotFound()
		{
			// Arrange
			int validId = 1;

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(validId))
				.Returns(true);
			_pilotServiceMock
				.Setup(service => service.PilotExists(validId))
				.ReturnsAsync(false);

			// Act
			var result = await _controller.GetPilot(validId);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetPilot")]
		public async Task GetPilot_ReturnsPilotDto_WhenPilotExists()
		{
			// Arrange
			int validId = 1;

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(validId))
				.Returns(true);
			_pilotServiceMock
				.Setup(service => service.PilotExists(validId))
				.ReturnsAsync(true);
			_pilotServiceMock
				.Setup(service => service.GetPilot(validId))
				.ReturnsAsync(pilotEntity);
			_mapperMock
				.Setup(m => m.Map<PilotDto>(pilotEntity))
				.Returns(pilotDto);

			// Act
			var result = await _controller.GetPilot(validId);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PilotDto>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var returnedPilotDto = Assert.IsType<PilotDto>(okResult.Value);
			Assert.Equal(pilotDto, returnedPilotDto);
		}

		[Fact]
		[Trait("Category", "GetPilotsByName")]
		public async Task GetPilotsByName_InvalidName_ReturnsBadRequest()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			string invalidName = string.Empty;
			var expectedBadRequestResult = new BadRequestObjectResult("Both first name and last name are missing in the request.");

			_inputValidationServiceMock
				.Setup(x => x.IsValidString(invalidName))
				.Returns(false);

			// Act
			var result = await _controller.GetPilotsByName(cancellationToken, invalidName);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
			var badRequestResult = result.Result as BadRequestObjectResult;
			Assert.Equal(expectedBadRequestResult.Value, badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "GetPilotsByName")]
		public async Task GetPilotsByName_InvalidPaginationParameters_ReturnsBadRequest()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			string validName1 = "ValidName1";
			string validName2 = "ValidName2";
			int invalidPage = -1;
			int invalidPageSize = 0;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid pagination parameters.");

			_inputValidationServiceMock
				.Setup(x => x.IsValidString(validName1))
				.Returns(true);
			_inputValidationServiceMock
				.Setup(x => x.IsValidString(validName2))
				.Returns(true);
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(invalidPage, invalidPageSize, It.IsAny<int>()))
				.Returns((false, 0, expectedBadRequestResult));

			// Act
			var result = await _controller.GetPilotsByName(cancellationToken, validName1, validName2, invalidPage, invalidPageSize);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetPilotsByName")]
		public async Task GetPilotsByName_PilotsNotFound_ReturnsNotFound()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			string validName = "NonExistentName";
			int validPage = 1;
			int validPageSize = 10;

			_inputValidationServiceMock
				.Setup(x => x.IsValidString(validName))
				.Returns(true);
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(validPage, validPageSize, It.IsAny<int>()))
				.Returns((true, validPageSize, null));
			_pilotServiceMock
				.Setup(service => service.GetPilotsByName(cancellationToken, validPage, validPageSize, validName, null))
				.ReturnsAsync(new List<PilotEntity>());

			// Act
			var result = await _controller.GetPilotsByName(cancellationToken, validName, null, validPage, validPageSize);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetPilotsByName")]
		public async Task GetPilotsByName_ReturnsPagedListOfPilots_WhenPilotsFound()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			string validName1 = "ValidName1";
			string validName2 = "ValidName2";
			int validPage = 1;
			int validPageSize = 10;
			var pilotEntities = new List<PilotEntity> { pilotEntity };
			var pilotDtos = new List<PilotDto> { pilotDto };
			var totalItems = 1;

			_inputValidationServiceMock
				.Setup(x => x.IsValidString(validName1))
				.Returns(true);
			_inputValidationServiceMock
				.Setup(x => x.IsValidString(validName2))
				.Returns(true);
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(validPage, validPageSize, It.IsAny<int>()))
				.Returns((true, validPageSize, null));
			_pilotServiceMock
				.Setup(service => service.GetPilotsByName(cancellationToken, validPage, validPageSize, validName1, validName2))
				.ReturnsAsync(pilotEntities);
			_pilotServiceMock
				.Setup(service => service.PilotsCount(cancellationToken, validName1, validName2))
				.ReturnsAsync(totalItems);
			_mapperMock
				.Setup(m => m.Map<IEnumerable<PilotDto>>(pilotEntities))
				.Returns(pilotDtos);

			// Act
			var result = await _controller.GetPilotsByName(cancellationToken, validName1, validName2, validPage, validPageSize);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PagedResponse<PilotDto>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var response = Assert.IsType<PagedResponse<PilotDto>>(okResult.Value);
			Assert.Equal(validPage, response.PageNumber);
			Assert.Equal(validPageSize, response.PageSize);
			Assert.Equal(totalItems, response.TotalCount);
			Assert.Equal(pilotDtos, response.Data);
		}

		[Fact]
		[Trait("Category", "PostPilot")]
		public async Task PostPilot_ReturnsCreatedAtActionResult_WhenPilotIsCreatedSuccessfully()
		{
			// Arrange
			var pilotCreateDto = new PilotCreateDto();
			var pilotEntity = new PilotEntity { Id = 1 };
			var pilotDto = new PilotDto { Id = 1 };

			// Set up the mapper to return the expected values
			_mapperMock.Setup(m => m.Map<PilotEntity>(pilotCreateDto)).Returns(pilotEntity);
			_mapperMock.Setup(m => m.Map<PilotDto>(pilotEntity)).Returns(pilotDto);

			// Adjust service setup to return the pilotEntity wrapped in a Task
			_pilotServiceMock.Setup(service => service.PostPilot(pilotEntity))
							   .ReturnsAsync(pilotEntity);

			// Act
			var result = await _controller.PostPilot(pilotCreateDto);

			// Assert
			var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
			var returnedValue = Assert.IsType<PilotDto>(actionResult.Value);
			Assert.Equal(pilotDto.Id, returnedValue.Id);
			Assert.Equal("GetPilot", actionResult.ActionName);
			Assert.Equal(1, actionResult.RouteValues["id"]);
		}

		[Fact]
		[Trait("Category", "PostPilot")]
		public async Task PostPilot_ThrowsException_WhenServiceFails()
		{
			// Arrange
			var pilotCreateDto = new PilotCreateDto();
			var pilotEntity = new PilotEntity();
			_mapperMock.Setup(m => m.Map<PilotEntity>(pilotCreateDto)).Returns(pilotEntity);

			// Set up the service to throw an exception
			_pilotServiceMock.Setup(service => service.PostPilot(pilotEntity))
							   .ThrowsAsync(new Exception("Simulated exception"));

			// Act & Assert
			await Assert.ThrowsAsync<Exception>(async () => await _controller.PostPilot(pilotCreateDto));
		}

		[Fact]
		[Trait("Category", "PutPilot")]
		public async Task PutPilot_ReturnsNoContent_WhenUpdateIsSuccessful()
		{
			// Arrange
			int id = 1;
			var pilotDto = new PilotDto { Id = id };
			var pilotEntity = new PilotEntity { Id = id };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_mapperMock.Setup(m => m.Map<PilotEntity>(pilotDto)).Returns(pilotEntity);
			_pilotServiceMock.Setup(service => service.PilotExists(id)).ReturnsAsync(true);
			_pilotServiceMock.Setup(service => service.PutPilot(pilotEntity)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.PutPilot(id, pilotDto);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "PutPilot")]
		public async Task PutPilot_ReturnsBadRequest_WhenIdIsInvalid()
		{
			// Arrange
			int invalidId = -1;
			var pilotDto = new PilotDto { Id = invalidId };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(invalidId)).Returns(false);

			// Act
			var result = await _controller.PutPilot(invalidId, pilotDto);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Invalid input. The ID must be a non-negative integer.", badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "PutPilot")]
		public async Task PutPilot_ReturnsBadRequest_WhenIdInDtoDoesNotMatchIdInUrl()
		{
			// Arrange
			int id = 1;
			var pilotDto = new PilotDto { Id = 2 };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);

			// Act
			var result = await _controller.PutPilot(id, pilotDto);

			// Assert
			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		[Trait("Category", "PutPilot")]
		public async Task PutPilot_ReturnsNotFound_WhenPilotDoesNotExist()
		{
			// Arrange
			int id = 1;
			var pilotDto = new PilotDto { Id = id };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_pilotServiceMock.Setup(service => service.PilotExists(id)).ReturnsAsync(false);

			// Act
			var result = await _controller.PutPilot(id, pilotDto);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "PatchPilot")]
		public async Task PatchPilot_ReturnsOk_WhenUpdateIsSuccessful()
		{
			// Arrange
			int id = 1;
			var pilotDocument = new JsonPatchDocument();
			var updatedPilot = new PilotEntity { Id = id };
			var pilotDto = new PilotDto { Id = id };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_pilotServiceMock.Setup(service => service.PilotExists(id)).ReturnsAsync(true);
			_pilotServiceMock.Setup(service => service.PatchPilot(id, pilotDocument)).ReturnsAsync(updatedPilot);
			_mapperMock.Setup(m => m.Map<PilotDto>(updatedPilot)).Returns(pilotDto);

			// Act
			var result = await _controller.PatchPilot(id, pilotDocument);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(pilotDto, okResult.Value);
		}

		[Fact]
		[Trait("Category", "PatchPilot")]
		public async Task PatchPilot_ReturnsBadRequest_WhenIdIsInvalid()
		{
			// Arrange
			int invalidId = -1;
			var pilotDocument = new JsonPatchDocument();

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(invalidId)).Returns(false);

			// Act
			var result = await _controller.PatchPilot(invalidId, pilotDocument);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Invalid input. The ID must be a non-negative integer.", badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "PatchPilot")]
		public async Task PatchPilot_ReturnsNotFound_WhenPilotDoesNotExist()
		{
			// Arrange
			int id = 1;
			var pilotDocument = new JsonPatchDocument();

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_pilotServiceMock.Setup(service => service.PilotExists(id)).ReturnsAsync(false);

			// Act
			var result = await _controller.PatchPilot(id, pilotDocument);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "DeletePilot")]
		public async Task DeletePilot_ReturnsNoContent_WhenDeletionIsSuccessful()
		{
			// Arrange
			int id = 1;
			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_pilotServiceMock.Setup(service => service.PilotExists(id)).ReturnsAsync(true);
			_pilotServiceMock.Setup(service => service.DeletePilot(id)).ReturnsAsync(true);

			// Act
			var result = await _controller.DeletePilot(id);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "DeletePilot")]
		public async Task DeletePilot_ReturnsBadRequest_WhenIdIsInvalid()
		{
			// Arrange
			int invalidId = -1;
			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(invalidId)).Returns(false);

			// Act
			var result = await _controller.DeletePilot(invalidId);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Invalid input. The ID must be a non-negative integer.", badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "DeletePilot")]
		public async Task DeletePilot_ReturnsNotFound_WhenPilotDoesNotExist()
		{
			// Arrange
			int id = 1;
			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_pilotServiceMock.Setup(service => service.PilotExists(id)).ReturnsAsync(false);

			// Act
			var result = await _controller.DeletePilot(id);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "DeletePilot")]
		public async Task DeletePilot_ReturnsConflict_WhenPilotCannotBeDeleted()
		{
			// Arrange
			int id = 1;
			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_pilotServiceMock.Setup(service => service.PilotExists(id)).ReturnsAsync(true);
			_pilotServiceMock.Setup(service => service.DeletePilot(id)).ReturnsAsync(false);

			// Act
			var result = await _controller.DeletePilot(id);

			// Assert
			var conflictResult = Assert.IsType<ConflictObjectResult>(result);
			Assert.Equal("Pilot cannot be deleted because it is being referenced by other entities.", conflictResult.Value);
		}


	}
}