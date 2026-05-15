using AirportAutomation.Web.Models.Response;

namespace AirportAutomation.Web.Interfaces
{
	public interface ISearchHttpService
	{
		Task<PagedResponse<T>> GetDataByName<T>(string name, int page, int pageSize, CancellationToken cancellationToken = default);
		Task<PagedResponse<T>> GetDataBetweenDates<T>(string? startDate, string? endDate, int page, int pageSize, CancellationToken cancellationToken = default);
		Task<PagedResponse<T>> GetDataByFilter<T>(object filter, int page, int pageSize, CancellationToken cancellationToken = default);
	}
}