﻿using AirportAutomation.Core.Entities;
using AirportAutomation.Web.Interfaces;
using AirportAutomation.Web.Models.Response;
using AirportAutomation.Web.Models.TravelClass;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AirportAutomation.Web.Controllers
{
	[Route("[controller]")]
	public class TravelClassController : BaseController
	{
		private readonly IHttpCallService _httpCallService;
		private readonly IAlertService _alertService;
		private readonly IMapper _mapper;

		public TravelClassController(IHttpCallService httpCallService, IAlertService alertService, IMapper mapper)
		{
			_httpCallService = httpCallService;
			_alertService = alertService;
			_mapper = mapper;
		}

		public async Task<IActionResult> Index()
		{
			return View();
		}

		[HttpGet]
		[Route("GetTravelClasses")]
		public async Task<IActionResult> GetTravelClasses(int page = 1, int pageSize = 10)
		{
			if (page < 1)
			{
				_alertService.SetAlertMessage(TempData, "invalid_page_number", false);
				return Json(new { success = false, message = "Page number must be greater than or equal to 1." });
			}
			var response = await _httpCallService.GetDataList<TravelClassEntity>(page, pageSize);
			if (response == null)
			{
				return Json(new { success = false, message = "No travel classes found." });
			}
			var pagedResponse = _mapper.Map<PagedResponse<TravelClassViewModel>>(response);
			return Json(new { success = true, data = pagedResponse });
		}
	}
}