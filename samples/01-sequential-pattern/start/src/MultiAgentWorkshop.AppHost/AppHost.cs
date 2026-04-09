using MultiAgentWorkshop.AppHost.Extensions;

using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Add resource for Microsoft Foundry

// Add resource for agents on Microsoft Foundry

// Add backend agent service

// Add frontend web UI

await builder.Build().RunAsync();
