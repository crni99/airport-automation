using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Npgsql;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace AirportAutomation.Infrastructure.Middlewares
{
	public class GlobalExceptionHandler
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<GlobalExceptionHandler> _logger;

		public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext httpContext)
		{
			try
			{
				await _next(httpContext);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(httpContext, ex, _logger);
			}
		}

		private static async Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger<GlobalExceptionHandler> logger)
		{
			context.Response.ContentType = "application/json";
			var response = context.Response;
			var model = new ProblemDetails();
			int statusCode = (int)HttpStatusCode.InternalServerError;

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

				case DbUpdateException dbUpdateException:
					statusCode = (int)HttpStatusCode.Conflict;
					model.Type = "Conflict";
					model.Title = "Conflict";

					var innerException = dbUpdateException.InnerException;
					if (innerException is MySqlException mySqlException)
					{
						model.Detail = mySqlException.Number switch
						{
							1062 => "Duplicate key violation.",
							1451 or 1452 => "This entity is referenced in other records and cannot be deleted.",
							_ => "Database error: " + mySqlException.Message
						};
					}
					else if (innerException is PostgresException postgresException)
					{
						model.Detail = postgresException.SqlState switch
						{
							"23505" => "Duplicate key violation.",
							"23503" => "This entity is referenced in other records and cannot be deleted.",
							_ => "Database error: " + postgresException.Message
						};
					}
					else if (innerException is SqlException sqlException)
					{
						model.Detail = sqlException.Number switch
						{
							2601 or 2627 => "Duplicate key violation.",
							547 => "This entity is referenced in other records and cannot be deleted.",
							_ => "Database error: " + sqlException.Message
						};
					}
					else
					{
						model.Detail = "A database update conflict occurred.";
					}
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

			if (statusCode >= 500)
			{
				logger.LogError(exception, "An unhandled server exception occurred. Status: {StatusCode} | Path: {Path}", statusCode, context.Request.Path);
			}
			else
			{
				logger.LogWarning(exception, "A handled client-side exception occurred. Status: {StatusCode} | Path: {Path}", statusCode, context.Request.Path);
			}

			model.Status = statusCode;
			model.Instance = context.Request.Path;
			response.StatusCode = statusCode;

			var result = JsonSerializer.Serialize(model);
			await context.Response.WriteAsync(result);
		}

	}
}