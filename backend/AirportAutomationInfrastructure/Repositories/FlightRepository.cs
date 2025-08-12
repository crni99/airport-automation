using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IRepositories;
using AirportAutomation.Infrastructure.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace AirportAutomation.Infrastructure.Repositories
{
	public class FlightRepository : IFlightRepository
	{
		protected readonly DatabaseContext _context;

		public FlightRepository(DatabaseContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<IList<FlightEntity>> GetAllFlights(CancellationToken cancellationToken)
		{
			var collection = _context.Flight
				.Include(l => l.Airline)
				.Include(l => l.Destination)
				.Include(l => l.Pilot)
				.AsNoTracking();

			return await collection
				.OrderBy(c => c.Id)
				.ToListAsync(cancellationToken);
		}

		public async Task<IList<FlightEntity>> GetFlights(CancellationToken cancellationToken, int page, int pageSize)
		{
			var collection = _context.Flight
				.Include(l => l.Airline)
				.Include(l => l.Destination)
				.Include(l => l.Pilot)
				.AsNoTracking();

			return await collection
				.OrderBy(c => c.Id)
				.Skip(pageSize * (page - 1))
				.Take(pageSize)
				.ToListAsync(cancellationToken);
		}

		public async Task<FlightEntity?> GetFlight(int id)
		{
			return await _context.Flight
				.Include(l => l.Airline)
				.Include(l => l.Destination)
				.Include(l => l.Pilot)
				.AsNoTracking()
				.FirstOrDefaultAsync(l => l.Id == id);
		}

		public async Task<IList<FlightEntity?>> GetFlightsBetweenDates(CancellationToken cancellationToken, int page, int pageSize, DateOnly? startDate, DateOnly? endDate)
		{
			IQueryable<FlightEntity> query = _context.Flight
				.Include(f => f.Airline)
				.Include(f => f.Destination)
				.Include(f => f.Pilot)
				.AsNoTracking();

			if (startDate.HasValue)
			{
				query = query.Where(f => f.DepartureDate >= startDate);
			}
			if (endDate.HasValue)
			{
				query = query.Where(f => f.DepartureDate <= endDate);
			}
			return await query.OrderBy(c => c.Id)
					 .Skip(pageSize * (page - 1))
					 .Take(pageSize)
					 .ToListAsync(cancellationToken)
					 .ConfigureAwait(false);
		}

		public async Task<FlightEntity> PostFlight(FlightEntity flight)
		{
			_context.Flight.Add(flight);
			await _context.SaveChangesAsync();
			return flight;
		}

		public async Task PutFlight(FlightEntity flight)
		{
			_context.Entry(flight).State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}

		public async Task<FlightEntity> PatchFlight(int id, JsonPatchDocument flightDocument)
		{
			var flight = await GetFlight(id);
			flightDocument.ApplyTo(flight);
			await _context.SaveChangesAsync();
			return flight;
		}

		public async Task<bool> DeleteFlight(int id)
		{
			bool hasRelatedPlaneTickets = await _context.PlaneTicket.AnyAsync(pt => pt.FlightId == id);
			if (hasRelatedPlaneTickets)
			{
				return false;
			}
			var flight = await GetFlight(id);
			_context.Flight.Remove(flight);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> FlightExists(int id)
		{
			return await _context.Flight.AsNoTracking().AnyAsync(e => e.Id == id);
		}

		public async Task<int> FlightsCount(CancellationToken cancellationToken, DateOnly? startDate = null, DateOnly? endDate = null)
		{
			IQueryable<FlightEntity> query = _context.Flight.AsNoTracking();

			if (startDate.HasValue)
			{
				query = query.Where(f => f.DepartureDate >= startDate);
			}

			if (endDate.HasValue)
			{
				query = query.Where(f => f.DepartureDate <= endDate);
			}
			return await query.CountAsync(cancellationToken).ConfigureAwait(false);
		}

	}
}