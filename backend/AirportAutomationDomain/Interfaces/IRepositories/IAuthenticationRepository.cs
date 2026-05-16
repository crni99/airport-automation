using AirportAutomation.Core.Entities;

namespace AirportAutomation.Core.Interfaces.IRepositories
{
	public interface IAuthenticationRepository
	{
		public Task<ApiUserEntity?> GetUserByUsername(string username);

	}
}
