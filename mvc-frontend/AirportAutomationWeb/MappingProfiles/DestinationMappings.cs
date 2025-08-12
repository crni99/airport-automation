using AirportAutomation.Core.Entities;
using AirportAutomation.Web.Models.Destination;
using AirportAutomation.Web.Models.Response;
using AutoMapper;

namespace AirportAutomation.Web.MappingProfiles
{
	public class DestinationMappings : Profile
	{
		public DestinationMappings()
		{
			CreateMap<DestinationEntity, DestinationViewModel>();
			CreateMap<DestinationEntity, DestinationCreateViewModel>();
			CreateMap<DestinationViewModel, DestinationEntity>();
			CreateMap<DestinationCreateViewModel, DestinationEntity>();

			CreateMap<PagedResponse<DestinationEntity>, PagedResponse<DestinationViewModel>>();
		}
	}
}