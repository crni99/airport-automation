using AirportAutomation.Web.Interfaces;
using AirportAutomation.Web.Models.Response;
using System.Net;
using System.Web;

namespace AirportAutomation.Web.Services
{
	public class SearchHttpService : BaseHttpService, ISearchHttpService
	{
		public SearchHttpService(
			IHttpClientFactory httpClientFactory,
			IHttpContextAccessor httpContextAccessor,
			IConfiguration configuration,
			ILogger<SearchHttpService> logger)
			: base(httpClientFactory, httpContextAccessor, configuration, logger)
		{
		}

		public async Task<PagedResponse<T>> GetDataByName<T>(string name, int page, int pageSize, CancellationToken cancellationToken = default)
		{
			var modelName = GetModelName<T>();
			var requestUri = $"{_apiUrl}/{modelName}{GetPluralSuffix(modelName)}/search?name={name}&page={page}&pageSize={pageSize}";

			using var httpClient = _httpClientFactory.CreateClient("AirportAutomationApi");
			ConfigureHttpClient(httpClient);

			var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), cancellationToken);

			if (response.StatusCode == HttpStatusCode.OK)
				return await response.Content.ReadFromJsonAsync<PagedResponse<T>>().ConfigureAwait(false);
			else if (response.StatusCode == HttpStatusCode.NoContent)
			{
				_logger.LogInformation("Data not found. Status code: {StatusCode}", response.StatusCode);
				return new PagedResponse<T>(Enumerable.Empty<T>(), page, pageSize, 0);
			}
			_logger.LogInformation("Failed to retrieve data. Status code: {StatusCode}", response.StatusCode);
			return null;
		}

		public async Task<PagedResponse<T>> GetDataBetweenDates<T>(string? startDate, string? endDate, int page, int pageSize, CancellationToken cancellationToken = default)
		{
			var modelName = GetModelName<T>();
			var requestUri = $"{_apiUrl}/{modelName}{GetPluralSuffix(modelName)}/search?";

			UriBuilder uriBuilder = new(requestUri);
			var query = HttpUtility.ParseQueryString(uriBuilder.Query);

			if (!string.IsNullOrEmpty(startDate)) query["startDate"] = startDate;
			if (!string.IsNullOrEmpty(endDate)) query["endDate"] = endDate;
			query["page"] = page.ToString();
			query["pageSize"] = pageSize.ToString();
			uriBuilder.Query = query.ToString();

			using var httpClient = _httpClientFactory.CreateClient("AirportAutomationApi");
			ConfigureHttpClient(httpClient);

			var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, uriBuilder.ToString()), cancellationToken);

			if (response.StatusCode == HttpStatusCode.OK)
				return await response.Content.ReadFromJsonAsync<PagedResponse<T>>().ConfigureAwait(false);
			else if (response.StatusCode == HttpStatusCode.NoContent)
			{
				_logger.LogInformation("Data not found. Status code: {StatusCode}", response.StatusCode);
				return new PagedResponse<T>(Enumerable.Empty<T>(), page, pageSize, 0);
			}
			_logger.LogInformation("Failed to retrieve data. Status code: {StatusCode}", response.StatusCode);
			return null;
		}

		public async Task<PagedResponse<T>> GetDataByFilter<T>(object filter, int page, int pageSize, CancellationToken cancellationToken = default)
		{
			var modelName = GetModelName<T>();
			var requestUri = BuildRequestUriByModelName(modelName, filter, page, pageSize);

			using var httpClient = _httpClientFactory.CreateClient("AirportAutomationApi");
			ConfigureHttpClient(httpClient);

			var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), cancellationToken);

			if (response.StatusCode == HttpStatusCode.OK)
				return await response.Content.ReadFromJsonAsync<PagedResponse<T>>().ConfigureAwait(false);
			else if (response.StatusCode == HttpStatusCode.NoContent)
			{
				_logger.LogInformation("Data not found. Status code: {StatusCode}", response.StatusCode);
				return new PagedResponse<T>(Enumerable.Empty<T>(), page, pageSize, 0);
			}
			_logger.LogInformation("Failed to retrieve data. Status code: {StatusCode}", response.StatusCode);
			return null;
		}
	}
}