using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using SalarySubmissionApi.Data;

var builder = WebApplication.CreateBuilder(args);

// SQL Server Connection
var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
var db = Environment.GetEnvironmentVariable("DB_NAME") ?? "TechSalary_Submission";
var user = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "12345";

var connectionString = $"Host={host};Port={port};Database={db};Username={user};Password={password}";

builder.Services.AddScoped<NpgsqlConnection>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new NpgsqlConnection(connectionString);
});

// Services
builder.Services.AddControllers();
builder.Services.AddScoped<SalaryRepository>();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();

// JWT Authentication (Optional - controlled by config)
var validateTokens = builder.Configuration.GetValue<bool>("Authentication:ValidateTokens");

if (validateTokens)
{
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var secretKey = jwtSettings["SecretKey"];

    if (!string.IsNullOrEmpty(secretKey))
    {
        var key = Encoding.ASCII.GetBytes(secretKey);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        builder.Services.AddAuthorization();
    }
}

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

// Enable CORS
app.UseCors("AllowAll");


if (validateTokens)
{
    app.UseAuthentication();
    app.UseAuthorization();
}

app.MapControllers();

app.MapGet("/", () => "Salary Submission API is running on http://localhost:5002");

logger.LogInformation("Salary Submission API started successfully!");

app.Run();
