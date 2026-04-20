using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi();

builder.Services.AddHttpClient("SalaryService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5002/");
});

builder.Services.AddScoped<StatsApi.Data.IStatsRepository, StatsApi.Data.StatsRepository>();

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
