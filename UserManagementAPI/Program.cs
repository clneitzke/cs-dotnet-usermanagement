var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseMiddleware<UserManagementAPI.Middleware.ErrorHandlingMiddleware>();
app.UseMiddleware<UserManagementAPI.Middleware.AuthenticationMiddleware>();
app.UseMiddleware<UserManagementAPI.Middleware.ApiCallCounterMiddleware>();
app.UseMiddleware<UserManagementAPI.Middleware.RequestResponseLoggingMiddleware>();

app.MapControllers();

app.MapGet("/", () => "Hello world!");

app.Run();
