using AirportAutomation.Core.Filters;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;

namespace AirportAutomation.Web.Services
{
	public abstract class BaseHttpService
	{
		protected readonly IHttpClientFactory _httpClientFactory;
		protected readonly IHttpContextAccessor _httpContextAccessor;
		protected readonly IConfiguration _configuration;
		protected readonly ILogger _logger;
		protected readonly string _apiUrl;

		private static readonly Dictionary<string, string> _pluralSuffixes = new()
		{
			{ "TravelClass", "es" }
		};

		protected BaseHttpService(
			IHttpClientFactory httpClientFactory,
			IHttpContextAccessor httpContextAccessor,
			IConfiguration configuration,
			ILogger logger)
		{
			_httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
			_httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_apiUrl = _configuration.GetValue<string>("ApiSettings:apiUrl");
		}

		public string GetToken()
		{
			string token = _httpContextAccessor.HttpContext.Session.GetString("AccessToken");
			if (!string.IsNullOrEmpty(token)) return token;
			_logger.LogInformation("Bearer token is missing or invalid.");
			return string.Empty;
		}

		protected void ConfigureHttpClient(HttpClient httpClient)
		{
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("HttpRequestsSample");
			string token = GetToken();
			if (!string.IsNullOrEmpty(token))
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			else
				_logger.LogInformation("Bearer token is missing or invalid.");
		}

		protected string GetModelName<T>()
		{
			var modelName = typeof(T).Name;
			const string entitySuffix = "Entity";
			if (modelName.EndsWith(entitySuffix))
				modelName = modelName[..^entitySuffix.Length];
			return modelName;
		}

		protected string GetPluralSuffix(string modelName)
		{
			return _pluralSuffixes.TryGetValue(modelName, out var suffix) ? suffix : "s";
		}

		protected string BuildRequestUriByModelName(string modelName, object filter, int page, int pageSize)
		{
			var pluralSuffix = GetPluralSuffix(modelName);
			var requestUri = $"{_apiUrl}/{modelName}{pluralSuffix}/search";
			var filterQuery = BuildFilterQueryString(modelName, filter);
			requestUri += $"?{filterQuery}&page={page}&pageSize={pageSize}";
			return requestUri;
		}

		protected string BuildFilterQueryString(string modelName, object filter)
		{
			if (filter == null) return string.Empty;

			var queryParameters = new List<string>();

			if (filter is string nameFilter)
			{
				if (!string.IsNullOrWhiteSpace(nameFilter))
					queryParameters.Add($"Name={Uri.EscapeDataString(nameFilter)}");
			}

			else if (filter is Dictionary<string, string> dictFilter)
			{
				foreach (var kvp in dictFilter)
				{
					if (!string.IsNullOrWhiteSpace(kvp.Value))
					{
						queryParameters.Add($"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}");
					}
				}
			}
			else
			{
				switch (modelName.ToLower())
				{
					case "apiuser":
						if (filter is ApiUserSearchFilter userFilter)
						{
							if (!string.IsNullOrEmpty(userFilter.UserName))
								queryParameters.Add($"UserName={Uri.EscapeDataString(userFilter.UserName)}");
							if (!string.IsNullOrEmpty(userFilter.Roles))
								queryParameters.Add($"Roles={Uri.EscapeDataString(userFilter.Roles)}");
						}
						break;

					case "destination":
						if (filter is DestinationSearchFilter destFilter)
						{
							if (!string.IsNullOrEmpty(destFilter.City))
								queryParameters.Add($"City={Uri.EscapeDataString(destFilter.City)}");
							if (!string.IsNullOrEmpty(destFilter.Airport))
								queryParameters.Add($"Airport={Uri.EscapeDataString(destFilter.Airport)}");
						}
						break;

					case "passenger":
						if (filter is PassengerSearchFilter passengerFilter)
						{
							if (!string.IsNullOrEmpty(passengerFilter.FirstName))
								queryParameters.Add($"FirstName={Uri.EscapeDataString(passengerFilter.FirstName)}");
							if (!string.IsNullOrEmpty(passengerFilter.LastName))
								queryParameters.Add($"LastName={Uri.EscapeDataString(passengerFilter.LastName)}");
							if (!string.IsNullOrEmpty(passengerFilter.UPRN))
								queryParameters.Add($"UPRN={Uri.EscapeDataString(passengerFilter.UPRN)}");
							if (!string.IsNullOrEmpty(passengerFilter.Passport))
								queryParameters.Add($"Passport={Uri.EscapeDataString(passengerFilter.Passport)}");
							if (!string.IsNullOrEmpty(passengerFilter.Address))
								queryParameters.Add($"Address={Uri.EscapeDataString(passengerFilter.Address)}");
							if (!string.IsNullOrEmpty(passengerFilter.Phone))
								queryParameters.Add($"Phone={Uri.EscapeDataString(passengerFilter.Phone)}");
						}
						break;

					case "pilot":
						if (filter is PilotSearchFilter pilotFilter)
						{
							if (!string.IsNullOrEmpty(pilotFilter.FirstName))
								queryParameters.Add($"FirstName={Uri.EscapeDataString(pilotFilter.FirstName)}");
							if (!string.IsNullOrEmpty(pilotFilter.LastName))
								queryParameters.Add($"LastName={Uri.EscapeDataString(pilotFilter.LastName)}");
							if (!string.IsNullOrEmpty(pilotFilter.UPRN))
								queryParameters.Add($"UPRN={Uri.EscapeDataString(pilotFilter.UPRN)}");
							if (pilotFilter.FlyingHours.HasValue)
								queryParameters.Add($"FlyingHours={pilotFilter.FlyingHours.Value}");
						}
						break;

					case "planeticket":
						if (filter is PlaneTicketSearchFilter ticketFilter)
						{
							if (ticketFilter.Price.HasValue)
								queryParameters.Add($"Price={ticketFilter.Price.Value}");
							if (ticketFilter.PurchaseDate.HasValue)
								queryParameters.Add($"PurchaseDate={Uri.EscapeDataString(ticketFilter.PurchaseDate.Value.ToString("yyyy-MM-dd"))}");
							if (ticketFilter.SeatNumber.HasValue)
								queryParameters.Add($"SeatNumber={ticketFilter.SeatNumber.Value}");
						}
						break;

					default:
						break;
				}
			}
			return string.Join("&", queryParameters);
		}
	}
}