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
		private readonly IHealthHttpService _healthHttpService;
		private readonly IMapper _mapper;

		public HealthCheckController(IHealthHttpService healthHttpService, IMapper mapper)
		{
			_healthHttpService = healthHttpService;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
		{
			var response = await _healthHttpService.GetHealthCheck<HealthCheckEntity>(cancellationToken);
			if (response == null)
			{
				return View();
			}
			return View(_mapper.Map<HealthCheckViewModel>(response));
		}
	}
}