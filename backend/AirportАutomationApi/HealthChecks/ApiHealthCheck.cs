using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AirportАutomation.Api.HealthChecks
{
	/// <summary>
	/// Performs health checks on the API by checking the status of a specified endpoint.
	/// </summary>
	public class ApiHealthCheck : IHealthCheck
	{
		private readonly IHttpClientFactory _httpClientFactory;

		/// <summary>
		/// Initializes a new instance of the <see cref="ApiHealthCheck"/> class.
		/// </summary>
		/// <param name="httpClientFactory">The HTTP client factory for creating HTTP clients.</param>
		public ApiHealthCheck(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		/// <summary>
		/// Checks the health of the API asynchronously.
		/// </summary>
		/// <param name="context">The context for the health check.</param>
		/// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
		/// <returns>A task representing the asynchronous operation, with a <see cref="HealthCheckResult"/> as its result.</returns>
		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
		{
			using (var httpClient = _httpClientFactory.CreateClient())
			{
				var response = await
				httpClient.GetAsync("https://localhost:44362/swagger/index.html", cancellationToken);
				if (response.IsSuccessStatusCode)
				{
					return await Task.FromResult(new HealthCheckResult(
					  status: HealthStatus.Healthy,
					  description: "The API is up and running."));
				}
				return await Task.FromResult(new HealthCheckResult(
				  status: HealthStatus.Unhealthy,
				  description: "The API is down."));
			}
		}
	}
}