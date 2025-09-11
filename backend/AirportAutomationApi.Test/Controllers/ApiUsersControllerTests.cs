using AirportAutomation.Api.Controllers;
using AirportAutomation.Api.Interfaces;
using AirportAutomation.Application.Dtos.ApiUser;
using AirportAutomation.Application.Dtos.Response;
using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Filters;
using AirportAutomation.Core.Interfaces.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace AirportAutomationApi.Test.Controllers
{
	public class ApiUsersControllerTests
	{
		private readonly ApiUsersController _controller;
		private readonly Mock<IApiUserService> _apiUserServiceMock;
		private readonly Mock<IPaginationValidationService> _paginationValidationServiceMock;
		private readonly Mock<IInputValidationService> _inputValidationServiceMock;
		private readonly Mock<IUtilityService> _utilityServiceMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<ILogger<ApiUsersController>> _loggerMock;
		private readonly Mock<IConfiguration> _configurationMock;

		private readonly ApiUserEntity apiUserEntity = new()
		{
			ApiUserId = 1,
			UserName = "og",
			Password = "og",
			Roles = "SuperAdmin"
		};

		private readonly ApiUserRoleDto apiUserRoleDto = new()
		{
			UserName = "og",
			Password = "og",
			Roles = "SuperAdmin"
		};

		public ApiUsersControllerTests()
		{
			_apiUserServiceMock = new Mock<IApiUserService>();
			_paginationValidationServiceMock = new Mock<IPaginationValidationService>();
			_inputValidationServiceMock = new Mock<IInputValidationService>();
			_utilityServiceMock = new Mock<IUtilityService>();
			_mapperMock = new Mock<IMapper>();
			_loggerMock = new Mock<ILogger<ApiUsersController>>();
			_configurationMock = new Mock<IConfiguration>();
			var configBuilder = new ConfigurationBuilder();
			configBuilder.AddInMemoryCollection(new Dictionary<string, string>
			{
				{"pageSettings:maxPageSize", "10"}
			});
			_configurationMock.Setup(x => x.GetSection(It.IsAny<string>()))
				.Returns(configBuilder.Build().GetSection(""));

			_controller = new ApiUsersController(
				_apiUserServiceMock.Object,
				_paginationValidationServiceMock.Object,
				_inputValidationServiceMock.Object,
				_mapperMock.Object,
				_loggerMock.Object,
				_configurationMock.Object
			);
		}

		[Theory]
		[Trait("Category", "Constructor")]
		[InlineData("apiUserService")]
		[InlineData("paginationValidationService")]
		[InlineData("inputValidationService")]
		[InlineData("mapper")]
		[InlineData("logger")]
		public void Constructor_WhenServiceIsNull_ThrowsArgumentNullException(string serviceName)
		{
			// Arrange
			var apiUserServiceMock = new Mock<IApiUserService>();
			var paginationValidationServiceMock = new Mock<IPaginationValidationService>();
			var inputValidationServiceMock = new Mock<IInputValidationService>();
			var mapperMock = new Mock<IMapper>();
			var loggerMock = new Mock<ILogger<ApiUsersController>>();
			var configurationMock = new Mock<IConfiguration>();

			// Set up mocks to return null based on the test case
			IApiUserService apiUserService = serviceName == "apiUserService" ? null : apiUserServiceMock.Object;
			IPaginationValidationService paginationValidationService = serviceName == "paginationValidationService" ? null : paginationValidationServiceMock.Object;
			IInputValidationService inputValidationService = serviceName == "inputValidationService" ? null : inputValidationServiceMock.Object;
			IMapper mapper = serviceName == "mapper" ? null : mapperMock.Object;
			ILogger<ApiUsersController> logger = serviceName == "logger" ? null : loggerMock.Object;

			// Act & Assert
			var exception = Record.Exception(() => new ApiUsersController(
				apiUserService,
				paginationValidationService,
				inputValidationService,
				mapper,
				logger,
				configurationMock.Object
			));

			// Assert
			Assert.NotNull(exception);
			Assert.IsType<ArgumentNullException>(exception);
			Assert.Contains(serviceName, exception.Message);
		}

		#region GetApiUsers

		[Fact]
		[Trait("Category", "GetApiUsers")]
		public async Task GetApiUsers_InvalidPaginationParameters_ReturnsBadRequest()
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
			var result = await _controller.GetApiUsers(cancellationToken, invalidPage, invalidPageSize);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetApiUsers")]
		public async Task GetApiUsers_ReturnsNoContent_WhenNoApiUsersFound()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_apiUserServiceMock.Setup(service => service.GetApiUsers(cancellationToken, It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(new List<ApiUserEntity>());

			// Act
			var result = await _controller.GetApiUsers(cancellationToken, page, pageSize);

			// Assert
			Assert.IsType<NoContentResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetApiUsers")]
		public async Task GetApiUsers_ReturnsNoContent_WhenApiUsersIsNull()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_apiUserServiceMock
				.Setup(service => service.GetApiUsers(cancellationToken, It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync((List<ApiUserEntity>)null);

			// Act
			var result = await _controller.GetApiUsers(cancellationToken, page, pageSize);

			// Assert
			Assert.IsType<NoContentResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetApiUsers")]
		public async Task GetApiUsers_ReturnsInternalServerError_WhenExceptionThrown()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_apiUserServiceMock.Setup(service => service.GetApiUsers(cancellationToken, It.IsAny<int>(), It.IsAny<int>()))
				.ThrowsAsync(new Exception("Simulated exception"));

			// Act & Assert
			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetApiUsers(cancellationToken, page, pageSize));
		}

		[Fact]
		[Trait("Category", "GetApiUsers")]
		public async Task GetApiUsers_ReturnsOk_WithPaginatedApiUsers()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 1;
			int pageSize = 10;
			var apiUsers = new List<ApiUserEntity>
			{
				new ApiUserEntity { /* Initialize properties */ },
				new ApiUserEntity { /* Initialize properties */ }
			};
			var totalItems = 2;

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_apiUserServiceMock
				.Setup(service => service.GetApiUsers(cancellationToken, page, pageSize))
				.ReturnsAsync(apiUsers);
			_apiUserServiceMock
				.Setup(service => service.ApiUsersCount(cancellationToken, null))
				.ReturnsAsync(totalItems);

			var expectedData = new List<ApiUserRoleDto>
			{
				new ApiUserRoleDto { /* Initialize properties */ },
				new ApiUserRoleDto { /* Initialize properties */ }
			};
			_mapperMock
				.Setup(m => m.Map<IEnumerable<ApiUserRoleDto>>(It.IsAny<IEnumerable<ApiUserEntity>>()))
				.Returns(expectedData);

			// Act
			var result = await _controller.GetApiUsers(cancellationToken, page, pageSize);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PagedResponse<ApiUserRoleDto>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var pagedResponse = Assert.IsType<PagedResponse<ApiUserRoleDto>>(okResult.Value);
			Assert.Equal(page, pagedResponse.PageNumber);
			Assert.Equal(pageSize, pagedResponse.PageSize);
			Assert.Equal(totalItems, pagedResponse.TotalCount);
			Assert.Equal(expectedData, pagedResponse.Data);
		}

		[Fact]
		[Trait("Category", "GetApiUsers")]
		public async Task GetApiUsers_ReturnsCorrectPageData()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			int page = 2;
			int pageSize = 5;
			var allApiUsers = new List<ApiUserEntity>
			{
				new ApiUserEntity { /* Initialize properties */ },
				new ApiUserEntity { /* Initialize properties */ },
				new ApiUserEntity { /* Initialize properties */ },
				new ApiUserEntity { /* Initialize properties */ },
				new ApiUserEntity { /* Initialize properties */ },
				new ApiUserEntity { /* Initialize properties */ },
				new ApiUserEntity { /* Initialize properties */ },
				new ApiUserEntity { /* Initialize properties */ },
				new ApiUserEntity { /* Initialize properties */ },
				new ApiUserEntity { /* Initialize properties */ }
			};
			var pagedApiUsers = allApiUsers.Skip((page - 1) * pageSize).Take(pageSize).ToList();

			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_apiUserServiceMock
				.Setup(service => service.GetApiUsers(cancellationToken, page, pageSize))
				.ReturnsAsync(pagedApiUsers);
			_apiUserServiceMock
				.Setup(service => service.ApiUsersCount(cancellationToken, null))
				.ReturnsAsync(allApiUsers.Count);

			var expectedData = new List<ApiUserRoleDto>
			{
				new ApiUserRoleDto { /* Initialize properties */ },
				new ApiUserRoleDto { /* Initialize properties */ },
				new ApiUserRoleDto { /* Initialize properties */ },
				new ApiUserRoleDto { /* Initialize properties */ },
				new ApiUserRoleDto { /* Initialize properties */ }
			};
			_mapperMock
				.Setup(m => m.Map<IEnumerable<ApiUserRoleDto>>(It.IsAny<IEnumerable<ApiUserEntity>>()))
				.Returns(expectedData);

			// Act
			var result = await _controller.GetApiUsers(cancellationToken, page, pageSize);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PagedResponse<ApiUserRoleDto>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var pagedResponse = Assert.IsType<PagedResponse<ApiUserRoleDto>>(okResult.Value);
			Assert.Equal(page, pagedResponse.PageNumber);
			Assert.Equal(pageSize, pagedResponse.PageSize);
			Assert.Equal(allApiUsers.Count, pagedResponse.TotalCount);
			Assert.Equal(expectedData, pagedResponse.Data);
		}

		#endregion

		#region GetApiUser

		[Fact]
		[Trait("Category", "GetApiUser")]
		public async Task GetApiUser_InvalidId_ReturnsBadRequest()
		{
			// Arrange
			int invalidId = -1;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid input. The ID must be a non-negative integer.");

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(invalidId))
				.Returns(false);

			// Act
			var result = await _controller.GetApiUser(invalidId);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
			var badRequestResult = result.Result as BadRequestObjectResult;
			Assert.Equal(expectedBadRequestResult.Value, badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "GetApiUser")]
		public async Task GetApiUser_ApiUserNotFound_ReturnsNotFound()
		{
			// Arrange
			int validId = 1;

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(validId))
				.Returns(true);
			_apiUserServiceMock
				.Setup(service => service.ApiUserExists(validId))
				.ReturnsAsync(false);

			// Act
			var result = await _controller.GetApiUser(validId);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetApiUser")]
		public async Task GetApiUser_ReturnsApiUserDto_WhenApiUserExists()
		{
			// Arrange
			int validId = 1;

			_inputValidationServiceMock
				.Setup(x => x.IsNonNegativeInt(validId))
				.Returns(true);
			_apiUserServiceMock
				.Setup(service => service.ApiUserExists(validId))
				.ReturnsAsync(true);
			_apiUserServiceMock
				.Setup(service => service.GetApiUser(validId))
				.ReturnsAsync(apiUserEntity);
			_mapperMock
				.Setup(m => m.Map<ApiUserRoleDto>(apiUserEntity))
				.Returns(apiUserRoleDto);

			// Act
			var result = await _controller.GetApiUser(validId);

			// Assert
			var actionResult = Assert.IsType<ActionResult<ApiUserRoleDto>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var returnedApiUserDto = Assert.IsType<ApiUserRoleDto>(okResult.Value);
			Assert.Equal(apiUserRoleDto, returnedApiUserDto);
		}

		#endregion

		#region GetApiUsersByRole

		[Fact]
		[Trait("Category", "GetApiUsersByRole")]
		public async Task GetApiUsersByRole_InvalidRole_ReturnsBadRequest()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			string invalidRole = string.Empty;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid input. The role must be a valid non-empty string.");

			_inputValidationServiceMock
				.Setup(x => x.IsValidString(invalidRole))
				.Returns(false);

			// Act
			var result = await _controller.GetApiUsersByRole(cancellationToken, invalidRole);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
			var badRequestResult = result.Result as BadRequestObjectResult;
			Assert.Equal(expectedBadRequestResult.Value, badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "GetApiUsersByRole")]
		public async Task GetApiUsersByRole_InvalidPaginationParameters_ReturnsBadRequest()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			string validRole = "ValidRole";
			int invalidPage = -1;
			int invalidPageSize = 0;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid pagination parameters.");

			_inputValidationServiceMock
				.Setup(x => x.IsValidString(validRole))
				.Returns(true);
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(invalidPage, invalidPageSize, It.IsAny<int>()))
				.Returns((false, 0, expectedBadRequestResult));

			// Act
			var result = await _controller.GetApiUsersByRole(cancellationToken, validRole, invalidPage, invalidPageSize);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetApiUsersByRole")]
		public async Task GetApiUsersByRole_ApiUsersNotFound_ReturnsNotFound()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			string validRole = "NonExistentRole";
			int validPage = 1;
			int validPageSize = 10;

			_inputValidationServiceMock
				.Setup(x => x.IsValidString(validRole))
				.Returns(true);
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(validPage, validPageSize, It.IsAny<int>()))
				.Returns((true, validPageSize, null));
			_apiUserServiceMock
				.Setup(service => service.GetApiUsersByRole(cancellationToken, validPage, validPageSize, validRole))
				.ReturnsAsync(new List<ApiUserEntity>());

			// Act
			var result = await _controller.GetApiUsersByRole(cancellationToken, validRole, validPage, validPageSize);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetApiUsersByRole")]
		public async Task GetApiUsersByRole_ApiUsersNull_ReturnsNotFound()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			string validRole = "NonExistentRole";
			int validPage = 1;
			int validPageSize = 10;

			_inputValidationServiceMock
				.Setup(x => x.IsValidString(validRole))
				.Returns(true);
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(validPage, validPageSize, It.IsAny<int>()))
				.Returns((true, validPageSize, null));
			_apiUserServiceMock
				.Setup(service => service.GetApiUsersByRole(cancellationToken, validPage, validPageSize, validRole))
				.ReturnsAsync((List<ApiUserEntity>)null);

			// Act
			var result = await _controller.GetApiUsersByRole(cancellationToken, validRole, validPage, validPageSize);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetApiUsersByRole")]
		public async Task GetApiUsersByRole_ReturnsPagedListOfApiUsers_WhenApiUsersFound()
		{
			// Arrange
			var cancellationToken = new CancellationToken();
			string validRole = "ValidRole";
			int validPage = 1;
			int validPageSize = 10;
			var apiUserEntities = new List<ApiUserEntity> { apiUserEntity };
			var apiUserRoleDtos = new List<ApiUserRoleDto> { apiUserRoleDto };
			var totalItems = 1;

			_inputValidationServiceMock
				.Setup(x => x.IsValidString(validRole))
				.Returns(true);
			_paginationValidationServiceMock
				.Setup(x => x.ValidatePaginationParameters(validPage, validPageSize, It.IsAny<int>()))
				.Returns((true, validPageSize, null));
			_apiUserServiceMock
				.Setup(service => service.GetApiUsersByRole(cancellationToken, validPage, validPageSize, validRole))
				.ReturnsAsync(apiUserEntities);
			_apiUserServiceMock
				.Setup(service => service.ApiUsersCount(cancellationToken, validRole))
				.ReturnsAsync(totalItems);
			_mapperMock
				.Setup(m => m.Map<IEnumerable<ApiUserRoleDto>>(apiUserEntities))
				.Returns(apiUserRoleDtos);

			// Act
			var result = await _controller.GetApiUsersByRole(cancellationToken, validRole, validPage, validPageSize);

			// Assert
			var actionResult = Assert.IsType<ActionResult<PagedResponse<ApiUserRoleDto>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var response = Assert.IsType<PagedResponse<ApiUserRoleDto>>(okResult.Value);
			Assert.Equal(validPage, response.PageNumber);
			Assert.Equal(validPageSize, response.PageSize);
			Assert.Equal(totalItems, response.TotalCount);
			Assert.Equal(apiUserRoleDtos, response.Data);
		}

		#endregion

		#region GetApiUsersByFilter

		[Fact]
		[Trait("Category", "GetApiUsersByFilter")]
		public async Task GetApiUsersByFilter_ReturnsBadRequest_WhenFilterIsEmpty()
		{
			// Arrange
			var filter = new ApiUserSearchFilter();

			// Act
			var result = await _controller.GetApiUsersByFilter(CancellationToken.None, filter);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal("At least one filter criterion must be provided.", badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "GetApiUsersByFilter")]
		public async Task GetApiUsersByFilter_ReturnsBadRequest_WhenPageIsInvalid()
		{
			// Arrange
			var filter = new ApiUserSearchFilter { UserName = "John" };
			var invalidPage = -1;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid page number.");

			_paginationValidationServiceMock
				.Setup(s => s.ValidatePaginationParameters(invalidPage, It.IsAny<int>(), It.IsAny<int>()))
				.Returns((false, 0, expectedBadRequestResult));

			// Act
			var result = await _controller.GetApiUsersByFilter(CancellationToken.None, filter, invalidPage);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal(expectedBadRequestResult.Value, badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "GetApiUsersByFilter")]
		public async Task GetApiUsersByFilter_ReturnsBadRequest_WhenPageSizeIsInvalid()
		{
			// Arrange
			var filter = new ApiUserSearchFilter { UserName = "John" };
			var invalidPageSize = 0;
			var expectedBadRequestResult = new BadRequestObjectResult("Invalid page size.");

			_paginationValidationServiceMock
				.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), invalidPageSize, It.IsAny<int>()))
				.Returns((false, 0, expectedBadRequestResult));

			// Act
			var result = await _controller.GetApiUsersByFilter(CancellationToken.None, filter, pageSize: invalidPageSize);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal(expectedBadRequestResult.Value, badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "GetApiUsersByFilter")]
		public async Task GetApiUsersByFilter_ReturnsNotFound_WhenUsersAreEmpty()
		{
			// Arrange
			var filter = new ApiUserSearchFilter { UserName = "testuser" };

			_paginationValidationServiceMock
				.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_apiUserServiceMock
				.Setup(s => s.GetApiUsersByFilter(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>(), filter))
				.ReturnsAsync(new List<ApiUserEntity>());

			// Act
			var result = await _controller.GetApiUsersByFilter(CancellationToken.None, filter);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetApiUsersByFilter")]
		public async Task GetApiUsersByFilter_ReturnsNotFound_WhenUsersIsNull()
		{
			// Arrange
			var filter = new ApiUserSearchFilter { UserName = "testuser" };

			_paginationValidationServiceMock
				.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_apiUserServiceMock
				.Setup(s => s.GetApiUsersByFilter(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>(), filter))
				.ReturnsAsync((List<ApiUserEntity>)null);

			// Act
			var result = await _controller.GetApiUsersByFilter(CancellationToken.None, filter);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		[Trait("Category", "GetApiUsersByFilter")]
		public async Task GetApiUsersByFilter_ReturnsOk_WithPagedResponse()
		{
			// Arrange
			var filter = new ApiUserSearchFilter { UserName = "Jane" };
			var page = 1;
			var pageSize = 10;
			var apiUsers = new List<ApiUserEntity>
			{
				new ApiUserEntity { ApiUserId = 1, UserName = "Jane", Password = "password", Roles = "User" }
			};
			var apiUserDtos = new List<ApiUserRoleDto>
			{
				new ApiUserRoleDto { UserName = "Jane", Password = "password", Roles = "User" }
			};
			var totalItems = apiUsers.Count;

			_paginationValidationServiceMock
				.Setup(s => s.ValidatePaginationParameters(page, pageSize, It.IsAny<int>()))
				.Returns((true, pageSize, null));
			_apiUserServiceMock
				.Setup(s => s.GetApiUsersByFilter(It.IsAny<CancellationToken>(), page, pageSize, filter))
				.ReturnsAsync(apiUsers);
			_apiUserServiceMock
				.Setup(s => s.ApiUsersCountFilter(It.IsAny<CancellationToken>(), filter))
				.ReturnsAsync(totalItems);
			_mapperMock
				.Setup(m => m.Map<IEnumerable<ApiUserRoleDto>>(apiUsers))
				.Returns(apiUserDtos);

			// Act
			var result = await _controller.GetApiUsersByFilter(CancellationToken.None, filter, page, pageSize);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var response = Assert.IsType<PagedResponse<ApiUserRoleDto>>(okResult.Value);
			Assert.Equal(page, response.PageNumber);
			Assert.Equal(pageSize, response.PageSize);
			Assert.Equal(totalItems, response.TotalCount);
			Assert.Equal(apiUserDtos, response.Data);
		}

		[Fact]
		[Trait("Category", "GetApiUsersByFilter")]
		public async Task GetApiUsersByFilter_ThrowsException_WhenServiceFails()
		{
			// Arrange
			var filter = new ApiUserSearchFilter { UserName = "Jane" };

			_paginationValidationServiceMock
				.Setup(s => s.ValidatePaginationParameters(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((true, 10, null));
			_apiUserServiceMock
				.Setup(s => s.GetApiUsersByFilter(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>(), filter))
				.ThrowsAsync(new Exception("Simulated exception"));

			// Act & Assert
			await Assert.ThrowsAsync<Exception>(async () => await _controller.GetApiUsersByFilter(CancellationToken.None, filter));
		}

		#endregion

		#region PutApiUser

		[Fact]
		[Trait("Category", "PutApiUser")]
		public async Task PutApiUser_ReturnsNoContent_WhenUpdateIsSuccessful()
		{
			// Arrange
			int id = 1;
			var apiUserRoleDto = new ApiUserRoleDto { ApiUserId = id };
			var apiUserEntity = new ApiUserEntity { ApiUserId = id };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_mapperMock.Setup(m => m.Map<ApiUserEntity>(apiUserRoleDto)).Returns(apiUserEntity);
			_apiUserServiceMock.Setup(service => service.ApiUserExists(id)).ReturnsAsync(true);
			_apiUserServiceMock.Setup(service => service.PutApiUser(apiUserEntity)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.PutApiUser(id, apiUserRoleDto);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "PutApiUser")]
		public async Task PutApiUser_ReturnsBadRequest_WhenIdIsInvalid()
		{
			// Arrange
			int invalidId = -1;
			var apiUserRoleDto = new ApiUserRoleDto { ApiUserId = invalidId };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(invalidId)).Returns(false);

			// Act
			var result = await _controller.PutApiUser(invalidId, apiUserRoleDto);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Invalid input. The ID must be a non-negative integer.", badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "PutApiUser")]
		public async Task PutApiUser_ReturnsBadRequest_WhenIdInDtoDoesNotMatchIdInUrl()
		{
			// Arrange
			int id = 1;
			var apiUserRoleDto = new ApiUserRoleDto { ApiUserId = 2 };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);

			// Act
			var result = await _controller.PutApiUser(id, apiUserRoleDto);

			// Assert
			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		[Trait("Category", "PutApiUser")]
		public async Task PutApiUser_ReturnsNotFound_WhenApiUserDoesNotExist()
		{
			// Arrange
			int id = 1;
			var apiUserRoleDto = new ApiUserRoleDto { ApiUserId = id };

			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_apiUserServiceMock.Setup(service => service.ApiUserExists(id)).ReturnsAsync(false);

			// Act
			var result = await _controller.PutApiUser(id, apiUserRoleDto);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		#endregion

		#region DeleteApiUser

		[Fact]
		[Trait("Category", "DeleteApiUser")]
		public async Task DeleteApiUser_ReturnsNoContent_WhenDeletionIsSuccessful()
		{
			// Arrange
			int id = 1;
			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_apiUserServiceMock.Setup(service => service.ApiUserExists(id)).ReturnsAsync(true);
			_apiUserServiceMock.Setup(service => service.DeleteApiUser(id)).ReturnsAsync(true);

			// Act
			var result = await _controller.DeleteApiUser(id);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		[Trait("Category", "DeleteApiUser")]
		public async Task DeleteApiUser_ReturnsBadRequest_WhenIdIsInvalid()
		{
			// Arrange
			int invalidId = -1;
			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(invalidId)).Returns(false);

			// Act
			var result = await _controller.DeleteApiUser(invalidId);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Invalid input. The ID must be a non-negative integer.", badRequestResult.Value);
		}

		[Fact]
		[Trait("Category", "DeleteApiUser")]
		public async Task DeleteApiUser_ReturnsNotFound_WhenApiUserDoesNotExist()
		{
			// Arrange
			int id = 1;
			_inputValidationServiceMock.Setup(service => service.IsNonNegativeInt(id)).Returns(true);
			_apiUserServiceMock.Setup(service => service.ApiUserExists(id)).ReturnsAsync(false);

			// Act
			var result = await _controller.DeleteApiUser(id);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		#endregion

	}
}