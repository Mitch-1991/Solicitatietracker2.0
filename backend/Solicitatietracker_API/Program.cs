using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using SolicitatieTracker.App.Services;
using SollicitatieTracker.App.Services;
using SollicitatieTracker.Dependency;
using SollicitatieTracker.Infrastructure.Data.Repos;
using Sollicitatietracker_API.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using SolicitatieTracker.Domain.Entities;
using SolicitatieTracker.App.Services.Auth;
using System.Text;
using SolicitatieTracker.Infrastructure.Data.Repos.Auth;
using SolicitatieTracker.Infrastructure.Data.Repos.Messaging;
using SolicitatieTracker.Infrastructure.Messaging;
using Microsoft.OpenApi.Models;
using Solicitatietracker_API.Services.Messaging;


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<FrontendSettings>(builder.Configuration.GetSection("Frontend"));
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Email:Smtp"));
builder.Services.Configure<EmailOutboxSettings>(builder.Configuration.GetSection("Email:Outbox"));

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();

if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Key))
{
    throw new Exception("JWT Key is missing. Configure Jwt:Key through user secrets or environment variables.");
}


// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ICalendarRepository, CalendarRepository>();
builder.Services.AddScoped<ICalendarService, CalendarService>();
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IApplicationNoteRepository, ApplicationNoteRepository>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailOutboxRepository, EmailOutboxRepository>();
builder.Services.AddScoped<IEmailMessagePublisher, EmailMessagePublisher>();
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<IResetPasswordLinkBuilder, ConfiguredResetPasswordLinkBuilder>();
builder.Services.AddScoped<IResetTokenResponsePolicy, DevelopmentResetTokenResponsePolicy>();
builder.Services.AddScoped<EmailOutboxProcessor>();
builder.Services.AddHostedService<EmailOutboxWorker>();
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var frontendBaseUrl = builder.Configuration["Frontend:BaseUrl"];
        var origins = new List<string> { "http://localhost:5173" };
        if (!string.IsNullOrWhiteSpace(frontendBaseUrl))
            origins.Add(frontendBaseUrl.Trim().TrimEnd('/'));

        policy.WithOrigins([.. origins])
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SolicitatieTracker API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Vul enkel je JWT token in. Swagger voegt zelf 'Bearer' toe."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Login and reset-password: 5 attempts per minute per IP
    options.AddPolicy("auth-strict", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: $"{context.Connection.RemoteIpAddress}|{context.Request.Headers.UserAgent}",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                Window = TimeSpan.FromMinutes(1),
                PermitLimit = 5,
                QueueLimit = 0,
                AutoReplenishment = true
            }));

    // Register and forgot-password: 3 attempts per hour per IP (these trigger emails/account creation)
    options.AddPolicy("auth-email", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: $"{context.Connection.RemoteIpAddress}|{context.Request.Headers.UserAgent}",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                Window = TimeSpan.FromHours(1),
                PermitLimit = 3,
                QueueLimit = 0,
                AutoReplenishment = true
            }));
});

var app = builder.Build();
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
Console.WriteLine($"Jwt issuer: {jwtSettings.Issuer}");
Console.WriteLine($"Jwt audience: {jwtSettings.Audience}");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SollicitatieTracker.Infrastructure.Data.SollicitatietrackerDbContext>();
    await DatabaseInitializer.InitializeAsync(dbContext);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SolicitatieTracker API v1");
        options.EnablePersistAuthorization();
    });
}

app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllers();

app.MapGet("/", () => "SolicitatieTracker API is running");
app.Run();
