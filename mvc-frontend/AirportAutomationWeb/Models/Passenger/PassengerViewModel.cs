using System.ComponentModel;

namespace AirportAutomation.Web.Models.Passenger
{
	public class PassengerViewModel
	{
		public int Id { get; set; }

		[DisplayName("First Name")]
		public string FirstName { get; set; }

		[DisplayName("Last Name")]
		public string LastName { get; set; }
		public string UPRN { get; set; }
		public string Passport { get; set; }
		public string Address { get; set; }
		public string Phone { get; set; }
	}
}
