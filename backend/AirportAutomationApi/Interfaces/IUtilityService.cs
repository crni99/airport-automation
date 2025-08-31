using AirportAutomation.Core.Enums;

namespace AirportAutomation.Api.Interfaces
{
	public interface IUtilityService
	{
		public string GetCurrentTime();

		public string GenerateUniqueFileName(string baseName, FileExtension extension);
	}
}
