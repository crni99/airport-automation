using AirportAutomation.Application.Services;
using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IRepositories;
using Moq;

namespace AirportAutomationApi.Test.Services
{
	public class ApiUserServiceTests
	{
		private readonly Mock<IApiUserRepository> _repositoryMock;
		private readonly ApiUserService _service;

		public ApiUserServiceTests()
		{
			_repositoryMock = new Mock<IApiUserRepository>();
			_service = new ApiUserService(_repositoryMock.Object);
		}

		[Fact]
		public async Task GetApiUsers_Should_Call_Repository_GetApiUsers()
		{
			var cancellationToken = new CancellationToken();
			await _service.GetApiUsers(cancellationToken, 1, 10);

			_repositoryMock.Verify(repo => repo.GetApiUsers(cancellationToken, 1, 10), Times.Once);
		}

		[Fact]
		public async Task GetApiUser_Should_Call_Repository_GetApiUser()
		{
			await _service.GetApiUser(1);

			_repositoryMock.Verify(repo => repo.GetApiUser(1), Times.Once);
		}

		[Fact]
		public async Task GetApiUsersByRole_Should_Call_Repository_GetApiUsersByRole()
		{
			var cancellationToken = new CancellationToken();
			await _service.GetApiUsersByRole(cancellationToken, 1, 10, "user");

			_repositoryMock.Verify(repo => repo.GetApiUsersByRole(cancellationToken, 1, 10, "user"), Times.Once);
		}

		[Fact]
		public async Task PutApiUser_Should_Call_Repository_PutApiUser()
		{
			var airline = new ApiUserEntity();

			await _service.PutApiUser(airline);

			_repositoryMock.Verify(repo => repo.PutApiUser(airline), Times.Once);
		}

		[Fact]
		public async Task DeleteApiUser_Should_Call_Repository_DeleteApiUser()
		{
			await _service.DeleteApiUser(1);

			_repositoryMock.Verify(repo => repo.DeleteApiUser(1), Times.Once);
		}

		[Fact]
		public async Task ApiUserExists_Should_Call_Repository_ApiUserExistsAsync()
		{
			var result = await _service.ApiUserExists(1);

			_repositoryMock.Verify(repo => repo.ApiUserExists(1), Times.Once);
			Assert.False(result);
		}

		[Fact]
		public async Task ApiUsersCount_ShouldReturnCorrectCount()
		{
			var cancellationToken = new CancellationToken();
			var expectedCount = 5;
			_repositoryMock.Setup(repo => repo.ApiUsersCount(cancellationToken, null)).ReturnsAsync(expectedCount);

			var count = await _service.ApiUsersCount(cancellationToken);

			Assert.Equal(expectedCount, count);
		}
	}
}

