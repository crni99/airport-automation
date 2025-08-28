using AirportAutomation.Core.Entities;
using AirportAutomation.Core.FilterExtensions;
using AirportAutomation.Core.Filters;
using AirportAutomation.Web.Interfaces;
using AirportAutomation.Web.Models.Pilot;
using AirportAutomation.Web.Models.Response;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AirportAutomation.Web.Controllers
{
	[Route("[controller]")]
	public class PilotController : BaseController
	{
		private readonly IHttpCallService _httpCallService;
		private readonly IAlertService _alertService;
		private readonly IMapper _mapper;

		public PilotController(IHttpCallService httpCallService, IAlertService alertService, IMapper mapper)
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
		[Route("GetPilots")]
		public async Task<IActionResult> GetPilots(int page = 1, int pageSize = 10)
		{
			if (page < 1)
			{
				_alertService.SetAlertMessage(TempData, "invalid_page_number", false);
				return Json(new { success = false, message = "Page number must be greater than or equal to 1." });
			}
			var response = await _httpCallService.GetDataList<PilotEntity>(page, pageSize);
			if (response == null)
			{
				return Json(new { success = false, message = "No pilots found." });
			}
			var pagedResponse = _mapper.Map<PagedResponse<PilotViewModel>>(response);
			return Json(new { success = true, data = pagedResponse });
		}

		[HttpGet]
		[Route("Details/{id:int}")]
		public async Task<IActionResult> Details(int id)
		{
			var response = await _httpCallService.GetData<PilotEntity>(id);
			if (response is null)
			{
				_alertService.SetAlertMessage(TempData, "data_not_found", false);
				return RedirectToAction("Index");
			}
			else
			{
				return View(_mapper.Map<PilotViewModel>(response));
			}
		}

		[HttpGet]
		[Route("GetPilotsByName")]
		public async Task<IActionResult> GetPilotsByName([FromQuery] string firstName, [FromQuery] string lastName, int page = 1, int pageSize = 10)
		{
			if (page < 1)
			{
				_alertService.SetAlertMessage(TempData, "invalid_page_number", false);
				return Json(new { success = false, message = "Page number must be greater than or equal to 1." });
			}
			if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
			{
				_alertService.SetAlertMessage(TempData, "missing_field", false);
				return RedirectToAction("Index");
			}
			var response = await _httpCallService.GetDataByFNameOrLName<PilotEntity>(firstName, lastName, page, pageSize);
			if (response == null)
			{
				return Json(new { success = false, message = "No pilots found." });
			}
			var pagedResponse = _mapper.Map<PagedResponse<PilotViewModel>>(response);
			return Json(new { success = true, data = pagedResponse });
		}

		[HttpGet]
		[Route("GetPilotsByFilter")]
		public async Task<IActionResult> GetPilotsByFilter([FromQuery] PilotSearchFilter filter, int page = 1, int pageSize = 10)
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
			var response = await _httpCallService.GetDataByFilter<PilotEntity>(filter, page, pageSize);
			if (response == null)
			{
				return Json(new { success = false, message = "No pilots found." });
			}
			var pagedResponse = _mapper.Map<PagedResponse<PilotViewModel>>(response);
			return Json(new { success = true, data = pagedResponse });
		}

		[HttpGet]
		[Route("Create")]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[Route("CreatePilot")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreatePilot(PilotCreateViewModel pilotCreateDto)
		{
			if (ModelState.IsValid)
			{
				var pilot = _mapper.Map<PilotEntity>(pilotCreateDto);
				var response = await _httpCallService.CreateData<PilotEntity>(pilot);
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
			var response = await _httpCallService.GetData<PilotEntity>(id);
			if (response is null)
			{
				_alertService.SetAlertMessage(TempData, "data_not_found", false);
				return RedirectToAction("Details", new { id });
			}
			else
			{
				return View(_mapper.Map<PilotViewModel>(response));
			}
		}

		[HttpPost]
		[Route("EditPilot")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditPilot(PilotViewModel pilotDto)
		{
			if (ModelState.IsValid)
			{
				var pilot = _mapper.Map<PilotEntity>(pilotDto);
				var response = await _httpCallService.EditData<PilotEntity>(pilot, pilot.Id);
				if (response)
				{
					_alertService.SetAlertMessage(TempData, "edit_data_success", true);
					return RedirectToAction("Details", new { id = pilotDto.Id });
				}
				else
				{
					_alertService.SetAlertMessage(TempData, "edit_data_failed", false);
					return RedirectToAction("Edit", new { id = pilotDto.Id });
				}
			}
			else { return RedirectToAction("Index"); }
		}

		[HttpGet]
		[Route("Delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var response = await _httpCallService.DeleteData<PilotEntity>(id);
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
			[FromQuery] PilotSearchFilter filter,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10,
			[FromQuery] bool getAll = false,
			[FromQuery] string fileType = "pdf")
		{
			var result = await _httpCallService.DownloadFileAsync<PilotEntity>(fileType, filter, page, pageSize, getAll);

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
				return RedirectToAction("Index", "Pilot");
			}

			if (result.Content == null || result.Content.Length == 0)
			{
				_alertService.SetAlertMessage(TempData, "no_content_available", false);
				return RedirectToAction("Index", "Pilot");
			}

			return File(result.Content, result.ContentType, result.FileName);
		}

	}
}