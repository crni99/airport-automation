using AirportAutomation.Core.Entities;

namespace AirportAutomation.Core.Interfaces.IRepositories
{
	public interface IAuthenticationRepository
	{
		public ApiUserEntity GetUserByUsername(string username);

	}
}
