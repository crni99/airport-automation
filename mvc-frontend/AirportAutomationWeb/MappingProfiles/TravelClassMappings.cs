using AirportAutomation.Core.Entities;
using AirportAutomation.Web.Models.Response;
using AirportAutomation.Web.Models.TravelClass;
using AutoMapper;

namespace AirportAutomation.Web.MappingProfiles
{
	public class TravelClassMappings : Profile
	{
		public TravelClassMappings()
		{
			CreateMap<TravelClassEntity, TravelClassViewModel>();
			CreateMap<TravelClassViewModel, TravelClassEntity>();

			CreateMap<PagedResponse<TravelClassEntity>, PagedResponse<TravelClassViewModel>>();
		}
	}
}