using AirportAutomation.Core.Entities;

namespace AirportAutomation.Core.Interfaces.IRepositories
{
	public interface ITravelClassRepository
	{
		Task<IList<TravelClassEntity>> GetTravelClasses(CancellationToken cancellationToken, int page, int pageSize);
		Task<TravelClassEntity?> GetTravelClass(int id);
		Task<bool> TravelClassExists(int id);
		Task<int> TravelClassesCount(CancellationToken cancellationToken);
	}
}
