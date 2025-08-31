using AirportAutomation.Infrastructure.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AirportAutomation.Api.HealthChecks
{
	/// <summary>
	/// Performs health checks on the database to ensure it is reachable.
	/// </summary>
	public class DatabaseHealthCheck : IHealthCheck
	{
		private readonly DatabaseContext _databaseContext;

		/// <summary>
		/// Initializes a new instance of the <see cref="DatabaseHealthCheck"/> class.
		/// </summary>
		/// <param name="databaseContext">The database context used to check the database connection.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="databaseContext"/> is null.</exception>
		public DatabaseHealthCheck(DatabaseContext databaseContext)
		{
			_databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
		}

		/// <summary>
		/// Checks the health of the database asynchronously.
		/// </summary>
		/// <param name="context">The context for the health check.</param>
		/// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
		/// <returns>A task representing the asynchronous operation, with a <see cref="HealthCheckResult"/> as its result.</returns>
		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
		{
			try
			{
				var isDatabaseHealthy = await _databaseContext.Database.CanConnectAsync(cancellationToken);

				return isDatabaseHealthy
					? HealthCheckResult.Healthy("Database is reachable.")
					: HealthCheckResult.Unhealthy("Database is not reachable.");
			}
			catch (Exception ex)
			{
				return HealthCheckResult.Unhealthy("An exception occurred while checking the database.", ex);
			}
		}
	}
}
