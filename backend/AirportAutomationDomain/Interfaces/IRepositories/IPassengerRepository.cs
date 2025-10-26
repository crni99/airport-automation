using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Filters;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomation.Core.Interfaces.IRepositories
{
	public interface IPassengerRepository
	{
		Task<IList<PassengerEntity>> GetAllPassengers(CancellationToken cancellationToken);
		Task<IList<PassengerEntity>> GetPassengers(CancellationToken cancellationToken, int page, int pageSize);
		Task<PassengerEntity?> GetPassenger(int id);
		Task<IList<PassengerEntity?>> SearchPassengers(CancellationToken cancellationToken, int page, int pageSize, PassengerSearchFilter filter);
		Task<PassengerEntity> PostPassenger(PassengerEntity passenger);
		Task PutPassenger(PassengerEntity passenger);
		Task<PassengerEntity> PatchPassenger(int id, JsonPatchDocument passengerDocument);
		Task<bool> DeletePassenger(int id);
		Task<bool> PassengerExists(int id);
		Task<bool> PassengerExistsByUPRN(string uprn, int? excludeId);
		Task<bool> PassengerExistsByPassport(string passport, int? excludeId);
		Task<int> PassengersCount(CancellationToken cancellationToken);
		Task<int> PassengersCountFilter(CancellationToken cancellationToken, PassengerSearchFilter filter);
	}
}
