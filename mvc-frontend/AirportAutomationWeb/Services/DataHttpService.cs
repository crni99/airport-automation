using AirportAutomation.Web.Interfaces;
using AirportAutomation.Web.Models.Response;
using System.Net;

namespace AirportAutomation.Web.Services
{
	public class DataHttpService : BaseHttpService, IDataHttpService
	{
		public DataHttpService(
			IHttpClientFactory httpClientFactory,
			IHttpContextAccessor httpContextAccessor,
			IConfiguration configuration,
			ILogger<DataHttpService> logger)
			: base(httpClientFactory, httpContextAccessor, configuration, logger)
		{
		}

		public async Task<PagedResponse<T>> GetDataList<T>(int page, int pageSize, CancellationToken cancellationToken = default)
		{
			var modelName = GetModelName<T>();
			var requestUri = $"{_apiUrl}/{modelName}{GetPluralSuffix(modelName)}/?page={page}&pageSize={pageSize}";

			using var httpClient = _httpClientFactory.CreateClient("AirportAutomationApi");
			ConfigureHttpClient(httpClient);

			var response = await httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);

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

		public async Task<PagedResponse<T>> GetDataList<T>(CancellationToken cancellationToken = default)
		{
			var modelName = GetModelName<T>();
			var requestUri = $"{_apiUrl}/{modelName}{GetPluralSuffix(modelName)}";

			using var httpClient = _httpClientFactory.CreateClient("AirportAutomationApi");
			ConfigureHttpClient(httpClient);

			var response = await httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);

			if (response.IsSuccessStatusCode)
				return await response.Content.ReadFromJsonAsync<PagedResponse<T>>().ConfigureAwait(false);

			_logger.LogInformation("Failed to retrieve data. Status code: {StatusCode}", response.StatusCode);
			return null;
		}

		public async Task<T> GetData<T>(int id, CancellationToken cancellationToken = default)
		{
			var modelName = GetModelName<T>();
			var requestUri = $"{_apiUrl}/{modelName}{GetPluralSuffix(modelName)}/{id}";

			using var httpClient = _httpClientFactory.CreateClient("AirportAutomationApi");
			ConfigureHttpClient(httpClient);

			var response = await httpClient.SendAsync(
				new HttpRequestMessage(HttpMethod.Get, requestUri), cancellationToken);

			if (response.IsSuccessStatusCode)
				return await response.Content.ReadFromJsonAsync<T>().ConfigureAwait(false);

			_logger.LogInformation("Failed to retrieve data. Status code: {StatusCode}", response.StatusCode);
			return default;
		}

		public async Task<T> CreateData<T>(T t, CancellationToken cancellationToken = default)
		{
			var modelName = GetModelName<T>();
			var requestUri = $"{_apiUrl}/{modelName}{GetPluralSuffix(modelName)}";

			using var httpClient = _httpClientFactory.CreateClient("AirportAutomationApi");
			ConfigureHttpClient(httpClient);

			var response = await httpClient.PostAsJsonAsync(requestUri, t, cancellationToken).ConfigureAwait(false);

			if (response.IsSuccessStatusCode)
				return await response.Content.ReadFromJsonAsync<T>().ConfigureAwait(false);

			_logger.LogInformation("Failed to create data. Status code: {StatusCode}", response.StatusCode);
			return default;
		}

		public async Task<bool> EditData<T>(T t, int id, CancellationToken cancellationToken = default)
		{
			var modelName = GetModelName<T>();
			var requestUri = $"{_apiUrl}/{modelName}{GetPluralSuffix(modelName)}/{id}";

			using var httpClient = _httpClientFactory.CreateClient("AirportAutomationApi");
			ConfigureHttpClient(httpClient);

			var response = await httpClient.PutAsJsonAsync(requestUri, t, cancellationToken);

			if (response.StatusCode is HttpStatusCode.NoContent) return true;

			_logger.LogInformation("Failed to edit data. Status code: {StatusCode}", response.StatusCode);
			return false;
		}

		public async Task<bool> DeleteData<T>(int id, CancellationToken cancellationToken = default)
		{
			var modelName = GetModelName<T>();
			var requestUri = $"{_apiUrl}/{modelName}{GetPluralSuffix(modelName)}/{id}";

			using var httpClient = _httpClientFactory.CreateClient("AirportAutomationApi");
			ConfigureHttpClient(httpClient);

			var response = await httpClient.DeleteAsync(requestUri, cancellationToken);

			if (response.StatusCode is HttpStatusCode.NoContent) return true;
			if (response.StatusCode is HttpStatusCode.Conflict) return false;

			_logger.LogInformation("Failed to delete data. Status code: {StatusCode}", response.StatusCode);
			return false;
		}
	}
}