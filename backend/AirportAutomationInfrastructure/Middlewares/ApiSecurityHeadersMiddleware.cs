using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace AirportAutomation.Infrastructure.Middlewares
{
	/// <summary>
	/// Middleware that adds security headers to all API responses.
	/// Protects against common web vulnerabilities (XSS, clickjacking, MIME type sniffing).
	/// 
	/// Security Headers Applied:
	/// - X-Content-Type-Options: nosniff - Prevents MIME type sniffing attacks
	/// - X-Frame-Options: DENY - Prevents clickjacking by disallowing framing
	/// - X-XSS-Protection: 1; mode=block - Legacy XSS protection (for older browsers)
	/// - Strict-Transport-Security: HSTS for forcing HTTPS in production
	/// </summary>
	public class ApiSecurityHeadersMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IHostEnvironment _env;

		public ApiSecurityHeadersMiddleware(RequestDelegate next, IHostEnvironment env)
		{
			_next = next ?? throw new ArgumentNullException(nameof(next));
			_env = env ?? throw new ArgumentNullException(nameof(env));
		}

		public async Task InvokeAsync(HttpContext context)
		{
			context.Response.OnStarting(() =>
			{
				// Prevent MIME type sniffing attacks
				// Without this header, browsers may infer content-type (e.g., script as stylesheet)
				context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

				// Prevent clickjacking attacks
				// DENY = page cannot be displayed in an iframe from ANY origin
				// SAMEORIGIN would allow iframing from same origin
				context.Response.Headers.Append("X-Frame-Options", "DENY");

				// Legacy XSS protection for older browsers
				// Modern browsers ignore this, but it doesn't hurt
				context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

				// Force HTTPS in production for 1 year (31536000 seconds)
				// includeSubDomains = applies to all subdomains as well
				if (!_env.IsDevelopment())
				{
					context.Response.Headers.Append(
						"Strict-Transport-Security",
						"max-age=31536000; includeSubDomains; preload"
					);
				}

				return Task.CompletedTask;
			});

			await _next(context);
		}
	}
}