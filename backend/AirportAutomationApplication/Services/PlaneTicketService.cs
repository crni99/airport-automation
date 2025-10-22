using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Filters;
using AirportAutomation.Core.Interfaces.IRepositories;
using AirportAutomation.Core.Interfaces.IServices;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomation.Application.Services
{
	public class PlaneTicketService : IPlaneTicketService
	{
		private readonly IPlaneTicketRepository _planeTicketRepository;

		public PlaneTicketService(IPlaneTicketRepository planeTicketRepository)
		{
			_planeTicketRepository = planeTicketRepository;
		}

		public async Task<IList<PlaneTicketEntity>> GetAllPlaneTickets(CancellationToken cancellationToken)
		{
			return await _planeTicketRepository.GetAllPlaneTickets(cancellationToken);
		}

		public async Task<IList<PlaneTicketEntity>> GetPlaneTickets(CancellationToken cancellationToken, int page, int pageSize)
		{
			return await _planeTicketRepository.GetPlaneTickets(cancellationToken, page, pageSize);
		}

		public async Task<PlaneTicketEntity?> GetPlaneTicket(int id)
		{
			return await _planeTicketRepository.GetPlaneTicket(id);
		}

		public async Task<IList<PlaneTicketEntity?>> SearchPlaneTickets(CancellationToken cancellationToken, int page, int pageSize, PlaneTicketSearchFilter filter)
		{
			return await _planeTicketRepository.SearchPlaneTickets(cancellationToken, page, pageSize, filter);
		}

		public async Task<PlaneTicketEntity> PostPlaneTicket(PlaneTicketEntity planeTicket)
		{
			return await _planeTicketRepository.PostPlaneTicket(planeTicket);
		}

		public async Task PutPlaneTicket(PlaneTicketEntity planeTicket)
		{
			await _planeTicketRepository.PutPlaneTicket(planeTicket);
		}

		public async Task<PlaneTicketEntity> PatchPlaneTicket(int id, JsonPatchDocument planeTicketDocument)
		{
			return await _planeTicketRepository.PatchPlaneTicket(id, planeTicketDocument);
		}

		public async Task<bool> DeletePlaneTicket(int id)
		{
			return await _planeTicketRepository.DeletePlaneTicket(id);
		}

		public async Task<bool> PlaneTicketExists(int id)
		{
			return await _planeTicketRepository.PlaneTicketExists(id);
		}

		public async Task<int> PlaneTicketsCount(CancellationToken cancellationToken, int? minPrice = null, int? maxPrice = null)
		{
			return await _planeTicketRepository.PlaneTicketsCount(cancellationToken, minPrice, maxPrice);
		}

		public async Task<int> PlaneTicketsCountFilter(CancellationToken cancellationToken, PlaneTicketSearchFilter filter)
		{
			return await _planeTicketRepository.PlaneTicketsCountFilter(cancellationToken, filter);
		}

	}
}
