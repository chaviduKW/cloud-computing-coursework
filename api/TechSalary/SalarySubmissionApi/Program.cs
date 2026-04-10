using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SalarySubmissionApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on port 5002
builder.WebHost.UseUrls("http://localhost:5002");

// Services
builder.Services.AddControllers();
builder.Services.AddSingleton<SalaryRepository>();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Enable Swagger in all environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Salary Submission API v1");
    c.RoutePrefix = "swagger";
});

if (validateTokens)
{
    app.UseAuthentication();
    app.UseAuthorization();
}

app.MapControllers();

app.MapGet("/", () => "Salary Submission API is running on http://localhost:5002");

logger.LogInformation("Salary Submission API started successfully!");
logger.LogInformation("Swagger UI available at http://localhost:5002/swagger");

app.Run();
