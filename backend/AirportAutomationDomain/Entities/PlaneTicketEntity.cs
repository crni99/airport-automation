using System.ComponentModel.DataAnnotations.Schema;

namespace AirportAutomation.Core.Entities
{
	public class PlaneTicketEntity
	{
		public int Id { get; set; }

		[Column(TypeName = "decimal(8, 2)")]
		public decimal Price { get; set; }
		public DateOnly PurchaseDate { get; set; }
		public int SeatNumber { get; set; }
		public int PassengerId { get; set; }
		public int TravelClassId { get; set; }
		public int FlightId { get; set; }
		public PassengerEntity Passenger { get; set; }
		public TravelClassEntity TravelClass { get; set; }
		public FlightEntity Flight { get; set; }
	}
}
