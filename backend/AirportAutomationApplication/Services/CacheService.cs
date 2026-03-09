using AirportAutomation.Core.Configuration;
using AirportAutomation.Core.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AirportAutomation.Application.Services
{
	public class CacheService : ICacheService
	{
		private readonly IDistributedCache _cache;
		private readonly RedisSettings _settings;

		public CacheService(IDistributedCache cache, IOptions<RedisSettings> settings)
		{
			_cache = cache;
			_settings = settings.Value;
		}

		public async Task<T?> GetAsync<T>(string key)
		{
			var jsonData = await _cache.GetStringAsync(key);
			return jsonData == null ? default : JsonConvert.DeserializeObject<T>(jsonData);
		}

		public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
		{
			var options = new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = absoluteExpiration ?? TimeSpan.FromMinutes(_settings.AbsoluteExpirationInMinutes),
				SlidingExpiration = slidingExpiration ?? TimeSpan.FromMinutes(_settings.SlidingExpirationInMinutes)
			};

			var jsonData = JsonConvert.SerializeObject(value);
			await _cache.SetStringAsync(key, jsonData, options);
		}

		public async Task RemoveAsync(string key) => await _cache.RemoveAsync(key);
	}
}
