using System.Data.Common;

namespace MultiAgentWorkshop.Agent.Extensions;

public static class ConnectionStringExtensions
{
    public static (string? endpoint, string? deploymentName, IEnumerable<string>? agentNames) GetAgentDetails(this IConfiguration config, string key)
    {
        var connectionString = config.GetConnectionString(key) ?? throw new InvalidOperationException($"Connection string '{key}' is not configured.");
        var connection = new DbConnectionStringBuilder() { ConnectionString = connectionString };

        var endpoint = connection.TryGetValue("Endpoint", out var endpointValue)
                     ? endpointValue?.ToString()
                     : throw new InvalidOperationException("Missing Foundry Project Endpoint");

        var deploymentName = connection.TryGetValue("Deployment", out var deploymentNameValue)
                           ? deploymentNameValue?.ToString()
                           : throw new InvalidOperationException("Missing Foundry Project Deployment Name");

        var agentNames = connection.TryGetValue("Agents", out var agentNamesValue)
                        ? agentNamesValue?.ToString()!.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim())
                        : throw new InvalidOperationException("Missing Foundry Project Agents");

        return (endpoint, deploymentName, agentNames);
    }
}
