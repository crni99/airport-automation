﻿using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Filters;
using AirportAutomation.Core.Interfaces.IRepositories;
using AirportAutomation.Infrastructure.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace AirportAutomation.Infrastructure.Repositories
{
	public class PassengerRepository : IPassengerRepository
	{
		protected readonly DatabaseContext _context;

		public PassengerRepository(DatabaseContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<IList<PassengerEntity>> GetAllPassengers(CancellationToken cancellationToken)
		{
			return await _context.Passenger
				.OrderBy(c => c.Id)
				.AsNoTracking()
				.ToListAsync(cancellationToken);
		}

		public async Task<IList<PassengerEntity>> GetPassengers(CancellationToken cancellationToken, int page, int pageSize)
		{
			return await _context.Passenger
				.OrderBy(c => c.Id)
				.Skip(pageSize * (page - 1))
				.Take(pageSize)
				.AsNoTracking()
				.ToListAsync(cancellationToken);
		}

		public async Task<PassengerEntity?> GetPassenger(int id)
		{
			return await _context.Passenger.FindAsync(id);
		}

		public async Task<IList<PassengerEntity?>> GetPassengersByName(CancellationToken cancellationToken, int page, int pageSize, string firstName = null, string lastName = null)
		{
			IQueryable<PassengerEntity> query = _context.Passenger.AsNoTracking();

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

		public async Task<IList<PassengerEntity?>> GetPassengersByFilter(
			CancellationToken cancellationToken,
			int page,
			int pageSize,
			PassengerSearchFilter filter)
		{
			IQueryable<PassengerEntity> query = _context.Passenger.AsNoTracking();

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
			if (!string.IsNullOrWhiteSpace(filter.Passport))
			{
				query = query.Where(p => p.Passport.Contains(filter.Passport));
			}
			if (!string.IsNullOrWhiteSpace(filter.Address))
			{
				query = query.Where(p => p.Address.Contains(filter.Address));
			}
			if (!string.IsNullOrWhiteSpace(filter.Phone))
			{
				query = query.Where(p => p.Phone.Contains(filter.Phone));
			}
			return await query.OrderBy(p => p.Id)
				.Skip(pageSize * (page - 1))
				.Take(pageSize)
				.ToListAsync(cancellationToken);
		}


		public async Task<PassengerEntity> PostPassenger(PassengerEntity passenger)
		{
			_context.Passenger.Add(passenger);
			await _context.SaveChangesAsync();
			return passenger;
		}

		public async Task PutPassenger(PassengerEntity passenger)
		{
			_context.Entry(passenger).State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}

		public async Task<PassengerEntity> PatchPassenger(int id, JsonPatchDocument passengerDocument)
		{
			var passenger = await GetPassenger(id);
			passengerDocument.ApplyTo(passenger);
			await _context.SaveChangesAsync();
			return passenger;
		}

		public async Task<bool> DeletePassenger(int id)
		{
			bool hasRelatedPlaneTickets = await _context.PlaneTicket.AnyAsync(pt => pt.PassengerId == id);
			if (hasRelatedPlaneTickets)
			{
				return false;
			}
			var passenger = await GetPassenger(id);
			_context.Passenger.Remove(passenger);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> PassengerExists(int id)
		{
			return (_context.Passenger?.Any(e => e.Id == id)).GetValueOrDefault();
		}

		public async Task<int> PassengersCount(CancellationToken cancellationToken, string firstName = null, string lastName = null)
		{
			IQueryable<PassengerEntity> query = _context.Passenger;

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

		public async Task<int> PassengersCountFilter(CancellationToken cancellationToken, PassengerSearchFilter filter)
		{
			IQueryable<PassengerEntity> query = _context.Passenger.AsNoTracking();

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
			if (!string.IsNullOrWhiteSpace(filter.Passport))
			{
				query = query.Where(p => p.Passport.Contains(filter.Passport));
			}
			if (!string.IsNullOrWhiteSpace(filter.Address))
			{
				query = query.Where(p => p.Address.Contains(filter.Address));
			}
			if (!string.IsNullOrWhiteSpace(filter.Phone))
			{
				query = query.Where(p => p.Phone.Contains(filter.Phone));
			}
			return await query.CountAsync(cancellationToken);
		}

	}
}
