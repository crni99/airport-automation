using AirportAutomation.Core.Entities;

namespace AirportАutomation.Api.Interfaces
{
    public interface IAuthenticationRepository
    {
        public ApiUserEntity GetUserByUsername(string username);

    }
}
