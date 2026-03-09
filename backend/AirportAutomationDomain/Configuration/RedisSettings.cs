namespace AirportAutomation.Core.Configuration
{
	public class RedisSettings
	{
		public bool Enabled { get; set; }
		public string InstanceName { get; set; } = "AirportAutomation:";
		public int AbsoluteExpirationInMinutes { get; set; } = 30;
		public int SlidingExpirationInMinutes { get; set; } = 10;
	}
}
