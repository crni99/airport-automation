using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Filters;
using AirportAutomation.Core.Interfaces.IRepositories;
using AirportAutomation.Core.Interfaces.IServices;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomation.Application.Services
{
	public class PassengerService : IPassengerService
	{
		private readonly IPassengerRepository _passengerRepository;

		public PassengerService(IPassengerRepository passengerRepository)
		{
			_passengerRepository = passengerRepository;
		}

		public async Task<IList<PassengerEntity>> GetAllPassengers(CancellationToken cancellationToken)
		{
			return await _passengerRepository.GetAllPassengers(cancellationToken);
		}

		public async Task<IList<PassengerEntity>> GetPassengers(CancellationToken cancellationToken, int page, int pageSize)
		{
			return await _passengerRepository.GetPassengers(cancellationToken, page, pageSize);
		}

		public async Task<PassengerEntity?> GetPassenger(int id)
		{
			return await _passengerRepository.GetPassenger(id);
		}

		public async Task<IList<PassengerEntity?>> SearchPassengers(CancellationToken cancellationToken, int page, int pageSize, PassengerSearchFilter filter)
		{
			return await _passengerRepository.SearchPassengers(cancellationToken, page, pageSize, filter);
		}

		public async Task<bool> PassengerExistsByUPRN(string uprn)
		{
			return await _passengerRepository.PassengerExistsByUPRN(uprn);
		}

		public async Task<bool> PassengerExistsByPassport(string passport)
		{
			return await _passengerRepository.PassengerExistsByPassport(passport);
		}

		public async Task<PassengerEntity> PostPassenger(PassengerEntity passenger)
		{
			return await _passengerRepository.PostPassenger(passenger);
		}

		public async Task PutPassenger(PassengerEntity passenger)
		{
			await _passengerRepository.PutPassenger(passenger);
		}

		public async Task<PassengerEntity> PatchPassenger(int id, JsonPatchDocument passengerDocument)
		{
			return await _passengerRepository.PatchPassenger(id, passengerDocument);
		}

		public async Task<bool> DeletePassenger(int id)
		{
			return await _passengerRepository.DeletePassenger(id);
		}

		public async Task<bool> PassengerExists(int id)
		{
			return await _passengerRepository.PassengerExists(id);
		}

		public async Task<int> PassengersCount(CancellationToken cancellationToken)
		{
			return await _passengerRepository.PassengersCount(cancellationToken);
		}

		public async Task<int> PassengersCountFilter(CancellationToken cancellationToken, PassengerSearchFilter filter)
		{
			return await _passengerRepository.PassengersCountFilter(cancellationToken, filter);
		}
	}
}
