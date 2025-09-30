using AirportAutomation.Api.Interfaces;
using AirportAutomation.Core.Entities;
using AirportAutomation.Infrastructure.Data;

namespace AirportAutomation.Api.Repositories
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
