using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IRepositories;
using AirportAutomation.Core.Interfaces.IServices;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomation.Application.Services
{
	public class FlightService : IFlightService
	{
		private readonly IFlightRepository _flightRepository;

		public FlightService(IFlightRepository flightRepository)
		{
			_flightRepository = flightRepository;
		}

		public async Task<IList<FlightEntity>> GetAllFlights(CancellationToken cancellationToken)
		{
			return await _flightRepository.GetAllFlights(cancellationToken);
		}

		public async Task<IList<FlightEntity>> GetFlights(CancellationToken cancellationToken, int page, int pageSize)
		{
			return await _flightRepository.GetFlights(cancellationToken, page, pageSize);
		}

		public async Task<FlightEntity?> GetFlight(int id)
		{
			return await _flightRepository.GetFlight(id);
		}

		public async Task<IList<FlightEntity?>> GetFlightsBetweenDates(CancellationToken cancellationToken, int page, int pageSize, DateOnly? startDate, DateOnly? endDate)
		{
			return await _flightRepository.GetFlightsBetweenDates(cancellationToken, page, pageSize, startDate, endDate);
		}

		public async Task<FlightEntity> PostFlight(FlightEntity flight)
		{
			return await _flightRepository.PostFlight(flight);
		}

		public async Task PutFlight(FlightEntity flight)
		{
			await _flightRepository.PutFlight(flight);
		}

		public async Task<FlightEntity> PatchFlight(int id, JsonPatchDocument flightDocument)
		{
			return await _flightRepository.PatchFlight(id, flightDocument);
		}

		public async Task<bool> DeleteFlight(int id)
		{
			return await _flightRepository.DeleteFlight(id);
		}

		public async Task<bool> FlightExists(int id)
		{
			return await _flightRepository.FlightExists(id);
		}

		public async Task<int> FlightsCount(CancellationToken cancellationToken, DateOnly? startDate = null, DateOnly? endDate = null)
		{
			return await _flightRepository.FlightsCount(cancellationToken, startDate, endDate);
		}
	}
}
