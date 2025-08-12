using AirportAutomation.Core.Entities;
using AirportAutomation.Web.Interfaces;
using AirportAutomation.Web.Models.HealthCheck;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AirportAutomation.Web.Controllers
{
	[Route("[controller]")]
	public class HealthCheckController : BaseController
	{
		private readonly IHttpCallService _httpCallService;
		private readonly IMapper _mapper;

		public HealthCheckController(IHttpCallService httpCallService, IMapper mapper)
		{
			_httpCallService = httpCallService;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var response = await _httpCallService.GetHealthCheck<HealthCheckEntity>();
			if (response == null)
			{
				return View();
			}
			return View(_mapper.Map<HealthCheckViewModel>(response));
		}

	}
}