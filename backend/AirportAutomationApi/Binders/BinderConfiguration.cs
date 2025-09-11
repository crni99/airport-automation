using AirportAutomation.Api.Interfaces;
using AirportAutomation.Api.MappingProfiles;
using AirportAutomation.Api.Repositories;
using AirportAutomation.Api.Services;
using AirportAutomation.Application.Services;
using AirportAutomation.Core.Interfaces.IRepositories;
using AirportAutomation.Core.Interfaces.IServices;
using AirportAutomation.Infrastructure.Repositories;
using AspNetCoreRateLimit;

namespace AirportAutomation.Api.Binders
{
	public static class BinderConfiguration
	{
		public static void Binders(IServiceCollection services)
		{
			services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
			services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
			services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
			services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

			services.AddAutoMapper(typeof(PassengerMappings));
			services.AddAutoMapper(typeof(TravelClassMappings));
			services.AddAutoMapper(typeof(DestinationMappings));
			services.AddAutoMapper(typeof(PilotMappings));
			services.AddAutoMapper(typeof(AirlineMappings));
			services.AddAutoMapper(typeof(FlightMappings));
			services.AddAutoMapper(typeof(PlaneTicketMappings));

			services.AddScoped<IPassengerRepository, PassengerRepository>();
			services.AddScoped<IPassengerService, PassengerService>();

			services.AddScoped<ITravelClassRepository, TravelClassRepository>();
			services.AddScoped<ITravelClassService, TravelClassService>();

			services.AddScoped<IDestinationRepository, DestinationRepository>();
			services.AddScoped<IDestinationService, DestinationService>();

			services.AddScoped<IPilotRepository, PilotRepository>();
			services.AddScoped<IPilotService, PilotService>();

			services.AddScoped<IAirlineRepository, AirlineRepository>();
			services.AddScoped<IAirlineService, AirlineService>();

			services.AddScoped<IFlightRepository, FlightRepository>();
			services.AddScoped<IFlightService, FlightService>();

			services.AddScoped<IPlaneTicketRepository, PlaneTicketRepository>();
			services.AddScoped<IPlaneTicketService, PlaneTicketService>();

			services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();

			services.AddScoped<IApiUserRepository, ApiUserRepository>();
			services.AddScoped<IApiUserService, ApiUserService>();

			services.AddScoped<IPaginationValidationService, PaginationValidationService>();
			services.AddScoped<IInputValidationService, InputValidationService>();
			services.AddScoped<IUtilityService, UtilityService>();
			services.AddScoped<IExportService, ExportService>();
		}
	}
}
