using Npgsql;
using SalarySubmissionApi.Data;

var builder = WebApplication.CreateBuilder(args);

// SQL Server Connection - Use centralized database
var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "25432";
var db = Environment.GetEnvironmentVariable("DB_NAME") ?? "TechSalaryDB";
var user = Environment.GetEnvironmentVariable("DB_USER") ?? "keycloak";
var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "keycloak";

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? $"Host={host};Port={port};Database={db};Username={user};Password={password}";

builder.Services.AddScoped<NpgsqlConnection>(sp =>
{
    return new NpgsqlConnection(connectionString);
});

// Services
builder.Services.AddControllers();
builder.Services.AddScoped<SalaryRepository>();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// Log startup information
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Salary Submission API starting on http://localhost:5002");
logger.LogInformation("Authentication handled by API Gateway - trusting X-User-* headers");

// Enable CORS
app.UseCors("AllowAll");

// Enable Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.MapGet("/", () => "Salary Submission API is running on http://localhost:5002");

logger.LogInformation("Salary Submission API started successfully!");

app.Run();
