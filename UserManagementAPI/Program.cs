using UserManagementAPI.Services;

var builder = WebApplication.CreateBuilder(args);

//Services

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//DI: in-memory user store for now

builder.Services.AddSingleton<IUserStore, InMemoryUserStore>();

var app = builder.Build();

//Swagger for easy testing (enabled in Development by default)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
