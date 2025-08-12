using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IRepositories;
using AirportAutomation.Core.Interfaces.IServices;

namespace AirportAutomation.Application.Services
{
	public class TravelClassService : ITravelClassService
	{
		private readonly ITravelClassRepository _travelClassRepository;

		public TravelClassService(ITravelClassRepository travelClassRepository)
		{
			_travelClassRepository = travelClassRepository;
		}

		public async Task<IList<TravelClassEntity>> GetTravelClasses(CancellationToken cancellationToken, int page, int pageSize)
		{
			return await _travelClassRepository.GetTravelClasses(cancellationToken, page, pageSize);
		}

		public async Task<TravelClassEntity?> GetTravelClass(int id)
		{
			return await _travelClassRepository.GetTravelClass(id);
		}

		public async Task<bool> TravelClassExists(int id)
		{
			return await _travelClassRepository.TravelClassExists(id);
		}

		public async Task<int> TravelClassesCount(CancellationToken cancellationToken)
		{
			return await _travelClassRepository.TravelClassesCount(cancellationToken);
		}

	}
}
