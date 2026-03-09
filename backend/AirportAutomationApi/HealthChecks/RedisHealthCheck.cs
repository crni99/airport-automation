using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace AirportAutomation.Api.HealthChecks
{
	public class RedisHealthCheck : IHealthCheck
	{
		private readonly string _connectionString;

		public RedisHealthCheck(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("Redis");
		}

		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
		{
			try
			{
				using var redis = await ConnectionMultiplexer.ConnectAsync(_connectionString);
				if (redis.IsConnected)
				{
					return HealthCheckResult.Healthy("Redis connection is working.");
				}
				return HealthCheckResult.Unhealthy("Redis is disconnected.");
			}
			catch (Exception ex)
			{
				return HealthCheckResult.Unhealthy($"Redis health check failed: {ex.Message}");
			}
		}
	}
}
