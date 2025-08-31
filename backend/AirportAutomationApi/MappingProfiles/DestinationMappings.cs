using AirportAutomation.Application.Dtos.Destination;
using AirportAutomation.Core.Entities;
using AutoMapper;

namespace AirportAutomation.Api.MappingProfiles
{
	public class DestinationMappings : Profile
	{
		public DestinationMappings()
		{
			CreateMap<DestinationEntity, DestinationDto>();
			CreateMap<DestinationEntity, DestinationCreateDto>();
			CreateMap<DestinationDto, DestinationEntity>();
			CreateMap<DestinationCreateDto, DestinationEntity>();
		}
	}
}