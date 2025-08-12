using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IRepositories;
using AirportAutomation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AirportAutomation.Infrastructure.Repositories
{
	public class TravelClassRepository : ITravelClassRepository
	{
		protected readonly DatabaseContext _context;

		public TravelClassRepository(DatabaseContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<IList<TravelClassEntity>> GetTravelClasses(CancellationToken cancellationToken, int page, int pageSize)
		{
			return await _context.TravelClass
				.OrderBy(c => c.Id)
				.Skip(pageSize * (page - 1))
				.Take(pageSize)
				.AsNoTracking()
				.ToListAsync(cancellationToken);
		}

		public async Task<TravelClassEntity?> GetTravelClass(int id)
		{
			return await _context.TravelClass.FindAsync(id);
		}

		public async Task<bool> TravelClassExists(int id)
		{
			return (_context.TravelClass?.Any(e => e.Id == id)).GetValueOrDefault();
		}

		public async Task<int> TravelClassesCount(CancellationToken cancellationToken)
		{
			return await _context.TravelClass.CountAsync(cancellationToken);
		}

	}
}