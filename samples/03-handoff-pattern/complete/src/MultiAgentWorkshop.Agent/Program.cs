using Azure.AI.Projects;
using Azure.Identity;

using Microsoft.Agents.AI;
using Microsoft.Agents.AI.DevUI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

using MultiAgentWorkshop.Agent.Extensions;
using MultiAgentWorkshop.Agent.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

var (endpoint, deploymentName, agentNames) = config.GetAgentDetails("foundry");

builder.AddServiceDefaults();

var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions()
{
    TenantId = config["AZURE_TENANT_ID"],
    ExcludeManagedIdentityCredential = builder.Environment.IsDevelopment()
});
var projectClient = new AIProjectClient(endpoint: new Uri(endpoint!), tokenProvider: credential);

var chatClient = projectClient.ProjectOpenAIClient
                              .GetResponsesClient()
                              .AsIChatClient(deploymentName!);

foreach (var agentName in agentNames!)
{
    var instruction = await File.ReadAllTextAsync(
        Path.Combine(AppContext.BaseDirectory, "Prompts", $"{agentName}.txt"));

    var agent = new ChatClientAgent(
        chatClient,
        instructions: instruction,
        name: agentName);

    builder.Services.AddKeyedSingleton<AIAgent>(agentName, agent);
}

builder.AddWorkflow("publisher", (sp, key) =>
{
    var triage = sp.GetRequiredKeyedService<AIAgent>("triage-agent");
    var academicCurriculum = sp.GetRequiredKeyedService<AIAgent>("academic-curriculum-agent");
    var studentWellbeing = sp.GetRequiredKeyedService<AIAgent>("student-wellbeing-agent");
    var collegeCareer = sp.GetRequiredKeyedService<AIAgent>("college-career-agent");

    var specialists = new[] { academicCurriculum, studentWellbeing, collegeCareer };

    var workflow = AgentWorkflowBuilder.CreateHandoffBuilderWith(triage)
        // Triage can hand off to any specialist
        .WithHandoffs(triage, specialists)
        // Each specialist can hand off to other specialists
        .WithHandoffs(academicCurriculum, [studentWellbeing, collegeCareer])
        .WithHandoffs(studentWellbeing, [academicCurriculum, collegeCareer])
        .WithHandoffs(collegeCareer, [academicCurriculum, studentWellbeing])
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
builder.Services.AddDevUI();

builder.Services.AddAGUIServer();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapOpenAIResponses();
app.MapOpenAIConversations();

app.MapAGUIServer(
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

await app.RunAsync();
