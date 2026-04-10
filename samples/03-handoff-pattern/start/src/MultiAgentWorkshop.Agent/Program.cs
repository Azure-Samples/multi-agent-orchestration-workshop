using Azure.AI.OpenAI;
using Azure.Identity;

using Microsoft.Agents.AI;
using Microsoft.Agents.AI.DevUI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

using MultiAgentWorkshop.Agent.Extensions;
using MultiAgentWorkshop.Agent.Infrastructure;
using MultiAgentWorkshop.Models.Configuration;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var foundry = config.GetSection("Foundry").Get<FoundrySettings>() ?? throw new InvalidOperationException("Foundry settings are not configured");
var project = foundry.Project ?? throw new InvalidOperationException("Foundry project settings are not configured");
var endpoint = project.Endpoint ?? throw new InvalidOperationException("Missing Foundry Endpoint");
var model = project.Model ?? throw new InvalidOperationException("Missing Foundry Model");
var agents = project.Agents ?? throw new InvalidOperationException("Missing Foundry Agents configuration");

builder.AddServiceDefaults();

// Create AzureOpenAIClient instance with EntraID authentication

// Register all agents passed from Aspire

// Build a handoff workflow pattern with the agents registered

builder.Services.AddOpenAIResponses();
builder.Services.AddOpenAIConversations();

builder.Services.AddAGUI();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapOpenAIResponses();
app.MapOpenAIConversations();

// Map AGUI to the publisher workflow agent

if (builder.Environment.IsDevelopment() == true)
{
    app.MapDevUI();
}
else
{
    app.UseHttpsRedirection();
}

await app.RunAsync();
