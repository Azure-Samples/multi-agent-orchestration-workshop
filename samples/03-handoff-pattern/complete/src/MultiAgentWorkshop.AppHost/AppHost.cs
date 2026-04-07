using MultiAgentWorkshop.AppHost.Extensions;

using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var foundry = builder.AddFoundry("foundry");
var agents = builder.AddAgents("agents");

var agent = builder.AddProject<MultiAgentWorkshop_Agent>("agent")
                   .WithExternalHttpEndpoints()
                   .WithReference(foundry);

var webUI = builder.AddProject<MultiAgentWorkshop_WebUI>("webui")
                   .WithExternalHttpEndpoints()
                   .WithReference(agents)
                   .WithReference(agent)
                   .WaitFor(agent);

await builder.Build().RunAsync();
