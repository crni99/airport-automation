namespace AirportAutomation.Core.Interfaces
{
	public interface ICacheService
	{
		Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T?>> factory, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null);
		Task<T?> GetAsync<T>(string key);
		Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null);
		Task RemoveAsync(string key);
	}
}
