using AirportAutomation.Api.Controllers;
using AirportAutomation.Api.Interfaces;
using AirportAutomation.Application.Dtos.Destination;
using AirportAutomation.Application.Dtos.Response;
using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Enums;
using AirportAutomation.Core.Filters;
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

		[Theory]
		[Trait("Category", "Constructor")]
		[InlineData("destinationService")]
		[InlineData("paginationValidationService")]
		[InlineData("inputValidationService")]
		[InlineData("utilityService")]
		[InlineData("exportService")]
		[InlineData("mapper")]
		[InlineData("logger")]
		public void Constructor_WhenServiceIsNull_ThrowsArgumentNullException(string serviceName)
		{
			// Arrange
			var destinationServiceMock = new Mock<IDestinationService>();
			var paginationValidationServiceMock = new Mock<IPaginationValidationService>();
			var inputValidationServiceMock = new Mock<IInputValidationService>();
			var utilityServiceMock = new Mock<IUtilityService>();
			var exportServiceMock = new Mock<IExportService>();
			var mapperMock = new Mock<IMapper>();
			var loggerMock = new Mock<ILogger<DestinationsController>>();
			var configurationMock = new Mock<IConfiguration>();

			// Set up mocks to return null based on the test case
			IDestinationService destinationService = serviceName == "destinationService" ? null : destinationServiceMock.Object;
			IPaginationValidationService paginationValidationService = serviceName == "paginationValidationService" ? null : paginationValidationServiceMock.Object;
			IInputValidationService inputValidationService = serviceName == "inputValidationService" ? null : inputValidationServiceMock.Object;
			IUtilityService utilityService = serviceName == "utilityService" ? null : utilityServiceMock.Object;
			IExportService exportService = serviceName == "exportService" ? null : exportServiceMock.Object;
			IMapper mapper = serviceName == "mapper" ? null : mapperMock.Object;
			ILogger<DestinationsController> logger = serviceName == "logger" ? null : loggerMock.Object;

			// Act & Assert
			var exception = Record.Exception(() => new DestinationsController(
				destinationService,
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

		#region GetDestinations

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
		public async Task GetDestinations_ReturnsNoContent_WhenDestinationsIsNull()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_destinationServiceMock.Setup(service => service.GetDestinations(cancellationToken, It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync((List<DestinationEntity>)null);

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

		#endregion

		#region GetDestination

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

		#endregion

		#region SearchDestinations

		[Fact]
		[Trait("Category", "SearchDestinations")]
		public async Task SearchDestinations_ReturnsBadRequest_WhenFilterIsEmpty()
		{
			// Arrange
			var filter = new DestinationSearchFilter();

			// Act
			var result = await _controller.SearchDestinations(CancellationToken.None, filter);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal("At least one filter criterion must be provided.", badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "SearchDestinations")]
		public async Task SearchDestinations_ReturnsBadRequest_WhenPaginationIsInvalid()
		{
			// Arrange
			var filter = new DestinationSearchFilter { City = "Paris" };
			var invalidPage = -1;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid page number.");

			_paginationValidationServiceMock
				.Setup(s => s.ValidatePaginationParameters(invalidPage, It.IsAny<int>(), It.IsAny<int>()))
				.Returns((false, 0, expectedBadRequestResult));

			// Act
			var result = await _controller.SearchDestinations(CancellationToken.None, filter, invalidPage);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal(expectedBadRequestResult.Value, badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "SearchDestinations")]
		public async Task SearchDestinations_ReturnsNoContent_WhenDestinationsAreEmpty()
		{
			// Arrange
			var filter = new DestinationSearchFilter { City = "Paris" };

			_paginationValidationServiceMock
				.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_destinationServiceMock
				.Setup(s => s.SearchDestinations(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>(), filter))
				.ReturnsAsync(new List<DestinationEntity>());

			// Act
			var result = await _controller.SearchDestinations(CancellationToken.None, filter);

			// Assert
			Assert.IsType<NoContentResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "SearchDestinations")]
		public async Task SearchDestinations_ReturnsNoContent_WhenDestinationsAreNull()
		{
			// Arrange
			var filter = new DestinationSearchFilter { City = "Paris" };

			_paginationValidationServiceMock
				.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_destinationServiceMock
				.Setup(s => s.SearchDestinations(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>(), filter))
				.ReturnsAsync((List<DestinationEntity>)null);

			// Act
			var result = await _controller.SearchDestinations(CancellationToken.None, filter);

			// Assert
			Assert.IsType<NoContentResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "SearchDestinations")]
		public async Task SearchDestinations_ReturnsOk_WithPagedResponse()
		{
			// Arrange
			var filter = new DestinationSearchFilter { City = "Paris" };
			var page = 1;
			var pageSize = 10;
			var destinationEntities = new List<DestinationEntity>
			{
				new DestinationEntity { Id = 1, City = "Paris" }
			};
			var destinationDtos = new List<DestinationDto>
			{
				new DestinationDto { Id = 1, City = "Paris" }
			};
			var totalItems = destinationEntities.Count;

			_paginationValidationServiceMock
				.Setup(s => s.ValidatePaginationParameters(page, pageSize, It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_destinationServiceMock
				.Setup(s => s.SearchDestinations(It.IsAny<CancellationToken>(), page, pageSize, filter))
				.ReturnsAsync(destinationEntities);
			_destinationServiceMock
				.Setup(s => s.DestinationsCountFilter(It.IsAny<CancellationToken>(), filter))
				.ReturnsAsync(totalItems);
			_mapperMock
				.Setup(m => m.Map<IEnumerable<DestinationDto>>(destinationEntities))
				.Returns(destinationDtos);

			// Act
			var result = await _controller.SearchDestinations(CancellationToken.None, filter, page, pageSize);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var response = Assert.IsType<PagedResponse<DestinationDto>>(okResult.Value);
			Assert.Equal(page, response.PageNumber);
			Assert.Equal(pageSize, response.PageSize);
			Assert.Equal(totalItems, response.TotalCount);
			Assert.Equal(destinationDtos, response.Data);
		}

		[Fact]
		[Trait("Category", "SearchDestinations")]
		public async Task SearchDestinations_ThrowsException_WhenServiceFails()
		{
			// Arrange
			var filter = new DestinationSearchFilter { City = "Paris" };

			_paginationValidationServiceMock
				.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_destinationServiceMock
				.Setup(s => s.SearchDestinations(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>(), filter))
				.ThrowsAsync(new Exception("Simulated exception"));

			// Act & Assert
			await Assert.ThrowsAsync<Exception>(async () => await _controller.SearchDestinations(CancellationToken.None, filter));
		}

		#endregion

		#region PostDestination

		[Fact]
		[Trait("Category", "PostDestination")]
		public async Task PostDestination_ReturnsCreatedAtActionResult_WhenDestinationIsCreatedSuccessfully()
		{
			// Arrange
			var destinationCreateDto = new DestinationCreateDto();
			var destinationEntity = new DestinationEntity { Id = 1 };
			var destinationDto = new DestinationDto { Id = 1 };

			_mapperMock.Setup(m => m.Map<DestinationEntity>(destinationCreateDto)).Returns(destinationEntity);
			_mapperMock.Setup(m => m.Map<DestinationDto>(destinationEntity)).Returns(destinationDto);
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

		#endregion

		#region PutDestination

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

		#endregion

		#region PatchDestination

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

		#endregion

		#region DeleteDestination

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

		#endregion

		#region ExportToPdf

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_ReturnsNoContent_WhenGetAllAndNoDestinationsFound()
		{
			// Arrange
			var filter = new DestinationSearchFilter();
			_destinationServiceMock.Setup(s => s.GetAllDestinations(It.IsAny<CancellationToken>()))
				.ReturnsAsync(new List<DestinationEntity>());

			// Act
			var result = await _controller.ExportToPdf(CancellationToken.None, filter, getAll: true);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_ReturnsNoContent_WhenFilteredAndNoDestinationsFound()
		{
			// Arrange
			var filter = new DestinationSearchFilter { City = "NonExistentCity" };
			_paginationValidationServiceMock.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_destinationServiceMock.Setup(s => s.SearchDestinations(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>(), filter))
				.ReturnsAsync(new List<DestinationEntity>());

			// Act
			var result = await _controller.ExportToPdf(CancellationToken.None, filter);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_ReturnsNoContent_WhenGetDestinationsReturnsNull()
		{
			// Arrange
			var filter = new DestinationSearchFilter();

			_paginationValidationServiceMock.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_destinationServiceMock.Setup(s => s.GetDestinations(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync((IList<DestinationEntity>)null);

			// Act
			var result = await _controller.ExportToPdf(CancellationToken.None, filter);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_ReturnsBadRequest_WhenPaginationIsInvalid()
		{
			// Arrange
			var filter = new DestinationSearchFilter();
			var invalidPage = -1;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid page number.");

			_paginationValidationServiceMock
				.Setup(s => s.ValidatePaginationParameters(invalidPage, It.IsAny<int>(), It.IsAny<int>()))
				.Returns((false, 0, expectedBadRequestResult));

			// Act
			var result = await _controller.ExportToPdf(CancellationToken.None, filter, page: invalidPage);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(expectedBadRequestResult.Value, badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_ReturnsFileResult_WhenFilterIsEmptyAndDestinationsAreFound()
		{
			// Arrange
			var filter = new DestinationSearchFilter();
			var destinations = new List<DestinationEntity> { new DestinationEntity { Id = 1, City = "Paris" } };
			var pdfBytes = new byte[] { 1, 2, 3 };

			_paginationValidationServiceMock.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_destinationServiceMock.Setup(s => s.GetDestinations(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(destinations);
			_exportServiceMock.Setup(s => s.ExportToPDF(It.IsAny<string>(), It.IsAny<IList<DestinationEntity>>()))
				.Returns(pdfBytes);
			_utilityServiceMock.Setup(s => s.GenerateUniqueFileName(It.IsAny<string>(), It.IsAny<FileExtension>()))
				.Returns("TestFile.pdf");

			// Act
			var result = await _controller.ExportToPdf(CancellationToken.None, filter);

			// Assert
			var fileResult = Assert.IsType<FileContentResult>(result);
			Assert.Equal("application/pdf", fileResult.ContentType);
			Assert.Equal("TestFile.pdf", fileResult.FileDownloadName);
			Assert.Equal(pdfBytes, fileResult.FileContents);
		}

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_ReturnsFileResult_WhenFilteredAndDestinationsAreFound()
		{
			// Arrange
			var filter = new DestinationSearchFilter { City = "Paris" };
			var destinations = new List<DestinationEntity> { new DestinationEntity { Id = 1, City = "Paris" } };
			var pdfBytes = new byte[] { 1, 2, 3 };
			_paginationValidationServiceMock.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_destinationServiceMock.Setup(s => s.SearchDestinations(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>(), filter))
				.ReturnsAsync(destinations);
			_exportServiceMock.Setup(s => s.ExportToPDF(It.IsAny<string>(), It.IsAny<IList<DestinationEntity>>()))
				.Returns(pdfBytes);
			_utilityServiceMock.Setup(s => s.GenerateUniqueFileName(It.IsAny<string>(), It.IsAny<FileExtension>()))
				.Returns("TestFile.pdf");

			// Act
			var result = await _controller.ExportToPdf(CancellationToken.None, filter);

			// Assert
			var fileResult = Assert.IsType<FileContentResult>(result);
			Assert.Equal("application/pdf", fileResult.ContentType);
			Assert.Equal("TestFile.pdf", fileResult.FileDownloadName);
			Assert.Equal(pdfBytes, fileResult.FileContents);
		}

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_ReturnsFileResult_WhenGetAllIsTrue()
		{
			// Arrange
			var destinations = new List<DestinationEntity> { new DestinationEntity { Id = 1, City = "Paris" } };
			var pdfBytes = new byte[] { 1, 2, 3 };

			_destinationServiceMock.Setup(s => s.GetAllDestinations(It.IsAny<CancellationToken>()))
				.ReturnsAsync(destinations);
			_exportServiceMock.Setup(s => s.ExportToPDF(It.IsAny<string>(), It.IsAny<IList<DestinationEntity>>()))
				.Returns(pdfBytes);
			_utilityServiceMock.Setup(s => s.GenerateUniqueFileName(It.IsAny<string>(), It.IsAny<FileExtension>()))
				.Returns("TestFile.pdf");

			// Act
			var result = await _controller.ExportToPdf(CancellationToken.None, new DestinationSearchFilter(), getAll: true);

			// Assert
			var fileResult = Assert.IsType<FileContentResult>(result);
			Assert.Equal("application/pdf", fileResult.ContentType);
			Assert.Equal("TestFile.pdf", fileResult.FileDownloadName);
			Assert.Equal(pdfBytes, fileResult.FileContents);
		}

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_ReturnsStatusCode500_WhenPdfGenerationFails()
		{
			// Arrange
			var destinations = new List<DestinationEntity> { new DestinationEntity { Id = 1, City = "Paris" } };
			_destinationServiceMock.Setup(s => s.GetAllDestinations(It.IsAny<CancellationToken>()))
				.ReturnsAsync(destinations);
			_exportServiceMock.Setup(s => s.ExportToPDF(It.IsAny<string>(), It.IsAny<IList<DestinationEntity>>()))
				.Returns((byte[])null);

			// Act
			var result = await _controller.ExportToPdf(CancellationToken.None, new DestinationSearchFilter(), getAll: true);

			// Assert
			var statusCodeResult = Assert.IsType<ObjectResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
			Assert.Equal("Failed to generate PDF file.", statusCodeResult.Value);
		}

		#endregion

		#region ExportToExcel

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_ReturnsNoContent_WhenGetAllAndNoDestinationsFound()
		{
			// Arrange
			var filter = new DestinationSearchFilter();
			_destinationServiceMock.Setup(s => s.GetAllDestinations(It.IsAny<CancellationToken>()))
				.ReturnsAsync(new List<DestinationEntity>());

			// Act
			var result = await _controller.ExportToExcel(CancellationToken.None, filter, getAll: true);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_ReturnsNoContent_WhenFilteredAndNoDestinationsFound()
		{
			// Arrange
			var filter = new DestinationSearchFilter { City = "NonExistentCity" };
			_paginationValidationServiceMock.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_destinationServiceMock.Setup(s => s.SearchDestinations(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>(), filter))
				.ReturnsAsync(new List<DestinationEntity>());

			// Act
			var result = await _controller.ExportToExcel(CancellationToken.None, filter);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_ReturnsBadRequest_WhenPaginationIsInvalid()
		{
			// Arrange
			var filter = new DestinationSearchFilter();
			var invalidPage = -1;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid page number.");

			_paginationValidationServiceMock
				.Setup(s => s.ValidatePaginationParameters(invalidPage, It.IsAny<int>(), It.IsAny<int>()))
				.Returns((false, 0, expectedBadRequestResult));

			// Act
			var result = await _controller.ExportToExcel(CancellationToken.None, filter, page: invalidPage);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(expectedBadRequestResult.Value, badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_ReturnsFileResult_WhenFilterIsEmptyAndDestinationsAreFound()
		{
			// Arrange
			var filter = new DestinationSearchFilter();
			var destinations = new List<DestinationEntity> { new DestinationEntity { Id = 1, City = "Paris" } };
			var excelBytes = new byte[] { 1, 2, 3 };

			_paginationValidationServiceMock.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_destinationServiceMock.Setup(s => s.GetDestinations(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(destinations);
			_exportServiceMock.Setup(s => s.ExportToExcel(It.IsAny<string>(), It.IsAny<IList<DestinationEntity>>()))
				.Returns(excelBytes);
			_utilityServiceMock.Setup(s => s.GenerateUniqueFileName(It.IsAny<string>(), It.IsAny<FileExtension>()))
				.Returns("TestFile.xlsx");

			// Act
			var result = await _controller.ExportToExcel(CancellationToken.None, filter);

			// Assert
			var fileResult = Assert.IsType<FileContentResult>(result);
			Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileResult.ContentType);
			Assert.Equal("TestFile.xlsx", fileResult.FileDownloadName);
			Assert.Equal(excelBytes, fileResult.FileContents);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_ReturnsFileResult_WhenFilteredAndDestinationsAreFound()
		{
			// Arrange
			var filter = new DestinationSearchFilter { City = "Paris" };
			var destinations = new List<DestinationEntity> { new DestinationEntity { Id = 1, City = "Paris" } };
			var excelBytes = new byte[] { 1, 2, 3 };
			_paginationValidationServiceMock.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_destinationServiceMock.Setup(s => s.SearchDestinations(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>(), filter))
				.ReturnsAsync(destinations);
			_exportServiceMock.Setup(s => s.ExportToExcel(It.IsAny<string>(), It.IsAny<IList<DestinationEntity>>()))
				.Returns(excelBytes);
			_utilityServiceMock.Setup(s => s.GenerateUniqueFileName(It.IsAny<string>(), It.IsAny<FileExtension>()))
				.Returns("TestFile.xlsx");

			// Act
			var result = await _controller.ExportToExcel(CancellationToken.None, filter);

			// Assert
			var fileResult = Assert.IsType<FileContentResult>(result);
			Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileResult.ContentType);
			Assert.Equal("TestFile.xlsx", fileResult.FileDownloadName);
			Assert.Equal(excelBytes, fileResult.FileContents);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_ReturnsFileResult_WhenGetAllIsTrue()
		{
			// Arrange
			var destinations = new List<DestinationEntity> { new DestinationEntity { Id = 1, City = "Paris" } };
			var excelBytes = new byte[] { 1, 2, 3 };

			_destinationServiceMock.Setup(s => s.GetAllDestinations(It.IsAny<CancellationToken>()))
				.ReturnsAsync(destinations);
			_exportServiceMock.Setup(s => s.ExportToExcel(It.IsAny<string>(), It.IsAny<IList<DestinationEntity>>()))
				.Returns(excelBytes);
			_utilityServiceMock.Setup(s => s.GenerateUniqueFileName(It.IsAny<string>(), It.IsAny<FileExtension>()))
				.Returns("TestFile.xlsx");

			// Act
			var result = await _controller.ExportToExcel(CancellationToken.None, new DestinationSearchFilter(), getAll: true);

			// Assert
			var fileResult = Assert.IsType<FileContentResult>(result);
			Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileResult.ContentType);
			Assert.Equal("TestFile.xlsx", fileResult.FileDownloadName);
			Assert.Equal(excelBytes, fileResult.FileContents);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_ReturnsStatusCode500_WhenExcelGenerationFails_Null()
		{
			// Arrange
			var destinations = new List<DestinationEntity> { new DestinationEntity { Id = 1, City = "Paris" } };
			_destinationServiceMock.Setup(s => s.GetAllDestinations(It.IsAny<CancellationToken>()))
				.ReturnsAsync(destinations);
			_exportServiceMock.Setup(s => s.ExportToExcel(It.IsAny<string>(), It.IsAny<IList<DestinationEntity>>()))
				.Returns((byte[])null);

			// Act
			var result = await _controller.ExportToExcel(CancellationToken.None, new DestinationSearchFilter(), getAll: true);

			// Assert
			var statusCodeResult = Assert.IsType<ObjectResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
			Assert.Equal("Failed to generate Excel file.", statusCodeResult.Value);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_ReturnsStatusCode500_WhenExcelGenerationFails_EmptyArray()
		{
			// Arrange
			var destinations = new List<DestinationEntity> { new DestinationEntity { Id = 1, City = "Paris" } };
			_destinationServiceMock.Setup(s => s.GetAllDestinations(It.IsAny<CancellationToken>()))
				.ReturnsAsync(destinations);
			_exportServiceMock.Setup(s => s.ExportToExcel(It.IsAny<string>(), It.IsAny<IList<DestinationEntity>>()))
				.Returns(Array.Empty<byte>());

			// Act
			var result = await _controller.ExportToExcel(CancellationToken.None, new DestinationSearchFilter(), getAll: true);

			// Assert
			var statusCodeResult = Assert.IsType<ObjectResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
			Assert.Equal("Failed to generate Excel file.", statusCodeResult.Value);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_ReturnsNoContent_WhenGetDestinationsReturnsNull()
		{
			// Arrange
			var filter = new DestinationSearchFilter();
			_paginationValidationServiceMock.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_destinationServiceMock.Setup(s => s.GetDestinations(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync((IList<DestinationEntity>)null);

			// Act
			var result = await _controller.ExportToExcel(CancellationToken.None, filter);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_ReturnsNoContent_WhenGetAllReturnsNull()
		{
			// Arrange
			var filter = new DestinationSearchFilter();
			_destinationServiceMock.Setup(s => s.GetAllDestinations(It.IsAny<CancellationToken>()))
				.ReturnsAsync((IList<DestinationEntity>)null);

			// Act
			var result = await _controller.ExportToExcel(CancellationToken.None, filter, getAll: true);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		#endregion

	}
}