using AirportАutomation.Api.Interfaces;

namespace AirportАutomation.Api.Services
{
	public class UtilityService : IUtilityService
	{
		public string GetCurrentTime()
		{
			DateTime currentTime = DateTime.Now;
			string timeString = currentTime.ToString("HHmmss");
			return timeString;
		}

		public string GenerateUniqueFileName(string baseName)
		{
			string timeStamp = GetCurrentTime();
			string fileName = $"{baseName}-{timeStamp}.pdf";
			return fileName;
		}
	}
}
