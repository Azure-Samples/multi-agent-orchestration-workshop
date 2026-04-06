using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var foundry = builder.AddFoundry("foundry");

var agent = builder.AddProject<MultiAgentWorkshop_Agent>("agent")
                   .WithExternalHttpEndpoints()
                   .WithReference(foundry);

var webUI = builder.AddProject<MultiAgentWorkshop_WebUI>("webui")
                   .WithExternalHttpEndpoints()
                   .WithReference(agent)
                   .WaitFor(agent);

await builder.Build().RunAsync();

internal class FoundryResource(string name) : Resource(name), IResourceWithEnvironment
{
    public string? ProjectEndpoint { get; set; }
    public string? Model { get; set; }
    public string? AgentName { get; set; }
    public string? AgentVersion { get; set; }

    public List<string> GetMissingConfigKeys()
    {
        var placeholders = new[] { "{FOUNDRY_NAME}", "{FOUNDRY_PROJECT_NAME}" };
        var missing = new List<string>();
        if (string.IsNullOrWhiteSpace(ProjectEndpoint) || placeholders.Any(p => ProjectEndpoint.Contains(p)))
        {
            missing.Add("Foundry:Project:Endpoint");
        }
        if (string.IsNullOrWhiteSpace(Model))
        {
            missing.Add("Foundry:Project:Model");
        }
        if (string.IsNullOrWhiteSpace(AgentName))
        {
            missing.Add("Foundry:Project:Agent:Name");
        }
        if (string.IsNullOrWhiteSpace(AgentVersion))
        {
            missing.Add("Foundry:Project:Agent:Version");
        }
        return missing;
    }
}

internal static class FoundryResourceExtensions
{
    internal static IResourceBuilder<FoundryResource> AddFoundry(this IDistributedApplicationBuilder builder, string name)
    {
        var section = builder.Configuration.GetSection("Foundry:Project");

        var resource = new FoundryResource(name)
        {
            ProjectEndpoint = section["Endpoint"],
            Model = section["Model"],
            AgentName = section["Agent:Name"],
            AgentVersion = section["Agent:Version"],
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

        return builder.WithEnvironment("Foundry__Project__Endpoint", resource.ProjectEndpoint ?? "")
                      .WithEnvironment("Foundry__Project__Model", resource.Model ?? "")
                      .WithEnvironment("Foundry__Project__Agent__Name", resource.AgentName ?? "")
                      .WithEnvironment("Foundry__Project__Agent__Version", resource.AgentVersion ?? "");
    }
}