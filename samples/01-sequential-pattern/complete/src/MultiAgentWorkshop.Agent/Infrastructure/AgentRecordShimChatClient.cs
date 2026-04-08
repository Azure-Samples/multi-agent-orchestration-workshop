using Microsoft.Extensions.AI;

namespace MultiAgentWorkshop.Agent.Infrastructure;

// NOTE: projectClient.AsAIAgent() crashes due to Azure.AI.Projects.Agents 2.0.0
// renaming AgentRecord → ProjectsAgentRecord, while Microsoft.Agents.AI.AzureAI
// 1.0.0-rc5 still references the old type name in GetService().
// Workaround: use clientFactory to wrap the inner client and intercept GetService.

/// <summary>
/// Wraps an IChatClient to intercept GetService calls that would trigger loading
/// the missing AgentRecord type, preventing a TypeLoadException.
/// </summary>
internal sealed class AgentRecordShimChatClient(IChatClient inner) : DelegatingChatClient(inner)
{
    public override object? GetService(Type serviceType, object? serviceKey = null)
    {
        try
        {
            return base.GetService(serviceType, serviceKey);
        }
        catch (TypeLoadException)
        {
            return null;
        }
    }
}
