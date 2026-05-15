using AirportAutomation.Web.Interfaces;
using AirportAutomation.Web.Models.Export;
using System.Net;

namespace AirportAutomation.Web.Services
{
	public class ExportHttpService : BaseHttpService, IExportHttpService
	{
		public ExportHttpService(
			IHttpClientFactory httpClientFactory,
			IHttpContextAccessor httpContextAccessor,
			IConfiguration configuration,
			ILogger<ExportHttpService> logger)
			: base(httpClientFactory, httpContextAccessor, configuration, logger)
		{
		}

		public async Task<FileExportResult> DownloadFileAsync<T>(
			string fileType,
			object filter = null,
			int page = 1,
			int pageSize = 10,
			bool getAll = false,
			CancellationToken cancellationToken = default)
		{
			var modelName = GetModelName<T>();
			var exportEndpoint = fileType.ToLower() == "pdf" ? "export/pdf" : "export/excel";
			var baseUri = $"{_apiUrl}/{modelName}{GetPluralSuffix(modelName)}/{exportEndpoint}";

			var queryParameters = new List<string>
			{
				$"page={page}",
				$"pageSize={pageSize}",
				$"getAll={getAll.ToString().ToLower()}"
			};

			var filterQueryString = BuildFilterQueryString(modelName, filter);
			if (!string.IsNullOrWhiteSpace(filterQueryString))
				queryParameters.Add(filterQueryString);

			var requestUri = $"{baseUri}?{string.Join("&", queryParameters)}";

			using var httpClient = _httpClientFactory.CreateClient("AirportAutomationApi");
			ConfigureHttpClient(httpClient);

			var response = await httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);

			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
				var contentType = fileType.ToLower() == "pdf"
					? "application/pdf"
					: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
				var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ??
							   $"Export.{(fileType == "pdf" ? "pdf" : "xlsx")}";
				return new FileExportResult { Content = content, ContentType = contentType, FileName = fileName };
			}
			else if (response.StatusCode == HttpStatusCode.NoContent)
			{
				_logger.LogInformation("No data found for {FileType} export.", fileType);
				return new FileExportResult { Content = Array.Empty<byte>(), ContentType = "application/octet-stream", FileName = $"NoData.{fileType.ToLower()}" };
			}
			else if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				_logger.LogWarning("Unauthorized access during {FileType} export.", fileType);
				return new FileExportResult { IsUnauthorized = true };
			}
			else if (response.StatusCode == HttpStatusCode.Forbidden)
			{
				_logger.LogWarning("Forbidden access during {FileType} export.", fileType);
				return new FileExportResult { IsForbidden = true };
			}
			_logger.LogError("Unexpected error during {FileType} export. Status code: {StatusCode}", fileType, response.StatusCode);
			return new FileExportResult { HasError = true };
		}
	}
}