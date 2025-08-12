using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportAutomation.Core.Entities
{
	[Table("ApiUser")]
	public class ApiUserEntity
	{
		[Key]
		public int ApiUserId { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string Roles { get; set; }
	}
}
