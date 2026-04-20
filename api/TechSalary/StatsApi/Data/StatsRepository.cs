using System.Linq;
using System.Collections.Generic;
using System.Net.Http.Json;
using StatsApi.Models;
using Microsoft.AspNetCore.WebUtilities;

namespace StatsApi.Data;

public sealed class StatsRepository : IStatsRepository
{

    private readonly IHttpClientFactory _httpClientFactory;

    public StatsRepository(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<StatsResponse?> CalculateStatsAsync(
    string? role, string? country, string? company, string? experienceLevel)
    {
        var client = _httpClientFactory.CreateClient("SalaryService");

        var queryParams = new Dictionary<string, string?>();
        if (!string.IsNullOrEmpty(role)) queryParams.Add("role", role);
        if (!string.IsNullOrEmpty(country)) queryParams.Add("country", country);
        if (!string.IsNullOrEmpty(company)) queryParams.Add("company", company);
        if (!string.IsNullOrEmpty(experienceLevel)) queryParams.Add("experienceLevel", experienceLevel);

        // 1. Build the request URI with query parameters
        var requestUri = QueryHelpers.AddQueryString("/api/salaries/approved", queryParams);

        try
        {
            // 2. Direct internal call
            var response = await client.GetAsync(requestUri);


            if (!response.IsSuccessStatusCode) return null;

            var salaries = await response.Content.ReadFromJsonAsync<List<SalarySubmissionDto>>();

            if (salaries == null || !salaries.Any()) return null;

            // 3. Calculation logic
            var amounts = salaries.Select(s => s.SalaryAmount).OrderBy(a => a).ToList();

            return new StatsResponse
            {
                Role = role ?? "ALL",
                Country = country ?? "ALL",
                Company = company ?? "ALL",
                ExperienceLevel = experienceLevel ?? "ALL",
                RecordCount = amounts.Count,
                AverageSalary = (decimal)amounts.Average(),
                MedianSalary = Math.Round(GetPercentile(amounts, 0.5), 2),
                P25Salary = Math.Round(GetPercentile(amounts, 0.25), 2),
                P75Salary = Math.Round(GetPercentile(amounts, 0.75), 2),
                GeneratedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            // Log that the internal call failed
            Console.WriteLine($"Internal API Call Failed: {ex.Message}");
            throw;
        }
    }

    private decimal GetPercentile(List<int> sortedData, double percentile)
    {
        if (sortedData.Count == 0) return 0;
        double realIndex = percentile * (sortedData.Count - 1);
        int index = (int)realIndex;
        double fraction = realIndex - index;

        if (index + 1 < sortedData.Count)
            return sortedData[index] * (decimal)(1 - fraction) + sortedData[index + 1] * (decimal)fraction;

        return sortedData[index];
    }
}

