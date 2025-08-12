using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Net;
using System.Text.Json;

namespace AirportAutomation.Infrastructure.Middlewares
{
	public class GlobalExceptionHandler
	{
		private readonly RequestDelegate _next;

		public GlobalExceptionHandler(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext httpContext)
		{
			try
			{
				await _next(httpContext);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(httpContext, ex);
			}
		}

		private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			context.Response.ContentType = "application/json";
			var response = context.Response;
			var model = new ProblemDetails();
			int statusCode;

			switch (exception)
			{
				case ApplicationException:
				case ArgumentException:
				case ValidationException:
				case InvalidOperationException:
					statusCode = (int)HttpStatusCode.BadRequest;
					model.Type = "Bad Request";
					model.Title = "Bad Request";
					model.Detail = exception.Message;
					break;

				case DbUpdateException dbUpdateException when dbUpdateException.InnerException is SqlException sqlException:
					statusCode = (int)HttpStatusCode.Conflict;
					model.Type = "Conflict";
					model.Title = "Conflict";
					model.Detail = sqlException.Number switch
					{
						2601 or 2627 => "Duplicate key violation.",
						547 => "This entity is referenced in other records and cannot be deleted.",
						_ => "Database error: " + sqlException.Message
					};
					break;

				case UnauthorizedAccessException:
					statusCode = (int)HttpStatusCode.Unauthorized;
					model.Type = "Unauthorized access";
					model.Title = "Unauthorized access";
					break;

				case TaskCanceledException:
					statusCode = (int)HttpStatusCode.RequestTimeout;
					model.Type = "Request Timeout";
					model.Title = "Request Timeout";
					model.Detail = "The request was canceled due to a timeout.";
					break;

				default:
					statusCode = (int)HttpStatusCode.InternalServerError;
					model.Type = "Server error";
					model.Title = "Server error";
					model.Detail = "An internal server error occurred.";
					break;
			}
			model.Status = statusCode;
			response.StatusCode = statusCode;

			var result = JsonSerializer.Serialize(model);
			await context.Response.WriteAsync(result);
		}

	}
}