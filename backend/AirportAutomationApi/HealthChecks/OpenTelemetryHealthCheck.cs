using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Http.Headers;

namespace AirportAutomation.Api.HealthChecks
{
	public class OpenTelemetryHealthCheck : IHealthCheck
	{
		private readonly IConfiguration _configuration;
		private readonly IHttpClientFactory _httpClientFactory;

		public OpenTelemetryHealthCheck(IConfiguration configuration, IHttpClientFactory httpClientFactory)
		{
			_configuration = configuration;
			_httpClientFactory = httpClientFactory;
		}

		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
		{
			try
			{
				var endpoint = _configuration["OpenTelemetry:Endpoint"]?.TrimEnd('/');
				var authHeader = _configuration["OpenTelemetry:Headers"];

				if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(authHeader))
				{
					return HealthCheckResult.Unhealthy("OpenTelemetry configuration is missing (Endpoint or Headers).");
				}

				var client = _httpClientFactory.CreateClient();

				var headerParts = authHeader.Split('=');
				if (headerParts.Length == 2)
				{
					client.DefaultRequestHeaders.TryAddWithoutValidation(headerParts[0], headerParts[1]);
				}

				var response = await client.GetAsync($"{endpoint}/v1/traces", cancellationToken);

				if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.MethodNotAllowed)
				{
					return HealthCheckResult.Healthy("OpenTelemetry is reachable.");
				}

				if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
				{
					return HealthCheckResult.Unhealthy("OpenTelemetry authentication failed (401 Unauthorized). Check your token.");
				}

				return HealthCheckResult.Unhealthy($"OpenTelemetry gateway returned status: {response.StatusCode}");
			}
			catch (Exception ex)
			{
				return HealthCheckResult.Unhealthy($"OpenTelemetry health check failed: {ex.Message}");
			}
		}
	}
}