using Microsoft.AspNetCore.Mvc;

namespace AirportAutomation.Api.Controllers
{
	/// <summary>
	/// Serves as a base class for API controllers, providing common functionality and routing.
	/// </summary>
	/// <remarks>
	/// This class is intended to be inherited by other controllers in the API.
	/// </remarks>
	[ApiController]
	[Route("api/v{version:apiVersion}/[controller]")]
	public abstract class BaseController : ControllerBase
	{
	}
}
