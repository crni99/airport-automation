using AirportAutomation.Api.Interfaces;
using AirportAutomation.Application.Dtos.ApiUser;
using AirportAutomation.Core.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AirportAutomation.Api.Controllers
{
	/// <summary>
	/// Represents the controller for user authentication.
	/// </summary>
	[ApiVersion("1.0")]
	public class AuthenticationController : BaseController
	{
		private readonly IAuthenticationRepository _authenticationRepository;
		private readonly IConfiguration _configuration;
		private readonly IMapper _mapper;
		private readonly ILogger<AuthenticationController> _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="AuthenticationController"/> class.
		/// </summary>
		/// <param name="authenticationRepository">The repository for user authentication.</param>
		/// <param name="mapper">The mapper for object-to-object mapping.</param>
		/// <param name="logger">The logger for logging actions and errors.</param>
		/// <param name="configuration">The application configuration.</param>
		public AuthenticationController(IAuthenticationRepository authenticationRepository, IConfiguration configuration, IMapper mapper, ILogger<AuthenticationController> logger)
		{
			_authenticationRepository = authenticationRepository ?? throw new ArgumentNullException(nameof(authenticationRepository));
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <summary>
		/// Authenticates a user with the provided credentials and returns an access token.
		/// </summary>
		/// <param name="apiUserDto">The user's credentials for authentication.</param>
		/// <returns>
		/// An access token if authentication is successful.
		/// </returns>
		/// <response code="200">Returns an access token if authentication is successful.</response>
		/// <response code="400">If the request is invalid or if there's a validation error.</response>
		/// <response code="401">If authentication fails due to incorrect credentials.</response>
		[HttpPost]
		[ProducesResponseType(typeof(JsonWebToken), 200)]
		[ProducesResponseType(401)]
		public ActionResult<string> Authenticate(ApiUserDto apiUserDto)
		{
			var apiUser = _mapper.Map<ApiUserEntity>(apiUserDto);
			var user = _authenticationRepository.GetUserByUsername(apiUser.UserName);

			if (user is null || !BCrypt.Net.BCrypt.Verify(apiUser.Password, user.Password))
			{
				_logger.LogInformation("User with username: {UserName} and password: {Password} don’t have permission to access this resource",
					apiUser.UserName, apiUser.Password);
				return Unauthorized("Provided username or password is incorrect.");
			}
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:SecretForKey"]));
			var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

			var claimsForToken = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.UserName),
				new Claim(ClaimTypes.Role, user.Roles),
			};
			var jwtSecurityToken = new JwtSecurityToken(
				_configuration["Authentication:Issuer"],
				_configuration["Authentication:Audience"],
				claimsForToken,
				DateTime.UtcNow,
				DateTime.UtcNow.AddDays(1),
				signingCredentials);
			var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
			return Ok(tokenToReturn);
		}
	}
}
