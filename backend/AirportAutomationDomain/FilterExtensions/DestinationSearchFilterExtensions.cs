using AirportAutomation.Core.Filters;

namespace AirportAutomation.Core.FilterExtensions
{
	public static class DestinationSearchFilterExtensions
	{
		public static bool IsEmpty(this DestinationSearchFilter filter)
		{
			if (filter == null) return true;

			return string.IsNullOrWhiteSpace(filter.City)
				&& string.IsNullOrWhiteSpace(filter.Airport);
		}
	}

}
