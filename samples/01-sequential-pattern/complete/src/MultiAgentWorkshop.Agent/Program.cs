using Azure.AI.Projects;
using Azure.Identity;

using Microsoft.Agents.AI;
using Microsoft.Agents.AI.DevUI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

using MultiAgentWorkshop.Agent.Extensions;

using OpenAI.Chat;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

var (endpoint, deploymentName, agentNames) = config.GetAgentDetails("foundry");

builder.AddServiceDefaults();

var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions() { TenantId = config["AZURE_TENANT_ID"] });
var projectClient = new AIProjectClient(endpoint: new Uri(endpoint!), tokenProvider: credential);
foreach (var agentName in agentNames!)
{
    var agentRecord = await projectClient.AgentAdministrationClient
                                         .GetAgentAsync(agentName);
    var agent = projectClient.AsAIAgent(agentRecord);

    builder.Services.AddKeyedSingleton<AIAgent>(agentName, agent);
}

builder.AddWorkflow("publisher", (sp, key) => AgentWorkflowBuilder.BuildSequential(
    workflowName: key,
    agents: [.. agentNames!.Select(name => sp.GetRequiredKeyedService<AIAgent>(name))]
)).AddAsAIAgent("publisher");

builder.Services.AddOpenAIResponses();
builder.Services.AddOpenAIConversations();
builder.Services.AddDevUI();

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

await app.RunAsync();
