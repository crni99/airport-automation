using System.ComponentModel.DataAnnotations;

namespace AirportAutomation.Application.Dtos.PlaneTicket
{
	public class PlaneTicketUpdateDto
	{
		[Required(ErrorMessage = "Id is required.")]
		[Range(1, int.MaxValue, ErrorMessage = "Id must be a positive integer.")]
		public int Id { get; set; }

		[Required(ErrorMessage = "Price is required.")]
		[Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
		public decimal Price { get; set; }

		[Required(ErrorMessage = "Purchase Date is required.")]
		public DateOnly PurchaseDate { get; set; }

		[Required(ErrorMessage = "Seat Number is required.")]
		[Range(1, int.MaxValue, ErrorMessage = "Seat Number must be a positive integer.")]
		public int SeatNumber { get; set; }

		[Required(ErrorMessage = "Passenger Id is required.")]
		public int? PassengerId { get; set; }

		[Required(ErrorMessage = "Travel Class Id is required.")]
		public int TravelClassId { get; set; }

		[Required(ErrorMessage = "Flight Id is required.")]
		public int FlightId { get; set; }
	}
}
