namespace AirportAutomation.Core.Entities
{
	/// <summary>
	/// Stores idempotency requests to prevent duplicate processing
	/// </summary>
	public class IdempotencyRequestEntity
	{
		public int Id { get; set; }
		public required string IdempotencyKey { get; set; }
		public required string Endpoint { get; set; }
		public required string HttpMethod { get; set; }
		public required string RequestBody { get; set; }
		public required string ResponseBody { get; set; }
		public int ResponseStatusCode { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime ExpiresAt { get; set; }
		public string? UserId { get; set; }
	}
}
