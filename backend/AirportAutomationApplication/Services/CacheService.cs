using AirportAutomation.Core.Configuration;
using AirportAutomation.Core.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace AirportAutomation.Application.Services
{
	public class CacheService : ICacheService
	{
		private readonly IDistributedCache _cache;
		private readonly RedisSettings _settings;
		private readonly ILogger<CacheService> _logger;
		private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();
		private static readonly ConcurrentDictionary<string, byte> _trackedKeys = new();

		public CacheService(IDistributedCache cache, IOptions<RedisSettings> settings, ILogger<CacheService> logger)
		{
			_cache = cache;
			_settings = settings.Value;
			_logger = logger;
		}

		public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T?>> factory, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
		{
			var cached = await GetAsync<T>(key);
			if (cached != null) return cached;

			var semaphore = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
			await semaphore.WaitAsync();
			try
			{
				cached = await GetAsync<T>(key);
				if (cached != null) return cached;

				var result = await factory();
				if (result != null)
					await SetAsync(key, result, absoluteExpiration, slidingExpiration);

				return result;
			}
			finally
			{
				semaphore.Release();
				_locks.TryRemove(key, out _);
			}
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
				_trackedKeys.TryAdd(key, 0);
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
				_trackedKeys.TryRemove(key, out _);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Cache REMOVE failed for key {Key}", key);
			}
		}

		public async Task RemoveByPrefixAsync(string prefix)
		{
			try
			{
				var keys = _trackedKeys.Keys.Where(k => k.StartsWith(prefix)).ToList();
				var tasks = keys.Select(async key =>
				{
					await _cache.RemoveAsync(key);
					_trackedKeys.TryRemove(key, out _);
				});
				await Task.WhenAll(tasks);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Cache REMOVE BY PREFIX failed for prefix {Prefix}", prefix);
			}
		}

	}
}
