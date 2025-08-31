using AirportAutomation.Application.Dtos.Airline;
using AirportAutomation.Core.Entities;
using AutoMapper;

namespace AirportAutomation.Api.MappingProfiles
{
	public class AirlineMappings : Profile
	{
		public AirlineMappings()
		{
			CreateMap<AirlineEntity, AirlineDto>();
			CreateMap<AirlineEntity, AirlineCreateDto>();
			CreateMap<AirlineDto, AirlineEntity>();
			CreateMap<AirlineCreateDto, AirlineEntity>();
		}
	}
}