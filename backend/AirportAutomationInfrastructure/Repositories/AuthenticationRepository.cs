using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IRepositories;
using AirportAutomation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AirportAutomation.Infrastructure.Repositories
{
	public class AuthenticationRepository : IDisposable, IAuthenticationRepository
	{
		protected readonly DatabaseContext _context;

		public AuthenticationRepository(DatabaseContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<ApiUserEntity?> GetUserByUsername(string username)
		{
			return await _context.ApiUser.AsNoTracking().FirstOrDefaultAsync(user => user.UserName == username);
		}

		public void Dispose()
		{
			_context.Dispose();
		}

	}
}
