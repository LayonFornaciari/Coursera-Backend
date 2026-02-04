
using UserManagementAPI.Services;
using UserManagementAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Services

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI: in-memory user store

builder.Services.AddSingleton<IUserStore, InMemoryUserStore>();

// Allow environment variables to override config (e.g., ApiSecurity__ApiKey)
builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

// Middleware order (per Activity 3)
// 1) Global error handling

app.UseMiddleware<ErrorHandlingMiddleware>();

// 2) Authentication (API key for non-GET requests)
app.UseMiddleware<AuthenticationMiddleware>();

// 3) Request logging (method, path, status, duration)
app.UseMiddleware<RequestLoggingMiddleware>();

// Swagger for easy testing (enabled in Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
