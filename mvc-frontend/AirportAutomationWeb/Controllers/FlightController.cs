using AirportAutomation.Core.Entities;
using AirportAutomation.Web.Interfaces;
using AirportAutomation.Web.Models.Flight;
using AirportAutomation.Web.Models.Response;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AirportAutomation.Web.Controllers
{
	[Route("[controller]")]
	public class FlightController : BaseController
	{
		private readonly IDataHttpService _dataHttpService;
		private readonly ISearchHttpService _searchHttpService;
		private readonly IExportHttpService _exportHttpService;
		private readonly IAlertService _alertService;
		private readonly IMapper _mapper;

		public FlightController(IDataHttpService dataHttpService, ISearchHttpService searchHttpService, IExportHttpService exportHttpService, IAlertService alertService, IMapper mapper)
		{
			_dataHttpService = dataHttpService;
			_searchHttpService = searchHttpService;
			_exportHttpService = exportHttpService;
			_alertService = alertService;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			return View();
		}

		[HttpGet]
		[Route("GetFlights")]
		public async Task<IActionResult> GetFlights(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
		{
			if (page < 1)
			{
				_alertService.SetAlertMessage(TempData, "invalid_page_number", false);
				return Json(new { success = false, message = "Page number must be greater than or equal to 1." });
			}
			var response = await _dataHttpService.GetDataList<FlightEntity>(page, pageSize, cancellationToken);
			if (response == null)
			{
				return Json(new { success = false, message = "No flights found." });
			}
			var pagedResponse = _mapper.Map<PagedResponse<FlightViewModel>>(response);
			return Json(new { success = true, data = pagedResponse });
		}

		[HttpGet]
		[Route("Details/{id:int}")]
		public async Task<IActionResult> Details(int id, CancellationToken cancellationToken = default)
		{
			var response = await _dataHttpService.GetData<FlightEntity>(id, cancellationToken);
			if (response is null)
			{
				_alertService.SetAlertMessage(TempData, "data_not_found", false);
				return RedirectToAction("Index");
			}
			return View(_mapper.Map<FlightViewModel>(response));
		}

		[HttpGet]
		[Route("SearchFlights")]
		public async Task<IActionResult> SearchFlights(
			[FromQuery] string startDate,
			[FromQuery] string endDate,
			int page = 1,
			int pageSize = 10,
			CancellationToken cancellationToken = default)
		{
			if (page < 1)
			{
				_alertService.SetAlertMessage(TempData, "invalid_page_number", false);
				return Json(new { success = false, message = "Page number must be greater than or equal to 1." });
			}
			if (string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate))
			{
				_alertService.SetAlertMessage(TempData, "missing_field", false);
				return RedirectToAction("Index");
			}
			var response = await _searchHttpService.GetDataBetweenDates<FlightEntity>(startDate, endDate, page, pageSize, cancellationToken);
			if (response == null)
			{
				return Json(new { success = false, message = "No flights found." });
			}
			var pagedResponse = _mapper.Map<PagedResponse<FlightViewModel>>(response);
			return Json(new { success = true, data = pagedResponse });
		}

		[HttpGet]
		[Route("Create")]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[Route("CreateFlight")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateFlight(FlightCreateViewModel flightCreateDto, CancellationToken cancellationToken = default)
		{
			var flight = _mapper.Map<FlightEntity>(flightCreateDto);
			var response = await _dataHttpService.CreateData<FlightEntity>(flight, cancellationToken);
			if (response is null)
			{
				_alertService.SetAlertMessage(TempData, "create_data_failed", false);
				return RedirectToAction("Create");
			}
			return RedirectToAction("Details", new { id = response.Id });
		}

		[HttpGet]
		[Route("Edit/{id}")]
		public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
		{
			var response = await _dataHttpService.GetData<FlightEntity>(id, cancellationToken);
			if (response is null)
			{
				_alertService.SetAlertMessage(TempData, "data_not_found", false);
				return RedirectToAction("Details", new { id });
			}
			return View(_mapper.Map<FlightViewModel>(response));
		}

		[HttpPost]
		[Route("EditFlight")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditFlight(FlightViewModel flightDto, CancellationToken cancellationToken = default)
		{
			var flight = _mapper.Map<FlightEntity>(flightDto);
			var response = await _dataHttpService.EditData<FlightEntity>(flight, flight.Id, cancellationToken);
			if (response)
			{
				_alertService.SetAlertMessage(TempData, "edit_data_success", true);
				return RedirectToAction("Details", new { id = flightDto.Id });
			}
			else
			{
				_alertService.SetAlertMessage(TempData, "edit_data_failed", false);
				return RedirectToAction("Edit", new { id = flightDto.Id });
			}
		}

		[HttpPost]
		[Route("Delete/{id}")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
		{
			var response = await _dataHttpService.DeleteData<FlightEntity>(id, cancellationToken);
			if (response)
			{
				_alertService.SetAlertMessage(TempData, "delete_data_success", true);
				return RedirectToAction("Index");
			}
			else
			{
				_alertService.SetAlertMessage(TempData, "delete_data_failed", false);
				return RedirectToAction("Details", new { id });
			}
		}

		[HttpGet]
		[Route("Export")]
		public async Task<IActionResult> DownloadFile(
			[FromQuery] string startDate,
			[FromQuery] string endDate,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10,
			[FromQuery] bool getAll = false,
			[FromQuery] string fileType = "pdf",
			CancellationToken cancellationToken = default)
		{
			var filter = new Dictionary<string, string>();
			if (!string.IsNullOrWhiteSpace(startDate)) filter["startDate"] = startDate;
			if (!string.IsNullOrWhiteSpace(endDate)) filter["endDate"] = endDate;

			var result = await _exportHttpService.DownloadFileAsync<FlightEntity>(fileType, filter, page, pageSize, getAll, cancellationToken);
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
				return RedirectToAction("Index", "Flight");
			}
			if (result.Content == null || result.Content.Length == 0)
			{
				_alertService.SetAlertMessage(TempData, "no_content_available", false);
				return RedirectToAction("Index", "Flight");
			}
			return File(result.Content, result.ContentType, result.FileName);
		}
	}
}