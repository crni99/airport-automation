using AirportAutomation.Api.Interfaces;
using AirportAutomation.Application.Dtos.Passenger;
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
		}

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

		[Fact]
		[Trait("Category", "GetPassengersByName")]
		public async Task GetPassengersByName_InvalidName_ReturnsBadRequest()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			string invalidName = string.Empty;
			var expectedBadRequestResult = new BadRequestObjectResult("Both first name and last name are missing in the request.");

			_inputValidationServiceMock
				.Setup(x => x.IsValidString(invalidName))
				.Returns(false);

			// Act
			var result = await _controller.GetPassengersByName(cancellationToken, invalidName);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
			var badRequestResult = result.Result as BadRequestObjectResult;
			Assert.Equal(expectedBadRequestResult.Value, badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "GetPassengersByName")]
		public async Task GetPassengersByName_InvalidPaginationParameters_ReturnsBadRequest()
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
			var result = await _controller.GetPassengersByName(cancellationToken, validName1, validName2, invalidPage, invalidPageSize);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetPassengersByName")]
		public async Task GetPassengersByName_PassengersNotFound_ReturnsNotFound()
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
			_passengerServiceMock
				.Setup(service => service.GetPassengersByName(cancellationToken, validPage, validPageSize, validName, null))
				.ReturnsAsync(new List<PassengerEntity>());

			// Act
			var result = await _controller.GetPassengersByName(cancellationToken, validName, null, validPage, validPageSize);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetPassengersByName")]
		public async Task GetPassengersByName_ReturnsPagedListOfPassengers_WhenPassengersFound()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			string validName1 = "ValidName1";
			string validName2 = "ValidName2";
			int validPage = 1;
			int validPageSize = 10;
			var passengerEntities = new List<PassengerEntity> { passengerEntity };
			var passengerDtos = new List<PassengerDto> { passengerDto };
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
			_passengerServiceMock
				.Setup(service => service.GetPassengersByName(cancellationToken, validPage, validPageSize, validName1, validName2))
				.ReturnsAsync(passengerEntities);
			_passengerServiceMock
				.Setup(service => service.PassengersCount(cancellationToken, validName1, validName2))
				.ReturnsAsync(totalItems);
			_mapperMock
				.Setup(m => m.Map<IEnumerable<PassengerDto>>(passengerEntities))
				.Returns(passengerDtos);

			// Act
			var result = await _controller.GetPassengersByName(cancellationToken, validName1, validName2, validPage, validPageSize);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PagedResponse<PassengerDto>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var response = Assert.IsType<PagedResponse<PassengerDto>>(okResult.Value);
			Assert.Equal(validPage, response.PageNumber);
			Assert.Equal(validPageSize, response.PageSize);
			Assert.Equal(totalItems, response.TotalCount);
			Assert.Equal(passengerDtos, response.Data);
		}

		[Fact]
		[Trait("Category", "PostPassenger")]
		public async Task PostPassenger_ReturnsCreatedAtActionResult_WhenPassengerIsCreatedSuccessfully()
		{
			// Arrange
			var passengerCreateDto = new PassengerCreateDto();
			var passengerEntity = new PassengerEntity { Id = 1 };
			var passengerDto = new PassengerDto { Id = 1 };

			// Set up the mapper to return the expected values
			_mapperMock.Setup(m => m.Map<PassengerEntity>(passengerCreateDto)).Returns(passengerEntity);
			_mapperMock.Setup(m => m.Map<PassengerDto>(passengerEntity)).Returns(passengerDto);

			// Adjust service setup to return the passengerEntity wrapped in a Task
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

	}
}