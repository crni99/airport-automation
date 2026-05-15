using AirportAutomation.Core.Entities;
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
		private readonly IDataHttpService _dataHttpService;
		private readonly IExportHttpService _exportHttpService;
		private readonly IAlertService _alertService;
		private readonly IMapper _mapper;

		public TravelClassController(IDataHttpService dataHttpService, IExportHttpService exportHttpService, IAlertService alertService, IMapper mapper)
		{
			_dataHttpService = dataHttpService;
			_exportHttpService = exportHttpService;
			_alertService = alertService;
			_mapper = mapper;
		}

		public async Task<IActionResult> Index()
		{
			return View();
		}

		[HttpGet]
		[Route("GetTravelClasses")]
		public async Task<IActionResult> GetTravelClasses(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
		{
			if (page < 1)
			{
				_alertService.SetAlertMessage(TempData, "invalid_page_number", false);
				return Json(new { success = false, message = "Page number must be greater than or equal to 1." });
			}
			var response = await _dataHttpService.GetDataList<TravelClassEntity>(page, pageSize, cancellationToken);
			if (response == null)
			{
				return Json(new { success = false, message = "No travel classes found." });
			}
			var pagedResponse = _mapper.Map<PagedResponse<TravelClassViewModel>>(response);
			return Json(new { success = true, data = pagedResponse });
		}

		[HttpGet]
		[Route("Export")]
		public async Task<IActionResult> DownloadFile(
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10,
			[FromQuery] string fileType = "pdf",
			CancellationToken cancellationToken = default)
		{
			var result = await _exportHttpService.DownloadFileAsync<TravelClassEntity>(fileType, null, page, pageSize, true, cancellationToken);
			if (result is null || result.HasError)
			{
				_alertService.SetAlertMessage(TempData, "download_failed", false);
				return RedirectToAction("Index");
			}
			if (result.IsUnauthorized)
			{
				_alertService.SetAlertMessage(TempData, "unauthorized_access", false);
				return RedirectToAction("Index", "Home");
			}
			if (result.IsForbidden)
			{
				_alertService.SetAlertMessage(TempData, "forbidden_access", false);
				return RedirectToAction("Index", "TravelClass");
			}
			if (result.Content == null || result.Content.Length == 0)
			{
				_alertService.SetAlertMessage(TempData, "no_content_available", false);
				return RedirectToAction("Index", "TravelClass");
			}
			return File(result.Content, result.ContentType, result.FileName);
		}
	}
}