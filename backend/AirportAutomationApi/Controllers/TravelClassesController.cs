using AirportAutomation.Api.Helpers;
using AirportAutomation.Api.Interfaces;
using AirportAutomation.Application.Dtos.Response;
using AirportAutomation.Application.Dtos.TravelClass;
using AirportAutomation.Core.Configuration;
using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Enums;
using AirportAutomation.Core.Interfaces;
using AirportAutomation.Core.Interfaces.IServices;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AirportAutomation.Api.Controllers
{
	/// <summary>
	/// Represents the controller for managing Travel Classes.
	/// </summary>
	[Authorize]
	[ApiVersion("1.0")]
	[SwaggerControllerOrder(4)]
	public class TravelClassesController : BaseController
	{
		private readonly ITravelClassService _travelClassService;
		private readonly ICacheService _cacheService;
		private readonly IPaginationValidationService _paginationValidationService;
		private readonly IInputValidationService _inputValidationService;
		private readonly IUtilityService _utilityService;
		private readonly IExportService _exportService;
		private readonly IMapper _mapper;
		private readonly ILogger<TravelClassesController> _logger;
		private readonly int maxPageSize;

		/// <summary>
		/// Initializes a new instance of the <see cref="TravelClassesController"/> class.
		/// </summary>
		/// <param name="travelClassService">The service for managing travel classes.</param>
		/// <param name="cacheService">The service for managing data caching.</param>
		/// <param name="paginationValidationService">The service for validating pagination parameters.</param>
		/// <param name="inputValidationService">The service for validating input data.</param>
		/// <param name="utilityService">The utility service for various helper functions.</param>
		/// <param name="exportService">The service for exporting data.</param>
		/// <param name="mapper">The mapper for object-to-object mapping.</param>
		/// <param name="logger">The logger for logging actions and errors.</param>
		/// <param name="pageSettingsOptions">Typed pagination configuration.</param>
		public TravelClassesController(
			ITravelClassService travelClassService,
			ICacheService cacheService,
			IPaginationValidationService paginationValidationService,
			IInputValidationService inputValidationService,
			IUtilityService utilityService,
			IExportService exportService,
			IMapper mapper,
			ILogger<TravelClassesController> logger,
			IOptions<PageSettings> pageSettingsOptions
		)
		{
			_travelClassService = travelClassService ?? throw new ArgumentNullException(nameof(travelClassService));
			_cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
			_paginationValidationService = paginationValidationService ?? throw new ArgumentNullException(nameof(paginationValidationService));
			_inputValidationService = inputValidationService ?? throw new ArgumentNullException(nameof(inputValidationService));
			_utilityService = utilityService ?? throw new ArgumentNullException(nameof(utilityService));
			_exportService = exportService ?? throw new ArgumentNullException(nameof(exportService));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			maxPageSize = pageSettingsOptions?.Value?.MaxPageSize ?? 20;
		}

		/// <summary>
		/// Endpoint for retrieving a paginated list of travel classes.
		/// </summary>
		/// <param name="page">The page number for pagination (optional).</param>
		/// <param name="pageSize">The number of items per page (optional).</param>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <returns>A paginated list of travel classes.</returns>
		/// <response code="200">Returns a list of travel classes wrapped in a <see cref="PagedResponse{TravelClassDto}"/>.</response>
		/// <response code="204">If no travel classes are found.</response>
		/// <response code="401">If the user is not authenticated.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet]
		[ProducesResponseType(200, Type = typeof(PagedResponse<TravelClassDto>))]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public Task<ActionResult<PagedResponse<TravelClassDto>>> GetTravelClasses(
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10,
			CancellationToken cancellationToken = default)
			=> GetPagedAsync<TravelClassEntity, TravelClassDto>(
				_cacheService, _paginationValidationService, _mapper, _logger,
				page, pageSize, maxPageSize,
				cacheKey: CacheKeys.TravelClasses(page, pageSize),
				fetchItems: () => _travelClassService.GetTravelClasses(cancellationToken, page, pageSize),
				fetchCount: () => _travelClassService.TravelClassesCount(cancellationToken));

		/// <summary>
		/// Endpoint for retrieving a single travel class.
		/// </summary>
		/// <param name="id">The ID of the travel class to retrieve.</param>
		/// <returns>A single travel class that matches the specified ID.</returns>
		/// <response code="200">Returns a single travel class if found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no travel class is found.</response>
		/// <response code="401">If the user is not authenticated.</response>
		[HttpGet("{id}")]
		[ProducesResponseType(200, Type = typeof(TravelClassDto))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public Task<ActionResult<TravelClassDto>> GetTravelClass(int id)
			=> GetByIdAsync<TravelClassEntity, TravelClassDto>(
				_cacheService, _inputValidationService, _mapper, _logger,
				id,
				cacheKey: CacheKeys.TravelClass(id),
				fetchItem: () => _travelClassService.GetTravelClass(id));

		/// <summary>
		/// Endpoint for exporting travel class data to PDF.
		/// </summary>
		/// <param name="page">The page number for pagination (optional, default is 1).</param>
		/// <param name="pageSize">The page size for pagination (optional, default is 10).</param>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <returns>Returns the generated PDF document.</returns>
		/// <response code="200">Returns the generated PDF document.</response>
		/// <response code="204">No travel classes found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If the user is not authenticated.</response>
		/// <response code="403">If the authenticated user does not have permission to access the requested resource.</response>
		/// <response code="500">If an unexpected server error occurs, such as a failure during PDF generation.</response>
		[HttpGet("export/pdf")]
		[Authorize(Policy = "RequireAdminRole")]
		[Produces("application/pdf", "application/json")]
		[ProducesResponseType(200)]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		[ProducesResponseType(500)]
		public Task<ActionResult> ExportToPdf(
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10,
			CancellationToken cancellationToken = default)
			=> ExportAsync<TravelClassEntity>(
				entityName: "TravelClasses",
				fileType: FileExtension.Pdf,
				exportService: _exportService,
				paginationValidation: _paginationValidationService,
				inputValidation: _inputValidationService,
				logger: _logger,
				page, pageSize, maxPageSize,
				getAll: false,
				fetchAll: () => Task.FromResult<IList<TravelClassEntity>>(new List<TravelClassEntity>()),
				fetchPaged: (p, ps) => _travelClassService.GetTravelClasses(cancellationToken, p, ps),
				fetchSearch: null);

		/// <summary>
		/// Endpoint for exporting travel class data to Excel.
		/// </summary>
		/// <param name="page">The page number for pagination (optional, default is 1).</param>
		/// <param name="pageSize">The page size for pagination (optional, default is 10).</param>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <returns>Returns the generated Excel document.</returns>
		/// <response code="200">Returns the generated Excel document.</response>
		/// <response code="204">No travel classes found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If the user is not authenticated.</response>
		/// <response code="403">If the authenticated user does not have permission to access the requested resource.</response>
		/// <response code="500">If an unexpected server error occurs, such as a failure during Excel generation.</response>
		[HttpGet("export/excel")]
		[Authorize(Policy = "RequireAdminRole")]
		[Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/json")]
		[ProducesResponseType(200)]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		[ProducesResponseType(500)]
		public Task<ActionResult> ExportToExcel(
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10,
			CancellationToken cancellationToken = default)
			=> ExportAsync<TravelClassEntity>(
				entityName: "TravelClasses",
				fileType: FileExtension.Xlsx,
				exportService: _exportService,
				paginationValidation: _paginationValidationService,
				inputValidation: _inputValidationService,
				logger: _logger,
				page, pageSize, maxPageSize,
				getAll: false,
				fetchAll: () => Task.FromResult<IList<TravelClassEntity>>(new List<TravelClassEntity>()),
				fetchPaged: (p, ps) => _travelClassService.GetTravelClasses(cancellationToken, p, ps),
				fetchSearch: null);

	}
}
