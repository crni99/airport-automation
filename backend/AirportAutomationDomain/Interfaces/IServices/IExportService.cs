namespace AirportAutomation.Core.Interfaces.IServices
{
	public interface IExportService
	{
		byte[] ExportToPDF<T>(string name, IList<T> data);

		byte[] ExportToExcel<T>(string name, IList<T> data);
	}
}
