using AirportAutomation.Application.Dtos.Passenger;
using AirportAutomation.Core.Entities;
using AutoMapper;

namespace AirportAutomation.Api.MappingProfiles
{
	public class PassengerMappings : Profile
	{
		public PassengerMappings()
		{
			CreateMap<PassengerEntity, PassengerDto>();
			CreateMap<PassengerEntity, PassengerCreateDto>();
			CreateMap<PassengerDto, PassengerEntity>();
			CreateMap<PassengerCreateDto, PassengerEntity>();
		}
	}
}