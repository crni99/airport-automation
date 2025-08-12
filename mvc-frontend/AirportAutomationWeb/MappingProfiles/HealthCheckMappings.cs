using AirportAutomation.Core.Entities;
using AirportAutomation.Web.Models.HealthCheck;
using AutoMapper;

namespace AirportAutomation.Web.MappingProfiles
{
	public class HealthCheckMappings : Profile
	{
		public HealthCheckMappings()
		{
			CreateMap<HealthCheckEntity, HealthCheckViewModel>();
		}

	}
}
