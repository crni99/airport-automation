using AirportAutomation.Core.Filters;

namespace AirportAutomation.Core.FilterExtensions
{
	public static class ApiUserSearchFilterExtensions
	{
		public static bool IsEmpty(this ApiUserSearchFilter filter)
		{
			if (filter == null) return true;

			return string.IsNullOrWhiteSpace(filter.UserName)
				&& string.IsNullOrWhiteSpace(filter.Password)
				&& string.IsNullOrWhiteSpace(filter.Roles);
		}
	}

}
