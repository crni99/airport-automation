using AirportAutomation.Core.Converters;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AirportAutomation.Web.Models.PlaneTicket
{
	public class PlaneTicketCreateViewModel
	{
		[Required(ErrorMessage = "Price is required.")]
		[Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
		public string Price { get; set; }

		[DisplayName("Purchase Date")]
		[Required(ErrorMessage = "Purchase Date is required.")]
		[JsonConverter(typeof(DateOnlyJsonConverter))]
		public string PurchaseDate { get; set; }

		[DisplayName("Seat Number")]
		[Required(ErrorMessage = "Seat Number is required.")]
		[Range(1, 200, ErrorMessage = "Seat Number must be in range 1 to 200.")]
		public int SeatNumber { get; set; }

		[DisplayName("Passenger Id")]
		[Required(ErrorMessage = "Passenger Id is required.")]
		public int PassengerId { get; set; }

		[DisplayName("Travel Class Id")]
		[Required(ErrorMessage = "Travel Class Id is required.")]
		public int TravelClassId { get; set; }

		[DisplayName("Flight Id")]
		[Required(ErrorMessage = "Flight Id is required.")]
		public int FlightId { get; set; }
	}
}
