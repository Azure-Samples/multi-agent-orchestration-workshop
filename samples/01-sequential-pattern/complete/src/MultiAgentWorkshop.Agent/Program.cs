using Azure.AI.Extensions.OpenAI;
using Azure.AI.Projects;
using Azure.Identity;

using Microsoft.Agents.AI;
using Microsoft.Agents.AI.DevUI;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

using OpenAI.Chat;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

var endpoint = config["Foundry:Project:Endpoint"] ?? throw new InvalidOperationException("Missing Foundry Endpoint");
var model = config["Foundry:Project:Model"] ?? throw new InvalidOperationException("Missing Foundry Model");
var agentName = config["Foundry:Project:Agent:Name"] ?? "todo-agent";
var agentVersion = config["Foundry:Project:Agent:Version"] ?? "1";

if (builder.Environment.IsDevelopment() == true)
{
    var logger = new LoggerFactory().CreateLogger("MultiAgentWorkshop.Agent.Program");
    logger.LogInformation("Using configuration: {config}", config.GetDebugView());
    logger.LogInformation("Parsed connection string values: Endpoint={endpoint}", endpoint);
    logger.LogInformation("Parsed connection string values: Model={model}", model);
    logger.LogInformation("Parsed connection string values: AgentName={agentName}", agentName);
    logger.LogInformation("Parsed connection string values: AgentVersion={agentVersion}", agentVersion);
}

builder.AddServiceDefaults();

// Foundry Agent Client
// NOTE: projectClient.AsAIAgent() crashes due to Azure.AI.Projects.Agents 2.0.0
// renaming AgentRecord → ProjectsAgentRecord, while Microsoft.Agents.AI.AzureAI
// 1.0.0-rc5 still references the old type name in GetService().
// Workaround: use clientFactory to wrap the inner client and intercept GetService.
var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions() { TenantId = config["AZURE_TENANT_ID"] });
var projectClient = new AIProjectClient(endpoint: new Uri(endpoint), tokenProvider: credential);
var agentReference = new AgentReference(agentName, agentVersion);

var agent = projectClient.AsAIAgent(
    agentReference: agentReference,
    clientFactory: inner => new AgentRecordShimChatClient(inner)
);

builder.Services.AddKeyedSingleton<AIAgent>(agentName, agent);

builder.Services.AddOpenAIResponses();
builder.Services.AddOpenAIConversations();

builder.Services.AddAGUI();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapOpenAIResponses();
app.MapOpenAIConversations();

app.MapAGUI(
    pattern: "ag-ui",
    aiAgent: app.Services.GetRequiredKeyedService<AIAgent>(agentName)
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
