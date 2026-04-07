using Microsoft.Extensions.Configuration;

using MultiAgentWorkshop.AppHost.Resources;
using MultiAgentWorkshop.Models.Configuration;

namespace MultiAgentWorkshop.AppHost.Extensions;

public static class FoundryResourceExtensions
{
    public static IResourceBuilder<FoundryResource> AddFoundry(this IDistributedApplicationBuilder builder, string name)
    {
        var config = builder.Configuration;
        var foundry = config.GetSection("Foundry").Get<FoundrySettings>() ?? throw new InvalidOperationException("Foundry settings are not configured");
        var project = foundry.Project ?? throw new InvalidOperationException("Foundry project settings are not configured");
        var agents = project.Agents ?? throw new InvalidOperationException("Foundry project agents are not configured");

        var resource = new FoundryResource(name)
        {
            ProjectEndpoint = project.Endpoint,
            Model = project.Model,
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

    internal static IResourceBuilder<T> WithReference<T>(this IResourceBuilder<T> builder, IResourceBuilder<FoundryResource> foundry)
        where T : IResourceWithEnvironment
    {
        var resource = foundry.Resource;

        builder = builder.WithEnvironment("Foundry__Project__Endpoint", resource.ProjectEndpoint ?? "")
                         .WithEnvironment("Foundry__Project__Model", resource.Model ?? "");

        for (var i = 0; i < resource.Agents.Count; i++)
        {
            var agent = resource.Agents[i];
            builder = builder.WithEnvironment($"Foundry__Project__Agents__{i}__Name", agent.Name ?? "")
                             .WithEnvironment($"Foundry__Project__Agents__{i}__Version", agent.Version ?? "");
        }

        return builder;
    }
}