using AirportAutomation.Api.Interfaces;
using AirportAutomation.Api.Services;

namespace AirportAutomationApi.Test.Services
{
	public class InputValidationServiceTests
	{
		private readonly IInputValidationService _inputValidationService;

		public InputValidationServiceTests()
		{
			_inputValidationService = new InputValidationService();
		}

		// Tests for IsNonNegativeInt method
		[Theory]
		[InlineData(10)]
		[InlineData(0)]
		public void IsNonNegativeInt_PositiveOrZero_ReturnsTrue(int input)
		{
			Assert.True(_inputValidationService.IsNonNegativeInt(input));
		}

		[Theory]
		[InlineData(-5)]
		public void IsNonNegativeInt_NegativeNumber_ReturnsFalse(int input)
		{
			Assert.False(_inputValidationService.IsNonNegativeInt(input));
		}

		// Tests for IsValidString method
		[Theory]
		[InlineData("hello")]
		public void IsValidString_ValidInput_ReturnsTrue(string input)
		{
			Assert.True(_inputValidationService.IsValidString(input));
		}

		[Theory]
		[InlineData("")]
		[InlineData(null)]
		public void IsValidString_NullOrEmptyString_ReturnsFalse(string input)
		{
			Assert.False(_inputValidationService.IsValidString(input));
		}

		// Tests for IsValidDateOnly method
		[Fact]
		public void IsValidDateOnly_ValidDate_ReturnsTrue()
		{
			// Arrange
			DateOnly date = new DateOnly(2023, 10, 26);

			// Act
			bool result = _inputValidationService.IsValidDateOnly(date);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void IsValidDateOnly_NullDate_ReturnsTrue()
		{
			// Arrange
			DateOnly? date = null;

			// Act
			bool result = _inputValidationService.IsValidDateOnly(date);

			// Assert
			Assert.True(result);
		}
	}
}