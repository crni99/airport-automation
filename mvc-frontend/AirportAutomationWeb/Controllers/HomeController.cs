using AirportAutomation.Web.Interfaces;
using AirportAutomation.Web.Models.ApiUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirportAutomation.Web.Controllers
{
	[Route("")]
	[AllowAnonymous]
	public class HomeController : BaseController
	{
		private readonly IHttpCallService _httpCallService;
		private readonly IAlertService _alertService;

		public HomeController(IHttpCallService httpCallService, IAlertService alertService)
		{
			_httpCallService = httpCallService;
			_alertService = alertService;
		}

		[HttpGet]
		public IActionResult Index(bool logout = false)
		{
			if (logout)
			{
				_alertService.SetAlertMessage(TempData, "logout_success", true);
			}
			string token = _httpCallService.GetToken();
			if (!string.IsNullOrEmpty(token))
			{
				return Redirect("HealthCheck");
			}
			return View("Index");
		}

		[HttpPost]
		[Route("Authenticate")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Authenticate(UserViewModel user, CancellationToken cancellationToken = default)
		{
			if (ModelState.IsValid)
			{
				var response = await _httpCallService.Authenticate(user, cancellationToken);
				if (!response)
				{
					_alertService.SetAlertMessage(TempData, "login_failed", false);
					return Redirect("/");
				}
				return Redirect("HealthCheck");
			}
			else { return RedirectToAction("Index"); }
		}

		[HttpGet]
		[Route("SignOut")]
		public async Task<IActionResult> SignOut(CancellationToken cancellationToken = default)
		{
			bool removed = await _httpCallService.RemoveToken(cancellationToken);
			return (removed) ? Json(new { success = true }) : Json(new { success = false });
		}
	}
}

