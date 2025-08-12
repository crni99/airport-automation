using System.ComponentModel.DataAnnotations;

namespace AirportAutomation.Application.Dtos.Airline
{
	public class AirlineCreateDto
	{
		[Required(ErrorMessage = "Name is required.")]
		[MaxLength(255, ErrorMessage = "Name cannot be longer than 255 characters.")]
		public string Name { get; set; }
	}
}
