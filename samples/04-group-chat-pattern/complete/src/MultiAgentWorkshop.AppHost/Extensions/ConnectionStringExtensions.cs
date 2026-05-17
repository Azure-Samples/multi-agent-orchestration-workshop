using Microsoft.Extensions.Configuration;

using MultiAgentWorkshop.Models.Configuration;

namespace MultiAgentWorkshop.AppHost.Extensions;

public static class ConnectionStringExtensions
{
    public static IResourceBuilder<ConnectionStringResource> AddFoundryConnectionString(this IDistributedApplicationBuilder builder, string name)
    {
        var config = builder.Configuration;
        var foundry = config.GetSection("Foundry").Get<FoundrySettings>() ?? throw new InvalidOperationException("Foundry settings are not configured");
        var project = foundry.Project ?? throw new InvalidOperationException("Foundry project settings are not configured");
        var endpoint = project.Endpoint ?? throw new InvalidOperationException("Project endpoint is not configured");
        var model = project.Model ?? throw new InvalidOperationException("Project model is not configured");
        var agents = project.Agents ?? throw new InvalidOperationException("Foundry project agents are not configured");

        var reb = new ReferenceExpressionBuilder();
        reb.Append($"Endpoint={endpoint}");
        reb.Append($";Deployment={model}");
        reb.Append($";Agents={string.Join(",", agents)}");

        return builder.AddConnectionString(name, reb.Build());
    }

    public static IResourceBuilder<ConnectionStringResource> AddFoundryAgentsConnectionString(this IDistributedApplicationBuilder builder, string name)
    {
        var config = builder.Configuration;
        var foundry = config.GetSection("Foundry").Get<FoundrySettings>() ?? throw new InvalidOperationException("Foundry settings are not configured");
        var project = foundry.Project ?? throw new InvalidOperationException("Foundry project settings are not configured");
        var agents = project.Agents ?? throw new InvalidOperationException("Foundry project agents are not configured");

        var reb = new ReferenceExpressionBuilder();
        reb.Append($"Agents={string.Join(",", agents)}");

        return builder.AddConnectionString(name, reb.Build());
    }
}
