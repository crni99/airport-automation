using AirportAutomation.Api.Interfaces;

namespace AirportAutomation.Api.Services
{
	public class InputValidationService : IInputValidationService
	{
		public bool IsNonNegativeInt(int input)
		{
			return input >= 0;
		}

		public bool IsValidString(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return false;
			}
			return true;
		}

		public bool IsValidDateOnly(DateOnly? date)
		{
			if (date < DateOnly.MinValue || date > DateOnly.MaxValue)
			{
				return false;
			}
			return true;
		}
	}
}
