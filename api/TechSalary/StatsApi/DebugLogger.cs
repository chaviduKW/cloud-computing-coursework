using System.Text.Json;

namespace StatsApi;

internal static class DebugLogger
{
    private const string SessionId = "956ad6";
    private const string LogPath = "d:\\cloud-computing-coursework\\debug-956ad6.log";

    public static void Log(string runId, string hypothesisId, string location, string message, object? data = null)
    {
        try
        {
            var payload = new
            {
                sessionId = SessionId,
                id = $"log_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{Guid.NewGuid():N}".Substring(0, 24),
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                location,
                message,
                data,
                runId,
                hypothesisId
            };
            var line = JsonSerializer.Serialize(payload) + Environment.NewLine;
            System.IO.File.AppendAllText(LogPath, line);
        }
        catch
        {
            // no-throw logging
        }
    }
}

