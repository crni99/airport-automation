using AirportAutomation.Application.Dtos.Pilot;
using AirportAutomation.Core.Entities;
using AutoMapper;

namespace AirportAutomation.Api.MappingProfiles
{
	public class PilotMappings : Profile
	{
		public PilotMappings()
		{
			CreateMap<PilotEntity, PilotDto>();
			CreateMap<PilotEntity, PilotCreateDto>();
			CreateMap<PilotDto, PilotEntity>();
			CreateMap<PilotCreateDto, PilotEntity>();
		}
	}
}