using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Filters;
using AirportAutomation.Core.Interfaces.IRepositories;
using AirportAutomation.Core.Interfaces.IServices;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomation.Application.Services
{
	public class PilotService : IPilotService
	{
		private readonly IPilotRepository _pilotRepository;

		public PilotService(IPilotRepository pilotRepository)
		{
			_pilotRepository = pilotRepository;
		}

		public async Task<IList<PilotEntity>> GetAllPilots(CancellationToken cancellationToken)
		{
			return await _pilotRepository.GetAllPilots(cancellationToken);
		}

		public async Task<IList<PilotEntity>> GetPilots(CancellationToken cancellationToken, int page, int pageSize)
		{
			return await _pilotRepository.GetPilots(cancellationToken, page, pageSize);
		}

		public async Task<PilotEntity?> GetPilot(int id)
		{
			return await _pilotRepository.GetPilot(id);
		}

		public async Task<IList<PilotEntity?>> SearchPilots(CancellationToken cancellationToken, int page, int pageSize, PilotSearchFilter filter)
		{
			return await _pilotRepository.SearchPilots(cancellationToken, page, pageSize, filter);
		}

		public async Task<PilotEntity> PostPilot(PilotEntity pilot)
		{
			return await _pilotRepository.PostPilot(pilot);
		}

		public async Task PutPilot(PilotEntity pilot)
		{
			await _pilotRepository.PutPilot(pilot);
		}
		public async Task<PilotEntity> PatchPilot(int id, JsonPatchDocument passengerDocument)
		{
			return await _pilotRepository.PatchPilot(id, passengerDocument);
		}

		public async Task<bool> DeletePilot(int id)
		{
			return await _pilotRepository.DeletePilot(id);
		}

		public async Task<bool> PilotExists(int id)
		{
			return await _pilotRepository.PilotExists(id);
		}

		public async Task<bool> PilotExistsByUPRN(string uprn, int? excludeId)
		{
			return await _pilotRepository.PilotExistsByUPRN(uprn, excludeId);
		}

		public async Task<int> PilotsCount(CancellationToken cancellationToken, string firstName = null, string lastName = null)
		{
			return await _pilotRepository.PilotsCount(cancellationToken, firstName, lastName);
		}

		public async Task<int> PilotsCountFilter(CancellationToken cancellationToken, PilotSearchFilter filter)
		{
			return await _pilotRepository.PilotsCountFilter(cancellationToken, filter);
		}
	}
}
