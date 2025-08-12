using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AirportAutomation.Web.Models.Passenger
{
	public class PassengerCreateViewModel
	{
		[DisplayName("First Name")]
		[Required(ErrorMessage = "First Name is required.")]
		[MaxLength(50, ErrorMessage = "First Name cannot be longer than 50 characters.")]
		public string FirstName { get; set; }

		[DisplayName("Last Name")]
		[Required(ErrorMessage = "Last Name is required.")]
		[MaxLength(50, ErrorMessage = "Last Name cannot be longer than 50 characters.")]
		public string LastName { get; set; }

		[Required(ErrorMessage = "UPRN is required.")]
		[MaxLength(13, ErrorMessage = "UPRN must be a maximum of 13 characters long.")]
		[MinLength(13, ErrorMessage = "UPRN must be a minimum of 13 characters long.")]
		public string UPRN { get; set; }

		[Required(ErrorMessage = "Passport is required.")]
		[MaxLength(9, ErrorMessage = "Passport must be a maximum of 9 characters long.")]
		[MinLength(9, ErrorMessage = "Passport must be a minimum of 9 characters long.")]
		public string Passport { get; set; }

		[Required(ErrorMessage = "Address is required.")]
		[MaxLength(200, ErrorMessage = "Address cannot be longer than 200 characters.")]
		public string Address { get; set; }

		[Required(ErrorMessage = "Phone number is required.")]
		[MaxLength(30, ErrorMessage = "Phone number cannot be longer than 30 characters.")]
		public string Phone { get; set; }
	}
}
