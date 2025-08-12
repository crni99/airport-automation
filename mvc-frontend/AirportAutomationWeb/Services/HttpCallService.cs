using AirportAutomation.Core.Filters;
using AirportAutomation.Web.Interfaces;
using AirportAutomation.Web.Models.ApiUser;
using AirportAutomation.Web.Models.Response;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.JsonWebTokens;
using QuestPDF.Helpers;
using System.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Web;

namespace AirportAutomation.Web.Services
{
	/// <summary>
	/// Service for making HTTP calls to an API.
	/// </summary>
	public class HttpCallService : IHttpCallService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IConfiguration _configuration;
		private readonly ILogger<HttpCallService> _logger;
		private readonly string apiURL;

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpCallService"/> class.
		/// </summary>
		/// <param name="httpClientFactory">Factory for creating HTTP clients.</param>
		/// <param name="httpContextAccessor">Accessor for accessing HTTP context information. Can be null if not available.</param>
		/// <param name="configuration">Configuration settings for the application.</param>
		/// <param name="logger">Logger for logging information and errors.</param>
		public HttpCallService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor,
			IConfiguration configuration, ILogger<HttpCallService> logger)
		{
			_httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
			_httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			apiURL = _configuration.GetValue<string>("ApiSettings:apiUrl");
		}

		/// <summary>
		/// Sends user data to authenticate and retrieves an access token.
		/// </summary>
		/// <param name="user">The user data for authentication.</param>
		/// <returns>Returns true if authentication is successful; otherwise, false.</returns>
		/// <remarks>
		/// If the authentication is successful, the access token is stored in the session.
		/// </remarks>
		public async Task<bool> Authenticate(UserViewModel user)
		{
			string token = GetToken();
			if (!string.IsNullOrEmpty(token))
			{
				return true;
			}

			var bearerToken = string.Empty;
			var requestUri = $"{apiURL}/Authentication";

			using var httpClient = _httpClientFactory.CreateClient("AirportAutomationApi");
			ConfigureHttpClient(httpClient);

			var response = await httpClient.PostAsJsonAsync(requestUri, user).ConfigureAwait(false);

			if (response.IsSuccessStatusCode)
			{
				bearerToken += await response.Content.ReadFromJsonAsync<string>().ConfigureAwait(false);
				if (!SetApiUserRole(bearerToken))
				{
					return false;
				}
				_httpContextAccessor.HttpContext.Session.SetString("AccessToken", bearerToken);
				return true;
			}
			else if (((int)response.StatusCode) == 400 || ((int)response.StatusCode) == 401)
			{
				_logger.LogInformation("Unauthorized. Status code: {StatusCode}", response.StatusCode);
			}
			else
			{
				_logger.LogInformation("Failed to authenticate. Status code: {StatusCode}", response.StatusCode);
			}
			return false;
		}

		/// <summary>
		/// Adds the Bearer token to the headers for authorization.
		/// </summary>
		/// <param name="httpClient">The HttpClient instance to which the Authorization header will be added.</param>
		/// <remarks>
		/// If the Bearer token is missing or invalid, an error is logged.
		/// </remarks>
		private void AddAuthorizationHeader(HttpClient httpClient)
		{
			string token = GetToken();
			if (!string.IsNullOrEmpty(token))
			{
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			}
			else
			{
				_logger.LogInformation("Bearer token is missing or invalid.");
			}
		}

		/// <summary>
		/// Retrieves the Bearer token from the session.
		/// </summary>
		/// <returns>Returns the Bearer token if it exists; otherwise, an empty string.</returns>
		/// <remarks>
		/// If the Bearer token is missing or invalid, an error is logged.
		/// </remarks>
		public string GetToken()
		{
			string token = _httpContextAccessor.HttpContext.Session.GetString("AccessToken");
			if (!string.IsNullOrEmpty(token))
			{
				return token;
			}
			else
			{
				_logger.LogInformation("Bearer token is missing or invalid.");
			}
			return string.Empty;
		}

		/// <summary>
		/// Removes the Bearer token from the session for sign-out.
		/// </summary>
		/// <returns>Returns true if the token is successfully removed; otherwise, false.</returns>
		/// <remarks>
		/// If the Bearer token is missing or invalid, the removal is not performed.
		/// </remarks>
		public bool RemoveToken()
		{
			string token = GetToken();
			if (!string.IsNullOrEmpty(token))
			{
				_httpContextAccessor.HttpContext.Session.Remove("AccessToken");
				_httpContextAccessor.HttpContext.Session.Remove("AccessRole");
				_httpContextAccessor.HttpContext.Session.CommitAsync().Wait();
				return true;
			}
			return false;
		}

		private bool SetApiUserRole(string token)
		{
			var handler = new JsonWebTokenHandler();
			if (handler.CanReadToken(token))
			{
				try
				{
					var jwtToken = handler.ReadJsonWebToken(token);
					var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type.Equals("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", StringComparison.OrdinalIgnoreCase));

					if (roleClaim != null)
					{
						_httpContextAccessor.HttpContext.Session.SetString("AccessRole", roleClaim.Value);
						return true;
					}
					else
					{
						_logger.LogInformation("Role claim not found in the token.");
						return false;
					}
				}
				catch (Exception ex)
				{
					_logger.LogInformation("Error decoding token: {Message}", ex.Message);
					return false;
				}
			}
			return false;
		}

		/// <summary>
		/// Gets a paginated list of data for a specified model type.
		/// </summary>
		/// <typeparam name="T">The type of data to retrieve.</typeparam>
		/// <param name="page">The page number of the data to retrieve.</param>
		/// <param name="pageSize">The number of items per page.</param>
		/// <returns>
		/// Returns a <see cref="PagedResponse{T}"/> containing the paginated list of data.
		/// If the data is not found, an empty response with a status code of 204 is returned.
		/// If the retrieval fails, a null value is returned with an error logged.
		/// </returns>
		public async Task<PagedResponse<T>> GetDataList<T>(int page, int pageSize)
		{
			var modelName = GetModelName<T>();
			var requestUri = $"{apiURL}/{modelName}";
			if (modelName.Equals("TravelClass"))
			{
				requestUri += $"es/";
			}
			else
			{
				requestUri += $"s/";
			}
			requestUri += $"?page={page}&pageSize={pageSize}";

			using var httpClient = _httpClientFactory.CreateClient("AirportAutomationApi");
			ConfigureHttpClient(httpClient);

			var response = await httpClient.GetAsync(requestUri).ConfigureAwait(false);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				return await response.Content.ReadFromJsonAsync<PagedResponse<T>>().ConfigureAwait(false);
			}
			else if (response.StatusCode == HttpStatusCode.NoContent)
			{
				_logger.LogInformation("Data not found. Status code: {StatusCode}", response.StatusCode);
				return new PagedResponse<T>(Enumerable.Empty<T>(), page, pageSize, 0);
			}
			else
			{
				_logger.LogInformation("Failed to retrieve data. Status code: {StatusCode}", response.StatusCode);
				return null;
			}
		}

		/// <summary>
		/// Gets data of a specified type by its unique identifier.
		/// </summary>
		/// <typeparam name="T">The type of data to retrieve.</typeparam>
		/// <param name="id">The unique identifier of the data.</param>
		/// <returns>
		/// Returns the data of type <typeparamref name="T"/> with the specified unique identifier.
		/// If the retrieval fails, a default value for <typeparamref name="T"/> is returned with an error logged.
		/// </returns>
		public async Task<T> GetData<T>(int id)
		{
			T data = default;
			var modelName = GetModelName<T>();

			string requestUri = $"{apiURL}/{modelName}";
			if (modelName.Equals("TravelClass"))
			{
				requestUri += $"es/{id}";
			}
			else
			{
				requestUri += $"s/{id}";
			}

			var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

			using var httpClient = _httpClientFactory.CreateClient("AirportAutomationApi");
			ConfigureHttpClient(httpClient);

			var response = await httpClient.SendAsync(httpRequestMessage);

			if (response.IsSuccessStatusCode)
			{
				data = await response.Content.ReadFromJsonAsync<T>().ConfigureAwait(false);
			}
			else
			{
				_logger.LogInformation("Failed to retrieve data. Status code: {StatusCode}", response.StatusCode);
			}
			return data;
		}

		/// <summary>
		/// Gets a list of data of a specified type in a paginated format.
		/// </summary>
		/// <typeparam name="T">The type of data to retrieve.</typeparam>
		/// <returns>
		/// Returns a paginated response containing a list of data of type <typeparamref name="T"/>.
		/// If the retrieval fails, returns null with an error logged.
		/// </returns>
		public async Task<PagedResponse<T>> GetDataList<T>()
		{
			var modelName = GetModelName<T>();

			string requestUri = $"{apiURL}/{modelName}";
			if (modelName.Equals("TravelClass"))
			{
				requestUri += "es";
			}
			else
			{
				requestUri += "s";
			}

			using var httpClient = _httpClientFactory.CreateClient("AirportAutomationApi");
			ConfigureHttpClient(httpClient);

			var response = await httpClient.GetAsync(requestUri).ConfigureAwait(false);

			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadFromJsonAsync<PagedResponse<T>>().ConfigureAwait(false);
			}
			else
			{
				_logger.LogInformation("Failed to retrieve data. Status code: {StatusCode}", response.StatusCode);
			}
			return null;
		}

		/// <summary>
		/// Gets data of a specified type by name in JSON format.
		/// </summary>
		/// <typeparam name="T">The type of data to retrieve.</typeparam>
		/// <param name="name">The name used to filter the data.</param>
		/// <param name="page">The page number of the data to retrieve.</param>
		/// <param name="pageSize">The number of items per page.</param>
		/// <returns>
		/// Returns a JSON string containing the data of type <typeparamref name="T"/> filtered by the specified name.
		/// If the retrieval fails, returns null with an error logged.
		/// </returns>
		public async Task<PagedResponse<T>> GetDataByName<T>(string name, int page, int pageSize)
		{
			var modelName = GetModelName<T>();

			string requestUri = $"{apiURL}/{modelName}";
			if (modelName.Equals("TravelClass"))
			{
				requestUri += $"es/byName/{name}";
			}
			else
			{
				requestUri += $"s/byName/{name}";
			}
			requestUri += $"?page={page}&pageSize={pageSize}";

			var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

			using var httpClient = _httpClientFactory.CreateClient("AirportAutomationApi");
			ConfigureHttpClient(httpClient);

			var response = await httpClient.SendAsync(httpRequestMessage);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				return await response.Content.ReadFromJsonAsync<PagedResponse<T>>().ConfigureAwait(false);
			}
			else if (response.StatusCode == HttpStatusCode.NoContent)
			{
				_logger.LogInformation("Data not found. Status code: {StatusCode}", response.StatusCode);
				return new PagedResponse<T>(Enumerable.Empty<T>(), page, pageSize, 0);
			}
			else
			{
				_logger.LogInformation("Failed to retrieve data. Status code: {StatusCode}", response.StatusCode);
				return null;
			}
		}

		/// <summary>
		/// Gets data of a specified type by first name or last name in JSON format.
		/// </summary>
		/// <typeparam name="T">The type of data to retrieve.</typeparam>
		/// <param name="firstName">The first name used to filter the data.</param>
		/// <param name="lastName">The last name used to filter the data.</param>
		/// <param name="page">The page number of the data to retrieve.</param>
		/// <param name="pageSize">The number of items per page.</param>
		/// <returns>
		/// Returns a JSON string containing the data of type <typeparamref name="T"/> filtered by the specified first name and/or last name.
		/// If the retrieval fails, returns null with an error logged.
		/// </returns>
		public async Task<PagedResponse<T>> GetDataByFNameOrLName<T>(string firstName, string lastName, int page, int pageSize)
		{
			var modelName = GetModelName<T>();

			string requestUri = $"{apiURL}/{modelName}";
			if (modelName.Equals("TravelClass"))
			{
				requestUri += $"es/byName";
			}
			else
			{
				requestUri += $"s/byName/";
			}
			UriBuilder uriBuilder = new(requestUri);
			var query = HttpUtility.ParseQueryString(uriBuilder.Query);

			if (!string.IsNullOrEmpty(firstName))
			{
				query["firstName"] = firstName;
			}
			if (!string.IsNullOrEmpty(lastName))
			{
				query["lastName"] = lastName;
			}
			query["page"] = page.ToString();
			query["pageSize"] = pageSize.ToString();
			uriBuilder.Query = query.ToString();
			requestUri = uriBuilder.ToString();

			var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

			using var httpClient = _httpClientFactory.CreateClient("AirportAutomationApi");
			ConfigureHttpClient(httpClient);

			var response = await httpClient.GetAsync(requestUri).ConfigureAwait(false);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				return await response.Content.ReadFromJsonAsync<PagedResponse<T>>().ConfigureAwait(false);
			}
			else if (response.StatusCode == HttpStatusCode.NoContent)
			{
				_logger.LogInformation("Data not found. Status code: {StatusCode}", response.StatusCode);
				return new PagedResponse<T>(Enumerable.Empty<T>(), page, pageSize, 0);
			}
			else
			{
				_logger.LogInformation("Failed to retrieve data. Status code: {StatusCode}", response.StatusCode);
				return null;
			}
		}

		/// <summary>
		/// Gets data of a specified type by price range in JSON format.
		/// </summary>
		/// <typeparam name="T">The type of data to retrieve.</typeparam>
		/// <param name="minPrice">The minimum price used to filter the data.</param>
		/// <param name="maxPrice">The maximum price used to filter the data.</param>
		/// <param name="page">The page number of the data to retrieve.</param>
		/// <param name="pageSize">The number of items per page.</param>
		/// <returns>
		/// Returns a JSON string containing the data of type <typeparamref name="T"/> filtered by the specified price range.
		/// If the retrieval fails, returns null with an error logged.
		/// </returns>
		public async Task<PagedResponse<T>> GetDataForPrice<T>(int? minPrice, int? maxPrice, int page, int pageSize)
		{
			var modelName = GetModelName<T>();

			string requestUri = $"{apiURL}/{modelName}";
			if (modelName.Equals("TravelClass"))
			{
				requestUri += $"es/byPrice";
			}
			else
			{
				requestUri += $"s/byPrice/";
			}
			UriBuilder uriBuilder = new(requestUri);
			var query = HttpUtility.ParseQueryString(uriBuilder.Query);

			if (minPrice != null)
			{
				query["minPrice"] = minPrice.ToString();
			}
			if (maxPrice != null)
			{
				query["maxPrice"] = maxPrice.ToString();
			}
			query["page"] = page.ToString();
			query["pageSize"] = pageSize.ToString();
			uriBuilder.Query = query.ToString();
			requestUri = uriBuilder.ToString();

			var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

			using var httpClient = _httpClientFactory.CreateClient("AirportAutomationApi");
			ConfigureHttpClient(httpClient);

			var response = await httpClient.SendAsync(httpRequestMessage);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				return await response.Content.ReadFromJsonAsync<PagedResponse<T>>().ConfigureAwait(false);
			}
			else if (response.StatusCode == HttpStatusCode.NoContent)
			{
				_logger.LogInformation("Data not found. Status code: {StatusCode}", response.StatusCode);
				return new PagedResponse<T>(Enumerable.Empty<T>(), page, pageSize, 0);
			}
			else
			{
				_logger.LogInformation("Failed to retrieve data. Status code: {StatusCode}", response.StatusCode);
				return null;
			}
		}

		/// <summary>
		/// Gets data of a specified type by date range in JSON format.
		/// </summary>
		/// <typeparam name="T">The type of data to retrieve.</typeparam>
		/// <param name="startDate">The start date used to filter the data.</param>
		/// <param name="endDate">The end date used to filter the data.</param>
		/// <param name="page">The page number of the data to retrieve.</param>
		/// <param name="pageSize">The number of items per page.</param>
		/// <returns>
		/// Returns a JSON string containing the data of type <typeparamref name="T"/> filtered by the specified date range.
		/// If the retrieval fails, returns null with an error logged.
		/// </returns>
		public async Task<PagedResponse<T>> GetDataBetweenDates<T>(string startDate, string endDate, int page, int pageSize)
		{
			var modelName = GetModelName<T>();

			string requestUri = $"{apiURL}/{modelName}";
			if (modelName.Equals("TravelClass"))
			{
				requestUri += $"es/byDate";
			}
			else
			{
				requestUri += $"s/byDate/";
			}
			UriBuilder uriBuilder = new(requestUri);
			var query = HttpUtility.ParseQueryString(uriBuilder.Query);

			if (!string.IsNullOrEmpty(startDate))
			{
				query["startDate"] = startDate;
			}
			if (!string.IsNullOrEmpty(endDate))
			{
				query["endDate"] = endDate;
			}
			query["page"] = page.ToString();
			query["pageSize"] = pageSize.ToString();
			uriBuilder.Query = query.ToString();
			requestUri = uriBuilder.ToString();

			var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

			using var httpClient = _httpClientFactory.CreateClient("AirportAutomationApi");
			ConfigureHttpClient(httpClient);

			var response = await httpClient.SendAsync(httpRequestMessage);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				return await response.Content.ReadFromJsonAsync<PagedResponse<T>>().ConfigureAwait(false);
			}
			else if (response.StatusCode == HttpStatusCode.NoContent)
			{
				_logger.LogInformation("Data not found. Status code: {StatusCode}", response.StatusCode);
				return new PagedResponse<T>(Enumerable.Empty<T>(), page, pageSize, 0);
			}
			else
			{
				_logger.LogInformation("Failed to retrieve data. Status code: {StatusCode}", response.StatusCode);
				return null;
			}
		}

		/// <summary>
		/// Gets data of a specified type by city or airport in JSON format.
		/// </summary>
		/// <typeparam name="T">The type of data to retrieve.</typeparam>
		/// <param name="city">The city used to filter the data.</param>
		/// <param name="airport">The airport used to filter the data.</param>
		/// <param name="page">The page number of the data to retrieve.</param>
		/// <param name="pageSize">The number of items per page.</param>
		/// <returns>
		/// Returns a JSON string containing the data of type <typeparamref name="T"/> filtered by the specified first name and/or last name.
		/// If the retrieval fails, returns null with an error logged.
		/// </returns>
		public async Task<PagedResponse<T>> GetDataByCityOrAirport<T>(string city, string airport, int page, int pageSize)
		{
			var modelName = GetModelName<T>();

			string requestUri = $"{apiURL}/{modelName}";
			if (modelName.Equals("TravelClass"))
			{
				requestUri += $"es/search";
			}
			else
			{
				requestUri += $"s/search/";
			}
			UriBuilder uriBuilder = new(requestUri);
			var query = HttpUtility.ParseQueryString(uriBuilder.Query);

			if (!string.IsNullOrEmpty(city))
			{
				query["city"] = city;
			}
			if (!string.IsNullOrEmpty(airport))
			{
				query["airport"] = airport;
			}
			query["page"] = page.ToString();
			query["pageSize"] = pageSize.ToString();
			uriBuilder.Query = query.ToString();
			requestUri = uriBuilder.ToString();

			var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

			using var httpClient = _httpClientFactory.CreateClient("AirportAutomationApi");
			ConfigureHttpClient(httpClient);

			var response = await httpClient.SendAsync(httpRequestMessage);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				return await response.Content.ReadFromJsonAsync<PagedResponse<T>>().ConfigureAwait(false);
			}
			else if (response.StatusCode == HttpStatusCode.NoContent)
			{
				_logger.LogInformation("Data not found. Status code: {StatusCode}", response.StatusCode);
				return new PagedResponse<T>(Enumerable.Empty<T>(), page, pageSize, 0);
			}
			else
			{
				_logger.LogInformation("Failed to retrieve data. Status code: {StatusCode}", response.StatusCode);
				return null;
			}
		}

		/// <summary>
		/// Gets data of a specified type by role in JSON format.
		/// </summary>
		/// <typeparam name="T">The type of data to retrieve.</typeparam>
		/// <param name="role">The role used to filter the data.</param>
		/// <param name="page">The page number of the data to retrieve.</param>
		/// <param name="pageSize">The number of items per page.</param>
		/// <returns>
		/// Returns a JSON string containing the data of type <typeparamref name="T"/> filtered by the specified role.
		/// If the retrieval fails, returns null with an error logged.
		/// </returns>
		public async Task<PagedResponse<T>> GetDataByRole<T>(string role, int page, int pageSize)
		{
			var modelName = GetModelName<T>();

			string requestUri = $"{apiURL}/{modelName}";
			if (modelName.Equals("TravelClass"))
			{
				requestUri += $"es/byRole/{role}";
			}
			else
			{
				requestUri += $"s/byRole/{role}";
			}
			requestUri += $"?page={page}&pageSize={pageSize}";

			var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

			using var httpClient = _httpClientFactory.CreateClient("AirportAutomationApi");
			ConfigureHttpClient(httpClient);

			var response = await httpClient.SendAsync(httpRequestMessage);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				return await response.Content.ReadFromJsonAsync<PagedResponse<T>>().ConfigureAwait(false);
			}
			else if (response.StatusCode == HttpStatusCode.NoContent)
			{
				_logger.LogInformation("Data not found. Status code: {StatusCode}", response.StatusCode);
				return new PagedResponse<T>(Enumerable.Empty<T>(), page, pageSize, 0);
			}
			else
			{
				_logger.LogInformation("Failed to retrieve data. Status code: {StatusCode}", response.StatusCode);
				return null;
			}
		}

		public async Task<PagedResponse<T>> GetDataByFilter<T>(object filter, int page, int pageSize)
		{
			var modelName = GetModelName<T>();

			string requestUri = BuildRequestUriByModelName(modelName, filter, page, pageSize);

			var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

			using var httpClient = _httpClientFactory.CreateClient("AirportAutomationApi");
			ConfigureHttpClient(httpClient);

			var response = await httpClient.SendAsync(httpRequestMessage);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				return await response.Content.ReadFromJsonAsync<PagedResponse<T>>().ConfigureAwait(false);
			}
			else if (response.StatusCode == HttpStatusCode.NoContent)
			{
				_logger.LogInformation("Data not found. Status code: {StatusCode}", response.StatusCode);
				return new PagedResponse<T>(Enumerable.Empty<T>(), page, pageSize, 0);
			}
			else
			{
				_logger.LogInformation("Failed to retrieve data. Status code: {StatusCode}", response.StatusCode);
				return null;
			}
		}

		/// <summary>
		/// Creates a new data entry of a specified type.
		/// </summary>
		/// <typeparam name="T">The type of data to create.</typeparam>
		/// <param name="t">The data object to be created.</param>
		/// <returns>
		/// Returns the newly created data object of type <typeparamref name="T"/>.
		/// If the creation fails, returns the default value of <typeparamref name="T"/> with an error logged.
		/// </returns>
		public async Task<T> CreateData<T>(T t)
		{
			T data = default;
			var modelName = GetModelName<T>();
			string requestUri = $"{apiURL}/{modelName}s";

			using var httpClient = _httpClientFactory.CreateClient("AirportAutomationApi");
			ConfigureHttpClient(httpClient);

			var response = await httpClient.PostAsJsonAsync(requestUri, t).ConfigureAwait(false);

			if (response.IsSuccessStatusCode)
			{
				data = await response.Content.ReadFromJsonAsync<T>().ConfigureAwait(false);
			}
			else
			{
				_logger.LogInformation("Failed to create data. Status code: {StatusCode}", response.StatusCode);
			}
			return data;
		}

		/// <summary>
		/// Edits an existing data entry of a specified type by updating it with the provided data.
		/// </summary>
		/// <typeparam name="T">The type of data to edit.</typeparam>
		/// <param name="t">The updated data object.</param>
		/// <param name="id">The identifier of the data entry to be edited.</param>
		/// <returns>
		/// Returns true if the data entry is successfully edited (HTTP status code 204 - No Content).
		/// Returns false if the edit fails or encounters an error, with an error message logged.
		/// </returns>
		public async Task<bool> EditData<T>(T t, int id)
		{
			var modelName = GetModelName<T>();
			string requestUri = $"{apiURL}/{modelName}s/{id}";

			using var httpClient = _httpClientFactory.CreateClient("AirportAutomationApi");
			ConfigureHttpClient(httpClient);

			var response = await httpClient.PutAsJsonAsync(requestUri, t);

			if (response.StatusCode is HttpStatusCode.NoContent)
			{
				return true;
			}
			else
			{
				_logger.LogInformation("Failed to edit data. Status code: {StatusCode}", response.StatusCode);
			}
			return false;
		}

		/// <summary>
		/// Deletes an existing data entry of a specified type with the given identifier.
		/// </summary>
		/// <typeparam name="T">The type of data to delete.</typeparam>
		/// <param name="id">The identifier of the data entry to be deleted.</param>
		/// <returns>
		/// Returns true if the data entry is successfully deleted (HTTP status code 204 - No Content).
		/// Returns false if the deletion fails, encounters a conflict (HTTP status code 409 - Conflict),
		/// or encounters any other error, with an error message logged.
		/// </returns>
		public async Task<bool> DeleteData<T>(int id)
		{
			var modelName = GetModelName<T>();
			var requestUri = $"{apiURL}/{modelName}s/{id}";

			using var httpClient = _httpClientFactory.CreateClient("AirportAutomationApi");
			ConfigureHttpClient(httpClient);

			var response = await httpClient.DeleteAsync(requestUri);

			if (response.StatusCode is HttpStatusCode.NoContent)
			{
				return true;
			}
			else if (response.StatusCode is HttpStatusCode.Conflict)
			{
				return false;
			}
			else
			{
				_logger.LogInformation("Failed to delete data. Status code: {StatusCode}", response.StatusCode);
			}
			return false;
		}

		/// <summary>
		/// Configures the provided HttpClient with default headers for JSON content
		/// and user agent, and adds authorization header if a bearer token is available.
		/// </summary>
		/// <param name="httpClient">The HttpClient to be configured.</param>
		private void ConfigureHttpClient(HttpClient httpClient)
		{
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("HttpRequestsSample");
			AddAuthorizationHeader(httpClient);
		}

		/// <summary>
		/// Gets health check data of a specified type.
		/// </summary>
		/// <typeparam name="T">The type of health check data to retrieve.</typeparam>
		/// <returns>
		/// Returns the health check data of type <typeparamref name="T"/>.
		/// If the retrieval fails, a default value for <typeparamref name="T"/> is returned with an error logged.
		/// </returns>
		public async Task<T> GetHealthCheck<T>()
		{
			T data = default;
			var modelName = GetModelName<T>();

			string requestUri = $"{apiURL}/{modelName}";

			var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

			using var httpClient = _httpClientFactory.CreateClient("AirportAutomationApi");
			ConfigureHttpClient(httpClient);

			var response = await httpClient.SendAsync(httpRequestMessage);

			if (response.IsSuccessStatusCode)
			{
				data = await response.Content.ReadFromJsonAsync<T>().ConfigureAwait(false);
			}
			else
			{
				_logger.LogInformation("Failed to retrieve data. Status code: {StatusCode}", response.StatusCode);
			}
			return data;
		}

		public string GetModelName<T>()
		{
			var modelName = typeof(T).Name;
			const string entitySuffix = "Entity";
			if (modelName.EndsWith(entitySuffix))
			{
				modelName = modelName.Substring(0, modelName.Length - entitySuffix.Length);
			}
			return modelName;
		}

		public string BuildRequestUriByModelName(string modelName, object filter, int page, int pageSize)
		{
			string requestUri;

			var specialModels = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			{
				"ApiUser",
				"Destination",
				"Passenger",
				"Pilot",
				"PlaneTicket"
			};
			requestUri = $"{apiURL}/{modelName}/byFilter";

			var filterQuery = BuildFilterQueryString(modelName, filter);

			string pluralSuffix = modelName.Equals("TravelClass", StringComparison.OrdinalIgnoreCase) ? "es" : "s";
			requestUri = $"{apiURL}/{modelName}{pluralSuffix}/byFilter";
			requestUri += $"?{filterQuery}&page={page}&pageSize={pageSize}";

			return requestUri;
		}

		private string BuildFilterQueryString(string modelName, object filter)
		{
			if (filter == null) return string.Empty;

			var queryParameters = new List<string>();

			switch (modelName.ToLower())
			{
				case "apiuser":
					if (filter is ApiUserSearchFilter userFilter)
					{
						if (!string.IsNullOrEmpty(userFilter.UserName))
							queryParameters.Add($"UserName={Uri.EscapeDataString(userFilter.UserName)}");
						if (!string.IsNullOrEmpty(userFilter.Password))
							queryParameters.Add($"Password={Uri.EscapeDataString(userFilter.Password)}");
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

			return string.Join("&", queryParameters);
		}

	}
}
