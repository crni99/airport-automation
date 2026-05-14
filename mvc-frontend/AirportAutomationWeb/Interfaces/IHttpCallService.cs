using AirportAutomation.Web.Models.ApiUser;
using AirportAutomation.Web.Models.Export;
using AirportAutomation.Web.Models.Response;

namespace AirportAutomation.Web.Interfaces
{
	public interface IHttpCallService
	{
		Task<bool> Authenticate(UserViewModel user, CancellationToken cancellationToken = default);
		string GetToken();
		Task<bool> RemoveToken(CancellationToken cancellationToken = default);
		Task<PagedResponse<T>> GetDataList<T>(int page, int pageSize, CancellationToken cancellationToken = default);
		Task<PagedResponse<T>> GetDataList<T>(CancellationToken cancellationToken = default);
		Task<T> GetData<T>(int id, CancellationToken cancellationToken = default);
		Task<T> CreateData<T>(T t, CancellationToken cancellationToken = default);
		Task<bool> EditData<T>(T t, int id, CancellationToken cancellationToken = default);
		Task<bool> DeleteData<T>(int id, CancellationToken cancellationToken = default);
		Task<PagedResponse<T>> GetDataByName<T>(string name, int page, int pageSize, CancellationToken cancellationToken = default);
		Task<PagedResponse<T>> GetDataBetweenDates<T>(string? startDate, string? endDate, int page, int pageSize, CancellationToken cancellationToken = default);
		Task<PagedResponse<T>> GetDataByFilter<T>(object filter, int page, int pageSize, CancellationToken cancellationToken = default);
		Task<FileExportResult> DownloadFileAsync<T>(string fileType, object filter = null, int page = 1, int pageSize = 10, bool getAll = false, CancellationToken cancellationToken = default);
		Task<T> GetHealthCheck<T>(CancellationToken cancellationToken = default);
		string GetModelName<T>();
	}
}