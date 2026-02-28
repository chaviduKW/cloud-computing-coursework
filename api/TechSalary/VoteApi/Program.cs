using Microsoft.EntityFrameworkCore;
using VoteApi.Data;
using VoteApi.Repositories;
using VoteApi.Services;

var builder = WebApplication.CreateBuilder(args);

// SQL Server Connection
builder.Services.AddDbContext<VoteDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection
builder.Services.AddScoped<IVoteRepository, VoteRepository>();
builder.Services.AddScoped<IVoteService, VoteService>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()   // Allow all domains
            .AllowAnyHeader()   // Allow all headers
            .AllowAnyMethod();  // Allow all HTTP methods
    });
});

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.UseCors("AllowAll");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
