using AirportAutomation.Core.Entities;
using AirportAutomation.Web.Models.Pilot;
using AirportAutomation.Web.Models.Response;
using AutoMapper;

namespace AirportAutomation.Web.MappingProfiles
{
	public class PilotMappings : Profile
	{
		public PilotMappings()
		{
			CreateMap<PilotEntity, PilotViewModel>();
			CreateMap<PilotEntity, PilotCreateViewModel>();
			CreateMap<PilotViewModel, PilotEntity>();
			CreateMap<PilotCreateViewModel, PilotEntity>();

			CreateMap<PagedResponse<PilotEntity>, PagedResponse<PilotViewModel>>();
		}
	}
}