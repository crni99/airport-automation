using AirportAutomation.Core.Entities;
using AirportAutomation.Web.MappingProfiles.TypeConverters;
using AirportAutomation.Web.Models.PlaneTicket;
using AirportAutomation.Web.Models.Response;
using AutoMapper;

namespace AirportAutomation.Web.MappingProfiles
{
	public class PlaneTicketMappings : Profile
	{
		public PlaneTicketMappings()
		{
			CreateMap<PlaneTicketEntity, PlaneTicketViewModel>();
			CreateMap<PlaneTicketEntity, PlaneTicketCreateViewModel>();

			CreateMap<PlaneTicketViewModel, PlaneTicketEntity>().ConvertUsing<PlaneTicketTypeConverter>();
			CreateMap<PlaneTicketCreateViewModel, PlaneTicketEntity>().ConvertUsing<PlaneTicketTypeConverter>();

			CreateMap<PagedResponse<PlaneTicketEntity>, PagedResponse<PlaneTicketViewModel>>();
		}
	}
}