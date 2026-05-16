namespace AirportAutomation.Core.Configuration
{
	public static class CacheKeys
	{
		public const string AirlinesPrefix = "airlines:";
		public const string DestinationsPrefix = "destinations:";
		public const string FlightsPrefix = "flights:";
		public const string PassengersPrefix = "passengers:";
		public const string PilotsPrefix = "pilots:";
		public const string PlaneTicketsPrefix = "plane_tickets:";
		public const string TravelClassesPrefix = "travel_classes:";

		public static string Airlines(int page, int pageSize) => $"{AirlinesPrefix}page={page}:size={pageSize}";
		public static string Airline(int id) => $"airline:{id}";

		public static string Destinations(int page, int pageSize) => $"{DestinationsPrefix}page={page}:size={pageSize}";
		public static string Destination(int id) => $"destination:{id}";

		public static string Flights(int page, int pageSize) => $"{FlightsPrefix}page={page}:size={pageSize}";
		public static string Flight(int id) => $"flight:{id}";

		public static string Passengers(int page, int pageSize) => $"{PassengersPrefix}page={page}:size={pageSize}";
		public static string Passenger(int id) => $"passenger:{id}";

		public static string Pilots(int page, int pageSize) => $"{PilotsPrefix}page={page}:size={pageSize}";
		public static string Pilot(int id) => $"pilot:{id}";

		public static string PlaneTickets(int page, int pageSize) => $"{PlaneTicketsPrefix}page={page}:size={pageSize}";
		public static string PlaneTicket(int id) => $"plane_ticket:{id}";

		public static string TravelClasses(int page, int pageSize) => $"{TravelClassesPrefix}page={page}:size={pageSize}";
		public static string TravelClass(int id) => $"travel_class:{id}";
	}
}