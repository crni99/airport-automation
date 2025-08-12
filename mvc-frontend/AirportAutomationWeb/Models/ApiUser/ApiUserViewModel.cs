using System.ComponentModel;

namespace AirportAutomation.Web.Models.ApiUser
{
	public class ApiUserViewModel
	{
		public int Id { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string Roles { get; set; }
	}
}
