using System.ComponentModel.DataAnnotations;

namespace AirportAutomation.Application.Dtos.ApiUser;

public class ApiUserRoleDto
{
	public int ApiUserId { get; set; }

	[Required(ErrorMessage = "User Name is required.")]
	[MaxLength(50, ErrorMessage = "User Name cannot be longer than 50 characters.")]
	public string UserName { get; set; }

	[Required(ErrorMessage = "Password is required.")]
	[MaxLength(100, ErrorMessage = "Password cannot be longer than 100 characters.")]
	public string Password { get; set; }

	[Required(ErrorMessage = "Role is required.")]
	[MaxLength(50, ErrorMessage = "Role cannot be longer than 50 characters.")]
	public string Roles { get; set; }
}
