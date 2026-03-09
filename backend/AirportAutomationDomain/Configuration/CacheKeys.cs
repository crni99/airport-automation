namespace AirportAutomation.Core.Configuration
{
	public static class CacheKeys
	{
		public static string Airline(int id) => $"airline_{id}";
		public static string Destination(int id) => $"destination_{id}";
		public static string Flight(int id) => $"flight_{id}";
		public static string Passenger(int id) => $"passenger_{id}";
		public static string Pilot(int id) => $"pilot_{id}";
		public static string PlaneTicket(int id) => $"plane_ticket_{id}";
		public static string TravelClass(int id) => $"travel_class_{id}";
	}
}
