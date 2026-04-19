using Microsoft.EntityFrameworkCore;
using VoteApi.Data;
using VoteApi.Repositories;
using VoteApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Database Connection - Use centralized database
var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "25432";
var db = Environment.GetEnvironmentVariable("DB_NAME") ?? "TechSalaryDB";
var user = Environment.GetEnvironmentVariable("DB_USER") ?? "keycloak";
var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "keycloak";

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? $"Host={host};Port={port};Database={db};Username={user};Password={password}";

builder.Services.AddDbContext<VoteDbContext>(options =>
    options.UseNpgsql(connectionString));

// HTTP client for SalarySubmissionApi
builder.Services.AddHttpClient("SalarySubmissionApi", client =>
{
    var baseUrl = builder.Configuration["Services:SalarySubmissionApi"]
        ?? throw new InvalidOperationException("Services:SalarySubmissionApi is not configured.");
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Dependency Injection
builder.Services.AddScoped<IVoteRepository, VoteRepository>();
builder.Services.AddScoped<IVoteService, VoteService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Apply migrations automatically
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<VoteDbContext>();
    if (!dbContext.Database.CanConnect())
    {
        dbContext.Database.Migrate();
    }
}

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Vote API starting on http://localhost:5215");
logger.LogInformation("Authentication handled by API Gateway - trusting X-User-* headers");

app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

logger.LogInformation("Vote API started successfully!");

app.Run();
