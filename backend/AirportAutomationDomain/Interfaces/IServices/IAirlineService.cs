using AirportAutomation.Core.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomation.Core.Interfaces.IServices
{
	public interface IAirlineService
	{
		Task<IList<AirlineEntity>> GetAllAirlines(CancellationToken cancellationToken);
		Task<IList<AirlineEntity>> GetAirlines(CancellationToken cancellationToken, int page, int pageSize);
		Task<AirlineEntity?> GetAirline(int id);
		Task<IList<AirlineEntity?>> GetAirlinesByName(CancellationToken cancellationToken, int page, int pageSize, string name);
		Task<AirlineEntity> PostAirline(AirlineEntity airline);
		Task PutAirline(AirlineEntity airline);
		Task<AirlineEntity> PatchAirline(int id, JsonPatchDocument airlineDocument);
		Task<bool> DeleteAirline(int id);
		Task<bool> AirlineExists(int id);
		Task<int> AirlinesCount(CancellationToken cancellationToken, string? name = null);
	}
}
