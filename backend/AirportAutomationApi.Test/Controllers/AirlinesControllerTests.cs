using AirportAutomation.Api.Interfaces;
using AirportAutomation.Application.Dtos.Airline;
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
				new AirlineEntity { /* Initialize properties */ },
				new AirlineEntity { /* Initialize properties */ }
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
				new AirlineDto { /* Initialize properties */ },
				new AirlineDto { /* Initialize properties */ }
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
				new AirlineEntity { /* Initialize properties */ },
				new AirlineEntity { /* Initialize properties */ },
				new AirlineEntity { /* Initialize properties */ },
				new AirlineEntity { /* Initialize properties */ },
				new AirlineEntity { /* Initialize properties */ },
				new AirlineEntity { /* Initialize properties */ },
				new AirlineEntity { /* Initialize properties */ },
				new AirlineEntity { /* Initialize properties */ },
				new AirlineEntity { /* Initialize properties */ },
				new AirlineEntity { /* Initialize properties */ }
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
				new AirlineDto { /* Initialize properties */ },
				new AirlineDto { /* Initialize properties */ },
				new AirlineDto { /* Initialize properties */ },
				new AirlineDto { /* Initialize properties */ },
				new AirlineDto { /* Initialize properties */ }
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

		[Fact]
		[Trait("Category", "GetAirlinesByName")]
		public async Task GetAirlinesByName_AirlinesNotFound_ReturnsNotFound()
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
			_airlineServiceMock
				.Setup(service => service.GetAirlinesByName(cancellationToken, validPage, validPageSize, validName))
				.ReturnsAsync(new List<AirlineEntity>());

			// Act
			var result = await _controller.GetAirlinesByName(cancellationToken, validName, validPage, validPageSize);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetAirlinesByName")]
		public async Task GetAirlinesByName_ReturnsPagedListOfAirlines_WhenAirlinesFound()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			string validName = "ValidName";
			int validPage = 1;
			int validPageSize = 10;
			var airlineEntities = new List<AirlineEntity> { airlineEntity };
			var airlineDtos = new List<AirlineDto> { airlineDto };
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

	}
}