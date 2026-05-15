using AirportAutomation.Web.Models.ApiUser;

namespace AirportAutomation.Web.Interfaces
{
	public interface IAuthHttpService
	{
		Task<bool> Authenticate(UserViewModel user, CancellationToken cancellationToken = default);
		string GetToken();
		Task<bool> RemoveToken(CancellationToken cancellationToken = default);
	}
}