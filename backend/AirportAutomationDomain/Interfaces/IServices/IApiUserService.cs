using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Filters;

namespace AirportAutomation.Core.Interfaces.IServices
{
	public interface IApiUserService
	{
		Task<IList<ApiUserEntity>> GetApiUsers(CancellationToken cancellationToken, int page, int pageSize);
		Task<ApiUserEntity?> GetApiUser(int id);
		Task<IList<ApiUserEntity?>> SearchApiUsers(CancellationToken cancellationToken, int page, int pageSize, ApiUserSearchFilter filter);
		Task PutApiUser(ApiUserEntity apiUser);
		Task<bool> DeleteApiUser(int id);
		Task<bool> ApiUserExists(int id);
		Task<int> ApiUsersCount(CancellationToken cancellationToken, string? role = null);
		Task<int> ApiUsersCountFilter(CancellationToken cancellationToken, ApiUserSearchFilter filter);
	}
}
