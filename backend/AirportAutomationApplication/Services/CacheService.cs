using AirportAutomation.Core.Configuration;
using AirportAutomation.Core.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AirportAutomation.Application.Services
{
	public class CacheService : ICacheService
	{
		private readonly IDistributedCache _cache;
		private readonly RedisSettings _settings;
		private readonly ILogger<CacheService> _logger;

		public CacheService(IDistributedCache cache, IOptions<RedisSettings> settings, ILogger<CacheService> logger)
		{
			_cache = cache;
			_settings = settings.Value;
			_logger = logger;
		}

		public async Task<T?> GetAsync<T>(string key)
		{
			try
			{
				var jsonData = await _cache.GetStringAsync(key);
				return jsonData == null ? default : JsonConvert.DeserializeObject<T>(jsonData);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Cache GET failed for key {Key}", key);
				return default;
			}
		}

		public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
		{
			try
			{
				var options = new DistributedCacheEntryOptions
				{
					AbsoluteExpirationRelativeToNow = absoluteExpiration ?? TimeSpan.FromMinutes(_settings.AbsoluteExpirationInMinutes),
					SlidingExpiration = slidingExpiration ?? TimeSpan.FromMinutes(_settings.SlidingExpirationInMinutes)
				};
				var jsonData = JsonConvert.SerializeObject(value);
				await _cache.SetStringAsync(key, jsonData, options);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Cache SET failed for key {Key}", key);
			}
		}

		public async Task RemoveAsync(string key)
		{
			try
			{
				await _cache.RemoveAsync(key);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Cache REMOVE failed for key {Key}", key);
			}
		}

	}
}
