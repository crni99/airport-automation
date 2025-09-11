using AirportAutomation.Api.Controllers;
using AirportAutomation.Api.Interfaces;
using AirportAutomation.Application.Dtos.ApiUser;
using AirportAutomation.Core.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace AirportAutomationApi.Test.Controllers
{
	public class AuthenticationControllerTests
	{
		private readonly Mock<IAuthenticationRepository> _authenticationRepositoryMock;
		private readonly Mock<IConfiguration> _configurationMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<ILogger<AuthenticationController>> _loggerMock;
		private readonly AuthenticationController _controller;

		private readonly string _testSecret = "thisisalongersecretkeyforauthenticationthatisatleast64byteslong_1234567890";

		public AuthenticationControllerTests()
		{
			_authenticationRepositoryMock = new Mock<IAuthenticationRepository>();
			_configurationMock = new Mock<IConfiguration>();
			_mapperMock = new Mock<IMapper>();
			_loggerMock = new Mock<ILogger<AuthenticationController>>();

			// Mock the configuration for the JWT secret key
			_configurationMock.Setup(c => c["Authentication:SecretForKey"]).Returns(_testSecret);
			_configurationMock.Setup(c => c["Authentication:Issuer"]).Returns("test-issuer");
			_configurationMock.Setup(c => c["Authentication:Audience"]).Returns("test-audience");

			_controller = new AuthenticationController(
				_authenticationRepositoryMock.Object,
				_configurationMock.Object,
				_mapperMock.Object,
				_loggerMock.Object);
		}

		[Theory]
		[Trait("Category", "Constructor")]
		[InlineData("authenticationRepository")]
		[InlineData("configuration")]
		[InlineData("mapper")]
		[InlineData("logger")]
		public void Constructor_WhenServiceIsNull_ThrowsArgumentNullException(string serviceName)
		{
			// Arrange
			var authenticationRepositoryMock = new Mock<IAuthenticationRepository>();
			var configurationMock = new Mock<IConfiguration>();
			var mapperMock = new Mock<IMapper>();
			var loggerMock = new Mock<ILogger<AuthenticationController>>();

			// Set up mocks to return null based on the test case
			IAuthenticationRepository authenticationRepository = serviceName == "authenticationRepository" ? null : authenticationRepositoryMock.Object;
			IConfiguration configuration = serviceName == "configuration" ? null : configurationMock.Object;
			IMapper mapper = serviceName == "mapper" ? null : mapperMock.Object;
			ILogger<AuthenticationController> logger = serviceName == "logger" ? null : loggerMock.Object;

			// Act & Assert
			var exception = Record.Exception(() => new AuthenticationController(
				authenticationRepository,
				configuration,
				mapper,
				logger
			));

			// Assert
			Assert.NotNull(exception);
			Assert.IsType<ArgumentNullException>(exception);
			Assert.Contains(serviceName, exception.Message);
		}

		#region Authenticate

		[Trait("Category", "Authenticate")]
		[Fact]
		public void Authenticate_ValidUser_ReturnsOkWithToken()
		{
			// Arrange
			var apiUserDto = new ApiUserDto { UserName = "testuser", Password = "password123" };
			var apiUserEntity = new ApiUserEntity { UserName = "testuser", Password = "password123" };
			var hashedPassword = BCrypt.Net.BCrypt.HashPassword(apiUserEntity.Password);
			var userFromRepo = new ApiUserEntity { UserName = "testuser", Password = hashedPassword, Roles = "Admin" };

			_mapperMock.Setup(m => m.Map<ApiUserEntity>(It.IsAny<ApiUserDto>())).Returns(apiUserEntity);
			_authenticationRepositoryMock.Setup(r => r.GetUserByUsername("testuser")).Returns(userFromRepo);

			// Act
			var result = _controller.Authenticate(apiUserDto);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			Assert.IsType<string>(okResult.Value);
			Assert.False(string.IsNullOrEmpty((string)okResult.Value));
		}

		[Trait("Category", "Authenticate")]
		[Fact]
		public void Authenticate_InvalidPassword_ReturnsUnauthorized()
		{
			// Arrange
			var apiUserDto = new ApiUserDto { UserName = "testuser", Password = "wrongpassword" };
			var apiUserEntity = new ApiUserEntity { UserName = "testuser", Password = "wrongpassword" };
			var hashedPassword = BCrypt.Net.BCrypt.HashPassword("correctpassword");
			var userFromRepo = new ApiUserEntity { UserName = "testuser", Password = hashedPassword, Roles = "User" };

			_mapperMock.Setup(m => m.Map<ApiUserEntity>(It.IsAny<ApiUserDto>())).Returns(apiUserEntity);
			_authenticationRepositoryMock.Setup(r => r.GetUserByUsername("testuser")).Returns(userFromRepo);

			// Act
			var result = _controller.Authenticate(apiUserDto);

			// Assert
			var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
			Assert.Equal(StatusCodes.Status401Unauthorized, unauthorizedResult.StatusCode);
			Assert.Equal("Provided username or password is incorrect.", unauthorizedResult.Value);
		}

		[Trait("Category", "Authenticate")]
		[Fact]
		public void Authenticate_InvalidUsername_ReturnsUnauthorized()
		{
			// Arrange
			var apiUserDto = new ApiUserDto { UserName = "nonexistentuser", Password = "password123" };
			var apiUserEntity = new ApiUserEntity { UserName = "nonexistentuser", Password = "password123" };

			_mapperMock.Setup(m => m.Map<ApiUserEntity>(It.IsAny<ApiUserDto>())).Returns(apiUserEntity);
			_authenticationRepositoryMock.Setup(r => r.GetUserByUsername("nonexistentuser")).Returns((ApiUserEntity)null);

			// Act
			var result = _controller.Authenticate(apiUserDto);

			// Assert
			var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
			Assert.Equal(StatusCodes.Status401Unauthorized, unauthorizedResult.StatusCode);
			Assert.Equal("Provided username or password is incorrect.", unauthorizedResult.Value);
		}

		[Trait("Category", "Authenticate")]
		[Fact]
		public void Authenticate_ValidUser_TokenContainsCorrectClaims()
		{
			// Arrange
			var apiUserDto = new ApiUserDto { UserName = "testuser", Password = "password123" };
			var apiUserEntity = new ApiUserEntity { UserName = "testuser", Password = "password123" };
			var hashedPassword = BCrypt.Net.BCrypt.HashPassword(apiUserEntity.Password);
			var userFromRepo = new ApiUserEntity { UserName = "testuser", Password = hashedPassword, Roles = "Admin" };

			_mapperMock.Setup(m => m.Map<ApiUserEntity>(It.IsAny<ApiUserDto>())).Returns(apiUserEntity);
			_authenticationRepositoryMock.Setup(r => r.GetUserByUsername("testuser")).Returns(userFromRepo);

			// Act
			var result = _controller.Authenticate(apiUserDto);
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var tokenString = okResult.Value as string;

			// Assert
			var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
			var jwtToken = handler.ReadJwtToken(tokenString);

			Assert.Equal("testuser", jwtToken.Claims.First(c => c.Type == ClaimTypes.Name).Value);
			Assert.Equal("Admin", jwtToken.Claims.First(c => c.Type == ClaimTypes.Role).Value);
		}

		#endregion

	}
}
