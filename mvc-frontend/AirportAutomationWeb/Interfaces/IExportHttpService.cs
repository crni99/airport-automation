using AirportAutomation.Web.Models.Export;

namespace AirportAutomation.Web.Interfaces
{
	public interface IExportHttpService
	{
		Task<FileExportResult> DownloadFileAsync<T>(string fileType, object filter = null, int page = 1, int pageSize = 10, bool getAll = false, CancellationToken cancellationToken = default);
	}
}