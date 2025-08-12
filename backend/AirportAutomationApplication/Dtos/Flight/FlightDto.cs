using AirportAutomation.Application.Dtos.Airline;
using AirportAutomation.Application.Dtos.Destination;
using AirportAutomation.Application.Dtos.Pilot;

namespace AirportAutomation.Application.Dtos.Flight
{
	public class FlightDto
	{
		public int Id { get; set; }
		public DateOnly DepartureDate { get; set; }
		public TimeOnly DepartureTime { get; set; }
		public int AirlineId { get; set; }
		public int DestinationId { get; set; }
		public int PilotId { get; set; }
		public AirlineDto Airline { get; set; }
		public DestinationDto Destination { get; set; }
		public PilotDto Pilot { get; set; }
	}
}
