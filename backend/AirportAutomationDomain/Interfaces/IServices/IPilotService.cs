using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Filters;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomation.Core.Interfaces.IServices
{
	public interface IPilotService
	{
		Task<IList<PilotEntity>> GetAllPilots(CancellationToken cancellationToken);
		Task<IList<PilotEntity>> GetPilots(CancellationToken cancellationToken, int page, int pageSize);
		Task<PilotEntity?> GetPilot(int id);
		Task<IList<PilotEntity?>> SearchPilots(CancellationToken cancellationToken, int page, int pageSize, PilotSearchFilter filter);
		Task<PilotEntity> PostPilot(PilotEntity pilot);
		Task PutPilot(PilotEntity pilot);
		Task<PilotEntity> PatchPilot(int id, JsonPatchDocument passengerDocument);
		Task<bool> DeletePilot(int id);
		Task<bool> PilotExists(int id);
		Task<int> PilotsCount(CancellationToken cancellationToken, string firstName = null, string lastName = null);
		Task<int> PilotsCountFilter(CancellationToken cancellationToken, PilotSearchFilter filter);
	}
}
