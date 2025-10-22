using AirportAutomation.Api.Helpers;
using AirportAutomation.Api.Interfaces;
using AirportAutomation.Application.Dtos.Pilot;
using AirportAutomation.Application.Dtos.Response;
using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Enums;
using AirportAutomation.Core.FilterExtensions;
using AirportAutomation.Core.Filters;
using AirportAutomation.Core.Interfaces.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AirportAutomation.Api.Controllers
{
	/// <summary>
	/// Represents the controller for managing Pilots.
	/// </summary>
	[Authorize]
	[ApiVersion("1.0")]
	[SwaggerControllerOrder(5)]
	public class PilotsController : BaseController
	{
		private readonly IPilotService _pilotService;
		private readonly IPaginationValidationService _paginationValidationService;
		private readonly IInputValidationService _inputValidationService;
		private readonly IUtilityService _utilityService;
		private readonly IExportService _exportService;
		private readonly IMapper _mapper;
		private readonly ILogger<PilotsController> _logger;
		private readonly int maxPageSize;

		/// <summary>
		/// Initializes a new instance of the <see cref="PilotsController"/> class.
		/// </summary>
		/// <param name="pilotService">The service for managing pilots.</param>
		/// <param name="paginationValidationService">The service for validating pagination parameters.</param>
		/// <param name="inputValidationService">The service for validating input data.</param>
		/// <param name="utilityService">The utility service for various helper functions.</param>
		/// <param name="exportService">The service for exporting data.</param>
		/// <param name="mapper">The mapper for object-to-object mapping.</param>
		/// <param name="logger">The logger for logging actions and errors.</param>
		/// <param name="configuration">The application configuration.</param>
		public PilotsController(
			IPilotService pilotService,
			IPaginationValidationService paginationValidationService,
			IInputValidationService inputValidationService,
			IUtilityService utilityService,
			IExportService exportService,
			IMapper mapper,
			ILogger<PilotsController> logger,
			IConfiguration configuration)
		{
			_pilotService = pilotService ?? throw new ArgumentNullException(nameof(pilotService));
			_paginationValidationService = paginationValidationService ?? throw new ArgumentNullException(nameof(paginationValidationService));
			_inputValidationService = inputValidationService ?? throw new ArgumentNullException(nameof(inputValidationService));
			_utilityService = utilityService ?? throw new ArgumentNullException(nameof(utilityService));
			_exportService = exportService ?? throw new ArgumentNullException(nameof(exportService));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			maxPageSize = configuration.GetValue<int>("pageSettings:maxPageSize");
		}

		/// <summary>
		/// Endpoint for retrieving a paginated list of pilots.
		/// </summary>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <param name="page">The page number for pagination (optional).</param>
		/// <param name="pageSize">The number of items per page (optional).</param>
		/// <returns>A paginated list of pilots.</returns>
		/// <response code="200">Returns a list of pilots wrapped in a <see cref="PagedResponse{PilotDto}"/>.</response>
		/// <response code="204">If no pilots are found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If the user is not authenticated.</response>
		[HttpGet]
		[ProducesResponseType(200, Type = typeof(PagedResponse<PilotDto>))]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PagedResponse<PilotDto>>> GetPilots(
			CancellationToken cancellationToken,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10)
		{
			var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
			if (!isValid)
			{
				return result;
			}
			var pilots = await _pilotService.GetPilots(cancellationToken, page, correctedPageSize);
			if (pilots is null || !pilots.Any())
			{
				_logger.LogInformation("Pilots not found.");
				return NoContent();
			}
			var totalItems = await _pilotService.PilotsCount(cancellationToken);
			var data = _mapper.Map<IEnumerable<PilotDto>>(pilots);
			var response = new PagedResponse<PilotDto>(data, page, correctedPageSize, totalItems);
			return Ok(response);
		}

		/// <summary>
		/// Endpoint for retrieving a single pilot.
		/// </summary>
		/// <param name="id">The ID of the pilot to retrieve.</param>
		/// <returns>A single pilot that matches the specified ID.</returns>
		/// <response code="200">Returns a single pilot if found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no pilot is found.</response>
		/// <response code="401">If the user is not authenticated.</response>
		[HttpGet("{id}")]
		[ProducesResponseType(200, Type = typeof(PilotDto))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PilotDto>> GetPilot(int id)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {Id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (!await _pilotService.PilotExists(id))
			{
				_logger.LogInformation("Pilot with id {Id} not found.", id);
				return NotFound();
			}
			var pilot = await _pilotService.GetPilot(id);
			var pilotDto = _mapper.Map<PilotDto>(pilot);
			return Ok(pilotDto);
		}

		/// <summary>
		/// Endpoint for retrieving a paginated list of pilots matching the specified search filter criteria.
		/// </summary>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <param name="filter">The search filter containing pilot fields to filter by.</param>
		/// <param name="page">The page number for pagination (optional, default is 1).</param>
		/// <param name="pageSize">The number of items per page for pagination (optional, default is 10).</param>
		/// <returns>A paged response containing the list of pilots that match the filter criteria.</returns>
		/// <response code="200">Returns a paged list of pilots if found.</response>
		/// <response code="400">If the request is invalid or the filter criteria are missing or invalid.</response>
		/// <response code="404">If no pilots matching the filter criteria are found.</response>
		/// <response code="401">If the user is not authenticated.</response>
		[HttpGet("search")]
		[ProducesResponseType(200, Type = typeof(PagedResponse<PilotDto>))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PagedResponse<PilotDto>>> SearchPilots(
			CancellationToken cancellationToken,
			[FromQuery] PilotSearchFilter filter,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10)
		{
			if (filter.IsEmpty())
			{
				_logger.LogInformation("At least one filter criterion must be provided.");
				return BadRequest("At least one filter criterion must be provided.");
			}
			var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
			if (!isValid)
			{
				return result;
			}
			var pilots = await _pilotService.SearchPilots(cancellationToken, page, correctedPageSize, filter);
			if (pilots == null || pilots.Count == 0)
			{
				_logger.LogInformation("Pilots not found.");
				return NotFound();
			}
			var totalItems = await _pilotService.PilotsCountFilter(cancellationToken, filter);
			var data = _mapper.Map<IEnumerable<PilotDto>>(pilots);
			var response = new PagedResponse<PilotDto>(data, page, correctedPageSize, totalItems);
			return Ok(response);
		}

		/// <summary>
		/// Endpoint for creating a new pilot.
		/// </summary>
		/// <param name="pilotCreateDto">The data to create the new pilot.</param>
		/// <returns>The created pilot.</returns>
		/// <response code="201">Returns the created pilot if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If the user is not authenticated.</response>
		/// <response code="403">If the authenticated user does not have permission to access the requested resource.</response>
		[HttpPost]
		[Authorize(Policy = "RequireAdminRole")]
		[ProducesResponseType(201, Type = typeof(PilotDto))]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		public async Task<ActionResult<PilotDto>> PostPilot(PilotCreateDto pilotCreateDto)
		{
			var pilot = _mapper.Map<PilotEntity>(pilotCreateDto);
			await _pilotService.PostPilot(pilot);
			var pilotDto = _mapper.Map<PilotDto>(pilot);
			return CreatedAtAction("GetPilot", new { id = pilotDto.Id }, pilotDto);
		}

		/// <summary>
		/// Endpoint for updating a single pilot.
		/// </summary>
		/// <param name="id">The ID of the pilot to update.</param>
		/// <param name="pilotDto">The data to update the pilot.</param>
		/// <returns>No content.</returns>
		/// <response code="204">Returns no content if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no pilot is found.</response>
		/// <response code="401">If the user is not authenticated.</response>
		/// <response code="403">If the authenticated user does not have permission to access the requested resource.</response>
		[HttpPut("{id}")]
		[Authorize(Policy = "RequireAdminRole")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		public async Task<IActionResult> PutPilot(int id, PilotDto pilotDto)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {Id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (id != pilotDto.Id)
			{
				_logger.LogInformation("Pilot with id {Id} is different from provided Pilot and his id.", id);
				return BadRequest();
			}
			if (!await _pilotService.PilotExists(id))
			{
				_logger.LogInformation("Pilot with id {Id} not found.", id);
				return NotFound();
			}
			var pilot = _mapper.Map<PilotEntity>(pilotDto);
			await _pilotService.PutPilot(pilot);
			return NoContent();
		}

		/// <summary>
		/// Endpoint for partially updating a single pilot.
		/// </summary>
		/// <param name="id">The ID of the pilot to partially update.</param>
		/// <param name="pilotDocument">The patch document containing the changes.</param>
		/// <returns>The updated pilot.</returns>
		/// <remarks>
		/// The JSON document should follow the JSON Patch standard (RFC 6902) and contain one or more operations.
		/// Example operation:
		/// {
		///     "op": "replace",
		///     "path": "/Name",
		///     "value": "NewName"
		/// }
		/// </remarks>
		/// <response code="200">Returns the updated pilot if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If the pilot with the specified ID is not found.</response>
		/// <response code="401">If the user is not authenticated.</response>
		/// <response code="403">If the authenticated user does not have permission to access the requested resource.</response>
		[HttpPatch("{id}")]
		[Authorize(Policy = "RequireAdminRole")]
		[ProducesResponseType(200, Type = typeof(PilotDto))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		public async Task<IActionResult> PatchPilot(int id, [FromBody] JsonPatchDocument pilotDocument)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {Id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (!await _pilotService.PilotExists(id))
			{
				_logger.LogInformation("Pilot with id {Id} not found.", id);
				return NotFound();
			}
			var updatedPilot = await _pilotService.PatchPilot(id, pilotDocument);
			var pilotDto = _mapper.Map<PilotDto>(updatedPilot);
			return Ok(pilotDto);
		}

		/// <summary>
		/// Endpoint for deleting a single pilot.
		/// </summary>
		/// <param name="id">The ID of the pilot to delete.</param>
		/// <returns>No content.</returns>
		/// <response code="204">Returns no content if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no pilot is found.</response>
		/// <response code="401">If the user is not authenticated.</response>
		/// <response code="403">If the authenticated user does not have permission to access the requested resource.</response>
		/// <response code="409">Conflict. If the passenger cannot be deleted because it is being referenced by other entities.</response>
		[HttpDelete("{id}")]
		[Authorize(Policy = "RequireAdminRole")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		[ProducesResponseType(409)]
		public async Task<IActionResult> DeletePilot(int id)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {Id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (!await _pilotService.PilotExists(id))
			{
				_logger.LogInformation("Pilot with id {Id} not found.", id);
				return NotFound();
			}
			bool deleted = await _pilotService.DeletePilot(id);
			if (deleted)
			{
				return NoContent();
			}
			else
			{
				_logger.LogInformation("Pilot with id {Id} is being referenced by other entities and cannot be deleted.", id);
				return Conflict("Pilot cannot be deleted because it is being referenced by other entities.");
			}
		}

		/// <summary>
		/// Endpoint for exporting pilot data to PDF.
		/// </summary>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <param name="filter">The search filter containing destination fields to filter by.</param>
		/// <param name="page">The page number for pagination (optional, default is 1).</param>
		/// <param name="pageSize">The page size for pagination (optional, default is 10).</param>
		/// <param name="getAll">Flag indicating whether to retrieve all data (optional, default is false).</param>
		/// <returns>Returns the generated PDF document.</returns>
		/// <response code="200">Returns the generated PDF document.</response>
		/// <response code="204">No pilots found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If the user is not authenticated.</response>
		/// <response code="403">If the authenticated user does not have permission to access the requested resource.</response>
		/// <response code="500">If an unexpected server error occurs, such as a failure during PDF generation.</response>
		[HttpGet("export/pdf")]
		[Authorize(Policy = "RequireAdminRole")]
		[Produces("application/pdf")]
		[ProducesResponseType(200)]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		[ProducesResponseType(500)]
		public async Task<ActionResult> ExportToPdf(
			CancellationToken cancellationToken,
			[FromQuery] PilotSearchFilter filter,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10,
			[FromQuery] bool getAll = false)
		{
			IList<PilotEntity> pilots;
			if (getAll)
			{
				pilots = await _pilotService.GetAllPilots(cancellationToken);
			}
			else
			{
				var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
				if (!isValid)
				{
					return result;
				}
				if (filter.IsEmpty())
				{
					pilots = await _pilotService.GetPilots(cancellationToken, page, correctedPageSize);
				}
				else
				{
					pilots = await _pilotService.SearchPilots(cancellationToken, page, correctedPageSize, filter);
				}
			}
			if (pilots is null || !pilots.Any())
			{
				_logger.LogInformation("No pilots found for page {Page}, pageSize {PageSize}, getAll {GetAll}, filter {Filter}.",
					page, pageSize, getAll, JsonConvert.SerializeObject(filter));
				return NoContent();
			}
			var pdf = _exportService.ExportToPDF("Pilots", pilots);
			if (pdf == null)
			{
				_logger.LogError("PDF generation failed.");
				return StatusCode(500, "Failed to generate PDF file.");
			}
			string fileName = _utilityService.GenerateUniqueFileName("Pilots", FileExtension.Pdf);
			return File(pdf, "application/pdf", fileName);
		}

		/// <summary>
		/// Endpoint for exporting pilot data to Excel.
		/// </summary>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <param name="filter">The search filter containing pilot fields to filter by.</param>
		/// <param name="page">The page number for pagination (optional, default is 1).</param>
		/// <param name="pageSize">The page size for pagination (optional, default is 10).</param>
		/// <param name="getAll">Flag indicating whether to retrieve all data (optional, default is false).</param>
		/// <returns>Returns the generated Excel document.</returns>
		/// <response code="200">Returns the generated Excel document.</response>
		/// <response code="204">No pilots found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If the user is not authenticated.</response>
		/// <response code="403">If the authenticated user does not have permission to access the requested resource.</response>
		/// <response code="500">If an unexpected server error occurs, such as a failure during Excel generation.</response>
		[HttpGet("export/excel")]
		[Authorize(Policy = "RequireAdminRole")]
		[Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
		[ProducesResponseType(200)]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		[ProducesResponseType(500)]
		public async Task<ActionResult> ExportToExcel(
			CancellationToken cancellationToken,
			[FromQuery] PilotSearchFilter filter,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10,
			[FromQuery] bool getAll = false)
		{
			IList<PilotEntity> pilots;

			if (getAll)
			{
				pilots = await _pilotService.GetAllPilots(cancellationToken);
			}
			else
			{
				var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
				if (!isValid)
				{
					return result;
				}

				if (filter.IsEmpty())
				{
					pilots = await _pilotService.GetPilots(cancellationToken, page, correctedPageSize);
				}
				else
				{
					pilots = await _pilotService.SearchPilots(cancellationToken, page, correctedPageSize, filter);
				}
			}
			if (pilots == null || !pilots.Any())
			{
				_logger.LogInformation("No pilots found for page {Page}, pageSize {PageSize}, getAll {GetAll}, filter {Filter}.",
					page, pageSize, getAll, JsonConvert.SerializeObject(filter));
				return NoContent();
			}
			var excel = _exportService.ExportToExcel("Pilots", pilots);
			if (excel == null || excel.Length == 0)
			{
				_logger.LogError("Excel generation failed.");
				return StatusCode(500, "Failed to generate Excel file.");
			}
			string fileName = _utilityService.GenerateUniqueFileName("Pilots", FileExtension.Xlsx);
			return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
		}

	}
}
