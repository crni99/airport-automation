using System.ComponentModel.DataAnnotations;

namespace AirportAutomation.Application.Dtos.Destination
{
	public class DestinationCreateDto
	{
		[Required(ErrorMessage = "City is required.")]
		[MaxLength(255, ErrorMessage = "City cannot be longer than 255 characters.")]
		public string City { get; set; }

		[Required(ErrorMessage = "Airport is required.")]
		[MaxLength(255, ErrorMessage = "Airport cannot be longer than 255 characters.")]
		public string Airport { get; set; }
	}
}
