using AirportAutomation.Api.Interfaces;
using AirportAutomation.Application.Dtos.Flight;
using AirportAutomation.Application.Dtos.Response;
using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Enums;
using AirportAutomation.Core.Interfaces.IServices;
using AirportAutomation.Api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AirportAutomation.Api.Controllers
{
	/// <summary>
	/// Represents the controller for managing Flights.
	/// </summary>
	[Authorize]
	[ApiVersion("1.0")]
	public class FlightsController : BaseController
	{
		private readonly IFlightService _flightService;
		private readonly IPaginationValidationService _paginationValidationService;
		private readonly IInputValidationService _inputValidationService;
		private readonly IUtilityService _utilityService;
		private readonly IExportService _exportService;
		private readonly IMapper _mapper;
		private readonly ILogger<FlightsController> _logger;
		private readonly int maxPageSize;

		/// <summary>
		/// Initializes a new instance of the <see cref="FlightsController"/> class.
		/// </summary>
		/// <param name="flightService">The service for managing flights.</param>
		/// <param name="paginationValidationService">The service for validating pagination parameters.</param>
		/// <param name="inputValidationService">The service for validating input data.</param>
		/// <param name="utilityService">The utility service for various helper functions.</param>
		/// <param name="exportService">The service for exporting data.</param>
		/// <param name="mapper">The mapper for object-to-object mapping.</param>
		/// <param name="logger">The logger for logging actions and errors.</param>
		/// <param name="configuration">The application configuration.</param>
		public FlightsController(
			IFlightService flightService,
			IPaginationValidationService paginationValidationService,
			IInputValidationService inputValidationService,
			IUtilityService utilityService,
			IExportService exportService,
			IMapper mapper,
			ILogger<FlightsController> logger,
			IConfiguration configuration)
		{
			_flightService = flightService ?? throw new ArgumentNullException(nameof(flightService));
			_paginationValidationService = paginationValidationService ?? throw new ArgumentNullException(nameof(paginationValidationService));
			_inputValidationService = inputValidationService ?? throw new ArgumentNullException(nameof(inputValidationService));
			_utilityService = utilityService ?? throw new ArgumentNullException(nameof(utilityService));
			_exportService = exportService ?? throw new ArgumentNullException(nameof(exportService));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			maxPageSize = configuration.GetValue<int>("pageSettings:maxPageSize");
		}

		/// <summary>
		/// Endpoint for retrieving a paginated list of flights.
		/// </summary>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <param name="page">The page number for pagination (optional).</param>
		/// <param name="pageSize">The number of items per page (optional).</param>
		/// <returns>A paginated list of flights.</returns>
		/// <response code="200">Returns a list of flights wrapped in a <see cref="PagedResponse{FlightDto}"/>.</response>
		/// <response code="204">If no flights are found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If the user is not authenticated.</response>
		[HttpGet]
		[ProducesResponseType(200, Type = typeof(PagedResponse<FlightDto>))]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PagedResponse<FlightDto>>> GetFlights(
			CancellationToken cancellationToken,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10)
		{
			var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
			if (!isValid)
			{
				return result;
			}
			var flights = await _flightService.GetFlights(cancellationToken, page, correctedPageSize);
			if (flights is null || !flights.Any())
			{
				_logger.LogInformation("Flights not found.");
				return NoContent();
			}
			var totalItems = await _flightService.FlightsCount(cancellationToken);
			var data = _mapper.Map<IEnumerable<FlightDto>>(flights);
			var response = new PagedResponse<FlightDto>(data, page, correctedPageSize, totalItems);
			return Ok(response);
		}

		/// <summary>
		/// Endpoint for retrieving a single flight.
		/// </summary>
		/// <param name="id">The ID of the flight to retrieve.</param>
		/// <returns>A single flight that matches the specified ID.</returns>
		/// <response code="200">Returns a single flight if found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no flight is found.</response>
		/// <response code="401">If the user is not authenticated.</response>
		[HttpGet("{id}")]
		[ProducesResponseType(200, Type = typeof(FlightDto))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<FlightDto>> GetFlight(int id)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {Id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (!await _flightService.FlightExists(id))
			{
				_logger.LogInformation("Flight with id {Id} not found.", id);
				return NotFound();
			}
			var flight = await _flightService.GetFlight(id);
			var flightDto = _mapper.Map<FlightDto>(flight);
			return Ok(flightDto);
		}

		/// <summary>
		/// Endpoint for retrieving a paginated list of flights containing the specified name.
		/// </summary>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <param name="startDate">The start date for the search.</param>
		/// <param name = "endDate" > The end date for the search.</param>
		/// <param name="page">The page number for pagination (optional).</param>
		/// <param name="pageSize">The size of each page for pagination (optional).</param>
		/// <returns>A list of flights that match the specified name.</returns>
		/// <response code="200">Returns a paged list of flights if found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no flights are found.</response>
		/// <response code="401">If the user is not authenticated.</response>
		[HttpGet("byDate")]
		[ProducesResponseType(200, Type = typeof(PagedResponse<FlightDto>))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PagedResponse<FlightDto>>> GetFlightsBetweenDates(
			CancellationToken cancellationToken,
			[FromQuery] DateOnly? startDate = null,
			[FromQuery] DateOnly? endDate = null,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10)
		{
			var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
			if (!isValid)
			{
				return result;
			}
			if (!startDate.HasValue && !endDate.HasValue)
			{
				_logger.LogInformation("Both start date and end date are missing in the request.");
				return BadRequest("Both start date and end date are missing in the request.");
			}
			if (!_inputValidationService.IsValidDateOnly(startDate) || !_inputValidationService.IsValidDateOnly(endDate))
			{
				_logger.LogInformation("Invalid input. The start and end dates must be valid dates.");
				return BadRequest("Invalid input. The start and end dates must be valid dates.");
			}
			var flights = await _flightService.GetFlightsBetweenDates(cancellationToken, page, correctedPageSize, startDate, endDate);
			if (flights == null || flights.Count == 0)
			{
				_logger.LogInformation("Flights not found.");
				return NotFound();
			}
			var totalItems = await _flightService.FlightsCount(cancellationToken, startDate, endDate);
			var data = _mapper.Map<IEnumerable<FlightDto>>(flights);
			var response = new PagedResponse<FlightDto>(data, page, correctedPageSize, totalItems);
			return Ok(response);
		}

		/// <summary>
		/// Endpoint for creating a new flight.
		/// </summary>
		/// <param name="flightCreateDto">The data to create the new flight.</param>
		/// <returns>The created flight.</returns>
		/// <response code="201">Returns the created flight if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If the user is not authenticated.</response>
		/// <response code="403">If the authenticated user does not have permission to access the requested resource.</response>
		[HttpPost]
		[Authorize(Policy = "RequireAdminRole")]
		[ProducesResponseType(201, Type = typeof(FlightDto))]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		public async Task<ActionResult<FlightEntity>> PostFlight(FlightCreateDto flightCreateDto)
		{
			var flight = _mapper.Map<FlightEntity>(flightCreateDto);
			await _flightService.PostFlight(flight);
			var flightDto = _mapper.Map<FlightDto>(flight);
			return CreatedAtAction("GetFlight", new { id = flightDto.Id }, flightDto);
		}

		/// <summary>
		/// Endpoint for updating a single flight.
		/// </summary>
		/// <param name="id">The ID of the flight to update.</param>
		/// <param name="flightUpdateDto">The data to update the flight.</param>
		/// <returns>No content.</returns>
		/// <response code="204">Returns no content if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no flight is found.</response>
		/// <response code="401">If the user is not authenticated.</response>
		/// <response code="403">If the authenticated user does not have permission to access the requested resource.</response>
		[HttpPut("{id}")]
		[Authorize(Policy = "RequireAdminRole")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		public async Task<IActionResult> PutFlight(int id, FlightUpdateDto flightUpdateDto)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {Id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (id != flightUpdateDto.Id)
			{
				_logger.LogInformation("Flight with id {Id} is different from provided Flight and his id.", id);
				return BadRequest();
			}
			if (!await _flightService.FlightExists(id))
			{
				_logger.LogInformation("Flight with id {Id} not found.", id);
				return NotFound();
			}
			var flight = _mapper.Map<FlightEntity>(flightUpdateDto);
			await _flightService.PutFlight(flight);
			return NoContent();
		}

		/// <summary>
		/// Endpoint for partially updating a single flight.
		/// </summary>
		/// <param name="id">The ID of the flight to partially update.</param>
		/// <param name="flightDocument">The patch document containing the changes.</param>
		/// <returns>The updated flight.</returns>
		/// <remarks>
		/// The JSON document should follow the JSON Patch standard (RFC 6902) and contain one or more operations.
		/// Example operation:
		/// {
		///     "op": "replace",
		///     "path": "/Name",
		///     "value": "NewName"
		/// }
		/// </remarks>
		/// <response code="200">Returns the updated flight if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If the flight with the specified ID is not found.</response>
		/// <response code="401">If the user is not authenticated.</response>
		/// <response code="403">If the authenticated user does not have permission to access the requested resource.</response>
		[HttpPatch("{id}")]
		[Authorize(Policy = "RequireAdminRole")]
		[ProducesResponseType(200, Type = typeof(FlightDto))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		public async Task<IActionResult> PatchFlight(int id, [FromBody] JsonPatchDocument flightDocument)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {Id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (!await _flightService.FlightExists(id))
			{
				_logger.LogInformation("Flight with id {Id} not found.", id);
				return NotFound();
			}
			var updatedFlight = await _flightService.PatchFlight(id, flightDocument);
			var flightDto = _mapper.Map<FlightDto>(updatedFlight);
			return Ok(flightDto);
		}

		/// <summary>
		/// Endpoint for deleting a single flight.
		/// </summary>
		/// <param name="id">The ID of the flight to delete.</param>
		/// <returns>No content.</returns>
		/// <response code="204">Returns no content if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no flight is found.</response>
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
		public async Task<IActionResult> DeleteFlight(int id)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {Id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (!await _flightService.FlightExists(id))
			{
				_logger.LogInformation("Flight with id {Id} not found.", id);
				return NotFound();
			}
			bool deleted = await _flightService.DeleteFlight(id);
			if (deleted)
			{
				return NoContent();
			}
			else
			{
				_logger.LogInformation("Flight with id {Id} is being referenced by other entities and cannot be deleted.", id);
				return Conflict("Flight cannot be deleted because it is being referenced by other entities.");
			}
		}

		/// <summary>
		/// Endpoint for exporting flight data to PDF.
		/// </summary>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <param name="page">The page number for pagination (optional, default is 1).</param>
		/// <param name="pageSize">The page size for pagination (optional, default is 10).</param>
		/// <param name="getAll">Flag indicating whether to retrieve all data (optional, default is false).</param>
		/// <param name="startDate">The start date for the search (optional, default is null).</param>
		/// <param name = "endDate" > The end date for the search (optional, default is null).</param>
		/// <returns>Returns the generated PDF document.</returns>
		/// <response code="204">No flights found.</response>
		/// <response code="200">Returns the generated PDF document.</response>
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
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10,
			[FromQuery] bool getAll = false,
			[FromQuery] DateOnly? startDate = null,
			[FromQuery] DateOnly? endDate = null)
		{
			IList<FlightEntity> flights = new List<FlightEntity>();
			if (getAll)
			{
				flights = await _flightService.GetAllFlights(cancellationToken);
			}
			else
			{
				var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
				if (!isValid)
				{
					return result;
				}
				if (startDate.HasValue && endDate.HasValue)
				{
					if (_inputValidationService.IsValidDateOnly(startDate) && _inputValidationService.IsValidDateOnly(endDate))
					{
						flights = await _flightService.GetFlightsBetweenDates(cancellationToken, page, correctedPageSize, startDate, endDate);
					}
				}
				else
				{
					flights = await _flightService.GetFlights(cancellationToken, page, correctedPageSize);
				}
			}
			if (flights is null || !flights.Any())
			{
				_logger.LogInformation("No flights found for page {Page}, pageSize {PageSize}, getAll {GetAll}, startDate {StartDate}, endDate {EndDate}.",
					page, pageSize, getAll, startDate, endDate);
				return NoContent();
			}
			var pdf = _exportService.ExportToPDF("Flights", flights);
			if (pdf == null)
			{
				_logger.LogError("PDF generation failed.");
				return StatusCode(500, "Failed to generate PDF.");
			}
			string fileName = _utilityService.GenerateUniqueFileName("Flights", FileExtension.Pdf);
			return File(pdf, "application/pdf", fileName);
		}

		/// <summary>
		/// Endpoint for exporting flight data to Excel.
		/// </summary>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <param name="page">The page number for pagination (optional, default is 1).</param>
		/// <param name="pageSize">The page size for pagination (optional, default is 10).</param>
		/// <param name="getAll">Flag indicating whether to retrieve all data (optional, default is false).</param>
		/// <param name="startDate">The start date for the search (optional, default is null).</param>
		/// <param name="endDate">The end date for the search (optional, default is null).</param>
		/// <returns>Returns the generated Excel document.</returns>
		/// <response code="204">No flights found.</response>
		/// <response code="200">Returns the generated Excel document.</response>
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
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10,
			[FromQuery] bool getAll = false,
			[FromQuery] DateOnly? startDate = null,
			[FromQuery] DateOnly? endDate = null)
		{
			IList<FlightEntity> flights = new List<FlightEntity>();

			if (getAll)
			{
				flights = await _flightService.GetAllFlights(cancellationToken);
			}
			else
			{
				var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
				if (!isValid)
				{
					return result;
				}

				if (startDate.HasValue && endDate.HasValue)
				{
					if (_inputValidationService.IsValidDateOnly(startDate) && _inputValidationService.IsValidDateOnly(endDate))
					{
						flights = await _flightService.GetFlightsBetweenDates(cancellationToken, page, correctedPageSize, startDate, endDate);
					}
					else
					{
						return BadRequest("Invalid date parameters.");
					}
				}
				else
				{
					flights = await _flightService.GetFlights(cancellationToken, page, correctedPageSize);
				}
			}
			if (flights == null || !flights.Any())
			{
				_logger.LogInformation("No flights found for page {Page}, pageSize {PageSize}, getAll {GetAll}, startDate {StartDate}, endDate {EndDate}.",
					page, pageSize, getAll, startDate, endDate);
				return NoContent();
			}
			var excel = _exportService.ExportToExcel("Flights", flights);
			if (excel == null || excel.Length == 0)
			{
				_logger.LogError("Excel generation failed.");
				return StatusCode(500, "Failed to generate Excel file.");
			}
			string fileName = _utilityService.GenerateUniqueFileName("Flights", FileExtension.Xlsx);
			return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
		}

	}
}
