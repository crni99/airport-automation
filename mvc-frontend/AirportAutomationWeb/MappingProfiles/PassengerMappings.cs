using AirportAutomation.Core.Entities;
using AirportAutomation.Web.Models.Passenger;
using AirportAutomation.Web.Models.Response;
using AutoMapper;

namespace AirportAutomation.Web.MappingProfiles
{
	public class PassengerMappings : Profile
	{
		public PassengerMappings()
		{
			CreateMap<PassengerEntity, PassengerViewModel>();
			CreateMap<PassengerEntity, PassengerCreateViewModel>();
			CreateMap<PassengerViewModel, PassengerEntity>();
			CreateMap<PassengerCreateViewModel, PassengerEntity>();

			CreateMap<PagedResponse<PassengerEntity>, PagedResponse<PassengerViewModel>>();
		}
	}
}