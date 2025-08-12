using AirportAutomation.Application.Services;
using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IRepositories;
using Microsoft.AspNetCore.JsonPatch;
using Moq;

namespace AirportAutomationApi.Test.Services
{
	public class PlaneTicketServiceTests
	{
		private readonly Mock<IPlaneTicketRepository> _repositoryMock;
		private readonly PlaneTicketService _service;

		public PlaneTicketServiceTests()
		{
			_repositoryMock = new Mock<IPlaneTicketRepository>();
			_service = new PlaneTicketService(_repositoryMock.Object);
		}

		[Fact]
		public async Task GetAllPlaneTickets_Should_Call_Repository_GetAllPlaneTickets()
		{
			var cancellationToken = new CancellationToken();
			await _service.GetAllPlaneTickets(cancellationToken);

			_repositoryMock.Verify(repo => repo.GetAllPlaneTickets(cancellationToken), Times.Once);
		}

		[Fact]
		public async Task GetPlaneTickets_Should_Call_Repository_GetPlaneTickets()
		{
			var cancellationToken = new CancellationToken();
			await _service.GetPlaneTickets(cancellationToken, 1, 10);

			_repositoryMock.Verify(repo => repo.GetPlaneTickets(cancellationToken, 1, 10), Times.Once);
		}

		[Fact]
		public async Task GetPlaneTicket_Should_Call_Repository_GetPlaneTicket()
		{
			await _service.GetPlaneTicket(1);

			_repositoryMock.Verify(repo => repo.GetPlaneTicket(1), Times.Once);
		}

		[Fact]
		public async Task GetPlaneTicketsForPrice_Should_Call_Repository_GetPlaneTicketsForPrice()
		{
			var cancellationToken = new CancellationToken();
			await _service.GetPlaneTicketsForPrice(cancellationToken, 1, 10, 100, 200);

			_repositoryMock.Verify(repo => repo.GetPlaneTicketsForPrice(cancellationToken, 1, 10, 100, 200), Times.Once);
		}

		[Fact]
		public async Task PostPlaneTicket_Should_Call_Repository_PostPlaneTicket()
		{
			var pilot = new PlaneTicketEntity();

			await _service.PostPlaneTicket(pilot);

			_repositoryMock.Verify(repo => repo.PostPlaneTicket(pilot), Times.Once);
		}

		[Fact]
		public async Task PutPlaneTicket_Should_Call_Repository_PutPlaneTicket()
		{
			var pilot = new PlaneTicketEntity();

			await _service.PutPlaneTicket(pilot);

			_repositoryMock.Verify(repo => repo.PutPlaneTicket(pilot), Times.Once);
		}

		[Fact]
		public async Task PatchPlaneTicket_Should_Call_Repository_PatchPlaneTicket()
		{
			var pilotDocument = new JsonPatchDocument();

			await _service.PatchPlaneTicket(1, pilotDocument);

			_repositoryMock.Verify(repo => repo.PatchPlaneTicket(1, pilotDocument), Times.Once);
		}

		[Fact]
		public async Task DeletePlaneTicket_Should_Call_Repository_DeletePlaneTicket()
		{
			await _service.DeletePlaneTicket(1);

			_repositoryMock.Verify(repo => repo.DeletePlaneTicket(1), Times.Once);
		}

		[Fact]
		public async Task PlaneTicketExists_Should_Call_Repository_PlaneTicketExists()
		{
			var result = await _service.PlaneTicketExists(1);

			_repositoryMock.Verify(repo => repo.PlaneTicketExists(1), Times.Once);
			Assert.False(result);
		}

		[Fact]
		public async Task PlaneTicketClassesCount_ShouldReturnCorrectCount()
		{
			var cancellationToken = new CancellationToken();
			var expectedCount = 5;
			_repositoryMock.Setup(repo => repo.PlaneTicketsCount(cancellationToken, null, null)).ReturnsAsync(expectedCount);

			int count = await _service.PlaneTicketsCount(cancellationToken);

			Assert.Equal(expectedCount, count);
		}

	}
}
