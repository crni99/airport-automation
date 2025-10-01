using AirportAutomation.Api.Binders;
using AirportAutomation.Api.HealthChecks;
using AirportAutomation.Api.Helpers;
using AirportAutomation.Infrastructure.Data;
using AirportAutomation.Infrastructure.Middlewares;
using AspNetCoreRateLimit;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;
using Serilog;
using System;
using System.Security.Claims;
using System.Text;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
	loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddCors(options =>
{
	options.AddPolicy("_AllowAll", builder =>
	{
		builder.AllowAnyOrigin();
		builder.AllowAnyHeader();
		builder.AllowAnyMethod();
	});
});

builder.Services.AddControllers(
		options => options.UseDateOnlyTimeOnlyStringConverters())
	.AddJsonOptions(options => options.UseDateOnlyTimeOnlyStringConverters())
	.AddNewtonsoftJson();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
	options.InvalidModelStateResponseFactory = context =>
	{
		var problemDetails = new ValidationProblemDetails(context.ModelState)
		{
			Type = "https://tools.ietf.org/html/rfc9457",
			Title = "Validation failed.",
			Status = StatusCodes.Status400BadRequest,
			Instance = context.HttpContext.Request.Path
		};

		problemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;

		return new BadRequestObjectResult(problemDetails)
		{
			ContentTypes = { "application/problem+json", "application/json" }
		};
	};
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setupAction =>
{
	setupAction.SwaggerDoc("v1", new OpenApiInfo
	{
		Description = "Airport Automation Api - Version 1",
		Title = "AirportAutomationApi",
		Version = "v1",
		Contact = new OpenApiContact
		{
			Name = "Ognjen Andjelic",
			Email = "andjelicb.ognjen@gmail.com",
			Url = new Uri("https://github.com/crni99")
		},

	});

	/*
	setupAction.SwaggerDoc("v2", new OpenApiInfo
	{
		Description = "Airport Automation Api - Version 2",
		Title = "AirportAutomationApi",
		Version = "v2",
		Contact = new OpenApiContact
		{
			Name = "Ognjen Andjelic",
			Email = "andjelicb.ognjen@gmail.com",
			Url = new Uri("https://github.com/crni99")
		},

	});
	*/

	setupAction.AddSecurityDefinition("AirportAutomationApiBearerAuth", new OpenApiSecurityScheme()
	{
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "Input a valid token to access this API"
	});

	setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference {
					Type = ReferenceType.SecurityScheme,
					Id = "AirportAutomationApiBearerAuth" }
			}, new List<string>() }
	});
	var filePath = Path.Combine(AppContext.BaseDirectory, "AirportAutomation.Api.xml");
	setupAction.IncludeXmlComments(filePath);
	setupAction.UseDateOnlyTimeOnlyStringConverters();
	setupAction.DocumentFilter<JsonPatchDocumentFilter>();
	setupAction.DocumentFilter<HealthChecksFilter>();
});

BinderConfiguration.Binders(builder.Services);

var databaseProvider = builder.Configuration.GetValue<string>("DatabaseProvider");
var connectionString = builder.Configuration.GetConnectionString(databaseProvider + "Connection");

builder.Services.AddDbContext<DatabaseContext>(options =>
{
	switch (databaseProvider?.ToLowerInvariant())
	{
		case "mysql":
			var serverVersion = ServerVersion.AutoDetect(connectionString);
			options.UseMySql(connectionString, serverVersion);
			break;

		case "postgres":
		case "postgresql":
		case "npgsql":
			options.UseNpgsql(connectionString);
			break;

		case "sqlserver":
		case "mssql":
		case "tsql":
			options.UseSqlServer(connectionString);
			break;

		default:
			throw new Exception($"Unsupported provider: {databaseProvider}");
	}
});

builder.Services.AddAuthentication(options =>
	{
		options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
	})
	.AddJwtBearer(o =>
	{
		o.IncludeErrorDetails = true;
		o.TokenValidationParameters = new()
		{
			RoleClaimType = ClaimTypes.Role,
			ValidTypes = new[] { "JWT" },
			ValidIssuer = builder.Configuration["Authentication:Issuer"],
			ValidAudience = builder.Configuration["Authentication:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecretForKey"])),
			ValidateIssuer = true,
			ValidateAudience = false,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true
		};
	}
	);
builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("RequireSuperAdminRole", policy => policy.RequireRole("SuperAdmin"));
	options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin", "SuperAdmin"));
	options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User", "Admin", "SuperAdmin"));
});

builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
	options.EnableEndpointRateLimiting = true;
	options.StackBlockedRequests = false;
	options.HttpStatusCode = 429;
	options.RealIpHeader = "X-Real-IP";
	options.ClientIdHeader = "X-ClientId";
	options.GeneralRules = new List<RateLimitRule>
	{
		new RateLimitRule
		{
			Endpoint = "*",
			Period = "60s",
			Limit = 120
		}
	};
});

builder.Services.AddVersionedApiExplorer(o =>
{
	o.GroupNameFormat = "'v'VVV";
	o.SubstituteApiVersionInUrl = true;
});

builder.Services.AddApiVersioning(setupAction =>
{
	setupAction.AssumeDefaultVersionWhenUnspecified = true;
	setupAction.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
	setupAction.ReportApiVersions = true;
	setupAction.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
															new HeaderApiVersionReader("x-api-version"),
															new MediaTypeApiVersionReader("x-api-version"));
});

builder.Services.AddHttpClient();

builder.Services
	.AddHealthChecks()
	.AddCheck<ApiHealthCheck>("API")
	.AddCheck<SqlServerHealthCheck>("SQL Server")
	.AddCheck<DatabaseHealthCheck>("Database");

var app = builder.Build();

app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "Airport Automation API V1");
	// c.SwaggerEndpoint("/swagger/v2/swagger.json", "Airport Automation API V2");
	c.DefaultModelsExpandDepth(-1);
	c.InjectJavascript("/swagger-ui/swagger-theme-toggle.js");
});

app.UseHttpsRedirection();
app.UseMiddleware<RequestLogContextMiddleware>();
app.UseSerilogRequestLogging();
app.MapHealthChecks("/api/v{version:apiVersion}/HealthCheck", new()
{
	ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
	ResultStatusCodes =
	{
		[HealthStatus.Healthy] = StatusCodes.Status200OK,
		[HealthStatus.Degraded] = StatusCodes.Status429TooManyRequests,
		[HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
	}
}).RequireAuthorization();
app.UseCors("_AllowAll");
app.UseIpRateLimiting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionHandler>();

app.MapControllers();

app.Run();
