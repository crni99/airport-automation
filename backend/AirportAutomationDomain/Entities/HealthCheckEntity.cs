namespace AirportAutomation.Core.Entities
{
	public class HealthCheckEntity
	{
		public string Status { get; set; }
		public string TotalDuration { get; set; }
		public Dictionary<string, HealthCheckEntry> Entries { get; set; }
	}
}
