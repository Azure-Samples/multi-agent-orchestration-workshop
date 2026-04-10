using MultiAgentWorkshop.WebUI.Components;

using Microsoft.Agents.AI.AGUI;
using Microsoft.Extensions.AI;

using MultiAgentWorkshop.Models.Configuration;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var agents = config.GetSection("Agents").Get<IEnumerable<AgentSettings>>() ?? throw new InvalidOperationException("Agents settings are not configured");

builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

// Register all agents passed from Aspire

// Register the backend agent service as an HTTP client

// Register AGUI client

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() == false)
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseAntiforgery();

app.UseStaticFiles();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

await app.RunAsync();
