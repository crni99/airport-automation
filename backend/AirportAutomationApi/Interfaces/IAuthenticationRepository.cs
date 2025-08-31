using AirportAutomation.Core.Entities;

namespace AirportAutomation.Api.Interfaces
{
    public interface IAuthenticationRepository
    {
        public ApiUserEntity GetUserByUsername(string username);

    }
}
