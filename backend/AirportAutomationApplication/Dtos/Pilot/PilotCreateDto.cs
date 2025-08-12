using System.ComponentModel.DataAnnotations;

namespace AirportAutomation.Application.Dtos.Pilot
{
	public class PilotCreateDto
	{
		[Required(ErrorMessage = "First Name is required.")]
		[MaxLength(50, ErrorMessage = "First Name cannot be longer than 50 characters.")]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "Last Name is required.")]
		[MaxLength(50, ErrorMessage = "Last Name cannot be longer than 50 characters.")]
		public string LastName { get; set; }

		[Required(ErrorMessage = "UPRN is required.")]
		[StringLength(13, MinimumLength = 13, ErrorMessage = "UPRN must be 13 characters long.")]
		public string UPRN { get; set; }
		public int? FlyingHours { get; set; }
	}
}
