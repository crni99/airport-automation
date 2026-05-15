using AirportAutomation.Core.Entities;
using AirportAutomation.Web.Interfaces;
using AirportAutomation.Web.Models.Airline;
using AirportAutomation.Web.Models.Response;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AirportAutomation.Web.Controllers
{
	[Route("[controller]")]
	public class AirlineController : BaseController
	{
		private readonly IDataHttpService _dataHttpService;
		private readonly ISearchHttpService _searchHttpService;
		private readonly IExportHttpService _exportHttpService;
		private readonly IAlertService _alertService;
		private readonly IMapper _mapper;

		public AirlineController(IDataHttpService dataHttpService, ISearchHttpService searchHttpService, IExportHttpService exportHttpService, IAlertService alertService, IMapper mapper)
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
		[Route("GetAirlines")]
		public async Task<IActionResult> GetAirlines(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
		{
			if (page < 1)
			{
				_alertService.SetAlertMessage(TempData, "invalid_page_number", false);
				return Json(new { success = false, message = "Page number must be greater than or equal to 1." });
			}
			var response = await _dataHttpService.GetDataList<AirlineEntity>(page, pageSize, cancellationToken);
			if (response == null)
			{
				return Json(new { success = false, message = "No airlines found." });
			}
			var pagedResponse = _mapper.Map<PagedResponse<AirlineViewModel>>(response);
			return Json(new { success = true, data = pagedResponse });
		}

		[HttpGet]
		[Route("Details/{id:int}")]
		public async Task<IActionResult> Details(int id, CancellationToken cancellationToken = default)
		{
			var response = await _dataHttpService.GetData<AirlineEntity>(id, cancellationToken);
			if (response is null)
			{
				_alertService.SetAlertMessage(TempData, "data_not_found", false);
				return RedirectToAction("Index");
			}
			return View(_mapper.Map<AirlineViewModel>(response));
		}

		[HttpGet]
		[Route("GetAirlinesByName")]
		public async Task<IActionResult> GetAirlinesByName([FromQuery] string name, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
		{
			if (page < 1)
			{
				_alertService.SetAlertMessage(TempData, "invalid_page_number", false);
				return Json(new { success = false, message = "Page number must be greater than or equal to 1." });
			}
			if (string.IsNullOrEmpty(name))
			{
				_alertService.SetAlertMessage(TempData, "missing_field", false);
				return RedirectToAction("Index");
			}
			var response = await _searchHttpService.GetDataByName<AirlineEntity>(name, page, pageSize, cancellationToken);
			if (response == null)
			{
				return Json(new { success = false, message = "No airlines found." });
			}
			var pagedResponse = _mapper.Map<PagedResponse<AirlineViewModel>>(response);
			return Json(new { success = true, data = pagedResponse });
		}

		[HttpGet]
		[Route("Create")]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[Route("CreateAirline")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateAirline(AirlineCreateViewModel airlineCreateDto, CancellationToken cancellationToken = default)
		{
			if (ModelState.IsValid)
			{
				var airline = _mapper.Map<AirlineEntity>(airlineCreateDto);
				var response = await _dataHttpService.CreateData<AirlineEntity>(airline, cancellationToken);
				if (response is null)
				{
					_alertService.SetAlertMessage(TempData, "create_data_failed", false);
					return RedirectToAction("Create");
				}
				return RedirectToAction("Details", new { id = response.Id });
			}
			else { return RedirectToAction("Index"); }
		}

		[HttpGet]
		[Route("Edit/{id}")]
		public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
		{
			var response = await _dataHttpService.GetData<AirlineEntity>(id, cancellationToken);
			if (response is null)
			{
				_alertService.SetAlertMessage(TempData, "data_not_found", false);
				return RedirectToAction("Details", new { id });
			}
			return View(_mapper.Map<AirlineViewModel>(response));
		}

		[HttpPost]
		[Route("EditAirline")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditAirline(AirlineViewModel airlineDto, CancellationToken cancellationToken = default)
		{
			if (ModelState.IsValid)
			{
				var airline = _mapper.Map<AirlineEntity>(airlineDto);
				var response = await _dataHttpService.EditData<AirlineEntity>(airline, airline.Id, cancellationToken);
				if (response)
				{
					_alertService.SetAlertMessage(TempData, "edit_data_success", true);
					return RedirectToAction("Details", new { id = airlineDto.Id });
				}
				else
				{
					_alertService.SetAlertMessage(TempData, "edit_data_failed", false);
					return RedirectToAction("Edit", new { id = airlineDto.Id });
				}
			}
			else { return RedirectToAction("Index"); }
		}

		[HttpPost]
		[Route("Delete/{id}")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
		{
			var response = await _dataHttpService.DeleteData<AirlineEntity>(id, cancellationToken);
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
			[FromQuery] string name,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10,
			[FromQuery] bool getAll = false,
			[FromQuery] string fileType = "pdf",
			CancellationToken cancellationToken = default)
		{
			var result = await _exportHttpService.DownloadFileAsync<AirlineEntity>(fileType, name, page, pageSize, getAll, cancellationToken);
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
				return RedirectToAction("Index", "Airline");
			}
			if (result.Content == null || result.Content.Length == 0)
			{
				_alertService.SetAlertMessage(TempData, "no_content_available", false);
				return RedirectToAction("Index", "Airline");
			}
			return File(result.Content, result.ContentType, result.FileName);
		}
	}
}