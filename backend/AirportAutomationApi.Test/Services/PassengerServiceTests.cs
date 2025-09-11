using AirportAutomation.Application.Services;
using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IRepositories;
using Microsoft.AspNetCore.JsonPatch;
using Moq;

namespace AirportAutomationApi.Test.Services
{
	public class PassengerServiceTests
	{
		private readonly Mock<IPassengerRepository> _repositoryMock;
		private readonly PassengerService _service;

		public PassengerServiceTests()
		{
			_repositoryMock = new Mock<IPassengerRepository>();
			_service = new PassengerService(_repositoryMock.Object);
		}

		[Fact]
		public async Task GetAllPassengers_Should_Call_Repository_GetAllPassengers()
		{
			var cancellationToken = new CancellationToken();
			await _service.GetAllPassengers(cancellationToken);

			_repositoryMock.Verify(repo => repo.GetAllPassengers(cancellationToken), Times.Once);
		}

		[Fact]
		public async Task GetPassengers_Should_Call_Repository_GetPassengers()
		{
			var cancellationToken = new CancellationToken();
			await _service.GetPassengers(cancellationToken, 1, 10);

			_repositoryMock.Verify(repo => repo.GetPassengers(cancellationToken, 1, 10), Times.Once);
		}

		[Fact]
		public async Task GetPassenger_Should_Call_Repository_GetPassenger()
		{
			await _service.GetPassenger(1);

			_repositoryMock.Verify(repo => repo.GetPassenger(1), Times.Once);
		}

		[Fact]
		public async Task GetPassengersByName_Should_Call_Repository_GetPassengersByName()
		{
			var cancellationToken = new CancellationToken();
			await _service.GetPassengersByName(cancellationToken, 1, 10, "John", "Doe");

			_repositoryMock.Verify(repo => repo.GetPassengersByName(cancellationToken, 1, 10, "John", "Doe"), Times.Once);
		}

		[Fact]
		public async Task GetPassengersByFilter_Should_Call_Repository_GetPassengersByFilter()
		{
			var cancellationToken = new CancellationToken();
			await _service.GetPassengersByFilter(cancellationToken, 1, 10, null);

			_repositoryMock.Verify(repo => repo.GetPassengersByFilter(cancellationToken, 1, 10, null), Times.Once);
		}

		[Fact]
		public async Task PostPassenger_Should_Call_Repository_PostPassenger()
		{
			var passenger = new PassengerEntity();

			await _service.PostPassenger(passenger);

			_repositoryMock.Verify(repo => repo.PostPassenger(passenger), Times.Once);
		}

		[Fact]
		public async Task PutPassenger_Should_Call_Repository_PutPassenger()
		{
			var passenger = new PassengerEntity();

			await _service.PutPassenger(passenger);

			_repositoryMock.Verify(repo => repo.PutPassenger(passenger), Times.Once);
		}

		[Fact]
		public async Task DeletePassenger_Should_Call_Repository_DeletePassenger()
		{
			await _service.DeletePassenger(1);

			_repositoryMock.Verify(repo => repo.DeletePassenger(1), Times.Once);
		}

		[Fact]
		public async Task PassengerExists_Should_Call_Repository_PassengerExistsAsync()
		{
			var result = await _service.PassengerExists(1);

			_repositoryMock.Verify(repo => repo.PassengerExists(1), Times.Once);
			Assert.False(result);
		}

		[Fact]
		public async Task PatchPassenger_Should_Call_Repository_PatchPassenger()
		{
			var passengerDocument = new JsonPatchDocument();

			await _service.PatchPassenger(1, passengerDocument);

			_repositoryMock.Verify(repo => repo.PatchPassenger(1, passengerDocument), Times.Once);
		}

		[Fact]
		public async Task PassengersCount_ShouldReturnCorrectCount()
		{
			var cancellationToken = new CancellationToken();
			var expectedCount = 5;
			_repositoryMock.Setup(repo => repo.PassengersCount(cancellationToken, null, null)).ReturnsAsync(expectedCount);

			int count = await _service.PassengersCount(cancellationToken);

			Assert.Equal(expectedCount, count);
		}

		[Fact]
		public async Task PassengersCountFilter_ShouldReturnCorrectCount()
		{
			var cancellationToken = new CancellationToken();
			var expectedCount = 5;
			_repositoryMock.Setup(repo => repo.PassengersCountFilter(cancellationToken, null)).ReturnsAsync(expectedCount);

			int count = await _service.PassengersCountFilter(cancellationToken, null);

			Assert.Equal(expectedCount, count);
		}

	}
}

