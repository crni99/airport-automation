using AirportAutomation.Core.Entities;
using AirportAutomation.Web.Models.Airline;
using AirportAutomation.Web.Models.Response;
using AutoMapper;

namespace AirportAutomation.Web.MappingProfiles
{
	public class AirlineMappings : Profile
	{
		public AirlineMappings()
		{
			CreateMap<AirlineEntity, AirlineViewModel>();
			CreateMap<AirlineEntity, AirlineCreateViewModel>();
			CreateMap<AirlineViewModel, AirlineEntity>();
			CreateMap<AirlineCreateViewModel, AirlineEntity>();

			CreateMap<PagedResponse<AirlineEntity>, PagedResponse<AirlineViewModel>>();
		}
	}
}