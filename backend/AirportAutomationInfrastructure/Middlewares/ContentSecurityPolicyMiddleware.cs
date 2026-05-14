using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace AirportAutomation.Infrastructure.Middlewares
{
	/// <summary>
	/// Middleware that adds Content Security Policy (CSP) headers to every HTTP response.
	/// Restricts which sources the browser is allowed to load scripts, styles, frames, and other resources from,
	/// reducing the attack surface for XSS and data injection attacks.
	/// </summary>
	public class ContentSecurityPolicyMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly bool _isDevelopment;

		/// <summary>
		/// Initializes a new instance of the <see cref="ContentSecurityPolicyMiddleware"/> class.
		/// </summary>
		/// <param name="next">The next middleware in the pipeline.</param>
		public ContentSecurityPolicyMiddleware(RequestDelegate next, IWebHostEnvironment env)
		{
			_next = next ?? throw new ArgumentNullException(nameof(next));
			_isDevelopment = env.IsDevelopment();
		}

		/// <summary>
		/// Processes the HTTP request and appends CSP security headers before passing to the next middleware.
		/// </summary>
		/// <param name="context">The current HTTP context.</param>
		public async Task InvokeAsync(HttpContext context)
		{
			context.Response.OnStarting(() =>
			{
				if (!context.Response.Headers.ContainsKey("Content-Security-Policy"))
				{
					var csp = BuildCspHeader();
					context.Response.Headers.Append("Content-Security-Policy", csp);
				}

				context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
				context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
				context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

				return Task.CompletedTask;
			});

			await _next(context);
		}

		/// <summary>
		/// Builds the Content-Security-Policy header value based on the resources used by the application.
		/// </summary>
		/// <remarks>
		/// - Scripts and styles are served locally from wwwroot/lib — no external script CDNs needed.
		/// - Google Maps is embedded via iframe in _MapEmbedPartial.cshtml — maps.google.com must be allowed under frame-src.
		/// - FontAwesome webfonts are served locally — no external font sources needed.
		/// - 'unsafe-inline' for scripts is required for jQuery validation unobtrusive and inline Bootstrap JS attributes.
		///   To remove it in the future, extract inline scripts to external .js files and use a nonce-based policy.
		/// </remarks>
		private string BuildCspHeader()
		{
			var directives = new Dictionary<string, string>
			{
				// Only load scripts from self (wwwroot/lib/jquery, bootstrap, etc.)
				// 'unsafe-inline' is needed for Bootstrap's data-bs-* attributes and jQuery unobtrusive validation
				["script-src"] = "'self' 'unsafe-inline'",

				// Only load styles from self (wwwroot/lib/bootstrap, fontawesome, site.css)
				// 'unsafe-inline' is needed for Bootstrap's dynamic style manipulations
				["style-src"] = "'self' 'unsafe-inline' https://fonts.googleapis.com",

				// Webfonts are served locally from wwwroot/lib/fontawesome/webfonts
				["font-src"] = "'self' https://fonts.gstatic.com",

				// All images from self; data: allows base64-encoded images (Bootstrap icons, etc.)
				["img-src"] = "'self' data: https://maps.gstatic.com https://maps.googleapis.com",

				// Only allow framing Google Maps (used in _MapEmbedPartial.cshtml)
				["frame-src"] = "https://maps.google.com https://www.google.com",

				// All API calls go to self (MVC controllers) — the backend API is called server-side, not from browser
				["connect-src"] = "'self'" + (_isDevelopment ? " http://localhost:* ws://localhost:* wss://localhost:*" : ""),

				// Only allow the page itself to be framed by same origin (backup for X-Frame-Options)
				["frame-ancestors"] = "'self'",

				// Disallow <object>, <embed>, <applet>
				["object-src"] = "'none'",

				// Restrict base tag to prevent base tag hijacking
				["base-uri"] = "'self'",

				// All other resource types default to self only
				["default-src"] = "'self'",
			};

			return string.Join("; ", directives.Select(kvp => $"{kvp.Key} {kvp.Value}"));
		}
	}
}