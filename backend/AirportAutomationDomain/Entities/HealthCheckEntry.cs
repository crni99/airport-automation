namespace AirportAutomation.Core.Entities
{
	public class HealthCheckEntry
	{
		public object Data { get; set; }
		public string Description { get; set; }
		public string Duration { get; set; }
		public string Status { get; set; }
		public List<string> Tags { get; set; }
	}
}
