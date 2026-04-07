using Azure.AI.Extensions.OpenAI;
using Azure.AI.OpenAI;
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

// For the handoff pattern, use ChatClientAgent instead of Foundry prompt agents.
// Foundry prompt agents don't support dynamically injected handoff tools at invocation time.
// ChatClientAgent allows the framework to inject handoff_to_* tools via ChatOptions.Tools.
var url = $"{string.Join("://", endpoint.Split([ ':', '/' ], StringSplitOptions.RemoveEmptyEntries).Take(2))}/openai/v1/";
Console.WriteLine($"Url: {url}");
IChatClient chatClient = new AgentRecordShimChatClient(
    new AzureOpenAIClient(new Uri(url), credential)
        .GetResponsesClient()
        .AsIChatClient(model));

foreach (var agentSettings in agents)
{
    var instruction = await File.ReadAllTextAsync(
        Path.Combine(builder.Environment.ContentRootPath, "..", "MultiAgentWorkshop.PromptAgent", $"{agentSettings.Name}.txt"));

    var agent = new ChatClientAgent(
        chatClient,
        instructions: instruction,
        name: agentSettings.Name);

    builder.Services.AddKeyedSingleton<AIAgent>(agentSettings.Name, agent);
}

builder.AddWorkflow("publisher", (sp, key) =>
{
    var triage = sp.GetRequiredKeyedService<AIAgent>("triage-agent");
    var generalSupport = sp.GetRequiredKeyedService<AIAgent>("general-support-agent");
    var networkSpecialist = sp.GetRequiredKeyedService<AIAgent>("network-specialist-agent");
    var warranty = sp.GetRequiredKeyedService<AIAgent>("warranty-agent");

    var specialists = new[] { generalSupport, networkSpecialist, warranty };

    var workflow = AgentWorkflowBuilder.CreateHandoffBuilderWith(triage)
        // Triage can hand off to any specialist
        .WithHandoffs(triage, specialists)
        // Each specialist can hand off to other specialists
        .WithHandoffs(generalSupport, [networkSpecialist, warranty])
        .WithHandoffs(networkSpecialist, [generalSupport, warranty])
        .WithHandoffs(warranty, [generalSupport, networkSpecialist])
        // All specialists hand back to triage
        .WithHandoffs(specialists, triage, "Hand back to triage when the issue is resolved or needs further routing")
        .Build();

    // HandoffWorkflowBuilder.Build() doesn't set the workflow name.
    // Set it via reflection so AddWorkflow's name validation passes.
    typeof(Workflow).GetProperty("Name")!.SetValue(workflow, key);

    return workflow;
}).AddAsAIAgent("publisher");

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
                         .CreateFixedAgent()
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
