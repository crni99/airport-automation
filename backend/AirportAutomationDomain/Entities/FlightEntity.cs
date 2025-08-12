namespace AirportAutomation.Core.Entities
{
	public class FlightEntity
	{
		public int Id { get; set; }
		public DateOnly DepartureDate { get; set; }
		public TimeOnly DepartureTime { get; set; }
		public int AirlineId { get; set; }
		public int DestinationId { get; set; }
		public int PilotId { get; set; }
		public AirlineEntity Airline { get; set; }
		public DestinationEntity Destination { get; set; }
		public PilotEntity Pilot { get; set; }
	}
}
