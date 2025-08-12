using AirportAutomation.Core.Converters;
using AirportAutomation.Infrastructure.Middlewares;
using AirportAutomation.Web.Binders;
using Microsoft.IdentityModel.Logging;
using Serilog;

IdentityModelEventSource.ShowPII = true;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
	options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
});

builder.Services.AddSession();
builder.Services.AddAntiforgery();

BinderConfiguration.Binders(builder.Services);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseMiddleware<RequestLogContextMiddleware>();
app.UseSerilogRequestLogging();
app.UseStaticFiles();
app.UseCors("_AllowSpecificMethods");
app.UseRouting();
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionHandler>();
app.UseSession();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
	name: "passenger",
	pattern: "{controller=Passenger}/{action=Index}/{id?}");

app.Run();