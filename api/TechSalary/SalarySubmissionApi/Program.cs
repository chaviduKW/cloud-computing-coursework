using Npgsql;
using SalarySubmissionApi.Data;

var builder = WebApplication.CreateBuilder(args);

// SQL Server Connection
var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
var db = Environment.GetEnvironmentVariable("DB_NAME") ?? "salary";
var user = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "Thami1028@";

var connectionString = $"Host={host};Port={port};Database={db};Username={user};Password={password}";

builder.Services.AddScoped<NpgsqlConnection>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new NpgsqlConnection(connectionString);
});

// Services
builder.Services.AddControllers();
builder.Services.AddScoped<SalaryRepository>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "Salary Submission API is running");

app.Run();