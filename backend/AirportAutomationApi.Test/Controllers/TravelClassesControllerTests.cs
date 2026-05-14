using AirportAutomation.Api.Controllers;
using AirportAutomation.Api.Interfaces;
using AirportAutomation.Application.Dtos.Response;
using AirportAutomation.Application.Dtos.TravelClass;
using AirportAutomation.Core.Configuration;
using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Enums;
using AirportAutomation.Core.Interfaces;
using AirportAutomation.Core.Interfaces.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace AirportAutomationApi.Test.Controllers
{
	public class TravelClassesControllerTests
	{
		private readonly TravelClassesController _controller;
		private readonly Mock<ITravelClassService> _travelClassServiceMock;
		private readonly Mock<ICacheService> _cacheServiceMock;
		private readonly Mock<IPaginationValidationService> _paginationValidationServiceMock;
		private readonly Mock<IInputValidationService> _inputValidationServiceMock;
		private readonly Mock<IUtilityService> _utilityServiceMock;
		private readonly Mock<IExportService> _exportServiceMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<ILogger<TravelClassesController>> _loggerMock;
		private readonly IOptions<PageSettings> _pageSettingsOptions;

		private readonly TravelClassEntity travelClassEntity = new()
		{
			Id = 1,
			Type = "Business"
		};

		private readonly TravelClassDto travelClassDto = new()
		{
			Id = 2,
			Type = "First"
		};

		public TravelClassesControllerTests()
		{
			_travelClassServiceMock = new Mock<ITravelClassService>();
			_cacheServiceMock = new Mock<ICacheService>();
			_paginationValidationServiceMock = new Mock<IPaginationValidationService>();
			_inputValidationServiceMock = new Mock<IInputValidationService>();
			_utilityServiceMock = new Mock<IUtilityService>();
			_exportServiceMock = new Mock<IExportService>();
			_mapperMock = new Mock<IMapper>();
			_loggerMock = new Mock<ILogger<TravelClassesController>>();
			_pageSettingsOptions = Options.Create(new PageSettings { MaxPageSize = 10 });

			_controller = new TravelClassesController(
				_travelClassServiceMock.Object,
				_cacheServiceMock.Object,
				_paginationValidationServiceMock.Object,
				_inputValidationServiceMock.Object,
				_utilityServiceMock.Object,
				_exportServiceMock.Object,
				_mapperMock.Object,
				_loggerMock.Object,
				_pageSettingsOptions
			);
		}

		[Theory]
		[Trait("Category", "Constructor")]
		[InlineData("travelClassService")]
		[InlineData("cacheService")]
		[InlineData("paginationValidationService")]
		[InlineData("inputValidationService")]
		[InlineData("utilityService")]
		[InlineData("exportService")]
		[InlineData("mapper")]
		[InlineData("logger")]
		public void Constructor_WhenServiceIsNull_ThrowsArgumentNullException(string serviceName)
		{
			// Arrange
			var travelClassServiceMock = new Mock<ITravelClassService>();
			var cacheServiceMock = new Mock<ICacheService>();
			var paginationValidationServiceMock = new Mock<IPaginationValidationService>();
			var inputValidationServiceMock = new Mock<IInputValidationService>();
			var utilityServiceMock = new Mock<IUtilityService>();
			var exportServiceMock = new Mock<IExportService>();
			var mapperMock = new Mock<IMapper>();
			var loggerMock = new Mock<ILogger<TravelClassesController>>();
			var pageSettingsOptions = Options.Create(new PageSettings { MaxPageSize = 10 });

			// Set up mocks to return null based on the test case
			ITravelClassService travelClassService = serviceName == "travelClassService" ? null : travelClassServiceMock.Object;
			ICacheService cacheService = serviceName == "cacheService" ? null : cacheServiceMock.Object;
			IPaginationValidationService paginationValidationService = serviceName == "paginationValidationService" ? null : paginationValidationServiceMock.Object;
			IInputValidationService inputValidationService = serviceName == "inputValidationService" ? null : inputValidationServiceMock.Object;
			IUtilityService utilityService = serviceName == "utilityService" ? null : utilityServiceMock.Object;
			IExportService exportService = serviceName == "exportService" ? null : exportServiceMock.Object;
			IMapper mapper = serviceName == "mapper" ? null : mapperMock.Object;
			ILogger<TravelClassesController> logger = serviceName == "logger" ? null : loggerMock.Object;

			// Act & Assert
			var exception = Record.Exception(() => new TravelClassesController(
				travelClassService,
				cacheService,
				paginationValidationService,
				inputValidationService,
				utilityService,
				exportService,
				mapper,
				logger,
				pageSettingsOptions
			));

			// Assert
			Assert.NotNull(exception);
			Assert.IsType<ArgumentNullException>(exception);
			Assert.Contains(serviceName, exception.Message);
		}

		[Theory]
		[Trait("Category", "Constructor")]
		[InlineData(true)]  // Tests when pageSettingsOptions is null
		[InlineData(false)] // Tests when pageSettingsOptions.Value is null
		public void Constructor_WhenPageSettingsAreMissing_SetsDefaultMaxPageSize(bool isNullOptions)
		{
			// Arrange
			IOptions<PageSettings> options = isNullOptions
				? null
				: Options.Create<PageSettings>(null);

			// Act
			var controller = new TravelClassesController(
				new Mock<ITravelClassService>().Object,
				new Mock<ICacheService>().Object,
				new Mock<IPaginationValidationService>().Object,
				new Mock<IInputValidationService>().Object,
				new Mock<IUtilityService>().Object,
				new Mock<IExportService>().Object,
				new Mock<IMapper>().Object,
				new Mock<ILogger<TravelClassesController>>().Object,
				options
			);

			// Assert
			Assert.NotNull(controller);
		}

		#region GetTravelClasses

		[Fact]
		[Trait("Category", "GetTravelClasses")]
		public async Task GetTravelClasses_InvalidPaginationParameters_ReturnsBadRequest()
		{
			// Arrange
			int invalidPage = -1;
			int invalidPageSize = 0;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid pagination parameters.");

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(invalidPage, invalidPageSize, It.IsAny<int>()))
				.Returns((false, 0, expectedBadRequestResult));

			// Act
			var result = await _controller.GetTravelClasses(invalidPage, invalidPageSize);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetTravelClasses")]
		public async Task GetTravelClasses_ReturnsNoContent_WhenNoTravelClassesFound()
		{
			// Arrange
			int page = 1;
			int pageSize = 10;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_travelClassServiceMock.Setup(service => service.GetTravelClasses(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(new List<TravelClassEntity>());

			// Act
			var result = await _controller.GetTravelClasses(page, pageSize);

			// Assert
			Assert.IsType<NoContentResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetTravelClasses")]
		public async Task GetTravelClasses_ReturnsNoContent_WhenServiceReturnsNull()
		{
			// Arrange
			int page = 1;
			int pageSize = 10;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_travelClassServiceMock
				.Setup(service => service.GetTravelClasses(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync((List<TravelClassEntity>)null);

			// Act
			var result = await _controller.GetTravelClasses(page, pageSize);

			// Assert
			Assert.IsType<NoContentResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetTravelClasses")]
		public async Task GetTravelClasses_ReturnsInternalServerError_WhenExceptionThrown()
		{
			// Arrange
			int page = 1;
			int pageSize = 10;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_cacheServiceMock
				.Setup(x => x.GetOrCreateAsync<PagedResponse<TravelClassDto>>(
					It.IsAny<string>(),
					It.IsAny<Func<Task<PagedResponse<TravelClassDto>?>>>(),
					null, null))
				.Returns<string, Func<Task<PagedResponse<TravelClassDto>?>>, TimeSpan?, TimeSpan?>(
					async (key, factory, abs, sld) => await factory());
			_travelClassServiceMock.Setup(service => service.GetTravelClasses(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()))
				.ThrowsAsync(new Exception("Simulated exception"));

			// Act & Assert
			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetTravelClasses(page, pageSize));
		}

		[Fact]
		[Trait("Category", "GetTravelClasses")]
		public async Task GetTravelClasses_ReturnsOk_WithPaginatedTravelClasses()
		{
			// Arrange
			int page = 1;
			int pageSize = 10;
			var travelClasses = new List<TravelClassEntity>
			{
				new TravelClassEntity { /* Initialize properties */ },
				new TravelClassEntity { /* Initialize properties */ }
			};
			var totalItems = 2;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_cacheServiceMock
				.Setup(x => x.GetOrCreateAsync<PagedResponse<TravelClassDto>>(
					It.IsAny<string>(),
					It.IsAny<Func<Task<PagedResponse<TravelClassDto>?>>>(),
					null, null))
				.Returns<string, Func<Task<PagedResponse<TravelClassDto>?>>, TimeSpan?, TimeSpan?>(
					async (key, factory, abs, sld) => await factory());
			_travelClassServiceMock
				.Setup(service => service.GetTravelClasses(It.IsAny<CancellationToken>(), page, pageSize))
				.ReturnsAsync(travelClasses);
			_travelClassServiceMock
				.Setup(service => service.TravelClassesCount(It.IsAny<CancellationToken>()))
				.ReturnsAsync(totalItems);

			var expectedData = new List<TravelClassDto>
			{
				new TravelClassDto { /* Initialize properties */ },
				new TravelClassDto { /* Initialize properties */ }
			};
			_mapperMock
				.Setup(m => m.Map<IEnumerable<TravelClassDto>>(It.IsAny<IEnumerable<TravelClassEntity>>()))
				.Returns(expectedData);

			// Act
			var result = await _controller.GetTravelClasses(page, pageSize);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PagedResponse<TravelClassDto>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var pagedResponse = Assert.IsType<PagedResponse<TravelClassDto>>(okResult.Value);
			Assert.Equal(page, pagedResponse.PageNumber);
			Assert.Equal(pageSize, pagedResponse.PageSize);
			Assert.Equal(totalItems, pagedResponse.TotalCount);
			Assert.Equal(expectedData, pagedResponse.Data);
		}

		[Fact]
		[Trait("Category", "GetTravelClasses")]
		public async Task GetTravelClasses_ReturnsCorrectPageData()
		{
			// Arrange
			int page = 2;
			int pageSize = 5;
			var allTravelClasses = new List<TravelClassEntity>
			{
				new TravelClassEntity { /* Initialize properties */ },
				new TravelClassEntity { /* Initialize properties */ },
				new TravelClassEntity { /* Initialize properties */ },
				new TravelClassEntity { /* Initialize properties */ },
				new TravelClassEntity { /* Initialize properties */ },
				new TravelClassEntity { /* Initialize properties */ },
				new TravelClassEntity { /* Initialize properties */ },
				new TravelClassEntity { /* Initialize properties */ },
				new TravelClassEntity { /* Initialize properties */ },
				new TravelClassEntity { /* Initialize properties */ }
			};
			var pagedTravelClasses = allTravelClasses.Skip((page - 1) * pageSize).Take(pageSize).ToList();

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_cacheServiceMock
				.Setup(x => x.GetOrCreateAsync<PagedResponse<TravelClassDto>>(
					It.IsAny<string>(),
					It.IsAny<Func<Task<PagedResponse<TravelClassDto>?>>>(),
					null, null))
				.Returns<string, Func<Task<PagedResponse<TravelClassDto>?>>, TimeSpan?, TimeSpan?>(
					async (key, factory, abs, sld) => await factory());
			_travelClassServiceMock
				.Setup(service => service.GetTravelClasses(It.IsAny<CancellationToken>(), page, pageSize))
				.ReturnsAsync(pagedTravelClasses);
			_travelClassServiceMock
				.Setup(service => service.TravelClassesCount(It.IsAny<CancellationToken>()))
				.ReturnsAsync(allTravelClasses.Count);

			var expectedData = new List<TravelClassDto>
			{
				new TravelClassDto { /* Initialize properties */ },
				new TravelClassDto { /* Initialize properties */ },
				new TravelClassDto { /* Initialize properties */ },
				new TravelClassDto { /* Initialize properties */ },
				new TravelClassDto { /* Initialize properties */ }
			};
			_mapperMock
				.Setup(m => m.Map<IEnumerable<TravelClassDto>>(It.IsAny<IEnumerable<TravelClassEntity>>()))
				.Returns(expectedData);

			// Act
			var result = await _controller.GetTravelClasses(page, pageSize);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PagedResponse<TravelClassDto>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var pagedResponse = Assert.IsType<PagedResponse<TravelClassDto>>(okResult.Value);
			Assert.Equal(page, pagedResponse.PageNumber);
			Assert.Equal(pageSize, pagedResponse.PageSize);
			Assert.Equal(allTravelClasses.Count, pagedResponse.TotalCount);
			Assert.Equal(expectedData, pagedResponse.Data);
		}

		[Fact]
		[Trait("Category", "GetTravelClasses")]
		public async Task GetTravelClasses_ReturnsCachedData_WhenCacheHit()
		{
			// Arrange
			int page = 1;
			int pageSize = 10;
			var cachedResponse = new PagedResponse<TravelClassDto>(
				new List<TravelClassDto> { new TravelClassDto { Id = 1, Type = "Business" } },
				page, pageSize, 1);

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_cacheServiceMock
				.Setup(x => x.GetOrCreateAsync<PagedResponse<TravelClassDto>>(
					It.IsAny<string>(),
					It.IsAny<Func<Task<PagedResponse<TravelClassDto>?>>>(),
					null, null))
				.ReturnsAsync(cachedResponse);

			// Act
			var result = await _controller.GetTravelClasses(page, pageSize);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			Assert.Equal(cachedResponse, okResult.Value);
			_travelClassServiceMock.Verify(x => x.GetTravelClasses(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
		}

		[Fact]
		[Trait("Category", "GetTravelClasses")]
		public async Task GetTravelClasses_SetsCache_WhenCacheMiss()
		{
			// Arrange
			int page = 1;
			int pageSize = 10;
			var travelClasses = new List<TravelClassEntity> { new TravelClassEntity { Id = 1, Type = "Business" } };

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_cacheServiceMock
				.Setup(x => x.GetOrCreateAsync<PagedResponse<TravelClassDto>>(
					It.IsAny<string>(),
					It.IsAny<Func<Task<PagedResponse<TravelClassDto>?>>>(),
					null, null))
				.Returns<string, Func<Task<PagedResponse<TravelClassDto>?>>, TimeSpan?, TimeSpan?>(
					async (key, factory, abs, sld) => await factory());
			_travelClassServiceMock
				.Setup(x => x.GetTravelClasses(It.IsAny<CancellationToken>(), page, pageSize))
				.ReturnsAsync(travelClasses);
			_travelClassServiceMock
				.Setup(x => x.TravelClassesCount(It.IsAny<CancellationToken>()))
				.ReturnsAsync(1);
			_mapperMock
				.Setup(m => m.Map<IEnumerable<TravelClassDto>>(It.IsAny<IEnumerable<TravelClassEntity>>()))
				.Returns(new List<TravelClassDto> { new TravelClassDto { Id = 1, Type = "Business" } });

			// Act
			var result = await _controller.GetTravelClasses(page, pageSize);

			// Assert
			Assert.IsType<OkObjectResult>(result.Result);
			_cacheServiceMock.Verify(x => x.GetOrCreateAsync<PagedResponse<TravelClassDto>>(
				It.IsAny<string>(),
				It.IsAny<Func<Task<PagedResponse<TravelClassDto>?>>>(),
				null, null), Times.Once);
		}

		#endregion

		#region GetTravelClass

		[Fact]
		[Trait("Category", "GetTravelClass")]
		public async Task GetTravelClass_InvalidId_ReturnsBadRequest()
		{
			// Arrange
			int invalidId = -1;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid input. The ID must be a non-negative integer.");

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(invalidId))
				.Returns(false);

			// Act
			var result = await _controller.GetTravelClass(invalidId);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
			var badRequestResult = result.Result as BadRequestObjectResult;
			Assert.Equal(expectedBadRequestResult.Value, badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "GetTravelClass")]
		public async Task GetTravelClass_TravelClassNotFound_ReturnsNotFound()
		{
			// Arrange
			int validId = 1;

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(validId))
				.Returns(true);
			_travelClassServiceMock
				.Setup(service => service.GetTravelClass(validId))
				.ReturnsAsync((TravelClassEntity)null);

			// Act
			var result = await _controller.GetTravelClass(validId);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetTravelClass")]
		public async Task GetTravelClass_ReturnsTravelClassDto_WhenTravelClassExists()
		{
			// Arrange
			int validId = 1;

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(validId))
				.Returns(true);
			_cacheServiceMock
				.Setup(x => x.GetOrCreateAsync<TravelClassDto>(
					It.IsAny<string>(),
					It.IsAny<Func<Task<TravelClassDto?>>>(),
					null, null))
				.Returns<string, Func<Task<TravelClassDto?>>, TimeSpan?, TimeSpan?>(
					async (key, factory, abs, sld) => await factory());
			_travelClassServiceMock
				.Setup(service => service.GetTravelClass(validId))
				.ReturnsAsync(travelClassEntity);
			_mapperMock
				.Setup(m => m.Map<TravelClassDto>(travelClassEntity))
				.Returns(travelClassDto);

			// Act
			var result = await _controller.GetTravelClass(validId);

			// Assert
			var actionResult = Assert.IsType<ActionResult<TravelClassDto>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var returnedTravelClassDto = Assert.IsType<TravelClassDto>(okResult.Value);
			Assert.Equal(travelClassDto, returnedTravelClassDto);
		}

		[Fact]
		[Trait("Category", "GetTravelClass")]
		public async Task GetTravelClass_ReturnsCachedData_WhenCacheHit()
		{
			// Arrange
			int id = 1;
			var cachedTravelClass = new TravelClassDto { Id = 1, Type = "Business" };

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(id))
				.Returns(true);
			_cacheServiceMock
				.Setup(x => x.GetOrCreateAsync<TravelClassDto>(
					It.IsAny<string>(),
					It.IsAny<Func<Task<TravelClassDto?>>>(),
					null, null))
				.ReturnsAsync(cachedTravelClass);

			// Act
			var result = await _controller.GetTravelClass(id);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			Assert.Equal(cachedTravelClass, okResult.Value);
			_travelClassServiceMock.Verify(x => x.GetTravelClass(It.IsAny<int>()), Times.Never);
		}

		[Fact]
		[Trait("Category", "GetTravelClass")]
		public async Task GetTravelClass_SetsCache_WhenCacheMiss()
		{
			// Arrange
			int id = 1;

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(id))
				.Returns(true);
			_cacheServiceMock
				.Setup(x => x.GetOrCreateAsync<TravelClassDto>(
					It.IsAny<string>(),
					It.IsAny<Func<Task<TravelClassDto?>>>(),
					null, null))
				.Returns<string, Func<Task<TravelClassDto?>>, TimeSpan?, TimeSpan?>(
					async (key, factory, abs, sld) => await factory());
			_travelClassServiceMock
				.Setup(x => x.GetTravelClass(id))
				.ReturnsAsync(travelClassEntity);
			_mapperMock
				.Setup(m => m.Map<TravelClassDto>(travelClassEntity))
				.Returns(travelClassDto);

			// Act
			var result = await _controller.GetTravelClass(id);

			// Assert
			Assert.IsType<OkObjectResult>(result.Result);
			_cacheServiceMock.Verify(x => x.GetOrCreateAsync<TravelClassDto>(
				It.IsAny<string>(),
				It.IsAny<Func<Task<TravelClassDto?>>>(),
				null, null), Times.Once);
		}

		#endregion

		#region ExportToPdf

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_InvalidPaginationParameters_ReturnsBadRequest()
		{
			// Arrange
			int invalidPage = -1;
			int invalidPageSize = 0;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid pagination parameters.");

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(invalidPage, invalidPageSize, It.IsAny<int>()))
				.Returns((false, 0, expectedBadRequestResult));

			// Act
			var result = await _controller.ExportToPdf(invalidPage, invalidPageSize);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_NoTravelClassesFound_ReturnsNoContent()
		{
			// Arrange
			int page = 1;
			int pageSize = 10;
			var travelClasses = new List<TravelClassEntity>();

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_travelClassServiceMock
				.Setup(service => service.GetTravelClasses(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(travelClasses);

			// Act
			var result = await _controller.ExportToPdf(page, pageSize);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_ServiceReturnsNull_ReturnsNoContent()
		{
			// Arrange
			int page = 1;
			int pageSize = 10;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_travelClassServiceMock
				.Setup(service => service.GetTravelClasses(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync((List<TravelClassEntity>)null);

			// Act
			var result = await _controller.ExportToPdf(page, pageSize);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_SuccessfulExport_ReturnsFileResult()
		{
			// Arrange
			var travelClasses = new List<TravelClassEntity> { new TravelClassEntity() };
			var pdfBytes = new byte[] { 1, 2, 3 };
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_travelClassServiceMock
				.Setup(service => service.GetTravelClasses(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(travelClasses);
			_exportServiceMock
				.Setup(x => x.ExportToPDF("TravelClasses", It.IsAny<IList<TravelClassEntity>>()))
				.Returns(pdfBytes);

			// Act
			var result = await _controller.ExportToPdf();

			// Assert
			var fileResult = Assert.IsType<FileContentResult>(result);
			Assert.Equal("application/pdf", fileResult.ContentType);
			Assert.Equal(pdfBytes, fileResult.FileContents);
		}

		[Fact]
		[Trait("Category", "ExportToPdf")]
		public async Task ExportToPdf_PdfGenerationFails_ReturnsInternalServerError()
		{
			// Arrange
			var travelClasses = new List<TravelClassEntity> { new TravelClassEntity() };
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_travelClassServiceMock
				.Setup(service => service.GetTravelClasses(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(travelClasses);
			_exportServiceMock
				.Setup(x => x.ExportToPDF("TravelClasses", It.IsAny<IList<TravelClassEntity>>()))
				.Returns((byte[])null);

			// Act
			var result = await _controller.ExportToPdf();

			// Assert
			var statusCodeResult = Assert.IsType<ObjectResult>(result);
			Assert.Equal(500, statusCodeResult.StatusCode);
			Assert.Equal("Failed to generate PDF file.", statusCodeResult.Value);
		}

		#endregion

		#region ExportToExcel

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_InvalidPaginationParameters_ReturnsBadRequest()
		{
			// Arrange
			int invalidPage = -1;
			int invalidPageSize = 0;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid pagination parameters.");

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(invalidPage, invalidPageSize, It.IsAny<int>()))
				.Returns((false, 0, expectedBadRequestResult));

			// Act
			var result = await _controller.ExportToExcel(invalidPage, invalidPageSize);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_NoTravelClassesFound_ReturnsNoContent()
		{
			// Arrange
			int page = 1;
			int pageSize = 10;
			var travelClasses = new List<TravelClassEntity>();

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_travelClassServiceMock
				.Setup(service => service.GetTravelClasses(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(travelClasses);

			// Act
			var result = await _controller.ExportToExcel(page, pageSize);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_ServiceReturnsNull_ReturnsNoContent()
		{
			// Arrange
			int page = 1;
			int pageSize = 10;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_travelClassServiceMock
				.Setup(service => service.GetTravelClasses(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync((List<TravelClassEntity>)null);

			// Act
			var result = await _controller.ExportToExcel(page, pageSize);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_SuccessfulExport_ReturnsFileResult()
		{
			// Arrange
			int page = 1;
			int pageSize = 10;
			var travelClasses = new List<TravelClassEntity> { new TravelClassEntity() };
			var excelBytes = new byte[] { 1, 2, 3 };

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_travelClassServiceMock
				.Setup(service => service.GetTravelClasses(It.IsAny<CancellationToken>(), page, pageSize))
				.ReturnsAsync(travelClasses);
			_exportServiceMock
				.Setup(x => x.ExportToExcel("TravelClasses", travelClasses))
				.Returns(excelBytes);

			// Act
			var result = await _controller.ExportToExcel(page, pageSize);

			// Assert
			var fileResult = Assert.IsType<FileContentResult>(result);
			Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileResult.ContentType);
			Assert.Equal(excelBytes, fileResult.FileContents);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_ExcelGenerationFails_ReturnsInternalServerError()
		{
			// Arrange
			int page = 1;
			int pageSize = 10;
			var travelClasses = new List<TravelClassEntity> { new TravelClassEntity() };

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_travelClassServiceMock
				.Setup(service => service.GetTravelClasses(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(travelClasses);
			_exportServiceMock
				.Setup(x => x.ExportToExcel("TravelClasses", travelClasses))
				.Returns((byte[])null);

			// Act
			var result = await _controller.ExportToExcel(page, pageSize);

			// Assert
			var statusCodeResult = Assert.IsType<ObjectResult>(result);
			Assert.Equal(500, statusCodeResult.StatusCode);
			Assert.Equal("Failed to generate Excel file.", statusCodeResult.Value);
		}

		[Fact]
		[Trait("Category", "ExportToExcel")]
		public async Task ExportToExcel_ExcelGenerationReturnsEmpty_ReturnsInternalServerError()
		{
			// Arrange
			int page = 1;
			int pageSize = 10;
			var travelClasses = new List<TravelClassEntity> { new TravelClassEntity() };
			var emptyExcelBytes = Array.Empty<byte>();

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_travelClassServiceMock
				.Setup(service => service.GetTravelClasses(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(travelClasses);
			_exportServiceMock
				.Setup(x => x.ExportToExcel("TravelClasses", travelClasses))
				.Returns(emptyExcelBytes);

			// Act
			var result = await _controller.ExportToExcel(page, pageSize);

			// Assert
			var statusCodeResult = Assert.IsType<ObjectResult>(result);
			Assert.Equal(500, statusCodeResult.StatusCode);
			Assert.Equal("Failed to generate Excel file.", statusCodeResult.Value);
		}

		#endregion
	}
}