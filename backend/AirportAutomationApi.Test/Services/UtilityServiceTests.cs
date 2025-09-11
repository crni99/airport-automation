using AirportAutomation.Api.Interfaces;
using AirportAutomation.Api.Services;
using AirportAutomation.Core.Enums;

namespace AirportAutomationApi.Test.Services
{
	public class UtilityServiceTests
	{
		private readonly IUtilityService _utilityService;

		public UtilityServiceTests()
		{
			_utilityService = new UtilityService();
		}

		[Fact]
		public void GetCurrentTime_ReturnsCorrectFormat()
		{
			// Act
			string result = _utilityService.GetCurrentTime();

			// Assert
			Assert.NotNull(result);
			Assert.Matches(@"\d{4}-\d{2}-\d{2}_\d{2}-\d{2}-\d{2}", result);
		}

		[Theory]
		[InlineData("report", FileExtension.Pdf, ".pdf")]
		[InlineData("data_export", FileExtension.Xlsx, ".xlsx")]
		public void GenerateUniqueFileName_ValidInput_ReturnsCorrectFormat(string baseName, FileExtension extension, string expectedExtension)
		{
			// Act
			string result = _utilityService.GenerateUniqueFileName(baseName, extension);

			// Assert
			Assert.NotNull(result);
			Assert.True(result.StartsWith(baseName + "-"));
			Assert.True(result.EndsWith(expectedExtension));
		}

		[Fact]
		public void GenerateUniqueFileName_UnsupportedExtension_ThrowsException()
		{
			// Arrange
			FileExtension invalidExtension = (FileExtension)99;

			// Act & Assert
			Assert.Throws<ArgumentOutOfRangeException>(() => _utilityService.GenerateUniqueFileName("test", invalidExtension));
		}
	}
}