namespace AirportАutomation.Api.Interfaces
{
	public interface IInputValidationService
	{
		public bool IsNonNegativeInt(int input);

		public bool IsValidString(string input);

		public bool IsValidDateOnly(DateOnly? date);
	}
}
