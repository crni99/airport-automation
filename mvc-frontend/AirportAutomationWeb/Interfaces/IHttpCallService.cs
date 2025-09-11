using AirportAutomation.Web.Models.ApiUser;
using AirportAutomation.Web.Models.Export;
using AirportAutomation.Web.Models.Response;

namespace AirportAutomation.Web.Interfaces
{
	public interface IHttpCallService
	{
		Task<bool> Authenticate(UserViewModel user);
		string GetToken();
		bool RemoveToken();
		Task<PagedResponse<T>> GetDataList<T>(int page, int pageSize);
		Task<PagedResponse<T>> GetDataList<T>();
		Task<T> GetData<T>(int id);
		Task<T> CreateData<T>(T t);
		Task<bool> EditData<T>(T t, int id);
		Task<bool> DeleteData<T>(int id);
		Task<PagedResponse<T>> GetDataByName<T>(string name, int page, int pageSize);
		Task<PagedResponse<T>> GetDataByFNameOrLName<T>(string? firstName, string? lastName, int page, int pageSize);
		Task<PagedResponse<T>> GetDataForPrice<T>(int? minPrice, int? maxPrice, int page, int pageSize);
		Task<PagedResponse<T>> GetDataBetweenDates<T>(string? startDate, string? endDate, int page, int pageSize);
		Task<PagedResponse<T>> GetDataByCityOrAirport<T>(string? city, string? airport, int page, int pageSize);
		Task<PagedResponse<T>> GetDataByRole<T>(string role, int page, int pageSize);
		Task<PagedResponse<T>> GetDataByFilter<T>(object filter, int page, int pageSize);
		Task<FileExportResult> DownloadFileAsync<T>(string fileType, object filter = null, int page = 1, int pageSize = 10, bool getAll = false);
		Task<T> GetHealthCheck<T>();
		string GetModelName<T>();
	}
}