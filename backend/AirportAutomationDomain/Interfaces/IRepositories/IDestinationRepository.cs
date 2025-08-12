﻿using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Filters;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomation.Core.Interfaces.IRepositories
{
	public interface IDestinationRepository
	{
		Task<IList<DestinationEntity>> GetAllDestinations(CancellationToken cancellationToken);
		Task<IList<DestinationEntity>> GetDestinations(CancellationToken cancellationToken, int page, int pageSize);
		Task<DestinationEntity?> GetDestination(int id);
		Task<IList<DestinationEntity?>> GetDestinationsByCityOrAirport(CancellationToken cancellationToken, int page, int pageSize, string city, string airport);
		Task<IList<DestinationEntity?>> GetDestinationsByFilter(CancellationToken cancellationToken, int page, int pageSize, DestinationSearchFilter filter);
		Task<DestinationEntity> PostDestination(DestinationEntity destination);
		Task PutDestination(DestinationEntity destination);
		Task<DestinationEntity> PatchDestination(int id, JsonPatchDocument destinationDocument);
		Task<bool> DeleteDestination(int id);
		Task<bool> DestinationExists(int id);
		Task<int> DestinationsCount(CancellationToken cancellationToken, string city = null, string airport = null);
		Task<int> DestinationsCountFilter(CancellationToken cancellationToken, DestinationSearchFilter filter);
	}
}
