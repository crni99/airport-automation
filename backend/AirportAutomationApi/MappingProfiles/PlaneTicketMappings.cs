using AirportAutomation.Application.Dtos.PlaneTicket;
using AirportAutomation.Core.Entities;
using AutoMapper;

namespace AirportAutomation.Api.MappingProfiles
{
	public class PlaneTicketMappings : Profile
	{
		public PlaneTicketMappings()
		{
			CreateMap<PlaneTicketEntity, PlaneTicketDto>();
			CreateMap<PlaneTicketEntity, PlaneTicketCreateDto>();
			CreateMap<PlaneTicketDto, PlaneTicketEntity>();
			CreateMap<PlaneTicketCreateDto, PlaneTicketEntity>();
			CreateMap<PlaneTicketEntity, PlaneTicketUpdateDto>();
			CreateMap<PlaneTicketUpdateDto, PlaneTicketEntity>();
		}
	}
}