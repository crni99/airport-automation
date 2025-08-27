using AirportAutomation.Api.Interfaces;
using AirportAutomation.Application.Dtos.Airline;
using AirportAutomation.Application.Dtos.Response;
using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Enums;
using AirportAutomation.Core.Interfaces.IServices;
using AirportАutomation.Api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace AirportАutomation.Api.Controllers
{
	/// <summary>
	/// Represents the controller for managing Airlines.
	/// </summary>
	[Authorize]
	[ApiVersion("1.0")]
	public class AirlinesController : BaseController
	{
		private readonly IAirlineService _airlineService;
		private readonly IPaginationValidationService _paginationValidationService;
		private readonly IInputValidationService _inputValidationService;
		private readonly IUtilityService _utilityService;
		private readonly IExportService _exportService;
		private readonly IMapper _mapper;
		private readonly ILogger<AirlinesController> _logger;
		private readonly int maxPageSize;

		/// <summary>
		/// Initializes a new instance of the <see cref="AirlinesController"/> class.
		/// </summary>
		/// <param name="airlineService">The service for managing airlines.</param>
		/// <param name="paginationValidationService">The service for validating pagination parameters.</param>
		/// <param name="inputValidationService">The service for validating input data.</param>
		/// <param name="utilityService">The utility service for various helper functions.</param>
		/// <param name="exportService">The service for exporting data.</param>
		/// <param name="mapper">The mapper for object-to-object mapping.</param>
		/// <param name="logger">The logger for logging actions and errors.</param>
		/// <param name="configuration">The application configuration.</param>
		public AirlinesController(
			IAirlineService airlineService,
			IPaginationValidationService paginationValidationService,
			IInputValidationService inputValidationService,
			IUtilityService utilityService,
			IExportService exportService,
			IMapper mapper,
			ILogger<AirlinesController> logger,
			IConfiguration configuration
		)
		{
			_airlineService = airlineService ?? throw new ArgumentNullException(nameof(airlineService));
			_paginationValidationService = paginationValidationService ?? throw new ArgumentNullException(nameof(paginationValidationService));
			_inputValidationService = inputValidationService ?? throw new ArgumentNullException(nameof(inputValidationService));
			_utilityService = utilityService ?? throw new ArgumentNullException(nameof(utilityService));
			_exportService = exportService ?? throw new ArgumentNullException(nameof(exportService));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			maxPageSize = configuration.GetValue<int>("pageSettings:maxPageSize");
		}

		/// <summary>
		/// Endpoint for retrieving a paginated list of airlines.
		/// </summary>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <param name="page">The page number for pagination (optional).</param>
		/// <param name="pageSize">The number of items per page (optional).</param>
		/// <returns>A paginated list of airlines.</returns>
		/// <response code="200">Returns a list of airlines wrapped in a <see cref="PagedResponse{AirlineDto}"/>.</response>
		/// <response code="204">If no airlines are found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If the user is not authenticated.</response>
		[HttpGet]
		[ProducesResponseType(200, Type = typeof(PagedResponse<AirlineDto>))]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PagedResponse<AirlineDto>>> GetAirlines(
			CancellationToken cancellationToken,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10)
		{

			var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
			if (!isValid)
			{
				return result;
			}
			var airlines = await _airlineService.GetAirlines(cancellationToken, page, correctedPageSize);
			if (airlines is null || !airlines.Any())
			{
				_logger.LogInformation("Airlines not found.");
				return NoContent();
			}
			var totalItems = await _airlineService.AirlinesCount(cancellationToken);
			var data = _mapper.Map<IEnumerable<AirlineDto>>(airlines);
			var response = new PagedResponse<AirlineDto>(data, page, correctedPageSize, totalItems);
			return Ok(response);
		}

		/// <summary>
		/// Endpoint for retrieving a single airline.
		/// </summary>
		/// <param name="id">The ID of the airline to retrieve.</param>
		/// <returns>A single airline that matches the specified ID.</returns>
		/// <response code="200">Returns a single airline if found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no airline is found.</response>
		/// <response code="401">If the user is not authenticated.</response>
		[HttpGet("{id}")]
		[ProducesResponseType(200, Type = typeof(AirlineDto))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<AirlineDto>> GetAirline(int id)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {Id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (!await _airlineService.AirlineExists(id))
			{
				_logger.LogInformation("Airline with id {Id} not found.", id);
				return NotFound();
			}
			var airline = await _airlineService.GetAirline(id);
			var airlineDto = _mapper.Map<AirlineDto>(airline);
			return Ok(airlineDto);
		}

		/// <summary>
		/// Endpoint for retrieving a paginated list of airlines containing the specified name.
		/// </summary>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <param name="name">The name to search for.</param>
		/// <param name="page">The page number for pagination (optional).</param>
		/// <param name="pageSize">The size of each page for pagination (optional).</param>
		/// <returns>A list of airlines that match the specified name.</returns>
		/// <response code="200">Returns a paged list of airlines if found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no airlines are found.</response>
		/// <response code="401">If the user is not authenticated.</response>
		[HttpGet("byName/{name}")]
		[ProducesResponseType(200, Type = typeof(PagedResponse<AirlineDto>))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PagedResponse<AirlineDto>>> GetAirlinesByName(
			CancellationToken cancellationToken,
			string name,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10)
		{
			if (!_inputValidationService.IsValidString(name))
			{
				_logger.LogInformation("Invalid input. The name must be a valid non-empty string.");
				return BadRequest("Invalid input. The name must be a valid non-empty string.");
			}
			var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
			if (!isValid)
			{
				return result;
			}
			var airlines = await _airlineService.GetAirlinesByName(cancellationToken, page, correctedPageSize, name);
			if (airlines is null || airlines.Count == 0)
			{
				_logger.LogInformation("Airline with name {Name} not found.", name);
				return NotFound();
			}
			var totalItems = await _airlineService.AirlinesCount(cancellationToken, name);
			var data = _mapper.Map<IEnumerable<AirlineDto>>(airlines);
			var response = new PagedResponse<AirlineDto>(data, page, correctedPageSize, totalItems);
			return Ok(response);
		}

		/// <summary>
		/// Endpoint for creating a new airline.
		/// </summary>
		/// <param name="airlineCreateDto">The data to create the new airline.</param>
		/// <returns>The created airline.</returns>
		/// <response code="201">Returns the created airline if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If the user is not authenticated.</response>
		/// <response code="403">If the authenticated user does not have permission to access the requested resource.</response>
		[HttpPost]
		[Authorize(Policy = "RequireAdminRole")]
		[ProducesResponseType(201, Type = typeof(AirlineDto))]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		public async Task<ActionResult<AirlineDto>> PostAirline(AirlineCreateDto airlineCreateDto)
		{
			var airline = _mapper.Map<AirlineEntity>(airlineCreateDto);
			await _airlineService.PostAirline(airline);
			var airlineDto = _mapper.Map<AirlineDto>(airline);
			return CreatedAtAction("GetAirline", new { id = airlineDto.Id }, airlineDto);
		}

		/// <summary>
		/// Endpoint for updating a single airline.
		/// </summary>
		/// <param name="id">The ID of the airline to update.</param>
		/// <param name="airlineDto">The data to update the airline.</param>
		/// <returns>No content.</returns>
		/// <response code="204">Returns no content if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no airline is found.</response>
		/// <response code="401">If the user is not authenticated.</response>
		/// <response code="403">If the authenticated user does not have permission to access the requested resource.</response>
		[HttpPut("{id}")]
		[Authorize(Policy = "RequireAdminRole")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		public async Task<IActionResult> PutAirline(int id, AirlineDto airlineDto)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {Id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (id != airlineDto.Id)
			{
				_logger.LogInformation("Airline with id {Id} is different from provided Airline and its id.", id);
				return BadRequest();
			}

			if (!await _airlineService.AirlineExists(id))
			{
				_logger.LogInformation("Airline with id {Id} not found.", id);
				return NotFound();
			}
			var airline = _mapper.Map<AirlineEntity>(airlineDto);
			await _airlineService.PutAirline(airline);
			return NoContent();
		}

		/// <summary>
		/// Endpoint for partially updating a single airline.
		/// </summary>
		/// <param name="id">The ID of the airline to partially update.</param>
		/// <param name="airlineDocument">The patch document containing the changes.</param>
		/// <returns>The updated airline.</returns>
		/// <remarks>
		/// The JSON document should follow the JSON Patch standard (RFC 6902) and contain one or more operations.
		/// Example operation:
		/// {
		///     "op": "replace",
		///     "path": "/Name",
		///     "value": "NewName"
		/// }
		/// </remarks>
		/// <response code="200">Returns the updated airline if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If the airline with the specified ID is not found.</response>
		/// <response code="401">If the user is not authenticated.</response>
		/// <response code="403">If the authenticated user does not have permission to access the requested resource.</response>
		[HttpPatch("{id}")]
		[Authorize(Policy = "RequireAdminRole")]
		[ProducesResponseType(200, Type = typeof(AirlineDto))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		public async Task<IActionResult> PatchAirline(int id, [FromBody] JsonPatchDocument airlineDocument)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {Id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (!await _airlineService.AirlineExists(id))
			{
				_logger.LogInformation("Airline with id {Id} not found.", id);
				return NotFound();
			}
			var updatedAirline = await _airlineService.PatchAirline(id, airlineDocument);
			var airlineDto = _mapper.Map<AirlineDto>(updatedAirline);
			return Ok(airlineDto);
		}

		/// <summary>
		/// Endpoint for deleting a single airline.
		/// </summary>
		/// <param name="id">The ID of the airline to delete.</param>
		/// <returns>No content.</returns>
		/// <response code="204">Returns no content if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no airline is found.</response>
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
		public async Task<IActionResult> DeleteAirline(int id)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {Id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (!await _airlineService.AirlineExists(id))
			{
				_logger.LogInformation("Airline with id {Id} not found.", id);
				return NotFound();
			}
			bool deleted = await _airlineService.DeleteAirline(id);
			if (deleted)
			{
				return NoContent();
			}
			else
			{
				_logger.LogInformation("Airline with id {Id} is being referenced by other entities and cannot be deleted.", id);
				return Conflict("Airline cannot be deleted because it is being referenced by other entities.");
			}
		}

		/// <summary>
		/// Endpoint for exporting airline data to PDF.
		/// </summary>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <param name="page">The page number for pagination (optional, default is 1).</param>
		/// <param name="pageSize">The page size for pagination (optional, default is 10).</param>
		/// <param name="getAll">Flag indicating whether to retrieve all data (optional, default is false).</param>
		/// <param name="name">The name to search for (optional, default is null).</param>
		/// <returns>Returns the generated PDF document.</returns>
		/// <response code="200">Returns the generated PDF document.</response>
		/// <response code="204">No airlines found.</response>
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
			[FromQuery] string? name = null)
		{
			IList<AirlineEntity> airlines;
			if (getAll)
			{
				airlines = await _airlineService.GetAllAirlines(cancellationToken);
			}
			else
			{
				var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
				if (!isValid)
				{
					return result;
				}
				if (_inputValidationService.IsValidString(name))
				{
					airlines = await _airlineService.GetAirlinesByName(cancellationToken, page, correctedPageSize, name);
				}
				else
				{
					airlines = await _airlineService.GetAirlines(cancellationToken, page, correctedPageSize);
				}
			}
			if (airlines is null || !airlines.Any())
			{
				_logger.LogInformation("No airlines found for page {Page}, pageSize {PageSize}, getAll {GetAll}, name {Name}.",
					page, pageSize, getAll, name);
				return NoContent();
			}
			var pdf = _exportService.ExportToPDF("Airlines", airlines);
			if (pdf == null)
			{
				_logger.LogError("PDF generation failed.");
				return StatusCode(500, "Failed to generate PDF.");
			}
			string fileName = _utilityService.GenerateUniqueFileName("Airlines", FileExtension.Pdf);
			return File(pdf, "application/pdf", fileName);
		}

		/// <summary>
		/// Endpoint for exporting airline data to Excel.
		/// </summary>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <param name="page">The page number for pagination (optional, default is 1).</param>
		/// <param name="pageSize">The page size for pagination (optional, default is 10).</param>
		/// <param name="getAll">Flag indicating whether to retrieve all data (optional, default is false).</param>
		/// <param name="name">The name to search for (optional, default is null).</param>
		/// <returns>Returns the generated Excel document.</returns>
		/// <response code="200">Returns the generated Excel document.</response>
		/// <response code="204">No airlines found.</response>
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
			[FromQuery] string? name = null)
		{
			IList<AirlineEntity> airlines;
			if (getAll)
			{
				airlines = await _airlineService.GetAllAirlines(cancellationToken);
			}
			else
			{
				var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
				if (!isValid)
				{
					return result;
				}
				if (_inputValidationService.IsValidString(name))
				{
					airlines = await _airlineService.GetAirlinesByName(cancellationToken, page, correctedPageSize, name);
				}
				else
				{
					airlines = await _airlineService.GetAirlines(cancellationToken, page, correctedPageSize);
				}
			}
			if (airlines is null || !airlines.Any())
			{
				_logger.LogInformation("No airlines found for page {Page}, pageSize {PageSize}, getAll {GetAll}, name {Name}.",
					page, pageSize, getAll, name);
				return NoContent();
			}
			var excel = _exportService.ExportToExcel("Airlines", airlines);
			if (excel == null || excel.Length == 0)
			{
				_logger.LogError("Excel generation failed.");
				return StatusCode(500, "Failed to generate Excel file.");
			}
			string fileName = _utilityService.GenerateUniqueFileName("Airlines", FileExtension.Xlsx);
			return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
		}

	}
}
