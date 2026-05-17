using System.Data.Common;

namespace MultiAgentWorkshop.WebUI.Extensions;

public static class ConnectionStringExtensions
{
    public static IEnumerable<string>? GetAgentDetails(this IConfiguration config, string key)
    {
        var connectionString = config.GetConnectionString(key) ?? throw new InvalidOperationException($"Connection string '{key}' is not configured.");
        var connection = new DbConnectionStringBuilder() { ConnectionString = connectionString };

        var agentNames = connection.TryGetValue("Agents", out var agentNamesValue)
                        ? agentNamesValue?.ToString()!.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim())
                        : throw new InvalidOperationException("Missing Foundry Project Agents");

        return agentNames;
    }
}
