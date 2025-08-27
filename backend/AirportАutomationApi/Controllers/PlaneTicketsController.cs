using AirportAutomation.Api.Interfaces;
using AirportAutomation.Application.Dtos.PlaneTicket;
using AirportAutomation.Application.Dtos.Response;
using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Enums;
using AirportAutomation.Core.FilterExtensions;
using AirportAutomation.Core.Filters;
using AirportAutomation.Core.Interfaces.IServices;
using AirportАutomation.Api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AirportАutomation.Api.Controllers
{
	/// <summary>
	/// Represents the controller for managing Plane Tickets.
	/// </summary>
	[Authorize]
	[ApiVersion("1.0")]
	public class PlaneTicketsController : BaseController
	{
		private readonly IPlaneTicketService _planeTicketService;
		private readonly IPaginationValidationService _paginationValidationService;
		private readonly IInputValidationService _inputValidationService;
		private readonly IUtilityService _utilityService;
		private readonly IExportService _exportService;
		private readonly IMapper _mapper;
		private readonly ILogger<PlaneTicketsController> _logger;
		private readonly int maxPageSize;

		/// <summary>
		/// Initializes a new instance of the <see cref="PlaneTicketsController"/> class.
		/// </summary>
		/// <param name="planeTicketService">The service for managing plane tickets.</param>
		/// <param name="paginationValidationService">The service for validating pagination parameters.</param>
		/// <param name="inputValidationService">The service for validating input data.</param>
		/// <param name="utilityService">The utility service for various helper functions.</param>
		/// <param name="exportService">The service for exporting data.</param>
		/// <param name="mapper">The mapper for object-to-object mapping.</param>
		/// <param name="logger">The logger for logging actions and errors.</param>
		/// <param name="configuration">The application configuration.</param>
		public PlaneTicketsController(
			IPlaneTicketService planeTicketService,
			IPaginationValidationService paginationValidationService,
			IInputValidationService inputValidationService,
			IUtilityService utilityService,
			IExportService exportService,
			IMapper mapper,
			ILogger<PlaneTicketsController> logger,
			IConfiguration configuration)
		{
			_planeTicketService = planeTicketService ?? throw new ArgumentNullException(nameof(planeTicketService));
			_paginationValidationService = paginationValidationService ?? throw new ArgumentNullException(nameof(paginationValidationService));
			_inputValidationService = inputValidationService ?? throw new ArgumentNullException(nameof(inputValidationService));
			_utilityService = utilityService ?? throw new ArgumentNullException(nameof(utilityService));
			_exportService = exportService ?? throw new ArgumentNullException(nameof(exportService));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			maxPageSize = configuration.GetValue<int>("pageSettings:maxPageSize");
		}

		/// <summary>
		/// Endpoint for retrieving a paginated list of plane tickets.
		/// </summary>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <param name="page">The page number for pagination (optional).</param>
		/// <param name="pageSize">The number of items per page (optional).</param>
		/// <returns>A paginated list of plane tickets.</returns>
		/// <response code="200">Returns a list of plane tickets wrapped in a <see cref="PagedResponse{PlaneTicketDto}"/>.</response>
		/// <response code="204">If no plane tickets are found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If the user is not authenticated.</response>
		[HttpGet]
		[ProducesResponseType(200, Type = typeof(PagedResponse<PlaneTicketDto>))]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PagedResponse<PlaneTicketDto>>> GetPlaneTickets(
			CancellationToken cancellationToken,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10)
		{
			var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
			if (!isValid)
			{
				return result;
			}
			var planeTickets = await _planeTicketService.GetPlaneTickets(cancellationToken, page, correctedPageSize);
			if (planeTickets is null || !planeTickets.Any())
			{
				_logger.LogInformation("Plane Tickets not found.");
				return NoContent();
			}
			var totalItems = await _planeTicketService.PlaneTicketsCount(cancellationToken);
			var data = _mapper.Map<IEnumerable<PlaneTicketDto>>(planeTickets);
			var response = new PagedResponse<PlaneTicketDto>(data, page, correctedPageSize, totalItems);
			return Ok(response);
		}

		/// <summary>
		/// Endpoint for retrieving a single plane ticket.
		/// </summary>
		/// <param name="id">The ID of the plane ticket to retrieve.</param>
		/// <returns>A single plane ticket that matches the specified ID.</returns>
		/// <response code="200">Returns a single plane ticket if found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no plane ticket is found.</response>
		/// <response code="401">If the user is not authenticated.</response>
		[HttpGet("{id}")]
		[ProducesResponseType(200, Type = typeof(PlaneTicketDto))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PlaneTicketDto>> GetPlaneTicket(int id)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {Id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (!await _planeTicketService.PlaneTicketExists(id))
			{
				_logger.LogInformation("Plane Ticket with id {Id} not found.", id);
				return NotFound();
			}
			var planeTicket = await _planeTicketService.GetPlaneTicket(id);
			var planeTicketDto = _mapper.Map<PlaneTicketDto>(planeTicket);
			return Ok(planeTicketDto);
		}

		/// <summary>
		/// Endpoint for retrieving a paginated list of plane tickets containing the specified name.
		/// </summary>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <param name="minPrice">The minimum price to search for.</param>
		/// <param name = "maxPrice" > The maximum price to search for.</param>
		/// <param name="page">The page number for pagination (optional).</param>
		/// <param name="pageSize">The size of each page for pagination (optional).</param>
		/// <returns>A list of plane tickets that match the specified name.</returns>
		/// <response code="200">Returns a paged list of plane tickets if found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no plane tickets are found.</response>
		/// <response code="401">If the user is not authenticated.</response>
		[HttpGet("byPrice")]
		[ProducesResponseType(200, Type = typeof(PagedResponse<PlaneTicketDto>))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PagedResponse<PlaneTicketDto>>> GetPlaneTicketsForPrice(
			CancellationToken cancellationToken,
			[FromQuery] int? minPrice = null,
			[FromQuery] int? maxPrice = null,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10)
		{
			if (minPrice == null && maxPrice == null)
			{
				_logger.LogInformation("Both min price and max price are missing in the request.");
				return BadRequest("Both min price and max price are missing in the request.");
			}
			var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
			if (!isValid)
			{
				return result;
			}
			var planeTickets = await _planeTicketService.GetPlaneTicketsForPrice(cancellationToken, page, correctedPageSize, minPrice, maxPrice);
			if (planeTickets is null || !planeTickets.Any())
			{
				_logger.LogInformation("Plane Tickets not found.");
				return NotFound();
			}
			var totalItems = await _planeTicketService.PlaneTicketsCount(cancellationToken, minPrice, maxPrice);
			var data = _mapper.Map<IEnumerable<PlaneTicketDto>>(planeTickets);
			var response = new PagedResponse<PlaneTicketDto>(data, page, correctedPageSize, totalItems);
			return Ok(response);
		}

		/// <summary>
		/// Retrieves a paginated list of plane tickets matching the specified search filter criteria.
		/// </summary>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <param name="filter">The search filter containing plane ticket fields to filter by.</param>
		/// <param name="page">The page number for pagination (optional, default is 1).</param>
		/// <param name="pageSize">The number of items per page for pagination (optional, default is 10).</param>
		/// <returns>A paged response containing the list of plane tickets that match the filter criteria.</returns>
		/// <response code="200">Returns a paged list of plane tickets if found.</response>
		/// <response code="400">If the request is invalid or the filter criteria are missing or invalid.</response>
		/// <response code="404">If no plane tickets matching the filter criteria are found.</response>
		/// <response code="401">If the user is not authenticated.</response>
		[HttpGet("byFilter")]
		[ProducesResponseType(200, Type = typeof(PagedResponse<PlaneTicketDto>))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PagedResponse<PlaneTicketDto>>> GetPlaneTicketsByFilter(
			CancellationToken cancellationToken,
			[FromQuery] PlaneTicketSearchFilter filter,
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
			var planeTickets = await _planeTicketService.GetPlaneTicketsByFilter(cancellationToken, page, correctedPageSize, filter);
			if (planeTickets is null || !planeTickets.Any())
			{
				_logger.LogInformation("Plane Tickets not found.");
				return NotFound();
			}
			var totalItems = await _planeTicketService.PlaneTicketsCountFilter(cancellationToken, filter);
			var data = _mapper.Map<IEnumerable<PlaneTicketDto>>(planeTickets);
			var response = new PagedResponse<PlaneTicketDto>(data, page, correctedPageSize, totalItems);
			return Ok(response);
		}

		/// <summary>
		/// Endpoint for creating a new plane ticket.
		/// </summary>
		/// <param name="planeTicketCreateDto">The data to create the new plane ticket.</param>
		/// <returns>The created plane ticket.</returns>
		/// <response code="201">Returns the created plane ticket if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If the user is not authenticated.</response>
		/// <response code="403">If the authenticated user does not have permission to access the requested resource.</response>
		[HttpPost]
		[Authorize(Policy = "RequireAdminRole")]
		[ProducesResponseType(201, Type = typeof(PlaneTicketDto))]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		public async Task<ActionResult<PlaneTicketEntity>> PostPlaneTicket(PlaneTicketCreateDto planeTicketCreateDto)
		{
			var planeTicket = _mapper.Map<PlaneTicketEntity>(planeTicketCreateDto);
			await _planeTicketService.PostPlaneTicket(planeTicket);
			var planeTicketDto = _mapper.Map<PlaneTicketDto>(planeTicket);
			return CreatedAtAction("GetPlaneTicket", new { id = planeTicketDto.Id }, planeTicketDto);
		}

		/// <summary>
		/// Endpoint for updating a single plane ticket.
		/// </summary>
		/// <param name="id">The ID of the plane ticket to update.</param>
		/// <param name="planeTicketUpdateDto">The data to update the plane ticket.</param>
		/// <returns>No content.</returns>
		/// <response code="204">Returns no content if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no plane ticket is found.</response>
		/// <response code="401">If the user is not authenticated.</response>
		/// <response code="403">If the authenticated user does not have permission to access the requested resource.</response>
		[HttpPut("{id}")]
		[Authorize(Policy = "RequireAdminRole")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		public async Task<IActionResult> PutPlaneTicket(int id, PlaneTicketUpdateDto planeTicketUpdateDto)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {Id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (id != planeTicketUpdateDto.Id)
			{
				_logger.LogInformation("Plane Ticket with id {Id} is different from provided Plane Ticket and his id.", id);
				return BadRequest();
			}
			if (!await _planeTicketService.PlaneTicketExists(id))
			{
				_logger.LogInformation("Plane Ticket with id {Id} not found.", id);
				return NotFound();
			}
			var planeTicket = _mapper.Map<PlaneTicketEntity>(planeTicketUpdateDto);
			await _planeTicketService.PutPlaneTicket(planeTicket);
			return NoContent();
		}

		/// <summary>
		/// Endpoint for partially updating a single plane ticket.
		/// </summary>
		/// <param name="id">The ID of the plane ticket to partially update.</param>
		/// <param name="planeTicketDocument">The patch document containing the changes.</param>
		/// <returns>The updated plane ticket.</returns>
		/// <remarks>
		/// The JSON document should follow the JSON Patch standard (RFC 6902) and contain one or more operations.
		/// Example operation:
		/// {
		///     "op": "replace",
		///     "path": "/Name",
		///     "value": "NewName"
		/// }
		/// </remarks>
		/// <response code="200">Returns the updated plane ticket if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If the plane ticket with the specified ID is not found.</response>
		/// <response code="401">If the user is not authenticated.</response>
		/// <response code="403">If the authenticated user does not have permission to access the requested resource.</response>
		[HttpPatch("{id}")]
		[Authorize(Policy = "RequireAdminRole")]
		[ProducesResponseType(200, Type = typeof(PlaneTicketDto))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		public async Task<IActionResult> PatchPlaneTicket(int id, [FromBody] JsonPatchDocument planeTicketDocument)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {Id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (!await _planeTicketService.PlaneTicketExists(id))
			{
				_logger.LogInformation("Plane Ticket with id {Id} not found.", id);
				return NotFound();
			}
			var updatedPlaneTicket = await _planeTicketService.PatchPlaneTicket(id, planeTicketDocument);
			var planeTicketDto = _mapper.Map<PlaneTicketDto>(updatedPlaneTicket);
			return Ok(planeTicketDto);
		}

		/// <summary>
		/// Endpoint for deleting a single plane ticket.
		/// </summary>
		/// <param name="id">The ID of the plane ticket to delete.</param>
		/// <returns>No content.</returns>
		/// <response code="204">Returns no content if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no plane ticket is found.</response>
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
		public async Task<IActionResult> DeletePlaneTicket(int id)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {Id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (!await _planeTicketService.PlaneTicketExists(id))
			{
				_logger.LogInformation("Plane Ticket with id {Id} not found.", id);
				return NotFound();
			}
			bool deleted = await _planeTicketService.DeletePlaneTicket(id);
			if (deleted)
			{
				return NoContent();
			}
			else
			{
				return Conflict();
			}
		}

		/// <summary>
		/// Endpoint for exporting plane ticket data to PDF.
		/// </summary>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <param name="filter">The search filter containing destination fields to filter by.</param>
		/// <param name="page">The page number for pagination (optional, default is 1).</param>
		/// <param name="pageSize">The page size for pagination (optional, default is 10).</param>
		/// <param name="getAll">Flag indicating whether to retrieve all data (optional, default is false).</param>
		/// <returns>Returns the generated PDF document.</returns>
		/// <response code="200">Returns the generated PDF document.</response>
		/// <response code="204">No plane tickets found.</response>
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
			[FromQuery] PlaneTicketSearchFilter filter,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10,
			[FromQuery] bool getAll = false)
		{
			IList<PlaneTicketEntity> planeTickets;
			if (getAll)
			{
				planeTickets = await _planeTicketService.GetAllPlaneTickets(cancellationToken);
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
					planeTickets = await _planeTicketService.GetPlaneTickets(cancellationToken, page, correctedPageSize);
				}
				else
				{
					planeTickets = await _planeTicketService.GetPlaneTicketsByFilter(cancellationToken, page, correctedPageSize, filter);
				}
			}
			if (planeTickets is null || !planeTickets.Any())
			{
				_logger.LogInformation("No plane tickets found for page {Page}, pageSize {PageSize}, getAll {GetAll}, filter {Filter}.",
					page, pageSize, getAll, JsonConvert.SerializeObject(filter));
				return NoContent();
			}
			var pdf = _exportService.ExportToPDF("Plane Tickets", planeTickets);
			if (pdf == null)
			{
				_logger.LogError("PDF generation failed.");
				return StatusCode(500, "Failed to generate PDF.");
			}
			string fileName = _utilityService.GenerateUniqueFileName("PlaneTickets", FileExtension.Pdf);
			return File(pdf, "application/pdf", fileName);
		}

		/// <summary>
		/// Endpoint for exporting plane ticket data to Excel.
		/// </summary>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <param name="filter">The search filter containing plane ticket fields to filter by.</param>
		/// <param name="page">The page number for pagination (optional, default is 1).</param>
		/// <param name="pageSize">The page size for pagination (optional, default is 10).</param>
		/// <param name="getAll">Flag indicating whether to retrieve all data (optional, default is false).</param>
		/// <returns>Returns the generated Excel document.</returns>
		/// <response code="200">Returns the generated Excel document.</response>
		/// <response code="204">No plane tickets found.</response>
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
			[FromQuery] PlaneTicketSearchFilter filter,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10,
			[FromQuery] bool getAll = false)
		{
			IList<PlaneTicketEntity> planeTickets;

			if (getAll)
			{
				planeTickets = await _planeTicketService.GetAllPlaneTickets(cancellationToken);
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
					planeTickets = await _planeTicketService.GetPlaneTickets(cancellationToken, page, correctedPageSize);
				}
				else
				{
					planeTickets = await _planeTicketService.GetPlaneTicketsByFilter(cancellationToken, page, correctedPageSize, filter);
				}
			}
			if (planeTickets == null || !planeTickets.Any())
			{
				_logger.LogInformation("No plane tickets found for page {Page}, pageSize {PageSize}, getAll {GetAll}, filter {Filter}.",
					page, pageSize, getAll, JsonConvert.SerializeObject(filter));
				return NoContent();
			}
			var excel = _exportService.ExportToExcel("PlaneTickets", planeTickets);
			if (excel == null || excel.Length == 0)
			{
				_logger.LogError("Excel generation failed.");
				return StatusCode(500, "Failed to generate Excel file.");
			}
			string fileName = _utilityService.GenerateUniqueFileName("PlaneTickets", FileExtension.Xlsx);
			return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
		}

	}
}
