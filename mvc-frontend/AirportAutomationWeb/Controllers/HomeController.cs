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
		private readonly IAuthHttpService _authHttpService;
		private readonly IAlertService _alertService;

		public HomeController(IAuthHttpService authHttpService, IAlertService alertService)
		{
			_authHttpService = authHttpService;
			_alertService = alertService;
		}

		[HttpGet]
		public IActionResult Index(bool logout = false)
		{
			if (logout)
			{
				_alertService.SetAlertMessage(TempData, "logout_success", true);
			}
			string token = _authHttpService.GetToken();
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
				var response = await _authHttpService.Authenticate(user, cancellationToken);
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
			bool removed = await _authHttpService.RemoveToken(cancellationToken);
			return (removed) ? Json(new { success = true }) : Json(new { success = false });
		}
	}
}