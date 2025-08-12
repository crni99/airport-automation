using AirportAutomation.Web.Models.Flight;
using AirportAutomation.Web.Models.Passenger;
using AirportAutomation.Web.Models.TravelClass;
using System.ComponentModel;

namespace AirportAutomation.Web.Models.PlaneTicket
{
	public class PlaneTicketViewModel
	{
		public int Id { get; set; }
		public decimal Price { get; set; }

		[DisplayName("Purchase Date")]
		public string PurchaseDate { get; set; }

		[DisplayName("Seat Number")]
		public int SeatNumber { get; set; }

		[DisplayName("Passenger Id")]
		public int PassengerId { get; set; }

		[DisplayName("Travel Class Id")]
		public int TravelClassId { get; set; }

		[DisplayName("Flight Id")]
		public int FlightId { get; set; }
		public PassengerViewModel Passenger { get; set; }
		public TravelClassViewModel TravelClass { get; set; }
		public FlightViewModel Flight { get; set; }
	}
}