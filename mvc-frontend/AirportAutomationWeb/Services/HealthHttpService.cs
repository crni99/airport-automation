using AirportAutomation.Web.Interfaces;

namespace AirportAutomation.Web.Services
{
	public class HealthHttpService : BaseHttpService, IHealthHttpService
	{
		public HealthHttpService(
			IHttpClientFactory httpClientFactory,
			IHttpContextAccessor httpContextAccessor,
			IConfiguration configuration,
			ILogger<HealthHttpService> logger)
			: base(httpClientFactory, httpContextAccessor, configuration, logger)
		{
		}

		public async Task<T> GetHealthCheck<T>(CancellationToken cancellationToken = default)
		{
			var modelName = GetModelName<T>();
			var requestUri = $"{_apiUrl}/{modelName}";

			using var httpClient = _httpClientFactory.CreateClient("AirportAutomationApi");
			ConfigureHttpClient(httpClient);

			var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), cancellationToken);

			if (response.IsSuccessStatusCode)
				return await response.Content.ReadFromJsonAsync<T>().ConfigureAwait(false);

			_logger.LogInformation("Failed to retrieve data. Status code: {StatusCode}", response.StatusCode);
			return default;
		}
	}
}