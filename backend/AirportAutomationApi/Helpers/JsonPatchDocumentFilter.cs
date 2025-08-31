using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AirportAutomation.Api.Helpers
{
	/// <summary>
	/// A filter that modifies the Swagger document to handle JSON Patch operations.
	/// </summary>
	/// <remarks>
	/// This filter removes default schemas related to JSON Patch and defines custom schemas for 
	/// operations and JSON Patch documents.
	/// </remarks>
	public class JsonPatchDocumentFilter : IDocumentFilter
	{
		/// <summary>
		/// Applies the modifications to the Swagger document.
		/// </summary>
		/// <param name="swaggerDoc">The Swagger document to modify.</param>
		/// <param name="context">The context for the Swagger document generation.</param>
		/// <remarks>
		/// This method removes existing "Operation" and "JsonPatchDocument" schemas and adds
		/// new definitions to represent JSON Patch operations.
		/// </remarks>
		public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
		{
			var schemas = swaggerDoc.Components.Schemas.ToList();
			foreach (var item in schemas)
			{
				if (item.Key.StartsWith("Operation") || item.Key.StartsWith("JsonPatchDocument"))
					swaggerDoc.Components.Schemas.Remove(item.Key);
			}

			swaggerDoc.Components.Schemas.Add("Operation", new OpenApiSchema
			{
				Type = "object",
				Properties = new System.Collections.Generic.Dictionary<string, OpenApiSchema>
				{
					{"op", new OpenApiSchema{ Type = "string" } },
					{"value", new OpenApiSchema{ Type = "string"} },
					{"path", new OpenApiSchema{ Type = "string" } }
				}
			});

			swaggerDoc.Components.Schemas.Add("JsonPatchDocument", new OpenApiSchema
			{
				Type = "array",
				Items = new OpenApiSchema
				{
					Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = "Operation" }
				},
				Description = "Array of operations to perform"
			});

			foreach (var apiDescription in context.ApiDescriptions)
			{
				var relativePath = apiDescription.RelativePath;
				if (swaggerDoc.Paths.ContainsKey(relativePath))
				{
					var pathItem = swaggerDoc.Paths[relativePath];
					foreach (var operation in pathItem.Operations)
					{
						if (operation.Key == OperationType.Patch)
						{
							foreach (var item in operation.Value.RequestBody.Content.Where(c => c.Key != "application/json-patch+json").ToList())
								operation.Value.RequestBody.Content.Remove(item.Key);

							var response = operation.Value.RequestBody.Content.FirstOrDefault(c => c.Key == "application/json-patch+json");
							if (response.Value != null)
							{
								response.Value.Schema = new OpenApiSchema
								{
									Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = "JsonPatchDocument" }
								};
							}
						}
					}
				}
			}
		}
	}
}