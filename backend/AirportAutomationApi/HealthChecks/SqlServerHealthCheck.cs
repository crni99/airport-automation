using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Data.SqlClient;

namespace AirportAutomation.Api.HealthChecks
{
	public class SqlServerHealthCheck : IHealthCheck
	{
		private readonly string _connectionString;

		public SqlServerHealthCheck(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("Default");
		}

		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
		{
			try
			{
				using var connection = new SqlConnection(_connectionString);
				await connection.OpenAsync(cancellationToken);

				return HealthCheckResult.Healthy("SQL Server connection established successfully.");
			}
			catch (Exception ex)
			{
				return HealthCheckResult.Unhealthy("SQL Server is not reachable.", ex);
			}
		}
	}
}
