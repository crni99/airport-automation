namespace AirportAutomation.Core.Configuration
{
	public static class CacheKeys
	{
		public static string Airlines(int page, int pageSize) => $"airlines:page={page}:size={pageSize}";
		public static string Airline(int id) => $"airline:{id}";

		public static string Destinations(int page, int pageSize) => $"destinations:page={page}:size={pageSize}";
		public static string Destination(int id) => $"destination:{id}";

		public static string Flights(int page, int pageSize) => $"flights:page={page}:size={pageSize}";
		public static string Flight(int id) => $"flight:{id}";

		public static string Passengers(int page, int pageSize) => $"passengers:page={page}:size={pageSize}";
		public static string Passenger(int id) => $"passenger:{id}";

		public static string Pilots(int page, int pageSize) => $"pilots:page={page}:size={pageSize}";
		public static string Pilot(int id) => $"pilot:{id}";

		public static string PlaneTickets(int page, int pageSize) => $"plane_tickets:page={page}:size={pageSize}";
		public static string PlaneTicket(int id) => $"plane_ticket:{id}";

		public static string TravelClasses(int page, int pageSize) => $"travel_classes:page={page}:size={pageSize}";
		public static string TravelClass(int id) => $"travel_class:{id}";
	}
}
