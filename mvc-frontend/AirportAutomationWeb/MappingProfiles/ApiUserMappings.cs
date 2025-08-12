using AirportAutomation.Core.Entities;
using AirportAutomation.Web.Models.ApiUser;
using AirportAutomation.Web.Models.Response;
using AutoMapper;

namespace AirportAutomation.Web.MappingProfiles
{
	public class ApiUserMappings : Profile
	{
		public ApiUserMappings()
		{
			CreateMap<ApiUserEntity, ApiUserViewModel>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ApiUserId));
			CreateMap<ApiUserViewModel, ApiUserEntity>().ForMember(dest => dest.ApiUserId, opt => opt.MapFrom(src => src.Id));

			CreateMap<PagedResponse<ApiUserEntity>, PagedResponse<ApiUserViewModel>>();
		}
	}
}