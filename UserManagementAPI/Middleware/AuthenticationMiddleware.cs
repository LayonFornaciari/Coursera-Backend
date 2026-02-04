namespace UserManagementAPI.Middleware;

public class AuthenticationMiddleware

{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthenticationMiddleware> _logger;
    private readonly string? _apiKey;

    public AuthenticationMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<AuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _apiKey = configuration["ApiSecurity:ApiKey"]; // set in appsettings.json or env var ApiSecurity__ApiKey

    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Allow GET without API key for easier peer review

        if (HttpMethods.IsGet(context.Request.Method))
        {
            await _next(context);
            return;
        }

        var providedKey = context.Request.Headers["X-API-Key"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(_apiKey) || providedKey != _apiKey)
        {
            _logger.LogWarning("Unauthorised request to {Path}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"error\":\"API key missing or invalid.\"}");
            return;
        }

        await _next(context);
    }
}
