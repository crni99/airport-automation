﻿using AirportAutomation.Api.Interfaces;
using AirportAutomation.Application.Dtos.Destination;
using AirportAutomation.Application.Dtos.Response;
using AirportAutomation.Core.Entities;
using AirportAutomation.Core.FilterExtensions;
using AirportAutomation.Core.Filters;
using AirportAutomation.Core.Interfaces.IServices;
using AirportАutomation.Api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace AirportАutomation.Api.Controllers
{
	/// <summary>
	/// Represents the controller for managing Destinations.
	/// </summary>
	[Authorize]
	[ApiVersion("1.0")]
	public class DestinationsController : BaseController
	{
		private readonly IDestinationService _destinationService;
		private readonly IPaginationValidationService _paginationValidationService;
		private readonly IInputValidationService _inputValidationService;
		private readonly IUtilityService _utilityService;
		private readonly IExportService _exportService;
		private readonly IMapper _mapper;
		private readonly ILogger<DestinationsController> _logger;
		private readonly int maxPageSize;

		/// <summary>
		/// Initializes a new instance of the <see cref="DestinationsController"/> class.
		/// </summary>
		/// <param name="destinationService">The service for managing destinations.</param>
		/// <param name="paginationValidationService">The service for validating pagination parameters.</param>
		/// <param name="inputValidationService">The service for validating input data.</param>
		/// <param name="utilityService">The utility service for various helper functions.</param>
		/// <param name="exportService">The service for exporting data.</param>
		/// <param name="mapper">The mapper for object-to-object mapping.</param>
		/// <param name="logger">The logger for logging actions and errors.</param>
		/// <param name="configuration">The application configuration.</param>
		public DestinationsController(
			IDestinationService destinationService,
			IPaginationValidationService paginationValidationService,
			IInputValidationService inputValidationService,
			IUtilityService utilityService,
			IExportService exportService,
			IMapper mapper,
			ILogger<DestinationsController> logger,
			IConfiguration configuration)
		{
			_destinationService = destinationService ?? throw new ArgumentNullException(nameof(destinationService));
			_paginationValidationService = paginationValidationService ?? throw new ArgumentNullException(nameof(paginationValidationService));
			_inputValidationService = inputValidationService ?? throw new ArgumentNullException(nameof(inputValidationService));
			_utilityService = utilityService ?? throw new ArgumentNullException(nameof(utilityService));
			_exportService = exportService ?? throw new ArgumentNullException(nameof(exportService));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			maxPageSize = configuration.GetValue<int>("pageSettings:maxPageSize");
		}

		/// <summary>
		/// Endpoint for retrieving a paginated list of destinations.
		/// </summary>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <param name="page">The page number for pagination (optional).</param>
		/// <param name="pageSize">The number of items per page (optional).</param>
		/// <returns>A paginated list of destinations.</returns>
		/// <response code="200">Returns a list of destinations wrapped in a <see cref="PagedResponse{DestinationDto}"/>.</response>
		/// <response code="204">If no destinations are found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet]
		[ProducesResponseType(200, Type = typeof(PagedResponse<DestinationDto>))]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PagedResponse<DestinationDto>>> GetDestinations(
			CancellationToken cancellationToken,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10)
		{
			var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
			if (!isValid)
			{
				return result;
			}
			var destinations = await _destinationService.GetDestinations(cancellationToken, page, correctedPageSize);
			if (destinations is null || !destinations.Any())
			{
				_logger.LogInformation("Destinations not found.");
				return NoContent();
			}
			var totalItems = await _destinationService.DestinationsCount(cancellationToken);
			var data = _mapper.Map<IEnumerable<DestinationDto>>(destinations);
			var response = new PagedResponse<DestinationDto>(data, page, correctedPageSize, totalItems);
			return Ok(response);
		}

		/// <summary>
		/// Endpoint for retrieving a single destination.
		/// </summary>
		/// <param name="id">The ID of the destination to retrieve.</param>
		/// <returns>A single destination that matches the specified ID.</returns>
		/// <response code="200">Returns a single destination if found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no destination is found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[HttpGet("{id}")]
		[ProducesResponseType(200, Type = typeof(DestinationDto))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<DestinationDto>> GetDestination(int id)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {Id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (!await _destinationService.DestinationExists(id))
			{
				_logger.LogInformation("Destination with id {Id} not found.", id);
				return NotFound();
			}
			var destination = await _destinationService.GetDestination(id);
			var destinationDto = _mapper.Map<DestinationDto>(destination);
			return Ok(destinationDto);
		}

		/// <summary>
		/// Endpoint for retrieving a paginated list of destinations containing the specified name.
		/// </summary>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <param name="city">The city name to search for.</param>
		/// <param name="airport">The airport name to search for.</param>
		/// <param name="page">The page number for pagination (optional, defaults to 1).</param>
		/// <param name="pageSize">The size of each page for pagination (optional, defaults to 10).</param>
		/// <returns>A paginated list of destinations that match the specified criteria.</returns>
		/// <response code="200">Returns a paged list of destinations if found.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no destinations are found.</response>
		/// <response code="401">If user does not have permission to access the requested resource.</response>
		[HttpGet("search/")]
		[ProducesResponseType(200, Type = typeof(PagedResponse<DestinationDto>))]
		[ProducesResponseType(400, Type = typeof(string))]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PagedResponse<DestinationDto>>> GetDestinationsByCityOrAirport(
			CancellationToken cancellationToken,
			[FromQuery] string? city = null,
			[FromQuery] string? airport = null,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10)
		{
			if (string.IsNullOrEmpty(city) && string.IsNullOrEmpty(airport))
			{
				_logger.LogInformation("Both city and airport are missing in the request.");
				return BadRequest("Both city and airport are missing in the request.");
			}
			var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
			if (!isValid)
			{
				return result;
			}
			var destinations = await _destinationService.GetDestinationsByCityOrAirport(cancellationToken, page, correctedPageSize, city, airport);
			if (destinations == null || destinations.Count == 0)
			{
				_logger.LogInformation("Destinations not found.");
				return NotFound();
			}
			var totalItems = await _destinationService.DestinationsCount(cancellationToken, city, airport);
			var data = _mapper.Map<IEnumerable<DestinationDto>>(destinations);
			var response = new PagedResponse<DestinationDto>(data, page, correctedPageSize, totalItems);
			return Ok(response);
		}

		/// <summary>
		/// Retrieves a paginated list of destinations matching the specified search filter criteria.
		/// </summary>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <param name="filter">The search filter containing destination fields to filter by.</param>
		/// <param name="page">The page number for pagination (optional, default is 1).</param>
		/// <param name="pageSize">The number of items per page for pagination (optional, default is 10).</param>
		/// <returns>A paged response containing the list of destinations that match the filter criteria.</returns>
		/// <response code="200">Returns a paged list of destinations if found.</response>
		/// <response code="400">If the request is invalid or the filter criteria are missing or invalid.</response>
		/// <response code="404">If no destinations matching the filter criteria are found.</response>
		/// <response code="401">If the user does not have permission to access the requested resource.</response>
		[HttpGet("byFilter")]
		[ProducesResponseType(200, Type = typeof(PagedResponse<DestinationDto>))]
		[ProducesResponseType(400, Type = typeof(string))]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<PagedResponse<DestinationDto>>> GetDestinationsByFilter(
			CancellationToken cancellationToken,
			[FromQuery] DestinationSearchFilter filter,
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
			var destinations = await _destinationService.GetDestinationsByFilter(cancellationToken, page, correctedPageSize, filter);
			if (destinations == null || destinations.Count == 0)
			{
				_logger.LogInformation("Destinations not found.");
				return NotFound();
			}
			var totalItems = await _destinationService.DestinationsCountFilter(cancellationToken, filter);
			var data = _mapper.Map<IEnumerable<DestinationDto>>(destinations);
			var response = new PagedResponse<DestinationDto>(data, page, correctedPageSize, totalItems);
			return Ok(response);
		}

		/// <summary>
		/// Endpoint for creating a new destination.
		/// </summary>
		/// <param name="destinationCreateDto">The data to create the new destination.</param>
		/// <returns>The created destination.</returns>
		/// <response code="201">Returns the created destination if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		/// <response code="403">If the user does not have permission to access the requested resource.</response>
		[HttpPost]
		[Authorize(Policy = "RequireAdminRole")]
		[ProducesResponseType(201, Type = typeof(DestinationDto))]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		public async Task<ActionResult<DestinationDto>> PostDestination(DestinationCreateDto destinationCreateDto)
		{
			var destination = _mapper.Map<DestinationEntity>(destinationCreateDto);
			await _destinationService.PostDestination(destination);
			var destinationDto = _mapper.Map<DestinationDto>(destination);
			return CreatedAtAction("GetDestination", new { id = destinationDto.Id }, destinationDto);
		}

		/// <summary>
		/// Endpoint for updating a single destination.
		/// </summary>
		/// <param name="id">The ID of the destination to update.</param>
		/// <param name="destinationDto">The data to update the destination.</param>
		/// <returns>No content.</returns>
		/// <response code="204">Returns no content if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no destination is found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		/// <response code="403">If the user does not have permission to access the requested resource.</response>
		[HttpPut("{id}")]
		[Authorize(Policy = "RequireAdminRole")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		public async Task<IActionResult> PutDestination(int id, DestinationDto destinationDto)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {Id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (id != destinationDto.Id)
			{
				_logger.LogInformation("Destination with id {Id} is different from provided Destination and his id.", id);
				return BadRequest();
			}
			if (!await _destinationService.DestinationExists(id))
			{
				_logger.LogInformation("Destination with id {Id} not found.", id);
				return NotFound();
			}
			var destination = _mapper.Map<DestinationEntity>(destinationDto);
			await _destinationService.PutDestination(destination);
			return NoContent();
		}

		/// <summary>
		/// Endpoint for partially updating a single destination.
		/// </summary>
		/// <param name="id">The ID of the destination to partially update.</param>
		/// <param name="destinationDocument">The patch document containing the changes.</param>
		/// <returns>The updated destination.</returns>
		/// <remarks>
		/// The JSON document should follow the JSON Patch standard (RFC 6902) and contain one or more operations.
		/// Example operation:
		/// {
		///     "op": "replace",
		///     "path": "/Name",
		///     "value": "NewName"
		/// }
		/// </remarks>
		/// <response code="200">Returns the updated destination if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If the destination with the specified ID is not found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		/// <response code="403">If the user does not have permission to access the requested resource.</response>
		[HttpPatch("{id}")]
		[Authorize(Policy = "RequireAdminRole")]
		[ProducesResponseType(200, Type = typeof(DestinationDto))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		public async Task<IActionResult> PatchDestination(int id, [FromBody] JsonPatchDocument destinationDocument)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {Id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (!await _destinationService.DestinationExists(id))
			{
				_logger.LogInformation("Destination with id {Id} not found.", id);
				return NotFound();
			}
			var updatedDestination = await _destinationService.PatchDestination(id, destinationDocument);
			var destinationDto = _mapper.Map<DestinationDto>(updatedDestination);
			return Ok(destinationDto);
		}

		/// <summary>
		/// Endpoint for deleting a single destination.
		/// </summary>
		/// <param name="id">The ID of the destination to delete.</param>
		/// <returns>No content.</returns>
		/// <response code="204">Returns no content if successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="404">If no destination is found.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		/// <response code="403">If the user does not have permission to access the requested resource.</response>
		/// <response code="409">Conflict. If the passenger cannot be deleted because it is being referenced by other entities.</response>
		[HttpDelete("{id}")]
		[Authorize(Policy = "RequireAdminRole")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		[ProducesResponseType(409)]
		public async Task<IActionResult> DeleteDestination(int id)
		{
			if (!_inputValidationService.IsNonNegativeInt(id))
			{
				_logger.LogInformation("Invalid input. The ID {Id} must be a non-negative integer.", id);
				return BadRequest("Invalid input. The ID must be a non-negative integer.");
			}
			if (!await _destinationService.DestinationExists(id))
			{
				_logger.LogInformation("Destination with id {Id} not found.", id);
				return NotFound();
			}
			bool deleted = await _destinationService.DeleteDestination(id);
			if (deleted)
			{
				return NoContent();
			}
			else
			{
				_logger.LogInformation("Destination with id {Id} is being referenced by other entities and cannot be deleted.", id);
				return Conflict("Destination cannot be deleted because it is being referenced by other entities.");
			}
		}

		/// <summary>
		/// Endpoint for exporting destination data to PDF.
		/// </summary>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <param name="page">The page number for pagination (optional, default is 1).</param>
		/// <param name="pageSize">The page size for pagination (optional, default is 10).</param>
		/// <param name="getAll">Flag indicating whether to retrieve all data (optional, default is false).</param>
		/// <param name="city">The city name to search for (optional, default is null).</param>
		/// <param name="airport">The airport name to search for (optional, default is null).</param>
		/// <returns>Returns the generated PDF document.</returns>
		/// <response code="200">Returns the generated PDF document.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		/// <response code="403">If the user does not have permission to access the requested resource.</response>
		[HttpGet("export/pdf")]
		[Authorize(Policy = "RequireAdminRole")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		public async Task<ActionResult> ExportToPdf(
			CancellationToken cancellationToken,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10,
			[FromQuery] bool getAll = false,
			[FromQuery] string? city = null,
			[FromQuery] string? airport = null)
		{
			IList<DestinationEntity> destinations;
			if (getAll)
			{
				destinations = await _destinationService.GetAllDestinations(cancellationToken);
			}
			else
			{
				var (isValid, correctedPageSize, result) = _paginationValidationService.ValidatePaginationParameters(page, pageSize, maxPageSize);
				if (!isValid)
				{
					return result;
				}
				if (string.IsNullOrEmpty(city) && string.IsNullOrEmpty(airport))
				{
					destinations = await _destinationService.GetDestinations(cancellationToken, page, correctedPageSize);
				}
				else
				{
					destinations = await _destinationService.GetDestinationsByCityOrAirport(cancellationToken, page, correctedPageSize, city, airport);
				}
			}
			if (destinations is null || !destinations.Any())
			{
				_logger.LogInformation("Destinations not found.");
				return NoContent();
			}
			var pdf = _exportService.ExportToPDF("Destinations", destinations);
			string fileName = _utilityService.GenerateUniqueFileName("Destinations");
			return File(pdf, "application/pdf", fileName);
		}

	}
}
