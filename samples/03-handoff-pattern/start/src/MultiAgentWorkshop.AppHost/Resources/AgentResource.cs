namespace MultiAgentWorkshop.AppHost.Resources;

public class AgentResource(string name) : Resource(name), IResourceWithEnvironment
{
    public List<FoundryAgent> Agents { get; set; } = [];

    public List<string> GetMissingConfigKeys()
    {
        var missing = new List<string>();
        if (Agents.Count == 0)
        {
            missing.Add("Foundry:Project:Agents");
        }
        return missing;
    }
}
