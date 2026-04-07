namespace MultiAgentWorkshop.AppHost.Resources;

public class FoundryResource(string name) : Resource(name), IResourceWithEnvironment
{
    public string? ProjectEndpoint { get; set; }
    public string? Model { get; set; }
    public List<FoundryAgent> Agents { get; set; } = [];

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
        if (Agents.Count == 0)
        {
            missing.Add("Foundry:Project:Agents");
        }
        return missing;
    }
}
