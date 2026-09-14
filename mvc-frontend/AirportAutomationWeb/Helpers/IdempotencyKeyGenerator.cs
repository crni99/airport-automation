using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace AirportAutomation.Web.Helpers
{
	/// <summary>
	/// Helper class for generating unique Idempotency Keys
	/// </summary>
	public static class IdempotencyKeyGenerator
	{
		/// <summary>
		/// Generates an idempotency key based on the data hash
		/// This ensures the same data always produces the same key
		/// </summary>
		public static string Generate<T>(string resourceType, T data)
		{
			var json = JsonSerializer.Serialize(data);
			var hash = ComputeSha256Hash(json);

			// Koristi samo prvih 12 karaktera hash-a
			return $"{resourceType}-{hash.Substring(0, 12).ToLower()}";
		}

		/// <summary>
		/// Generates an idempotency key for edit/delete operations based on ID
		/// </summary>
		public static string GenerateForId(string resourceType, string operation, int id)
		{
			var key = $"{resourceType}-{operation}-{id}";
			var hash = ComputeSha256Hash(key);
			return $"{resourceType}-{operation}-{hash.Substring(0, 8).ToLower()}";
		}

		private static string ComputeSha256Hash(string input)
		{
			using var sha256 = SHA256.Create();
			var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
			return Convert.ToBase64String(hashBytes)
				.Replace("+", "")
				.Replace("/", "")
				.Replace("=", "");
		}
	}
}