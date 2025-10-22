using AirportAutomation.Core.Entities;
using AirportAutomation.Core.FilterExtensions;
using AirportAutomation.Core.Filters;
using AirportAutomation.Web.Interfaces;
using AirportAutomation.Web.Models.Passenger;
using AirportAutomation.Web.Models.Response;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AirportAutomation.Web.Controllers
{
	[Route("[controller]")]
	public class PassengerController : BaseController
	{
		private readonly IHttpCallService _httpCallService;
		private readonly IAlertService _alertService;
		private readonly IMapper _mapper;

		public PassengerController(IHttpCallService httpCallService, IAlertService alertService, IMapper mapper)
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
		[Route("GetPassengers")]
		public async Task<IActionResult> GetPassengers(int page = 1, int pageSize = 10)
		{
			if (page < 1)
			{
				_alertService.SetAlertMessage(TempData, "invalid_page_number", false);
				return Json(new { success = false, message = "Page number must be greater than or equal to 1." });
			}
			var response = await _httpCallService.GetDataList<PassengerEntity>(page, pageSize);
			if (response == null)
			{
				return Json(new { success = false, message = "No passengers found." });
			}
			var pagedResponse = _mapper.Map<PagedResponse<PassengerViewModel>>(response);
			return Json(new { success = true, data = pagedResponse });
		}

		[HttpGet]
		[Route("Details/{id:int}")]
		public async Task<IActionResult> Details(int id)
		{
			var response = await _httpCallService.GetData<PassengerEntity>(id);
			if (response is null)
			{
				_alertService.SetAlertMessage(TempData, "data_not_found", false);
				return RedirectToAction("Index");
			}
			else
			{
				return View(_mapper.Map<PassengerViewModel>(response));
			}
		}

		[HttpGet]
		[Route("SearchPassengers")]
		public async Task<IActionResult> SearchPassengers([FromQuery] PassengerSearchFilter filter, int page = 1, int pageSize = 10)
		{
			if (page < 1)
			{
				_alertService.SetAlertMessage(TempData, "invalid_page_number", false);
				return Json(new { success = false, message = "Page number must be greater than or equal to 1." });
			}
			if (filter.IsEmpty())
			{
				_alertService.SetAlertMessage(TempData, "missing_field", false);
				return RedirectToAction("Index");
			}
			var response = await _httpCallService.GetDataByFilter<PassengerEntity>(filter, page, pageSize);
			if (response == null)
			{
				return Json(new { success = false, message = "No passengers found." });
			}
			var pagedResponse = _mapper.Map<PagedResponse<PassengerViewModel>>(response);
			return Json(new { success = true, data = pagedResponse });
		}

		[HttpGet]
		[Route("Create")]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[Route("CreatePassenger")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreatePassenger(PassengerCreateViewModel passengerCreateDto)
		{
			if (ModelState.IsValid)
			{
				var passenger = _mapper.Map<PassengerEntity>(passengerCreateDto);
				var response = await _httpCallService.CreateData<PassengerEntity>(passenger);
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
			else { return RedirectToAction("Index"); }
		}

		[HttpGet]
		[Route("Edit/{id}")]
		public async Task<IActionResult> Edit(int id)
		{
			var response = await _httpCallService.GetData<PassengerEntity>(id);
			if (response is null)
			{
				_alertService.SetAlertMessage(TempData, "data_not_found", false);
				return RedirectToAction("Details", new { id });
			}
			else
			{
				return View(_mapper.Map<PassengerViewModel>(response));
			}
		}

		[HttpPost]
		[Route("EditPassenger")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditPassenger(PassengerViewModel passengerDto)
		{
			if (ModelState.IsValid)
			{
				var passenger = _mapper.Map<PassengerEntity>(passengerDto);
				var response = await _httpCallService.EditData<PassengerEntity>(passenger, passenger.Id);
				if (response)
				{
					_alertService.SetAlertMessage(TempData, "edit_data_success", true);
					return RedirectToAction("Details", new { id = passengerDto.Id });
				}
				else
				{
					_alertService.SetAlertMessage(TempData, "edit_data_failed", false);
					return RedirectToAction("Edit", new { id = passengerDto.Id });
				}
			}
			else { return RedirectToAction("Index"); }
		}

		[HttpGet]
		[Route("Delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var response = await _httpCallService.DeleteData<PassengerEntity>(id);
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
			[FromQuery] PassengerSearchFilter filter,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10,
			[FromQuery] bool getAll = false,
			[FromQuery] string fileType = "pdf")
		{
			var result = await _httpCallService.DownloadFileAsync<PassengerEntity>(fileType, filter, page, pageSize, getAll);

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
				return RedirectToAction("Index", "Passenger");
			}

			if (result.Content == null || result.Content.Length == 0)
			{
				_alertService.SetAlertMessage(TempData, "no_content_available", false);
				return RedirectToAction("Index", "Passenger");
			}

			return File(result.Content, result.ContentType, result.FileName);
		}

	}
}