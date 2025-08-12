using AirportAutomation.Core.Entities;
using AirportAutomation.Web.MappingProfiles.TypeConverters;
using AirportAutomation.Web.Models.Flight;
using AirportAutomation.Web.Models.Response;
using AutoMapper;

namespace AirportAutomation.Web.MappingProfiles
{
	public class FlightMappings : Profile
	{
		public FlightMappings()
		{
			CreateMap<FlightEntity, FlightViewModel>();
			CreateMap<FlightViewModel, FlightEntity>().ConvertUsing<FlightTypeConverter>();

			CreateMap<FlightEntity, FlightCreateViewModel>();
			CreateMap<FlightCreateViewModel, FlightEntity>().ConvertUsing<FlightTypeConverter>();


			CreateMap<PagedResponse<FlightEntity>, PagedResponse<FlightViewModel>>();
		}
	}
}