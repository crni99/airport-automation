using AirportAutomation.Core.Enums;

namespace AirportАutomation.Api.Interfaces
{
	public interface IUtilityService
	{
		public string GetCurrentTime();

		public string GenerateUniqueFileName(string baseName, FileExtension extension);
	}
}
