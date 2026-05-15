using AirportAutomation.Core.Entities;
using AirportAutomation.Core.FilterExtensions;
using AirportAutomation.Core.Filters;
using AirportAutomation.Web.Interfaces;
using AirportAutomation.Web.Models.ApiUser;
using AirportAutomation.Web.Models.Response;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AirportAutomation.Web.Controllers
{
	[Route("[controller]")]
	public class ApiUserController : BaseController
	{
		private readonly IDataHttpService _dataHttpService;
		private readonly ISearchHttpService _searchHttpService;
		private readonly IAlertService _alertService;
		private readonly IMapper _mapper;

		public ApiUserController(IDataHttpService dataHttpService, ISearchHttpService searchHttpService, IAlertService alertService, IMapper mapper)
		{
			_dataHttpService = dataHttpService;
			_searchHttpService = searchHttpService;
			_alertService = alertService;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			return View();
		}

		[HttpGet]
		[Route("GetApiUsers")]
		public async Task<IActionResult> GetApiUsers(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
		{
			if (page < 1)
			{
				_alertService.SetAlertMessage(TempData, "invalid_page_number", false);
				return Json(new { success = false, message = "Page number must be greater than or equal to 1." });
			}
			var response = await _dataHttpService.GetDataList<ApiUserEntity>(page, pageSize, cancellationToken);
			if (response == null)
			{
				return Json(new { success = false, message = "No api users found." });
			}
			var pagedResponse = _mapper.Map<PagedResponse<ApiUserViewModel>>(response);
			return Json(new { success = true, data = pagedResponse });
		}

		[HttpGet]
		[Route("{id}")]
		public async Task<IActionResult> Details(int id, CancellationToken cancellationToken = default)
		{
			var response = await _dataHttpService.GetData<ApiUserEntity>(id, cancellationToken);
			if (response is null)
			{
				_alertService.SetAlertMessage(TempData, "data_not_found", false);
				return RedirectToAction("Index");
			}
			return View(_mapper.Map<ApiUserViewModel>(response));
		}

		[HttpGet]
		[Route("SearchApiUsers")]
		public async Task<IActionResult> SearchApiUsers([FromQuery] ApiUserSearchFilter filter, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
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
			var response = await _searchHttpService.GetDataByFilter<ApiUserEntity>(filter, page, pageSize, cancellationToken);
			if (response == null)
			{
				return Json(new { success = false, message = "No api users found." });
			}
			var pagedResponse = _mapper.Map<PagedResponse<ApiUserViewModel>>(response);
			return Json(new { success = true, data = pagedResponse });
		}

		[HttpGet]
		[Route("Create")]
		public IActionResult Create()
		{
			return View();
		}

		[HttpGet]
		[Route("Edit/{id}")]
		public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
		{
			var response = await _dataHttpService.GetData<ApiUserEntity>(id, cancellationToken);
			if (response is null)
			{
				_alertService.SetAlertMessage(TempData, "data_not_found", false);
				return RedirectToAction("Details", new { id });
			}
			return View(_mapper.Map<ApiUserViewModel>(response));
		}

		[HttpPost]
		[Route("EditApiUser")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditApiUser(ApiUserViewModel apiUserDto, CancellationToken cancellationToken = default)
		{
			if (ModelState.IsValid)
			{
				var apiUser = _mapper.Map<ApiUserEntity>(apiUserDto);
				var response = await _dataHttpService.EditData<ApiUserEntity>(apiUser, apiUser.ApiUserId, cancellationToken);
				if (response)
				{
					_alertService.SetAlertMessage(TempData, "edit_data_success", true);
					return RedirectToAction("Details", new { id = apiUserDto.Id });
				}
				else
				{
					_alertService.SetAlertMessage(TempData, "edit_data_failed", false);
					return RedirectToAction("Edit", new { id = apiUserDto.Id });
				}
			}
			else { return RedirectToAction("Index"); }
		}

		[HttpPost]
		[Route("Delete/{id}")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
		{
			var response = await _dataHttpService.DeleteData<ApiUserEntity>(id, cancellationToken);
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
	}
}