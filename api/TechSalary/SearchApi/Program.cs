using SearchApi.Services;

var builder = WebApplication.CreateBuilder(args);

// HTTP client for SalarySubmissionApi
builder.Services.AddHttpClient("SalarySubmissionApi", client =>
{
    var baseUrl = builder.Configuration["Services:SalarySubmissionApi"]
        ?? throw new InvalidOperationException("Services:SalarySubmissionApi is not configured.");
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// HTTP client for VoteApi
builder.Services.AddHttpClient("VoteApi", client =>
{
    var baseUrl = builder.Configuration["Services:VoteApi"]
        ?? throw new InvalidOperationException("Services:VoteApi is not configured.");
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Services
builder.Services.AddScoped<ISearchService, SearchService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();
