using Azure.AI.Extensions.OpenAI;
using Azure.AI.Projects;
using Azure.Identity;

using Microsoft.Agents.AI;
using Microsoft.Agents.AI.DevUI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

using MultiAgentWorkshop.Models.Configuration;

using OpenAI.Chat;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var foundry = config.GetSection("Foundry").Get<FoundrySettings>() ?? throw new InvalidOperationException("Foundry settings are not configured");
var project = foundry.Project ?? throw new InvalidOperationException("Foundry project settings are not configured");
var endpoint = project.Endpoint ?? throw new InvalidOperationException("Missing Foundry Endpoint");
var model = project.Model ?? throw new InvalidOperationException("Missing Foundry Model");
var agents = project.Agents ?? throw new InvalidOperationException("Missing Foundry Agents configuration");

builder.AddServiceDefaults();

// Foundry Agent Client
// NOTE: projectClient.AsAIAgent() crashes due to Azure.AI.Projects.Agents 2.0.0
// renaming AgentRecord → ProjectsAgentRecord, while Microsoft.Agents.AI.AzureAI
// 1.0.0-rc5 still references the old type name in GetService().
// Workaround: use clientFactory to wrap the inner client and intercept GetService.
var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions() { TenantId = config["AZURE_TENANT_ID"] });
var projectClient = new AIProjectClient(endpoint: new Uri(endpoint), tokenProvider: credential);

foreach (var agentSettings in agents)
{
    var agentReference = new AgentReference(agentSettings.Name, agentSettings.Version);

    var agent = projectClient.AsAIAgent(
        agentReference: agentReference,
        clientFactory: inner => new AgentRecordShimChatClient(inner)
    );

    builder.Services.AddKeyedSingleton<AIAgent>(agentSettings.Name, agent);
}

builder.AddWorkflow("publisher", (sp, key) => AgentWorkflowBuilder.BuildSequential(
    workflowName: key,
    agents: [.. agents.Select(a => sp.GetRequiredKeyedService<AIAgent>(a.Name))]
)).AddAsAIAgent("publisher");


builder.Services.AddOpenAIResponses();
builder.Services.AddOpenAIConversations();

builder.Services.AddAGUI();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapOpenAIResponses();
app.MapOpenAIConversations();

app.MapAGUI(
    pattern: "ag-ui",
    aiAgent: app.Services.GetRequiredKeyedService<AIAgent>("publisher")
);

if (builder.Environment.IsDevelopment() == true)
{
    app.MapDevUI();
}
else
{
    app.UseHttpsRedirection();
}

app.Run();

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
