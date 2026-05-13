using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IRepositories;
using AirportAutomation.Infrastructure.Data;

namespace AirportAutomation.Infrastructure.Repositories
{
	public class AuthenticationRepository : IDisposable, IAuthenticationRepository
	{
		protected readonly DatabaseContext _context;

		public AuthenticationRepository(DatabaseContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public ApiUserEntity GetUserByUsername(string username)
		{
			return _context.ApiUser.FirstOrDefault(user => user.UserName.Equals(username));
		}

		public void Dispose()
		{
			_context.Dispose();
		}

	}
}
