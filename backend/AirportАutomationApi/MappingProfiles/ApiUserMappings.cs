using AirportAutomation.Application.Dtos.ApiUser;
using AirportAutomation.Core.Entities;

using AutoMapper;

namespace AirportAutomation.Api.MappingProfiles
{
	public class ApiUserMappings : Profile
	{
		public ApiUserMappings()
		{
			CreateMap<ApiUserDto, ApiUserEntity>();
			CreateMap<ApiUserEntity, ApiUserDto>();

			CreateMap<ApiUserRoleDto, ApiUserEntity>();
			CreateMap<ApiUserEntity, ApiUserRoleDto>();
		}
	}
}
