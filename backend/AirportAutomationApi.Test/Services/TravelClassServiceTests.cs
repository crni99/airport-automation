using AirportAutomation.Application.Services;
using AirportAutomation.Core.Interfaces.IRepositories;
using Moq;

namespace AirportAutomationApi.Test.Services
{
	public class TravelClassServiceTests
	{
		private readonly Mock<ITravelClassRepository> _repositoryMock;
		private readonly TravelClassService _service;

		public TravelClassServiceTests()
		{
			_repositoryMock = new Mock<ITravelClassRepository>();
			_service = new TravelClassService(_repositoryMock.Object);
		}

		[Fact]
		public async Task GetTravelClasses_Should_Call_Repository_GetTravelClasses()
		{
			var cancellationToken = new CancellationToken();
			await _service.GetTravelClasses(cancellationToken, 1, 10);

			_repositoryMock.Verify(repo => repo.GetTravelClasses(cancellationToken, 1, 10), Times.Once);
		}

		[Fact]
		public async Task GetTravelClass_Should_Call_Repository_GetTravelClass()
		{
			await _service.GetTravelClass(1);

			_repositoryMock.Verify(repo => repo.GetTravelClass(1), Times.Once);
		}

		[Fact]
		public async Task TravelClassExists_Should_Call_Repository_TravelClassExists()
		{
			var result = await _service.TravelClassExists(1);

			_repositoryMock.Verify(repo => repo.TravelClassExists(1), Times.Once);
			Assert.False(result);
		}

		[Fact]
		public async Task TravelClassesCount_ShouldReturnCorrectCount()
		{
			var cancellationToken = new CancellationToken();
			var expectedCount = 5;
			_repositoryMock.Setup(repo => repo.TravelClassesCount(cancellationToken)).ReturnsAsync(expectedCount);

			int count = await _service.TravelClassesCount(cancellationToken);

			Assert.Equal(expectedCount, count);
		}
	}
}
