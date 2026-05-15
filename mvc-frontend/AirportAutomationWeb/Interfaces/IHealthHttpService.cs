namespace AirportAutomation.Web.Interfaces
{
	public interface IHealthHttpService
	{
		Task<T> GetHealthCheck<T>(CancellationToken cancellationToken = default);
	}
}