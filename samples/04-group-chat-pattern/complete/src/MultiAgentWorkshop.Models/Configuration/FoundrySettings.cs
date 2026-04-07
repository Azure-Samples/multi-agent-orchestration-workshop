namespace MultiAgentWorkshop.Models.Configuration;

public class FoundrySettings
{
    public ProjectSettings? Project { get; set; }
}

public class ProjectSettings
{
    public string? Endpoint { get; set; }
    public string? Model { get; set; }
    public List<AgentSettings>? Agents { get; set; }
}

public class AgentSettings
{
    public string? Name { get; set; }
    public string? Version { get; set; }
}
