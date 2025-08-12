using AirportAutomation.Core.Converters;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AirportAutomation.Web.Models.Flight
{
	public class FlightCreateViewModel
	{
		[DisplayName("Departure Date")]
		[Required(ErrorMessage = "Departure Date is required.")]
		[JsonConverter(typeof(DateOnlyJsonConverter))]
		public string DepartureDate { get; set; }

		[DisplayName("Departure Time")]
		[Required(ErrorMessage = "Departure Time is required.")]
		[JsonConverter(typeof(TimeOnlyJsonConverter))]
		public string DepartureTime { get; set; }

		[DisplayName("Airline Id")]
		[Required(ErrorMessage = "Airline Id is required.")]
		public int AirlineId { get; set; }

		[DisplayName("Destination Id")]
		[Required(ErrorMessage = "Destination Id is required.")]
		public int DestinationId { get; set; }

		[DisplayName("Pilot Id")]
		[Required(ErrorMessage = "Pilot Id is required.")]
		public int PilotId { get; set; }
	}
}
