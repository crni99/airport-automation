using AirportAutomation.Core.Enums;
using AirportАutomation.Api.Interfaces;

namespace AirportАutomation.Api.Services
{
	public class UtilityService : IUtilityService
	{
		public string GetCurrentTime()
		{
			DateTime currentTime = DateTime.Now;
			string timeString = currentTime.ToString("yyyy-MM-dd_HH-mm-ss");
			return timeString;
		}

		public string GenerateUniqueFileName(string baseName, FileExtension extension)
		{
			string timeStamp = GetCurrentTime();
			string fileExt = extension switch
			{
				FileExtension.Pdf => ".pdf",
				FileExtension.Xlsx => ".xlsx",
				_ => throw new ArgumentOutOfRangeException(nameof(extension), extension, "Unsupported file extension")
			};
			return $"{baseName}-{timeStamp}{fileExt}";
		}
	}
}
