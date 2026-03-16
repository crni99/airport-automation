using AirportAutomation.Application.Services;
using AirportAutomation.Core.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AirportAutomationApi.Test.Services
{
	public class CacheServiceTests
	{
		private readonly Mock<IDistributedCache> _cacheMock;
		private readonly Mock<ILogger<CacheService>> _loggerMock;
		private readonly CacheService _cacheService;

		public CacheServiceTests()
		{
			_cacheMock = new Mock<IDistributedCache>();
			_loggerMock = new Mock<ILogger<CacheService>>();
			var settings = Options.Create(new RedisSettings
			{
				AbsoluteExpirationInMinutes = 60,
				SlidingExpirationInMinutes = 30
			});
			_cacheService = new CacheService(_cacheMock.Object, settings, _loggerMock.Object);
		}

		[Fact]
		public async Task GetOrCreateAsync_ReturnsCachedValue_WhenCacheHit()
		{
			var expected = "cached_value";
			var json = JsonConvert.SerializeObject(expected);
			_cacheMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(System.Text.Encoding.UTF8.GetBytes(json));

			var result = await _cacheService.GetOrCreateAsync<string>("test_key", () => Task.FromResult<string?>("factory_value"));

			Assert.Equal(expected, result);
			_cacheMock.Verify(x => x.SetAsync(
				It.IsAny<string>(),
				It.IsAny<byte[]>(),
				It.IsAny<DistributedCacheEntryOptions>(),
				It.IsAny<CancellationToken>()), Times.Never);
		}

		[Fact]
		public async Task GetOrCreateAsync_CallsFactory_WhenCacheMiss()
		{
			var expected = "factory_value";
			_cacheMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync((byte[])null);

			var result = await _cacheService.GetOrCreateAsync<string>("test_key", () => Task.FromResult<string?>(expected));

			Assert.Equal(expected, result);
			_cacheMock.Verify(x => x.SetAsync(
				It.IsAny<string>(),
				It.IsAny<byte[]>(),
				It.IsAny<DistributedCacheEntryOptions>(),
				It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task GetOrCreateAsync_ReturnsNull_WhenFactoryReturnsNull()
		{
			_cacheMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync((byte[])null);

			var result = await _cacheService.GetOrCreateAsync<string>("test_key", () => Task.FromResult<string?>(null));

			Assert.Null(result);
			_cacheMock.Verify(x => x.SetAsync(
				It.IsAny<string>(),
				It.IsAny<byte[]>(),
				It.IsAny<DistributedCacheEntryOptions>(),
				It.IsAny<CancellationToken>()), Times.Never);
		}

		[Fact]
		public async Task GetOrCreateAsync_CallsFactory_WhenCacheThrowsException()
		{
			var expected = "factory_value";
			_cacheMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.ThrowsAsync(new Exception("Redis unavailable"));

			var result = await _cacheService.GetOrCreateAsync<string>("test_key", () => Task.FromResult<string?>(expected));

			Assert.Equal(expected, result);
		}

		[Fact]
		public async Task GetAsync_ReturnsDeserializedObject_WhenKeyExists()
		{
			var expected = new { Name = "Test" };
			var json = JsonConvert.SerializeObject(expected);
			_cacheMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(System.Text.Encoding.UTF8.GetBytes(json));

			var result = await _cacheService.GetAsync<dynamic>("test_key");

			Assert.NotNull(result);
		}

		[Fact]
		public async Task GetAsync_ReturnsDefault_WhenKeyDoesNotExist()
		{
			_cacheMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync((byte[])null);

			var result = await _cacheService.GetAsync<string>("test_key");

			Assert.Null(result);
		}

		[Fact]
		public async Task GetAsync_ReturnsDefault_WhenCacheThrowsException()
		{
			_cacheMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.ThrowsAsync(new Exception("Redis unavailable"));

			var result = await _cacheService.GetAsync<string>("test_key");

			Assert.Null(result);
		}

		[Fact]
		public async Task SetAsync_CallsSetStringAsync_WithCorrectKey()
		{
			var value = "test_value";

			await _cacheService.SetAsync("test_key", value);

			_cacheMock.Verify(x => x.SetAsync(
				"test_key",
				It.IsAny<byte[]>(),
				It.IsAny<DistributedCacheEntryOptions>(),
				It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task SetAsync_DoesNotThrow_WhenCacheThrowsException()
		{
			_cacheMock.Setup(x => x.SetAsync(
				It.IsAny<string>(),
				It.IsAny<byte[]>(),
				It.IsAny<DistributedCacheEntryOptions>(),
				It.IsAny<CancellationToken>()))
				.ThrowsAsync(new Exception("Redis unavailable"));

			var exception = await Record.ExceptionAsync(() => _cacheService.SetAsync("test_key", "value"));

			Assert.Null(exception);
		}

		[Fact]
		public async Task RemoveAsync_CallsRemoveAsync_WithCorrectKey()
		{
			await _cacheService.RemoveAsync("test_key");

			_cacheMock.Verify(x => x.RemoveAsync(
				"test_key",
				It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task RemoveAsync_DoesNotThrow_WhenCacheThrowsException()
		{
			_cacheMock.Setup(x => x.RemoveAsync(
				It.IsAny<string>(),
				It.IsAny<CancellationToken>()))
				.ThrowsAsync(new Exception("Redis unavailable"));

			var exception = await Record.ExceptionAsync(() => _cacheService.RemoveAsync("test_key"));

			Assert.Null(exception);
		}
	}
}
