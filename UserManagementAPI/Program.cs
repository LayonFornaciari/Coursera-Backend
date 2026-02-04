using Microsoft.OpenApi.Models;
using UserManagementAPI.Services;
using UserManagementAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 2. CONFIGURE SWAGGER TO EXPECT AN API KEY
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserManagementAPI", Version = "v1" });

    // Define the Security Scheme
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "ApiKey must appear in header",
        Type = SecuritySchemeType.ApiKey,
        Name = "X-API-Key",
        In = ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });

    // Apply the Security Scheme globally
    var key = new OpenApiSecurityScheme
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "ApiKey"
        },
        In = ParameterLocation.Header
    };
    var requirement = new OpenApiSecurityRequirement
    {
        { key, new List<string>() }
    };
    c.AddSecurityRequirement(requirement);
});

// DI: in-memory user store
builder.Services.AddSingleton<IUserStore, InMemoryUserStore>();

// Allow environment variables to override config
builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

// Middleware order
// 3. REORDERED MIDDLEWARE FOR BETTER LOGGING

// A) Global error handling (always first)
app.UseMiddleware<ErrorHandlingMiddleware>();

// B) Request logging (Moved BEFORE Auth)
// We do this so we can log "401 Unauthorized" attempts. 
// If it was after Auth, rejected requests wouldn't be logged.
app.UseMiddleware<RequestLoggingMiddleware>();

// C) Authentication (API key for non-GET requests)
app.UseMiddleware<AuthenticationMiddleware>();

// Swagger for easy testing (enabled in Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();