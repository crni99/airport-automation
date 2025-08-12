using AirportAutomation.Application.Dtos.Flight;
using AirportAutomation.Core.Entities;
using AutoMapper;

namespace AirportAutomation.Api.MappingProfiles
{
	public class FlightMappings : Profile
	{
		public FlightMappings()
		{
			CreateMap<FlightEntity, FlightDto>();
			CreateMap<FlightEntity, FlightCreateDto>();
			CreateMap<FlightDto, FlightEntity>();
			CreateMap<FlightCreateDto, FlightEntity>();
			CreateMap<FlightEntity, FlightUpdateDto>();
			CreateMap<FlightUpdateDto, FlightEntity>();
		}
	}
}