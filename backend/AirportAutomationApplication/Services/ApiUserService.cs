using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Filters;
using AirportAutomation.Core.Interfaces.IRepositories;
using AirportAutomation.Core.Interfaces.IServices;

namespace AirportAutomation.Application.Services
{
	public class ApiUserService : IApiUserService
	{

		private readonly IApiUserRepository _apiUserManagementRepository;

		public ApiUserService(IApiUserRepository apiUserManagementRepository)
		{
			_apiUserManagementRepository = apiUserManagementRepository;
		}

		public async Task<IList<ApiUserEntity>> GetApiUsers(CancellationToken cancellationToken, int page, int pageSize)
		{
			return await _apiUserManagementRepository.GetApiUsers(cancellationToken, page, pageSize);
		}

		public async Task<ApiUserEntity?> GetApiUser(int id)
		{
			return await _apiUserManagementRepository.GetApiUser(id);
		}

		public async Task<IList<ApiUserEntity?>> GetApiUsersByRole(CancellationToken cancellationToken, int page, int pageSize, string role)
		{
			return await _apiUserManagementRepository.GetApiUsersByRole(cancellationToken, page, pageSize, role);
		}

		public async Task<IList<ApiUserEntity?>> GetApiUsersByFilter(CancellationToken cancellationToken, int page, int pageSize, ApiUserSearchFilter filter)
		{
			return await _apiUserManagementRepository.GetApiUsersByFilter(cancellationToken, page, pageSize, filter);
		}

		public async Task PutApiUser(ApiUserEntity apiUser)
		{
			await _apiUserManagementRepository.PutApiUser(apiUser);
		}

		public async Task<bool> DeleteApiUser(int id)
		{
			return await _apiUserManagementRepository.DeleteApiUser(id);
		}

		public async Task<bool> ApiUserExists(int id)
		{
			return await _apiUserManagementRepository.ApiUserExists(id);
		}

		public async Task<int> ApiUsersCount(CancellationToken cancellationToken, string? role = null)
		{
			return await _apiUserManagementRepository.ApiUsersCount(cancellationToken, role);
		}

		public async Task<int> ApiUsersCountFilter(CancellationToken cancellationToken, ApiUserSearchFilter filter)
		{
			return await _apiUserManagementRepository.ApiUsersCountFilter(cancellationToken, filter);
		}
	}
}
