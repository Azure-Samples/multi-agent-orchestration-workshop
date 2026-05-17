using MultiAgentWorkshop.AppHost.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

var foundry = builder.AddFoundryConnectionString("foundry");
var agents = builder.AddFoundryAgentsConnectionString("agents");

var agent = builder.AddProject<Projects.MultiAgentWorkshop_Agent>("agent")
                   .WithReference(foundry)
                   .WaitFor(foundry);

var webUI = builder.AddProject<Projects.MultiAgentWorkshop_WebUI>("webui")
                   .WithExternalHttpEndpoints()
                   .WithReference(agents)
                   .WithReference(agent)
                   .WaitFor(agents)
                   .WaitFor(agent);

await builder.Build().RunAsync();
