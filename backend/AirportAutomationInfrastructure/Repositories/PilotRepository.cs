using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Filters;
using AirportAutomation.Core.Interfaces.IRepositories;
using AirportAutomation.Infrastructure.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace AirportAutomation.Infrastructure.Repositories
{
	public class PilotRepository : IPilotRepository
	{
		protected readonly DatabaseContext _context;

		public PilotRepository(DatabaseContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<IList<PilotEntity>> GetAllPilots(CancellationToken cancellationToken)
		{
			return await _context.Pilot
				.OrderBy(c => c.Id)
				.AsNoTracking()
				.ToListAsync(cancellationToken);
		}

		public async Task<IList<PilotEntity>> GetPilots(CancellationToken cancellationToken, int page, int pageSize)
		{
			return await _context.Pilot
				.OrderBy(c => c.Id)
				.Skip(pageSize * (page - 1))
				.Take(pageSize)
				.AsNoTracking()
				.ToListAsync(cancellationToken);
		}

		public async Task<PilotEntity?> GetPilot(int id)
		{
			return await _context.Pilot.FindAsync(id);
		}

		public async Task<IList<PilotEntity?>> GetPilotsByName(CancellationToken cancellationToken, int page, int pageSize, string firstName = null, string lastName = null)
		{
			IQueryable<PilotEntity> query = _context.Pilot.AsNoTracking();

			if (!string.IsNullOrEmpty(firstName))
			{
				query = query.Where(p => p.FirstName.Contains(firstName));
			}
			if (!string.IsNullOrEmpty(lastName))
			{
				query = query.Where(p => p.LastName.Contains(lastName));
			}

			return await query.OrderBy(c => c.Id)
								.Skip(pageSize * (page - 1))
								.Take(pageSize)
								.ToListAsync(cancellationToken)
								.ConfigureAwait(false);
		}

		public async Task<IList<PilotEntity?>> GetPilotsByFilter(
			CancellationToken cancellationToken,
			int page,
			int pageSize,
			PilotSearchFilter filter)
		{
			IQueryable<PilotEntity> query = _context.Pilot.AsNoTracking();

			if (!string.IsNullOrWhiteSpace(filter.FirstName))
			{
				query = query.Where(p => p.FirstName.Contains(filter.FirstName));
			}
			if (!string.IsNullOrWhiteSpace(filter.LastName))
			{
				query = query.Where(p => p.LastName.Contains(filter.LastName));
			}
			if (!string.IsNullOrWhiteSpace(filter.UPRN))
			{
				query = query.Where(p => p.UPRN.Contains(filter.UPRN));
			}
			if (filter.FlyingHours.HasValue)
			{
				query = query.Where(p => p.FlyingHours >= filter.FlyingHours);
			}
			return await query.OrderBy(p => p.Id)
				.Skip(pageSize * (page - 1))
				.Take(pageSize)
				.ToListAsync(cancellationToken);
		}

		public async Task<PilotEntity> PostPilot(PilotEntity pilot)
		{
			_context.Pilot.Add(pilot);
			await _context.SaveChangesAsync();
			return pilot;
		}

		public async Task PutPilot(PilotEntity pilot)
		{
			_context.Entry(pilot).State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}

		public async Task<PilotEntity> PatchPilot(int id, JsonPatchDocument passengerDocument)
		{
			var pilot = await GetPilot(id);
			passengerDocument.ApplyTo(pilot);
			await _context.SaveChangesAsync();
			return pilot;
		}

		public async Task<bool> DeletePilot(int id)
		{
			bool hasRelatedFlights = await _context.Flight.AnyAsync(pt => pt.PilotId == id);
			if (hasRelatedFlights)
			{
				return false;
			}
			var pilot = await GetPilot(id);
			_context.Pilot.Remove(pilot);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> PilotExists(int id)
		{
			return (_context.Pilot?.Any(e => e.Id == id)).GetValueOrDefault();
		}

		public async Task<int> PilotsCount(CancellationToken cancellationToken, string firstName = null, string lastName = null)
		{
			IQueryable<PilotEntity> query = _context.Pilot;

			if (!string.IsNullOrEmpty(firstName))
			{
				query = query.Where(p => p.FirstName.Contains(firstName));
			}
			if (!string.IsNullOrEmpty(lastName))
			{
				query = query.Where(p => p.LastName.Contains(lastName));
			}
			return await query.CountAsync(cancellationToken).ConfigureAwait(false);
		}

		public async Task<int> PilotsCountFilter(
			CancellationToken cancellationToken,
			PilotSearchFilter filter)
		{
			IQueryable<PilotEntity> query = _context.Pilot.AsNoTracking();

			if (!string.IsNullOrWhiteSpace(filter.FirstName))
			{
				query = query.Where(p => p.FirstName.Contains(filter.FirstName));
			}
			if (!string.IsNullOrWhiteSpace(filter.LastName))
			{
				query = query.Where(p => p.LastName.Contains(filter.LastName));
			}
			if (!string.IsNullOrWhiteSpace(filter.UPRN))
			{
				query = query.Where(p => p.UPRN.Contains(filter.UPRN));
			}
			if (filter.FlyingHours.HasValue)
			{
				query = query.Where(p => p.FlyingHours >= filter.FlyingHours.Value);
			}
			return await query.CountAsync(cancellationToken);
		}

	}
}