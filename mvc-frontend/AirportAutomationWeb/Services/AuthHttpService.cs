using AirportAutomation.Web.Interfaces;
using AirportAutomation.Web.Models.ApiUser;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace AirportAutomation.Web.Services
{
	public class AuthHttpService : BaseHttpService, IAuthHttpService
	{
		public AuthHttpService(
			IHttpClientFactory httpClientFactory,
			IHttpContextAccessor httpContextAccessor,
			IConfiguration configuration,
			ILogger<AuthHttpService> logger)
			: base(httpClientFactory, httpContextAccessor, configuration, logger)
		{
		}

		public async Task<bool> Authenticate(UserViewModel user, CancellationToken cancellationToken = default)
		{
			string token = GetToken();
			if (!string.IsNullOrEmpty(token)) return true;

			var requestUri = $"{_apiUrl}/Authentication";
			using var httpClient = _httpClientFactory.CreateClient("AirportAutomationApi");
			ConfigureHttpClient(httpClient);

			var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
			{
				Content = JsonContent.Create(user)
			};

			var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (response.IsSuccessStatusCode)
			{
				var bearerToken = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				bearerToken = bearerToken.Trim('"');

				if (!SetApiUserRole(bearerToken)) return false;

				_httpContextAccessor.HttpContext.Session.SetString("AccessToken", bearerToken);

				var role = _httpContextAccessor.HttpContext.Session.GetString("AccessRole") ?? string.Empty;
				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, bearerToken),
					new Claim(ClaimTypes.Role, role)
				};
				var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
				await _httpContextAccessor.HttpContext.SignInAsync(
					CookieAuthenticationDefaults.AuthenticationScheme,
					new ClaimsPrincipal(identity));

				return true;
			}
			else if ((int)response.StatusCode is 400 or 401)
				_logger.LogInformation("Unauthorized. Status code: {StatusCode}", response.StatusCode);
			else
				_logger.LogInformation("Failed to authenticate. Status code: {StatusCode}", response.StatusCode);

			return false;
		}

		public async Task<bool> RemoveToken(CancellationToken cancellationToken = default)
		{
			string token = GetToken();
			if (!string.IsNullOrEmpty(token))
			{
				_httpContextAccessor.HttpContext.Session.Remove("AccessToken");
				_httpContextAccessor.HttpContext.Session.Remove("AccessRole");
				await _httpContextAccessor.HttpContext.Session.CommitAsync(cancellationToken);
				await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
				return true;
			}
			return false;
		}

		private bool SetApiUserRole(string token)
		{
			var handler = new JsonWebTokenHandler();
			if (!handler.CanReadToken(token)) return false;
			try
			{
				var jwtToken = handler.ReadJsonWebToken(token);
				var roleClaim = jwtToken.Claims.FirstOrDefault(c =>
					c.Type.Equals("http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
					StringComparison.OrdinalIgnoreCase));

				if (roleClaim != null)
				{
					_httpContextAccessor.HttpContext.Session.SetString("AccessRole", roleClaim.Value);
					return true;
				}
				_logger.LogInformation("Role claim not found in the token.");
				return false;
			}
			catch (Exception ex)
			{
				_logger.LogInformation("Error decoding token: {Message}", ex.Message);
				return false;
			}
		}
	}
}