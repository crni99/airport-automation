using AirportAutomation.Api.Interfaces;
using AirportAutomation.Application.Dtos.Response;
using AirportAutomation.Core.Interfaces;
using AirportAutomationApi.Test.Helpers;
using AutoMapper;
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

	public BaseControllerTests()
	{
		_controller = new TestableBaseController();
		_cacheMock = new Mock<ICacheService>();
		_mapperMock = new Mock<IMapper>();
		_loggerMock = new Mock<ILogger>();
		_inputValMock = new Mock<IInputValidationService>();
		_pagValMock = new Mock<IPaginationValidationService>();

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

	#endregion

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

	#endregion
}