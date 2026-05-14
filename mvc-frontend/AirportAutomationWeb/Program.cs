using AirportAutomation.Core.Converters;
using AirportAutomation.Infrastructure.Middlewares;
using AirportAutomation.Web.Binders;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.IdentityModel.Logging;
using Polly;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

IdentityModelEventSource.ShowPII = builder.Environment.IsDevelopment();

builder.Host.UseSerilog((context, loggerConfig) =>
	loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddCors(options =>
{
	options.AddPolicy("_AllowSpecificMethods", builder =>
	{
		builder.AllowAnyOrigin();
		builder.AllowAnyHeader();
		builder.WithMethods("GET", "POST");
	});
});

builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
	options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
});

builder.Services.AddHttpClient("AirportAutomationApi")
	.AddResilienceHandler("AirportAutomationApi", pipeline =>
	{
		pipeline.AddTimeout(TimeSpan.FromSeconds(10));

		pipeline.AddRetry(new HttpRetryStrategyOptions
		{
			MaxRetryAttempts = 3,
			Delay = TimeSpan.FromSeconds(2),
			BackoffType = DelayBackoffType.Exponential,
			UseJitter = true
		});

		pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
		{
			SamplingDuration = TimeSpan.FromSeconds(30),
			BreakDuration = TimeSpan.FromSeconds(30),
			FailureRatio = 0.5,
			MinimumThroughput = 5
		});
	});

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromHours(1);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/Home/Index";
		options.AccessDeniedPath = "/Home/Index";
		options.ExpireTimeSpan = TimeSpan.FromHours(1);
		options.SlidingExpiration = true;
	});

builder.Services.AddAntiforgery();

BinderConfiguration.Binders(builder.Services);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseMiddleware<ContentSecurityPolicyMiddleware>();
app.UseMiddleware<RequestLogContextMiddleware>();
app.UseSerilogRequestLogging();
app.UseStaticFiles();
app.UseCors("_AllowSpecificMethods");
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionHandler>();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
	name: "passenger",
	pattern: "{controller=Passenger}/{action=Index}/{id?}");

app.Run();