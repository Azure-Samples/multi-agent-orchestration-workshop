#!/usr/bin/env dotnet

#:sdk Microsoft.NET.Sdk

#:package Azure.AI.Projects
#:package Azure.Identity
#:package Microsoft.Extensions.Hosting

#:project ../MultiAgentWorkshop.Models/MultiAgentWorkshop.Models.csproj

#:property UserSecretsId=eb5e880d-860a-4954-887d-489dc9b515d6

#pragma warning disable OPENAI001

using Azure.AI.Projects;
using Azure.AI.Projects.Agents;
using Azure.Identity;

using Microsoft.Extensions.Configuration;

using MultiAgentWorkshop.Models.Configuration;

using OpenAI.Responses;

using System.Runtime.CompilerServices;

var config = new ConfigurationBuilder()
                 .SetBasePath(GetScriptDirectory())
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                 .AddEnvironmentVariables()
                 .AddUserSecrets<Program>(optional: true, reloadOnChange: true)
                 .Build();

var foundry = config.GetSection("Foundry").Get<FoundrySettings>() ?? throw new InvalidOperationException("Foundry settings are not configured");
var project = foundry.Project ?? throw new InvalidOperationException("Foundry project settings are not configured");
var endpoint = project.Endpoint ?? throw new InvalidOperationException("Project endpoint is not configured");
var model = project.Model ?? "gpt-5-mini";
var agents = project.Agents ?? throw new InvalidOperationException("Agents are not configured");

var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions() { TenantId = config["AZURE_TENANT_ID"] });
var projectClient = new AIProjectClient(endpoint: new Uri(endpoint), tokenProvider: credential);
var agentClient = projectClient.AgentAdministrationClient;

foreach (var agentSettings in agents)
{
    var instruction = await File.ReadAllTextAsync(Path.Combine(GetScriptDirectory(), $"{agentSettings.Name}.txt"));
    var definition = ProjectsAgentDefinition.CreatePromptAgentDefinition(model)
                                            .AddInstruction(instruction);

    var agent = await agentClient.CreateAgentVersionAsync(
        agentName: agentSettings.Name,
        options: new ProjectsAgentVersionCreationOptions(definition));

    Console.WriteLine($"Agent created (id: {agent.Value.Id}, name: {agent.Value.Name}, version: {agent.Value.Version})");
}

static string GetScriptDirectory([CallerFilePath] string path = "") => Path.GetDirectoryName(path)!;

internal static class ProjectsAgentDefinitionExtensions
{
    internal static DeclarativeAgentDefinition AddInstruction(this DeclarativeAgentDefinition definition, string instruction)
    {
        definition.Instructions = instruction;

        return definition;
    }

    internal static DeclarativeAgentDefinition AddTools(this DeclarativeAgentDefinition definition, params ResponseTool[] tools)
    {
        foreach (var tool in tools)
        {
            definition.Tools.Add(tool);
        }

        return definition;
    }
}