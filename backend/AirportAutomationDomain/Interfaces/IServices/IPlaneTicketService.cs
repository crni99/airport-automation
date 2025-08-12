using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Filters;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomation.Core.Interfaces.IServices
{
	public interface IPlaneTicketService
	{
		Task<IList<PlaneTicketEntity>> GetAllPlaneTickets(CancellationToken cancellationToken);
		Task<IList<PlaneTicketEntity>> GetPlaneTickets(CancellationToken cancellationToken, int page, int pageSize);
		Task<PlaneTicketEntity?> GetPlaneTicket(int id);
		Task<IList<PlaneTicketEntity?>> GetPlaneTicketsForPrice(CancellationToken cancellationToken, int page, int pageSize, int? minPrice, int? maxPrice);
		Task<IList<PlaneTicketEntity?>> GetPlaneTicketsByFilter(CancellationToken cancellationToken, int page, int pageSize, PlaneTicketSearchFilter filter);
		Task<PlaneTicketEntity> PostPlaneTicket(PlaneTicketEntity planeTicket);
		Task PutPlaneTicket(PlaneTicketEntity planeTicket);
		Task<PlaneTicketEntity> PatchPlaneTicket(int id, JsonPatchDocument planeTicketDocument);
		Task<bool> DeletePlaneTicket(int id);
		Task<bool> PlaneTicketExists(int id);
		Task<int> PlaneTicketsCount(CancellationToken cancellationToken, int? minPrice = null, int? maxPrice = null);
		Task<int> PlaneTicketsCountFilter(CancellationToken cancellationToken, PlaneTicketSearchFilter filter);
	}
}
