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

builder.Services.AddSingleton(agents);

builder.Services.AddHttpClient("agent", client =>
{
    client.BaseAddress = new Uri("https+http://agent");
});

builder.Services.AddChatClient(sp => new AGUIChatClient(
    httpClient: sp.GetRequiredService<IHttpClientFactory>().CreateClient("agent"),
    endpoint: "ag-ui")
);

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
