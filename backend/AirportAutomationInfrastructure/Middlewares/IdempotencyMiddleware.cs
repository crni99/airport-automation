using AirportAutomation.Core.Entities;
using AirportAutomation.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;

namespace AirportAutomation.Infrastructure.Middlewares
{
	/// <summary>
	/// Middleware for handling idempotent requests using Idempotency-Key header
	/// Prevents duplicate ticket creation on network retries
	/// </summary>
	public class IdempotencyMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<IdempotencyMiddleware> _logger;

		public IdempotencyMiddleware(RequestDelegate next, ILogger<IdempotencyMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context, DatabaseContext dbContext)
		{
			if (!IsIdempotentRequest(context.Request.Method))
			{
				await _next(context);
				return;
			}

			if (!context.Request.Headers.TryGetValue("Idempotency-Key", out var idempotencyKey))
			{
				_logger.LogWarning("Idempotency-Key header missing for {Method} {Path}",
					context.Request.Method, context.Request.Path);
				await _next(context);
				return;
			}

			string key = idempotencyKey.ToString();
			string userId = ExtractUserId(context);
			string endpoint = context.Request.Path.ToString();

			context.Request.EnableBuffering();
			var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
			context.Request.Body.Position = 0;

			var existingRequest = await dbContext.IdempotencyRequest
				.Where(x =>
					x.IdempotencyKey == key &&
					x.UserId == userId &&
					x.Endpoint == endpoint)
				.FirstOrDefaultAsync();

			if (existingRequest != null && !IsExpired(existingRequest.ExpiresAt))
			{
				_logger.LogInformation(
					"Returning cached response for idempotent request with key: {IdempotencyKey}", key);

				context.Response.StatusCode = existingRequest.ResponseStatusCode;
				context.Response.ContentType = "application/json";
				await context.Response.WriteAsync(existingRequest.ResponseBody);
				return;
			}

			var originalBodyStream = context.Response.Body;
			using (var responseBodyStream = new MemoryStream())
			{
				context.Response.Body = responseBodyStream;

				try
				{
					await _next(context);

					context.Response.Body.Seek(0, SeekOrigin.Begin);
					string responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
					context.Response.Body.Seek(0, SeekOrigin.Begin);

					if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 400)
					{
						var idempotencyRequest = new IdempotencyRequestEntity
						{
							IdempotencyKey = key,
							Endpoint = endpoint,
							HttpMethod = context.Request.Method,
							RequestBody = body,
							ResponseBody = responseBody,
							ResponseStatusCode = context.Response.StatusCode,
							CreatedAt = DateTime.UtcNow,
							ExpiresAt = DateTime.UtcNow.AddHours(24),
							UserId = userId
						};

						dbContext.IdempotencyRequest.Add(idempotencyRequest);
						await dbContext.SaveChangesAsync();

						_logger.LogInformation(
							"Stored idempotent request with key: {IdempotencyKey}", key);
					}

					await responseBodyStream.CopyToAsync(originalBodyStream);
				}
				finally
				{
					context.Response.Body = originalBodyStream;
				}
			}
		}

		private static string ExtractUserId(HttpContext context)
		{
			if (context.User?.Identity?.IsAuthenticated != true)
			{
				return "anonymous";
			}

			var nameClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.Name);

			return nameClaim?.Value ?? "anonymous";
		}


		private static bool IsIdempotentRequest(string method) =>
			method == HttpMethods.Post ||
			method == HttpMethods.Put ||
			method == HttpMethods.Patch ||
			method == HttpMethods.Delete;

		private static bool IsExpired(DateTime expiresAt) => DateTime.UtcNow > expiresAt;
	}
}