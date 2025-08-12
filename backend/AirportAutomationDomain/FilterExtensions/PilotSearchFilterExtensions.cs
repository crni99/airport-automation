using AirportAutomation.Core.Filters;

namespace AirportAutomation.Core.FilterExtensions
{
	public static class PilotSearchFilterExtensions
	{
		public static bool IsEmpty(this PilotSearchFilter filter)
		{
			if (filter == null) return true;

			return string.IsNullOrWhiteSpace(filter.FirstName)
				&& string.IsNullOrWhiteSpace(filter.LastName)
				&& string.IsNullOrWhiteSpace(filter.UPRN)
				&& !filter.FlyingHours.HasValue;
		}
	}

}
