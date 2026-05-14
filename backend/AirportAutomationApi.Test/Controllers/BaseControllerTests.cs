using AirportAutomation.Api.Interfaces;
using AirportAutomation.Application.Dtos.Response;
using AirportAutomation.Core.Enums;
using AirportAutomation.Core.Interfaces;
using AirportAutomation.Core.Interfaces.IServices;
using AirportAutomationApi.Test.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

public class BaseControllerTests
{
	private readonly TestableBaseController _controller;
	private readonly Mock<ICacheService> _cacheMock;
	private readonly Mock<IMapper> _mapperMock;
	private readonly Mock<ILogger> _loggerMock;
	private readonly Mock<IInputValidationService> _inputValMock;
	private readonly Mock<IPaginationValidationService> _pagValMock;
	private readonly Mock<IExportService> _exportServiceMock;

	public BaseControllerTests()
	{
		_controller = new TestableBaseController();
		_cacheMock = new Mock<ICacheService>();
		_mapperMock = new Mock<IMapper>();
		_loggerMock = new Mock<ILogger>();
		_inputValMock = new Mock<IInputValidationService>();
		_pagValMock = new Mock<IPaginationValidationService>();
		_exportServiceMock = new Mock<IExportService>();

		_cacheMock.Setup(x => x.GetOrCreateAsync<It.IsAnyType>(
				It.IsAny<string>(),
				It.IsAny<Func<Task<It.IsAnyType>>>(),
				null,
				null))
			.Returns(new InvocationFunc(invocation =>
			{
				var factory = invocation.Arguments[1];
				var task = factory.GetType().GetMethod("Invoke").Invoke(factory, null);
				return task;
			}));
	}

	#region GetPagedAsync Tests

	[Fact]
	public async Task GetPagedAsync_InvalidPagination_ReturnsResultFromValidation()
	{
		var badRequest = new BadRequestObjectResult("Invalid Page");
		_pagValMock.Setup(x => x.ValidatePaginationParameters(0, 10, 20))
			.Returns((false, 10, badRequest));

		var result = await _controller.TestGetPagedAsync<object, object>(
			_cacheMock.Object, _pagValMock.Object, _mapperMock.Object, _loggerMock.Object,
			0, 10, 20, "key", null!, null!);

		Assert.Equal(badRequest, result.Result);
	}

	[Fact]
	public async Task GetPagedAsync_NoItemsFound_ReturnsNoContent()
	{
		_pagValMock.Setup(x => x.ValidatePaginationParameters(1, 10, 20))
			.Returns((true, 10, null!));

		var result = await _controller.TestGetPagedAsync<string, string>(
			_cacheMock.Object, _pagValMock.Object, _mapperMock.Object, _loggerMock.Object,
			1, 10, 20, "key",
			() => Task.FromResult<IList<string>?>(new List<string>()),
			() => Task.FromResult(0));

		Assert.IsType<NoContentResult>(result.Result);
	}

	[Fact]
	public async Task GetPagedAsync_ItemsExist_ReturnsOkWithPagedResponse()
	{
		// Arrange
		var items = new List<string> { "Item1" };
		var dtos = new List<string> { "Dto1" };
		_pagValMock.Setup(x => x.ValidatePaginationParameters(1, 10, 20)).Returns((true, 10, null!));
		_mapperMock.Setup(m => m.Map<IEnumerable<string>>(items)).Returns(dtos);

		// Act
		var result = await _controller.TestGetPagedAsync<string, string>(
			_cacheMock.Object, _pagValMock.Object, _mapperMock.Object, _loggerMock.Object,
			1, 10, 20, "key",
			() => Task.FromResult<IList<string>?>(items),
			() => Task.FromResult(1));

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result.Result);
		var response = Assert.IsType<PagedResponse<string>>(okResult.Value);
		Assert.Single(response.Data);
	}

	[Fact]
	public async Task GetPagedAsync_NullItems_ReturnsNoContent()
	{
		_pagValMock.Setup(x => x.ValidatePaginationParameters(1, 10, 20))
			.Returns((true, 10, null!));

		var result = await _controller.TestGetPagedAsync<string, string>(
			_cacheMock.Object, _pagValMock.Object, _mapperMock.Object, _loggerMock.Object,
			1, 10, 20, "key",
			() => Task.FromResult<IList<string>?>(null),
			() => Task.FromResult(0));

		Assert.IsType<NoContentResult>(result.Result);
	}

	#endregion

	#region GetByIdAsync Tests

	[Fact]
	public async Task GetByIdAsync_InvalidId_ReturnsBadRequest()
	{
		_inputValMock.Setup(x => x.IsNonNegativeInt(-1)).Returns(false);

		var result = await _controller.TestGetByIdAsync<object, object>(
			_cacheMock.Object, _inputValMock.Object, _mapperMock.Object, _loggerMock.Object,
			-1, "key", () => Task.FromResult<object?>(null));

		Assert.IsType<BadRequestObjectResult>(result.Result);
	}

	[Fact]
	public async Task GetByIdAsync_ItemNotFound_ReturnsNotFound()
	{
		_inputValMock.Setup(x => x.IsNonNegativeInt(1)).Returns(true);

		var result = await _controller.TestGetByIdAsync<string, string>(
			_cacheMock.Object, _inputValMock.Object, _mapperMock.Object, _loggerMock.Object,
			1, "key", () => Task.FromResult<string?>(null));

		Assert.IsType<NotFoundResult>(result.Result);
	}

	[Fact]
	public async Task GetByIdAsync_ItemFound_ReturnsOk()
	{
		var item = "TestItem";
		var dto = "TestDto";
		_inputValMock.Setup(x => x.IsNonNegativeInt(1)).Returns(true);
		_mapperMock.Setup(m => m.Map<string>(item)).Returns(dto);

		var result = await _controller.TestGetByIdAsync<string, string>(
			_cacheMock.Object, _inputValMock.Object, _mapperMock.Object, _loggerMock.Object,
			1, "key", () => Task.FromResult<string?>(item));

		var okResult = Assert.IsType<OkObjectResult>(result.Result);
		Assert.Equal(dto, okResult.Value);
	}

	#endregion

	#region ExportAsync Tests

	[Fact]
	public async Task ExportAsync_GetAll_ReturnsNoContent_WhenItemsNull()
	{
		_exportServiceMock.Setup(s => s.ExportToPDF(It.IsAny<string>(), It.IsAny<IList<object>>()))
			.Returns((byte[])null);

		var result = await _controller.TestExportAsync<object>(
			entityName: "Test",
			fileType: FileExtension.Pdf,
			exportService: _exportServiceMock.Object,
			paginationValidation: _pagValMock.Object,
			inputValidation: _inputValMock.Object,
			logger: _loggerMock.Object,
			page: 1, pageSize: 10, maxPageSize: 20,
			getAll: true,
			fetchAll: () => Task.FromResult<IList<object>>(null),
			fetchPaged: null!);

		Assert.IsType<NoContentResult>(result);
	}

	[Fact]
	public async Task ExportAsync_GetAll_ReturnsNoContent_WhenItemsEmpty()
	{
		var result = await _controller.TestExportAsync<object>(
			entityName: "Test",
			fileType: FileExtension.Pdf,
			exportService: _exportServiceMock.Object,
			paginationValidation: _pagValMock.Object,
			inputValidation: _inputValMock.Object,
			logger: _loggerMock.Object,
			page: 1, pageSize: 10, maxPageSize: 20,
			getAll: true,
			fetchAll: () => Task.FromResult<IList<object>>(new List<object>()),
			fetchPaged: null!);

		Assert.IsType<NoContentResult>(result);
	}

	[Fact]
	public async Task ExportAsync_InvalidPagination_ReturnsBadRequest()
	{
		var badRequest = new BadRequestObjectResult("Invalid pagination.");
		_pagValMock.Setup(x => x.ValidatePaginationParameters(0, 10, 20))
			.Returns((false, 0, badRequest));

		var result = await _controller.TestExportAsync<object>(
			entityName: "Test",
			fileType: FileExtension.Pdf,
			exportService: _exportServiceMock.Object,
			paginationValidation: _pagValMock.Object,
			inputValidation: _inputValMock.Object,
			logger: _loggerMock.Object,
			page: 0, pageSize: 10, maxPageSize: 20,
			getAll: false,
			fetchAll: null!,
			fetchPaged: (p, ps) => Task.FromResult<IList<object>>(new List<object>()));

		Assert.Equal(badRequest, result);
	}

	[Fact]
	public async Task ExportAsync_UsesFetchSearch_WhenProvided()
	{
		var items = new List<object> { new() };
		var pdfBytes = new byte[] { 1, 2, 3 };
		_pagValMock.Setup(x => x.ValidatePaginationParameters(1, 10, 20))
			.Returns((true, 10, null!));
		_exportServiceMock.Setup(s => s.ExportToPDF("Test", It.IsAny<IList<object>>()))
			.Returns(pdfBytes);

		var result = await _controller.TestExportAsync<object>(
			entityName: "Test",
			fileType: FileExtension.Pdf,
			exportService: _exportServiceMock.Object,
			paginationValidation: _pagValMock.Object,
			inputValidation: _inputValMock.Object,
			logger: _loggerMock.Object,
			page: 1, pageSize: 10, maxPageSize: 20,
			getAll: false,
			fetchAll: null!,
			fetchPaged: (p, ps) => Task.FromResult<IList<object>>(new List<object>()),
			fetchSearch: (p, ps) => Task.FromResult<IList<object>>(items));

		Assert.IsType<FileContentResult>(result);
	}

	[Fact]
	public async Task ExportAsync_Pdf_ReturnsInternalServerError_WhenPdfNull()
	{
		var items = new List<object> { new() };
		_pagValMock.Setup(x => x.ValidatePaginationParameters(1, 10, 20))
			.Returns((true, 10, null!));
		_exportServiceMock.Setup(s => s.ExportToPDF("Test", It.IsAny<IList<object>>()))
			.Returns((byte[])null);

		var result = await _controller.TestExportAsync<object>(
			entityName: "Test",
			fileType: FileExtension.Pdf,
			exportService: _exportServiceMock.Object,
			paginationValidation: _pagValMock.Object,
			inputValidation: _inputValMock.Object,
			logger: _loggerMock.Object,
			page: 1, pageSize: 10, maxPageSize: 20,
			getAll: false,
			fetchAll: null!,
			fetchPaged: (p, ps) => Task.FromResult<IList<object>>(items));

		var statusResult = Assert.IsType<ObjectResult>(result);
		Assert.Equal(StatusCodes.Status500InternalServerError, statusResult.StatusCode);
	}

	[Fact]
	public async Task ExportAsync_Excel_ReturnsInternalServerError_WhenExcelNull()
	{
		var items = new List<object> { new() };
		_pagValMock.Setup(x => x.ValidatePaginationParameters(1, 10, 20))
			.Returns((true, 10, null!));
		_exportServiceMock.Setup(s => s.ExportToExcel("Test", It.IsAny<IList<object>>()))
			.Returns((byte[])null);

		var result = await _controller.TestExportAsync<object>(
			entityName: "Test",
			fileType: FileExtension.Xlsx,
			exportService: _exportServiceMock.Object,
			paginationValidation: _pagValMock.Object,
			inputValidation: _inputValMock.Object,
			logger: _loggerMock.Object,
			page: 1, pageSize: 10, maxPageSize: 20,
			getAll: false,
			fetchAll: null!,
			fetchPaged: (p, ps) => Task.FromResult<IList<object>>(items));

		var statusResult = Assert.IsType<ObjectResult>(result);
		Assert.Equal(StatusCodes.Status500InternalServerError, statusResult.StatusCode);
	}

	[Fact]
	public async Task ExportAsync_UnsupportedFileType_ReturnsInternalServerError()
	{
		var items = new List<object> { new() };
		_pagValMock.Setup(x => x.ValidatePaginationParameters(1, 10, 20))
			.Returns((true, 10, null!));

		var result = await _controller.TestExportAsync<object>(
			entityName: "Test",
			fileType: (FileExtension)99,
			exportService: _exportServiceMock.Object,
			paginationValidation: _pagValMock.Object,
			inputValidation: _inputValMock.Object,
			logger: _loggerMock.Object,
			page: 1, pageSize: 10, maxPageSize: 20,
			getAll: false,
			fetchAll: null!,
			fetchPaged: (p, ps) => Task.FromResult<IList<object>>(items));

		var statusResult = Assert.IsType<ObjectResult>(result);
		Assert.Equal(StatusCodes.Status500InternalServerError, statusResult.StatusCode);
	}

	[Fact]
	public async Task ExportAsync_Excel_ReturnsFile_WhenExcelGenerated()
	{
		var items = new List<object> { new() };
		var excelBytes = new byte[] { 1, 2, 3 };
		_pagValMock.Setup(x => x.ValidatePaginationParameters(1, 10, 20))
			.Returns((true, 10, null!));
		_exportServiceMock.Setup(s => s.ExportToExcel("Test", It.IsAny<IList<object>>()))
			.Returns(excelBytes);

		var result = await _controller.TestExportAsync<object>(
			entityName: "Test",
			fileType: FileExtension.Xlsx,
			exportService: _exportServiceMock.Object,
			paginationValidation: _pagValMock.Object,
			inputValidation: _inputValMock.Object,
			logger: _loggerMock.Object,
			page: 1, pageSize: 10, maxPageSize: 20,
			getAll: false,
			fetchAll: null!,
			fetchPaged: (p, ps) => Task.FromResult<IList<object>>(items));

		var fileResult = Assert.IsType<FileContentResult>(result);
		Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileResult.ContentType);
		Assert.Equal(excelBytes, fileResult.FileContents);
	}

	[Fact]
	public async Task ExportAsync_Excel_ReturnsInternalServerError_WhenExcelEmpty()
	{
		var items = new List<object> { new() };
		_pagValMock.Setup(x => x.ValidatePaginationParameters(1, 10, 20))
			.Returns((true, 10, null!));
		_exportServiceMock.Setup(s => s.ExportToExcel("Test", It.IsAny<IList<object>>()))
			.Returns(Array.Empty<byte>());

		var result = await _controller.TestExportAsync<object>(
			entityName: "Test",
			fileType: FileExtension.Xlsx,
			exportService: _exportServiceMock.Object,
			paginationValidation: _pagValMock.Object,
			inputValidation: _inputValMock.Object,
			logger: _loggerMock.Object,
			page: 1, pageSize: 10, maxPageSize: 20,
			getAll: false,
			fetchAll: null!,
			fetchPaged: (p, ps) => Task.FromResult<IList<object>>(items));

		var statusResult = Assert.IsType<ObjectResult>(result);
		Assert.Equal(StatusCodes.Status500InternalServerError, statusResult.StatusCode);
	}

	#endregion

}