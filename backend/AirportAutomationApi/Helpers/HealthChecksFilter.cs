using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AirportAutomation.Api.Helpers
{
	/// <summary>
	/// A filter that applies custom health check endpoints to the Swagger document.
	/// </summary>
	/// <remarks>
	/// This filter modifies the Swagger document to include health check endpoints.
	/// </remarks>
	[SwaggerTag("ApiHealth")]
	public class HealthChecksFilter : IDocumentFilter
	{
		private const string HealthCheckEndpoint = @"/api/v1/HealthCheck";

		/// <summary>
		/// Applies the custom health check endpoint to the Swagger document.
		/// </summary>
		/// <param name="swaggerDoc">The Swagger document to modify.</param>
		/// <param name="context">The context for the Swagger document generation.</param>
		/// <remarks>
		/// Endpoint for retrieving health check status.
		/// </remarks>
		/// <response code="200">Returns health check status.</response>
		/// <response code="401">If user do not have permission to access the requested resource.</response>
		[SwaggerOperation(Summary = "Endpoint for retrieving health check status.",
			OperationId = "GetHealthCheckStatus", Tags = new[] { "ApiHealth" })]
		public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
		{
			var pathItem = new OpenApiPathItem();
			var operation = new OpenApiOperation();

			operation.Tags.Add(new OpenApiTag { Name = "ApiHealth" });

			var entriesProperties = new Dictionary<string, OpenApiSchema>
			{
				{ "ApiHealthCheck", new OpenApiSchema() { Type = "object" } },
				{ "DatabaseHealthCheck", new OpenApiSchema() { Type = "object" } },
			};

			var properties = new Dictionary<string, OpenApiSchema>
			{
				{ "status", new OpenApiSchema() { Type = "string", Example = new OpenApiString("Healthy") } },
				{ "totalDuration", new OpenApiSchema() { Type = "string", Example = new OpenApiString("00:00:00.1270010") } },
				{ "entries", new OpenApiSchema() { Type = "object", Properties = entriesProperties } }
			};

			var response200 = new OpenApiResponse
			{
				Description = "OK",
				Content = new Dictionary<string, OpenApiMediaType>
				{
					{
						"application/json", new OpenApiMediaType
						{
							Schema = new OpenApiSchema
							{
								Type = "object",
								Properties = properties,
								AdditionalPropertiesAllowed = true
							}
						}
					}
				}
			};

			var response401 = new OpenApiResponse
			{
				Description = "Unauthorized",
			};

			operation.Responses.Add("200", response200);
			operation.Responses.Add("401", response401);
			pathItem.AddOperation(OperationType.Get, operation);
			swaggerDoc?.Paths.Add(HealthCheckEndpoint, pathItem);
		}
	}
}
