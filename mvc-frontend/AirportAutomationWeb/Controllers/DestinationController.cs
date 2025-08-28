using AirportAutomation.Core.Entities;
using AirportAutomation.Core.FilterExtensions;
using AirportAutomation.Core.Filters;
using AirportAutomation.Web.Interfaces;
using AirportAutomation.Web.Models.Destination;
using AirportAutomation.Web.Models.Response;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace AirportAutomation.Web.Controllers
{
	[Route("[controller]")]
	public class DestinationController : BaseController
	{
		private readonly IHttpCallService _httpCallService;
		private readonly IAlertService _alertService;
		private readonly IMapper _mapper;

		public DestinationController(IHttpCallService httpCallService, IAlertService alertService, IMapper mapper)
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
		[Route("GetDestinations")]
		public async Task<IActionResult> GetDestinations(int page = 1, int pageSize = 10)
		{
			if (page < 1)
			{
				_alertService.SetAlertMessage(TempData, "invalid_page_number", false);
				return Json(new { success = false, message = "Page number must be greater than or equal to 1." });
			}
			var response = await _httpCallService.GetDataList<DestinationEntity>(page, pageSize);
			if (response == null)
			{
				return Json(new { success = false, message = "No destinations found." });
			}
			var pagedResponse = _mapper.Map<PagedResponse<DestinationViewModel>>(response);
			return Json(new { success = true, data = pagedResponse });
		}

		[HttpGet]
		[Route("Details/{id:int}")]
		public async Task<IActionResult> Details(int id)
		{
			var response = await _httpCallService.GetData<DestinationEntity>(id);
			if (response is null)
			{
				_alertService.SetAlertMessage(TempData, "data_not_found", false);
				return RedirectToAction("Index");
			}
			else
			{
				return View(_mapper.Map<DestinationViewModel>(response));
			}
		}

		[HttpGet]
		[Route("GetDestinationsByCityOrAirport")]
		public async Task<IActionResult> GetDestinationsByCityOrAirport([FromQuery] string city, [FromQuery] string airport, int page = 1, int pageSize = 10)
		{
			if (page < 1)
			{
				_alertService.SetAlertMessage(TempData, "invalid_page_number", false);
				return Json(new { success = false, message = "Page number must be greater than or equal to 1." });
			}
			if (string.IsNullOrEmpty(city) && string.IsNullOrEmpty(airport))
			{
				_alertService.SetAlertMessage(TempData, "missing_field", false);
				return RedirectToAction("Index");
			}
			var response = await _httpCallService.GetDataByCityOrAirport<DestinationEntity>(city, airport, page, pageSize);
			if (response == null)
			{
				return Json(new { success = false, message = "No destinations found." });
			}
			var pagedResponse = _mapper.Map<PagedResponse<DestinationViewModel>>(response);
			return Json(new { success = true, data = pagedResponse });
		}

		[HttpGet]
		[Route("GetDestinationsByFilter")]
		public async Task<IActionResult> GetDestinationsByFilter([FromQuery] DestinationSearchFilter filter, int page = 1, int pageSize = 10)
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
			var response = await _httpCallService.GetDataByFilter<DestinationEntity>(filter, page, pageSize);
			if (response == null)
			{
				return Json(new { success = false, message = "No destinations found." });
			}
			var pagedResponse = _mapper.Map<PagedResponse<DestinationViewModel>>(response);
			return Json(new { success = true, data = pagedResponse });
		}

		[HttpGet]
		[Route("Create")]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[Route("CreateDestination")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateDestination(DestinationCreateViewModel destinationCreateDto)
		{
			if (ModelState.IsValid)
			{
				var destination = _mapper.Map<DestinationEntity>(destinationCreateDto);
				var response = await _httpCallService.CreateData<DestinationEntity>(destination);
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
			var response = await _httpCallService.GetData<DestinationEntity>(id);
			if (response is null)
			{
				_alertService.SetAlertMessage(TempData, "data_not_found", false);
				return RedirectToAction("Details", new { id });
			}
			else
			{
				return View(_mapper.Map<DestinationViewModel>(response));
			}
		}

		[HttpPost]
		[Route("EditDestination")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditDestination(DestinationViewModel destinationDto)
		{
			if (ModelState.IsValid)
			{
				var destination = _mapper.Map<DestinationEntity>(destinationDto);
				var response = await _httpCallService.EditData<DestinationEntity>(destination, destination.Id);
				if (response)
				{
					_alertService.SetAlertMessage(TempData, "edit_data_success", true);
					return RedirectToAction("Details", new { id = destinationDto.Id });
				}
				else
				{
					_alertService.SetAlertMessage(TempData, "edit_data_failed", false);
					return RedirectToAction("Edit", new { id = destinationDto.Id });
				}
			}
			else { return RedirectToAction("Index"); }
		}

		[HttpGet]
		[Route("Delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var response = await _httpCallService.DeleteData<DestinationEntity>(id);
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
			[FromQuery] DestinationSearchFilter filter,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10,
			[FromQuery] bool getAll = false,
			[FromQuery] string fileType = "pdf")
		{
			var result = await _httpCallService.DownloadFileAsync<DestinationEntity>(fileType, filter, page, pageSize, getAll);

			if (result == null || result.Content.Length == 0)
			{
				return NoContent();
			}
			return File(result.Content, result.ContentType, result.FileName);
		}

	}
}