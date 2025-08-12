using AirportAutomation.Core.Filters;

namespace AirportAutomation.Core.FilterExtensions
{
	public static class PlaneTicketSearchFilterExtensions
	{
		public static bool IsEmpty(this PlaneTicketSearchFilter filter)
		{
			if (filter == null) return true;

			return !filter.Price.HasValue
				&& !filter.PurchaseDate.HasValue
				&& !filter.SeatNumber.HasValue;
		}
	}

}
