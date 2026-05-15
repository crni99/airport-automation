using AirportAutomation.Web.Models.Response;

namespace AirportAutomation.Web.Interfaces
{
	public interface IDataHttpService
	{
		Task<PagedResponse<T>> GetDataList<T>(int page, int pageSize, CancellationToken cancellationToken = default);
		Task<PagedResponse<T>> GetDataList<T>(CancellationToken cancellationToken = default);
		Task<T> GetData<T>(int id, CancellationToken cancellationToken = default);
		Task<T> CreateData<T>(T t, CancellationToken cancellationToken = default);
		Task<bool> EditData<T>(T t, int id, CancellationToken cancellationToken = default);
		Task<bool> DeleteData<T>(int id, CancellationToken cancellationToken = default);
	}
}