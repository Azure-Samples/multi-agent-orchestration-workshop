using Microsoft.Extensions.Configuration;

using MultiAgentWorkshop.AppHost.Resources;
using MultiAgentWorkshop.Models.Configuration;

namespace MultiAgentWorkshop.AppHost.Extensions;

public static class AgentResourceExtensions
{
    public static IResourceBuilder<AgentResource> AddAgents(this IDistributedApplicationBuilder builder, string name)
    {
        var config = builder.Configuration;
        var foundry = config.GetSection("Foundry").Get<FoundrySettings>() ?? throw new InvalidOperationException("Foundry settings are not configured");
        var project = foundry.Project ?? throw new InvalidOperationException("Foundry project settings are not configured");
        var agents = project.Agents ?? throw new InvalidOperationException("Foundry project agents are not configured");

        var resource = new AgentResource(name)
        {
            Agents = [.. agents.Select(a => new FoundryAgent { Name = a.Name, Version = a.Version })]
        };

        var resourceBuilder = builder.AddResource(resource)
                                     .ExcludeFromManifest();
        resourceBuilder.OnInitializeResource((res, e, ct) =>
        {
            var missing = res.GetMissingConfigKeys();
            if (missing.Count > 0)
            {
                return e.Notifications.PublishUpdateAsync(res, state => state with
                {
                    State = new ResourceStateSnapshot(
                        $"Missing config: {string.Join(", ", missing)}",
                        KnownResourceStateStyles.Error)
                });
            }

            return e.Notifications.PublishUpdateAsync(res, state => state with
            {
                State = new ResourceStateSnapshot(
                    "Running",
                    KnownResourceStateStyles.Success)
            });
        });

        return resourceBuilder;
    }

    public static IResourceBuilder<T> WithReference<T>(this IResourceBuilder<T> builder, IResourceBuilder<AgentResource> agentResource)
        where T : IResourceWithEnvironment
    {
        var resource = agentResource.Resource;

        for (var i = 0; i < resource.Agents.Count; i++)
        {
            var agent = resource.Agents[i];
            builder = builder.WithEnvironment($"Agents__{i}__Name", agent.Name ?? "")
                             .WithEnvironment($"Agents__{i}__Version", agent.Version ?? "");
        }

        return builder;
    }
}
