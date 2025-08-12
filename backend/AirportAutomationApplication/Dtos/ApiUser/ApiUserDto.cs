using System.ComponentModel.DataAnnotations;

namespace AirportAutomation.Application.Dtos.ApiUser;

public class ApiUserDto
{
	[Required(ErrorMessage = "User Name is required.")]
	[MaxLength(50, ErrorMessage = "User Name cannot be longer than 50 characters.")]
	public string UserName { get; set; }

	[Required(ErrorMessage = "Password is required.")]
	[MaxLength(50, ErrorMessage = "Password cannot be longer than 50 characters.")]
	public string Password { get; set; }
}
