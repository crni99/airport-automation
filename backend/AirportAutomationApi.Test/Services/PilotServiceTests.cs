using AirportAutomation.Application.Services;
using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IRepositories;
using Microsoft.AspNetCore.JsonPatch;
using Moq;

namespace AirportAutomationApi.Test.Services
{
	public class PilotServiceTests
	{
		private readonly Mock<IPilotRepository> _repositoryMock;
		private readonly PilotService _service;

		public PilotServiceTests()
		{
			_repositoryMock = new Mock<IPilotRepository>();
			_service = new PilotService(_repositoryMock.Object);
		}

		[Fact]
		public async Task GetAllPilots_Should_Call_Repository_GetAllPilots()
		{
			var cancellationToken = new CancellationToken();
			await _service.GetAllPilots(cancellationToken);

			_repositoryMock.Verify(repo => repo.GetAllPilots(cancellationToken), Times.Once);
		}

		[Fact]
		public async Task GetPilots_Should_Call_Repository_GetPilots()
		{
			var cancellationToken = new CancellationToken();
			await _service.GetPilots(cancellationToken, 1, 10);

			_repositoryMock.Verify(repo => repo.GetPilots(cancellationToken, 1, 10), Times.Once);
		}

		[Fact]
		public async Task GetPilot_Should_Call_Repository_GetPilot()
		{
			await _service.GetPilot(1);

			_repositoryMock.Verify(repo => repo.GetPilot(1), Times.Once);
		}

		[Fact]
		public async Task SearchPilots_Should_Call_Repository_SearchPilots()
		{
			var cancellationToken = new CancellationToken();
			await _service.SearchPilots(cancellationToken, 1, 10, null);

			_repositoryMock.Verify(repo => repo.SearchPilots(cancellationToken, 1, 10, null), Times.Once);
		}

		[Fact]
		public async Task PostPilot_Should_Call_Repository_PostPilot()
		{
			var pilot = new PilotEntity();

			await _service.PostPilot(pilot);

			_repositoryMock.Verify(repo => repo.PostPilot(pilot), Times.Once);
		}

		[Fact]
		public async Task PutPilot_Should_Call_Repository_PutPilot()
		{
			var pilot = new PilotEntity();

			await _service.PutPilot(pilot);

			_repositoryMock.Verify(repo => repo.PutPilot(pilot), Times.Once);
		}

		[Fact]
		public async Task PatchPilot_Should_Call_Repository_PatchPilot()
		{
			var pilotDocument = new JsonPatchDocument();

			await _service.PatchPilot(1, pilotDocument);

			_repositoryMock.Verify(repo => repo.PatchPilot(1, pilotDocument), Times.Once);
		}

		[Fact]
		public async Task DeletePilot_Should_Call_Repository_DeletePilot()
		{
			await _service.DeletePilot(1);

			_repositoryMock.Verify(repo => repo.DeletePilot(1), Times.Once);
		}

		[Fact]
		public async Task PilotExists_Should_Call_Repository_PilotExists()
		{
			var result = await _service.PilotExists(1);

			_repositoryMock.Verify(repo => repo.PilotExists(1), Times.Once);
			Assert.False(result);
		}

		[Fact]
		public async Task PilotsCount_ShouldReturnCorrectCount()
		{
			var cancellationToken = new CancellationToken();
			var expectedCount = 5;
			_repositoryMock.Setup(repo => repo.PilotsCount(cancellationToken, null, null)).ReturnsAsync(expectedCount);

			int count = await _service.PilotsCount(cancellationToken);

			Assert.Equal(expectedCount, count);
		}

		[Fact]
		public async Task PilotsCountFilter_ShouldReturnCorrectCount()
		{
			var cancellationToken = new CancellationToken();
			var expectedCount = 5;
			_repositoryMock.Setup(repo => repo.PilotsCountFilter(cancellationToken, null)).ReturnsAsync(expectedCount);

			int count = await _service.PilotsCountFilter(cancellationToken, null);

			Assert.Equal(expectedCount, count);
		}

	}
}
