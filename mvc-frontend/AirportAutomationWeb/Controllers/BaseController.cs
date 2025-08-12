using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AirportAutomation.Web.Controllers
{
	public class BaseController : Controller
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);

			ViewBag.ApiUserRole = HttpContext.Session.GetString("AccessRole");
		}
	}
}