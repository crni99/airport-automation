using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Filters;
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
		private readonly IHttpCallService _httpCallService;
		private readonly IAlertService _alertService;
		private readonly IMapper _mapper;

		public FlightController(IHttpCallService httpCallService, IAlertService alertService, IMapper mapper)
		{
			_httpCallService = httpCallService;
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
		public async Task<IActionResult> GetFlights(int page = 1, int pageSize = 10)
		{
			if (page < 1)
			{
				_alertService.SetAlertMessage(TempData, "invalid_page_number", false);
				return Json(new { success = false, message = "Page number must be greater than or equal to 1." });
			}
			var response = await _httpCallService.GetDataList<FlightEntity>(page, pageSize);
			if (response == null)
			{
				return Json(new { success = false, message = "No flights found." });
			}
			var pagedResponse = _mapper.Map<PagedResponse<FlightViewModel>>(response);
			return Json(new { success = true, data = pagedResponse });
		}

		[HttpGet]
		[Route("Details/{id:int}")]
		public async Task<IActionResult> Details(int id)
		{
			var response = await _httpCallService.GetData<FlightEntity>(id);
			if (response is null)
			{
				_alertService.SetAlertMessage(TempData, "data_not_found", false);
				return RedirectToAction("Index");
			}
			else
			{
				return View(_mapper.Map<FlightViewModel>(response));
			}
		}

		[HttpGet]
		[Route("GetFlightsBetweenDates")]
		public async Task<IActionResult> GetFlightsBetweenDates([FromQuery] string startDate, [FromQuery] string endDate, int page = 1, int pageSize = 10)
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
			var response = await _httpCallService.GetDataBetweenDates<FlightEntity>(startDate, endDate, page, pageSize);
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
		public async Task<IActionResult> CreateFlight(FlightCreateViewModel flightCreateDto)
		{
			var flight = _mapper.Map<FlightEntity>(flightCreateDto);
			var response = await _httpCallService.CreateData<FlightEntity>(flight);
			if (response is null)
			{
				_alertService.SetAlertMessage(TempData, "create_data_failed", false);
				return RedirectToAction("Create");
			}
			else
			{
				return RedirectToAction("Details", new { id = response.Id });
			}
		}

		[HttpGet]
		[Route("Edit/{id}")]
		public async Task<IActionResult> Edit(int id)
		{
			var response = await _httpCallService.GetData<FlightEntity>(id);
			if (response is null)
			{
				_alertService.SetAlertMessage(TempData, "data_not_found", false);
				return RedirectToAction("Details", new { id });
			}
			else
			{
				return View(_mapper.Map<FlightViewModel>(response));
			}
		}

		[HttpPost]
		[Route("EditFlight")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditFlight(FlightViewModel flightDto)
		{
			var flight = _mapper.Map<FlightEntity>(flightDto);
			var response = await _httpCallService.EditData<FlightEntity>(flight, flight.Id);
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

		[HttpGet]
		[Route("Delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var response = await _httpCallService.DeleteData<FlightEntity>(id);
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
			[FromQuery] string fileType = "pdf")
		{
			var filter = new Dictionary<string, string>();
			if (!string.IsNullOrWhiteSpace(startDate))
			{
				filter["startDate"] = startDate;
			}
			if (!string.IsNullOrWhiteSpace(endDate))
			{
				filter["endDate"] = endDate;
			}

			var result = await _httpCallService.DownloadFileAsync<FlightEntity>(fileType, filter, page, pageSize, getAll);

			if (result == null || result.Content.Length == 0)
			{
				return NoContent();
			}
			return File(result.Content, result.ContentType, result.FileName);
		}

	}
}