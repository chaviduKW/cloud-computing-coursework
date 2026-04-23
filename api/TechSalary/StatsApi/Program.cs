using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddHttpClient("SalaryService", client =>
{
    var baseUrl = builder.Configuration["Services:SalarySubmissionApi"]
        ?? throw new InvalidOperationException("Services:SalarySubmissionApi is not configured.");
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddScoped<StatsApi.Data.IStatsRepository, StatsApi.Data.StatsRepository>();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Stats API starting on http://localhost:5003");
logger.LogInformation("Authentication handled by API Gateway");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("AllowAll");

app.MapControllers();

logger.LogInformation("Stats API started successfully!");

app.Run();
