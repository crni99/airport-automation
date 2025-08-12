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
		private readonly IHttpCallService _httpCallService;
		private readonly IAlertService _alertService;
		private readonly IMapper _mapper;

		public ApiUserController(IHttpCallService httpCallService, IAlertService alertService, IMapper mapper)
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
		[Route("GetApiUsers")]
		public async Task<IActionResult> GetApiUsers(int page = 1, int pageSize = 10)
		{
			if (page < 1)
			{
				_alertService.SetAlertMessage(TempData, "invalid_page_number", false);
				return Json(new { success = false, message = "Page number must be greater than or equal to 1." });
			}
			var response = await _httpCallService.GetDataList<ApiUserEntity>(page, pageSize);
			if (response == null)
			{
				return Json(new { success = false, message = "No api users found." });
			}
			var pagedResponse = _mapper.Map<PagedResponse<ApiUserViewModel>>(response);
			return Json(new { success = true, data = pagedResponse });
		}

		[HttpGet]
		[Route("{id}")]
		public async Task<IActionResult> Details(int id)
		{
			var response = await _httpCallService.GetData<ApiUserEntity>(id);
			if (response is null)
			{
				_alertService.SetAlertMessage(TempData, "data_not_found", false);
				return RedirectToAction("Index");
			}
			else
			{
				return View(_mapper.Map<ApiUserViewModel>(response));
			}
		}

		[HttpGet]
		[Route("GetApiUsersByRole")]
		public async Task<IActionResult> GetApiUsersByName([FromQuery] string role, int page = 1, int pageSize = 10)
		{
			if (page < 1)
			{
				_alertService.SetAlertMessage(TempData, "invalid_page_number", false);
				return Json(new { success = false, message = "Page number must be greater than or equal to 1." });
			}
			if (string.IsNullOrEmpty(role))
			{
				_alertService.SetAlertMessage(TempData, "missing_field", false);
				return RedirectToAction("Index");
			}
			var response = await _httpCallService.GetDataByRole<ApiUserEntity>(role, page, pageSize);
			if (response == null)
			{
				return Json(new { success = false, message = "No api users found." });
			}
			var pagedResponse = _mapper.Map<PagedResponse<ApiUserViewModel>>(response);
			return Json(new { success = true, data = pagedResponse });
		}

		[HttpGet]
		[Route("GetApiUsersByFilter")]
		public async Task<IActionResult> GetApiUsersByFilter([FromQuery] ApiUserSearchFilter filter, int page = 1, int pageSize = 10)
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
			var response = await _httpCallService.GetDataByFilter<ApiUserEntity>(filter, page, pageSize);
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
		public async Task<IActionResult> Edit(int id)
		{
			var response = await _httpCallService.GetData<ApiUserEntity>(id);
			if (response is null)
			{
				_alertService.SetAlertMessage(TempData, "data_not_found", false);
				return RedirectToAction("Details", new { id });
			}
			else
			{
				return View(_mapper.Map<ApiUserViewModel>(response));
			}
		}

		[HttpPost]
		[Route("EditApiUser")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditApiUser(ApiUserViewModel apiUserDto)
		{
			if (ModelState.IsValid)
			{
				var apiUser = _mapper.Map<ApiUserEntity>(apiUserDto);
				var response = await _httpCallService.EditData<ApiUserEntity>(apiUser, apiUser.ApiUserId);
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

		[HttpGet]
		[Route("Delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var response = await _httpCallService.DeleteData<ApiUserEntity>(id);
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