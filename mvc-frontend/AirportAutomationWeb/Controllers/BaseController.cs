using AirportAutomation.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AirportAutomation.Web.Controllers
{
	public class BaseController : Controller
	{
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			base.OnActionExecuting(context);

			ViewBag.ApiUserRole = HttpContext.Session.GetString("AccessRole");

			string controllerName = context.RouteData.Values["controller"]?.ToString();
			if (string.Equals(controllerName, "Home", StringComparison.OrdinalIgnoreCase))
				return;

			string token = HttpContext.Session.GetString("AccessToken");

			if (string.IsNullOrEmpty(token))
			{
				var alertService = HttpContext.RequestServices.GetService(typeof(IAlertService)) as IAlertService;
				if (alertService != null)
				{
					alertService.SetAlertMessage(TempData, "unauthorized_access", false);
				}
				context.Result = new RedirectToActionResult("Index", "Home", null);
			}
		}
	}
}