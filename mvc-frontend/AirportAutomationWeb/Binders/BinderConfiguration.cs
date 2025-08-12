using AirportAutomation.Web.Interfaces;
using AirportAutomation.Web.MappingProfiles;
using AirportAutomation.Web.Services;

namespace AirportAutomation.Web.Binders
{
	public static class BinderConfiguration
	{
		public static void Binders(IServiceCollection services)
		{
			services.AddScoped<IHttpCallService, HttpCallService>();
			services.AddScoped<IAlertService, AlertService>();
			services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

			services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromMinutes(30);
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});

			services.AddAutoMapper(typeof(PassengerMappings));
			services.AddAutoMapper(typeof(TravelClassMappings));
			services.AddAutoMapper(typeof(DestinationMappings));
			services.AddAutoMapper(typeof(PilotMappings));
			services.AddAutoMapper(typeof(AirlineMappings));
			services.AddAutoMapper(typeof(FlightMappings));
			services.AddAutoMapper(typeof(PlaneTicketMappings));

		}
	}
}
