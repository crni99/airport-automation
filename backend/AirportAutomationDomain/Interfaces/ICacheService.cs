using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportAutomation.Core.Interfaces
{
	public interface ICacheService
	{
		Task<T?> GetAsync<T>(string key);
		Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null);
		Task RemoveAsync(string key);
	}
}
