using AirportAutomation.Core.Entities;

namespace AirportAutomation.Core.Interfaces.IServices
{
	public interface ITravelClassService
	{
		Task<IList<TravelClassEntity>> GetTravelClasses(CancellationToken cancellationToken, int page, int pageSize);
		Task<TravelClassEntity?> GetTravelClass(int id);
		Task<bool> TravelClassExists(int id);
		Task<int> TravelClassesCount(CancellationToken cancellationToken);
	}
}
