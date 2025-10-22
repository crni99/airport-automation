using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Filters;
using AirportAutomation.Core.Interfaces.IRepositories;
using AirportAutomation.Infrastructure.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace AirportAutomation.Infrastructure.Repositories
{
	public class PlaneTicketRepository : IPlaneTicketRepository
	{
		protected readonly DatabaseContext _context;

		public PlaneTicketRepository(DatabaseContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<IList<PlaneTicketEntity>> GetAllPlaneTickets(CancellationToken cancellationToken)
		{
			var collection = _context.PlaneTicket
				.Include(k => k.Passenger)
				.Include(k => k.TravelClass)
				.Include(k => k.Flight)
				.AsNoTracking();

			return await collection
				.OrderBy(c => c.Id)
				.ToListAsync(cancellationToken);
		}

		public async Task<IList<PlaneTicketEntity>> GetPlaneTickets(CancellationToken cancellationToken, int page, int pageSize)
		{
			var collection = _context.PlaneTicket
				.Include(k => k.Passenger)
				.Include(k => k.TravelClass)
				.Include(k => k.Flight)
				.AsNoTracking();

			return await collection
				.OrderBy(c => c.Id)
				.Skip(pageSize * (page - 1))
				.Take(pageSize)
				.ToListAsync(cancellationToken);
		}

		public async Task<PlaneTicketEntity?> GetPlaneTicket(int id)
		{
			return await _context.PlaneTicket
				.Include(k => k.Passenger)
				.Include(k => k.TravelClass)
				.Include(k => k.Flight)
				.AsNoTracking()
				.FirstOrDefaultAsync(k => k.Id == id);
		}

		public async Task<IList<PlaneTicketEntity?>> SearchPlaneTickets(
			CancellationToken cancellationToken,
			int page,
			int pageSize,
			PlaneTicketSearchFilter filter)
		{
			IQueryable<PlaneTicketEntity> query = _context.PlaneTicket
				.Include(k => k.Passenger)
				.Include(k => k.TravelClass)
				.Include(k => k.Flight)
				.AsNoTracking();

			if (filter.Price.HasValue)
			{
				query = query.Where(t => t.Price == filter.Price.Value);
			}
			if (filter.PurchaseDate.HasValue)
			{
				query = query.Where(t => t.PurchaseDate == filter.PurchaseDate.Value);
			}
			if (filter.SeatNumber.HasValue)
			{
				query = query.Where(t => t.SeatNumber == filter.SeatNumber.Value);
			}
			return await query.OrderBy(t => t.Id)
				.Skip(pageSize * (page - 1))
				.Take(pageSize)
				.ToListAsync(cancellationToken);
		}

		public async Task<PlaneTicketEntity> PostPlaneTicket(PlaneTicketEntity planeTicket)
		{
			_context.PlaneTicket.Add(planeTicket);
			await _context.SaveChangesAsync();
			return planeTicket;
		}

		public async Task PutPlaneTicket(PlaneTicketEntity planeTicket)
		{
			_context.Entry(planeTicket).State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}

		public async Task<PlaneTicketEntity> PatchPlaneTicket(int id, JsonPatchDocument planeTicketDocument)
		{
			var planeTicket = await GetPlaneTicket(id);
			planeTicketDocument.ApplyTo(planeTicket);
			await _context.SaveChangesAsync();
			return planeTicket;
		}

		public async Task<bool> DeletePlaneTicket(int id)
		{
			var planeTicket = await GetPlaneTicket(id);
			_context.PlaneTicket.Remove(planeTicket);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> PlaneTicketExists(int id)
		{
			return (_context.PlaneTicket?.Any(e => e.Id == id)).GetValueOrDefault();
		}

		public async Task<int> PlaneTicketsCount(CancellationToken cancellationToken, int? minPrice, int? maxPrice)
		{
			IQueryable<PlaneTicketEntity> query = _context.PlaneTicket;

			if (minPrice.HasValue)
			{
				query = query.Where(p => p.Price >= minPrice.Value);
			}
			if (maxPrice.HasValue)
			{
				query = query.Where(p => p.Price <= maxPrice.Value);
			}
			return await query.CountAsync(cancellationToken).ConfigureAwait(false);
		}

		public async Task<int> PlaneTicketsCountFilter(
			CancellationToken cancellationToken,
			PlaneTicketSearchFilter filter)
		{
			IQueryable<PlaneTicketEntity> query = _context.PlaneTicket
				.AsNoTracking();

			if (filter.Price.HasValue)
			{
				query = query.Where(t => t.Price == filter.Price.Value);
			}
			if (filter.PurchaseDate.HasValue)
			{
				query = query.Where(t => t.PurchaseDate == filter.PurchaseDate.Value);
			}
			if (filter.SeatNumber.HasValue)
			{
				query = query.Where(t => t.SeatNumber == filter.SeatNumber.Value);
			}
			return await query.CountAsync(cancellationToken);
		}

	}
}