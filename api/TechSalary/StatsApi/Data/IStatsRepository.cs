using StatsApi.Models;

namespace StatsApi.Data;

public interface IStatsRepository
{
    Task<StatsResponse?> CalculateStatsAsync(
        string? role,
        string? country,
        string? company,
        string? experienceLevel);
}

