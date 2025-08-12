using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AirportAutomation.Web.Models.Pilot
{
	public class PilotCreateViewModel
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
		[StringLength(13, MinimumLength = 13, ErrorMessage = "UPRN must be 13 characters long.")]
		public string UPRN { get; set; }

		[DisplayName("Flying Hours")]
		public int? FlyingHours { get; set; }
	}
}
