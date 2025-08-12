using AirportAutomation.Api.Interfaces;
using AirportAutomation.Application.Dtos.Destination;
using AirportAutomation.Application.Dtos.Response;
using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IServices;
using AirportАutomation.Api.Controllers;
using AirportАutomation.Api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace AirportAutomationApi.Test.Controllers
{
	public class DestinationsControllerTests
	{
		private readonly DestinationsController _controller;
		private readonly Mock<IDestinationService> _destinationServiceMock;
		private readonly Mock<IPaginationValidationService> _paginationValidationServiceMock;
		private readonly Mock<IInputValidationService> _inputValidationServiceMock;
		private readonly Mock<IUtilityService> _utilityServiceMock;
		private readonly Mock<IExportService> _exportServiceMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<ILogger<DestinationsController>> _loggerMock;
		private readonly Mock<IConfiguration> _configurationMock;

		private readonly DestinationEntity destinationEntity = new()
		{
			Id = 1,
			City = "Belgrade",
			Airport = "Belgrade Nikola Tesla Airport"
		};

		private readonly DestinationDto destinationDto = new()
		{
			Id = 1,
			City = "Belgrade",
			Airport = "Belgrade Nikola Tesla Airport"
		};
		public DestinationsControllerTests()
		{
			_destinationServiceMock = new Mock<IDestinationService>();
			_paginationValidationServiceMock = new Mock<IPaginationValidationService>();
			_inputValidationServiceMock = new Mock<IInputValidationService>();
			_utilityServiceMock = new Mock<IUtilityService>();
			_exportServiceMock = new Mock<IExportService>();
			_mapperMock = new Mock<IMapper>();
			_loggerMock = new Mock<ILogger<DestinationsController>>();
			_configurationMock = new Mock<IConfiguration>();
			var configBuilder = new ConfigurationBuilder();
			configBuilder.AddInMemoryCollection(new Dictionary<string, string>
			{
				{"pageSettings:maxPageSize", "10"}
			});
			_configurationMock.Setup(x => x.GetSection(It.IsAny<string>()))
				.Returns(configBuilder.Build().GetSection(""));

			_controller = new DestinationsController(
				_destinationServiceMock.Object,
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
		[Trait("Category", "GetDestinations")]
		public async Task GetDestinations_InvalidPaginationParameters_ReturnsBadRequest()
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
			var result = await _controller.GetDestinations(cancellationToken, invalidPage, invalidPageSize);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetDestinations")]
		public async Task GetDestinations_ReturnsNoContent_WhenNoDestinationsFound()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_destinationServiceMock.Setup(service => service.GetDestinations(cancellationToken, It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(new List<DestinationEntity>());

			// Act
			var result = await _controller.GetDestinations(cancellationToken, page, pageSize);

			// Assert
			Assert.IsType<NoContentResult>(result.Result);
		}


		[Fact]
		[Trait("Category", "GetDestinations")]
		public async Task GetDestinations_ReturnsInternalServerError_WhenExceptionThrown()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_destinationServiceMock.Setup(service => service.GetDestinations(cancellationToken, It.IsAny<int>(), It.IsAny<int>()))
				.ThrowsAsync(new Exception("Simulated exception"));

			// Act & Assert
			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetDestinations(cancellationToken, page, pageSize));
		}

		[Fact]
		[Trait("Category", "GetDestinations")]
		public async Task GetDestinations_ReturnsOk_WithPaginatedDestinations()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;
			var destinations = new List<DestinationEntity>
			{
				new DestinationEntity { /* Initialize properties */ },
				new DestinationEntity { /* Initialize properties */ }
			};
			var totalItems = 2;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_destinationServiceMock
				.Setup(service => service.GetDestinations(cancellationToken, page, pageSize))
				.ReturnsAsync(destinations);
			_destinationServiceMock
				.Setup(service => service.DestinationsCount(cancellationToken, null, null))
				.ReturnsAsync(totalItems);

			var expectedData = new List<DestinationDto>
			{
				new DestinationDto { /* Initialize properties */ },
				new DestinationDto { /* Initialize properties */ }
			};
			_mapperMock
				.Setup(m => m.Map<IEnumerable<DestinationDto>>(It.IsAny<IEnumerable<DestinationEntity>>()))
				.Returns(expectedData);

			// Act
			var result = await _controller.GetDestinations(cancellationToken, page, pageSize);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PagedResponse<DestinationDto>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var pagedResponse = Assert.IsType<PagedResponse<DestinationDto>>(okResult.Value);
			Assert.Equal(page, pagedResponse.PageNumber);
			Assert.Equal(pageSize, pagedResponse.PageSize);
			Assert.Equal(totalItems, pagedResponse.TotalCount);
			Assert.Equal(expectedData, pagedResponse.Data);
		}

		[Fact]
		[Trait("Category", "GetDestinations")]
		public async Task GetDestinations_ReturnsCorrectPageData()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 2;
			int pageSize = 5;
			var allDestinations = new List<DestinationEntity>
			{
				new DestinationEntity { /* Initialize properties */ },
				new DestinationEntity { /* Initialize properties */ },
				new DestinationEntity { /* Initialize properties */ },
				new DestinationEntity { /* Initialize properties */ },
				new DestinationEntity { /* Initialize properties */ },
				new DestinationEntity { /* Initialize properties */ },
				new DestinationEntity { /* Initialize properties */ },
				new DestinationEntity { /* Initialize properties */ },
				new DestinationEntity { /* Initialize properties */ },
				new DestinationEntity { /* Initialize properties */ }
			};
			var pagedDestinations = allDestinations.Skip((page - 1) * pageSize).Take(pageSize).ToList();

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_destinationServiceMock
				.Setup(service => service.GetDestinations(cancellationToken, page, pageSize))
				.ReturnsAsync(pagedDestinations);
			_destinationServiceMock
				.Setup(service => service.DestinationsCount(cancellationToken, null, null))
				.ReturnsAsync(allDestinations.Count);

			var expectedData = new List<DestinationDto>
			{
				new DestinationDto { /* Initialize properties */ },
				new DestinationDto { /* Initialize properties */ },
				new DestinationDto { /* Initialize properties */ },
				new DestinationDto { /* Initialize properties */ },
				new DestinationDto { /* Initialize properties */ }
			};
			_mapperMock
				.Setup(m => m.Map<IEnumerable<DestinationDto>>(It.IsAny<IEnumerable<DestinationEntity>>()))
				.Returns(expectedData);

			// Act
			var result = await _controller.GetDestinations(cancellationToken, page, pageSize);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PagedResponse<DestinationDto>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var pagedResponse = Assert.IsType<PagedResponse<DestinationDto>>(okResult.Value);
			Assert.Equal(page, pagedResponse.PageNumber);
			Assert.Equal(pageSize, pagedResponse.PageSize);
			Assert.Equal(allDestinations.Count, pagedResponse.TotalCount);
			Assert.Equal(expectedData, pagedResponse.Data);
		}

		[Fact]
		[Trait("Category", "GetDestination")]
		public async Task GetDestination_InvalidId_ReturnsBadRequest()
		{
			// Arrange
			int invalidId = -1;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid input. The ID must be a non-negative integer.");

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(invalidId))
				.Returns(false);

			// Act
			var result = await _controller.GetDestination(invalidId);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
			var badRequestResult = result.Result as BadRequestObjectResult;
			Assert.Equal(expectedBadRequestResult.Value, badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "GetDestination")]
		public async Task GetDestination_DestinationNotFound_ReturnsNotFound()
		{
			// Arrange
			int validId = 1;

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(validId))
				.Returns(true);
			_destinationServiceMock
				.Setup(service => service.DestinationExists(validId))
				.ReturnsAsync(false);

			// Act
			var result = await _controller.GetDestination(validId);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetDestination")]
		public async Task GetDestination_ReturnsDestinationDto_WhenDestinationExists()
		{
			// Arrange
			int validId = 1;

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(validId))
				.Returns(true);
			_destinationServiceMock
				.Setup(service => service.DestinationExists(validId))
				.ReturnsAsync(true);
			_destinationServiceMock
				.Setup(service => service.GetDestination(validId))
				.ReturnsAsync(destinationEntity);
			_mapperMock
				.Setup(m => m.Map<DestinationDto>(destinationEntity))
				.Returns(destinationDto);

			// Act
			var result = await _controller.GetDestination(validId);

			// Assert
			var actionResult = Assert.IsType<ActionResult<DestinationDto>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var returnedDestinationDto = Assert.IsType<DestinationDto>(okResult.Value);
			Assert.Equal(destinationDto, returnedDestinationDto);
		}

		[Fact]
		[Trait("Category", "GetDestinationsByCityOrAirport")]
		public async Task GetDestinationsByCityOrAirport_InvalidCityOrAirport_ReturnsBadRequest()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			string invalidName = string.Empty;
			var expectedBadRequestResult = new BadRequestObjectResult("Both city and airport are missing in the request.");

			_inputValidationServiceMock
				.Setup(x => x.IsValidString(invalidName))
				.Returns(false);

			// Act
			var result = await _controller.GetDestinationsByCityOrAirport(cancellationToken, invalidName, null);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
			var badRequestResult = result.Result as BadRequestObjectResult;
			Assert.Equal(expectedBadRequestResult.Value, badRequestResult.Value);
		}


		[Fact]
		[Trait("Category", "GetDestinationsByCityOrAirport")]
		public async Task GetDestinationsByCityOrAirport_InvalidPaginationParameters_ReturnsBadRequest()
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
			var result = await _controller.GetDestinationsByCityOrAirport(cancellationToken, validName, null, invalidPage, invalidPageSize);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
		}


		[Fact]
		[Trait("Category", "GetDestinationsByCityOrAirport")]
		public async Task GetDestinationsByCityOrAirport_DestinationsNotFound_ReturnsNotFound()
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
			_destinationServiceMock
				.Setup(service => service.GetDestinationsByCityOrAirport(cancellationToken, validPage, validPageSize, validName, null))
				.ReturnsAsync(new List<DestinationEntity>());

			// Act
			var result = await _controller.GetDestinationsByCityOrAirport(cancellationToken, validName, null, validPage, validPageSize);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}


		[Fact]
		[Trait("Category", "GetDestinationsByCityOrAirport")]
		public async Task GetDestinationsByCityOrAirport_ReturnsPagedListOfDestinations_WhenDestinationsFound()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			string validName1 = "ValidName1";
			string validName2 = "ValidName2";
			int validPage = 1;
			int validPageSize = 10;
			var destinationEntities = new List<DestinationEntity> { destinationEntity };
			var destinationDtos = new List<DestinationDto> { destinationDto };
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
			_destinationServiceMock
				.Setup(service => service.GetDestinationsByCityOrAirport(cancellationToken, validPage, validPageSize, validName1, validName2))
				.ReturnsAsync(destinationEntities);
			_destinationServiceMock
				.Setup(service => service.DestinationsCount(cancellationToken, validName1, validName2))
				.ReturnsAsync(totalItems);
			_mapperMock
				.Setup(m => m.Map<IEnumerable<DestinationDto>>(destinationEntities))
				.Returns(destinationDtos);

			// Act
			var result = await _controller.GetDestinationsByCityOrAirport(cancellationToken, validName1, validName2, validPage, validPageSize);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PagedResponse<DestinationDto>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var response = Assert.IsType<PagedResponse<DestinationDto>>(okResult.Value);
			Assert.Equal(validPage, response.PageNumber);
			Assert.Equal(validPageSize, response.PageSize);
			Assert.Equal(totalItems, response.TotalCount);
			Assert.Equal(destinationDtos, response.Data);
		}

		[Fact]
		[Trait("Category", "PostDestination")]
		public async Task PostDestination_ReturnsCreatedAtActionResult_WhenDestinationIsCreatedSuccessfully()
		{
			// Arrange
			var destinationCreateDto = new DestinationCreateDto();
			var destinationEntity = new DestinationEntity { Id = 1 };
			var destinationDto = new DestinationDto { Id = 1 };

			// Set up the mapper to return the expected values
			_mapperMock.Setup(m => m.Map<DestinationEntity>(destinationCreateDto)).Returns(destinationEntity);
			_mapperMock.Setup(m => m.Map<DestinationDto>(destinationEntity)).Returns(destinationDto);

			// Adjust service setup to return the destinationEntity wrapped in a Task
			_destinationServiceMock.Setup(service => service.PostDestination(destinationEntity))
							   .ReturnsAsync(destinationEntity);

			// Act
			var result = await _controller.PostDestination(destinationCreateDto);

			// Assert
			var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
			var returnedValue = Assert.IsType<DestinationDto>(actionResult.Value);
			Assert.Equal(destinationDto.Id, returnedValue.Id);
			Assert.Equal("GetDestination", actionResult.ActionName);
			Assert.Equal(1, actionResult.RouteValues["id"]);
		}

		[Fact]
		[Trait("Category", "PostDestination")]
		public async Task PostDestination_ThrowsException_WhenServiceFails()
		{
			// Arrange
			var destinationCreateDto = new DestinationCreateDto();
			var destinationEntity = new DestinationEntity();
			_mapperMock.Setup(m => m.Map<DestinationEntity>(destinationCreateDto)).Returns(destinationEntity);

			// Set up the service to throw an exception
			_destinationServiceMock.Setup(service => service.PostDestination(destinationEntity))
							   .ThrowsAsync(new Exception("Simulated exception"));

			// Act & Assert
			await Assert.ThrowsAsync<Exception>(async () => await _controller.PostDestination(destinationCreateDto));
		}

		[Fact]
		[Trait("Category", "PutDestination")]
		public async Task PutDestination_ReturnsNoContent_WhenUpdateIsSuccessful()
		{
			// Arrange
			int id = 1;
			var destinationDto = new DestinationDto { Id = id };
			var destinationEntity = new DestinationEntity { Id = id };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_mapperMock.Setup(m => m.Map<DestinationEntity>(destinationDto)).Returns(destinationEntity);
			_destinationServiceMock.Setup(service => service.DestinationExists(id)).ReturnsAsync(true);
			_destinationServiceMock.Setup(service => service.PutDestination(destinationEntity)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.PutDestination(id, destinationDto);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "PutDestination")]
		public async Task PutDestination_ReturnsBadRequest_WhenIdIsInvalid()
		{
			// Arrange
			int invalidId = -1;
			var destinationDto = new DestinationDto { Id = invalidId };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(invalidId)).Returns(false);

			// Act
			var result = await _controller.PutDestination(invalidId, destinationDto);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Invalid input. The ID must be a non-negative integer.", badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "PutDestination")]
		public async Task PutDestination_ReturnsBadRequest_WhenIdInDtoDoesNotMatchIdInUrl()
		{
			// Arrange
			int id = 1;
			var destinationDto = new DestinationDto { Id = 2 };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);

			// Act
			var result = await _controller.PutDestination(id, destinationDto);

			// Assert
			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		[Trait("Category", "PutDestination")]
		public async Task PutDestination_ReturnsNotFound_WhenDestinationDoesNotExist()
		{
			// Arrange
			int id = 1;
			var destinationDto = new DestinationDto { Id = id };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_destinationServiceMock.Setup(service => service.DestinationExists(id)).ReturnsAsync(false);

			// Act
			var result = await _controller.PutDestination(id, destinationDto);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "PatchDestination")]
		public async Task PatchDestination_ReturnsOk_WhenUpdateIsSuccessful()
		{
			// Arrange
			int id = 1;
			var destinationDocuemnt = new JsonPatchDocument();
			var updatedDestination = new DestinationEntity { Id = id };
			var destinationDto = new DestinationDto { Id = id };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_destinationServiceMock.Setup(service => service.DestinationExists(id)).ReturnsAsync(true);
			_destinationServiceMock.Setup(service => service.PatchDestination(id, destinationDocuemnt)).ReturnsAsync(updatedDestination);
			_mapperMock.Setup(m => m.Map<DestinationDto>(updatedDestination)).Returns(destinationDto);

			// Act
			var result = await _controller.PatchDestination(id, destinationDocuemnt);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(destinationDto, okResult.Value);
		}

		[Fact]
		[Trait("Category", "PatchDestination")]
		public async Task PatchDestination_ReturnsBadRequest_WhenIdIsInvalid()
		{
			// Arrange
			int invalidId = -1;
			var destinationDocument = new JsonPatchDocument();

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(invalidId)).Returns(false);

			// Act
			var result = await _controller.PatchDestination(invalidId, destinationDocument);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Invalid input. The ID must be a non-negative integer.", badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "PatchDestination")]
		public async Task PatchDestination_ReturnsNotFound_WhenDestinationDoesNotExist()
		{
			// Arrange
			int id = 1;
			var destinationDocument = new JsonPatchDocument();

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_destinationServiceMock.Setup(service => service.DestinationExists(id)).ReturnsAsync(false);

			// Act
			var result = await _controller.PatchDestination(id, destinationDocument);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "DeleteDestination")]
		public async Task DeleteDestination_ReturnsNoContent_WhenDeletionIsSuccessful()
		{
			// Arrange
			int id = 1;
			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_destinationServiceMock.Setup(service => service.DestinationExists(id)).ReturnsAsync(true);
			_destinationServiceMock.Setup(service => service.DeleteDestination(id)).ReturnsAsync(true);

			// Act
			var result = await _controller.DeleteDestination(id);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "DeleteDestination")]
		public async Task DeleteDestination_ReturnsBadRequest_WhenIdIsInvalid()
		{
			// Arrange
			int invalidId = -1;
			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(invalidId)).Returns(false);

			// Act
			var result = await _controller.DeleteDestination(invalidId);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Invalid input. The ID must be a non-negative integer.", badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "DeleteDestination")]
		public async Task DeleteDestination_ReturnsNotFound_WhenDestinationDoesNotExist()
		{
			// Arrange
			int id = 1;
			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_destinationServiceMock.Setup(service => service.DeleteDestination(id)).ReturnsAsync(false);

			// Act
			var result = await _controller.DeleteDestination(id);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Category", "DeleteDestination")]
		public async Task DeleteDestination_ReturnsConflict_WhenDestinationCannotBeDeleted()
		{
			// Arrange
			int id = 1;
			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_destinationServiceMock.Setup(service => service.DestinationExists(id)).ReturnsAsync(true);
			_destinationServiceMock.Setup(service => service.DeleteDestination(id)).ReturnsAsync(false);

			// Act
			var result = await _controller.DeleteDestination(id);

			// Assert
			var conflictResult = Assert.IsType<ConflictObjectResult>(result);
			Assert.Equal("Destination cannot be deleted because it is being referenced by other entities.", conflictResult.Value);
		}

	}

}