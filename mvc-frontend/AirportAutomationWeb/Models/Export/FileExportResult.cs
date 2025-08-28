namespace AirportAutomation.Web.Models.Export
{
	public class FileExportResult
	{
		public byte[] Content { get; set; } = default!;
		public string ContentType { get; set; } = "application/octet-stream";
		public string FileName { get; set; } = "ExportedFile";
		public bool IsUnauthorized { get; set; }
		public bool IsForbidden { get; set; }
		public bool HasError { get; set; }
	}
}
