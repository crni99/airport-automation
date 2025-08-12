using AirportAutomation.Core.Filters;

namespace AirportAutomation.Core.FilterExtensions
{
	public static class PassengerFilterExtensions
	{
		public static bool IsEmpty(this PassengerSearchFilter filter)
		{
			if (filter == null) return true;
			return string.IsNullOrWhiteSpace(filter.FirstName)
				&& string.IsNullOrWhiteSpace(filter.LastName)
				&& string.IsNullOrWhiteSpace(filter.UPRN)
				&& string.IsNullOrWhiteSpace(filter.Passport)
				&& string.IsNullOrWhiteSpace(filter.Address)
				&& string.IsNullOrWhiteSpace(filter.Phone);
		}
	}
}
