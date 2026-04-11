# 04 Group Chat Pattern

In a group chat pattern, multiple agents participate in a shared conversation, taking turns to contribute their expertise. An orchestrator manages the discussion flow, deciding which agent speaks next and when the conversation should end. This is well suited for cross-functional planning, creative brainstorming, or any task where diverse perspectives need to interact and build on each other iteratively.

## Scenario

You're on a cross-functional product team planning to launch a new product with agents &ndash; product strategy agent, user experience agent, technical agent, and business agent.

<div>
  <img src="./images/04-group-chat-pattern-architecture.png" alt="Architecture - Group Chat Pattern" width="640" />
</div>

## Get the repository root

1. Get the `$REPOSITORY_ROOT` variable first.

    ```bash
    # zsh/bash
    REPOSITORY_ROOT=$(git rev-parse --show-toplevel)
    ```

    ```powershell
    # PowerShell
    $REPOSITORY_ROOT = git rev-parse --show-toplevel
    ```

## Copy the start project

1. If you already have the `workshop` directory, rename or remove it first.

1. Run the setup script to copy the start project to the `workshop` directory.

    ```bash
    # zsh/bash
    bash $REPOSITORY_ROOT/scripts/setup.sh --session 04-group-chat-pattern
    ```

    ```powershell
    # PowerShell
    & $REPOSITORY_ROOT/scripts/setup.ps1 -Session 04-group-chat-pattern
    ```

## Deploy agents

1. Make sure you're in the `workshop` directory.

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. Open `src/MultiAgentWorkshop.PromptAgent/appsettings.json`, find the comment line `// Add agents`, and add the `Agents` property underneath it.

    ```jsonc
    {
      ...
      // Add agents
      "Agents": [
        {
          "Name": "product-strategy-agent",
          "Version": "1"
        },
        {
          "Name": "user-experience-agent",
          "Version": "1"
        },
        {
          "Name": "technical-agent",
          "Version": "1"
        },
        {
          "Name": "business-agent",
          "Version": "1"
        }
      ]
      ...
    }
    ```

1. Navigate to the `resources-foundry` directory.

    ```bash
    pushd resources-foundry
    ```

1. Run the following command to provision and deploy the agents defined above to Microsoft Foundry.

    ```bash
    azd up
    ```

   While provisioning, you'll be asked to enter an environment name, Azure subscription, and location.

1. Once provisioning and deployment are done, run the following command to confirm that the agents have been deployed successfully.

    ```bash
    # zsh/bash
    az cognitiveservices agent list \
        -a $(azd env get-value FOUNDRY_NAME) \
        -p $(azd env get-value FOUNDRY_PROJECT_NAME) \
        --query "[].id" -o tsv
    ```

    ```bash
    # PowerShell
    az cognitiveservices agent list `
        -a $(azd env get-value FOUNDRY_NAME) `
        -p $(azd env get-value FOUNDRY_PROJECT_NAME) `
        --query "[].id" -o tsv
    ```

   You should see the four agent names.

    ```text
    business-agent
    technical-agent
    user-experience-agent
    product-strategy-agent
    ```

1. Navigate back to the workshop directory.

    ```bash
    popd
    ```

## Configure Aspire orchestration

1. Make sure you're in the `workshop` directory.

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. Verify that all the necessary agent information has been recorded.

    ```bash
    dotnet user-secrets --project ./src/MultiAgentWorkshop.AppHost list
    ```

   You should see the `AZURE_TENANT_ID`, `FOUNDRY_NAME`, `FOUNDRY_PROJECT_NAME`, `FOUNDRY_RESOURCE_GROUP`, and `Foundry:Project:Endpoint` values.

1. Open `src/MultiAgentWorkshop.AppHost/appsettings.json`, find the comment line `// Add agents`, and add the `Agents` property underneath it.

    ```jsonc
    {
      ...
      // Add agents
      "Agents": [
        {
          "Name": "product-strategy-agent",
          "Version": "1"
        },
        {
          "Name": "user-experience-agent",
          "Version": "1"
        },
        {
          "Name": "technical-agent",
          "Version": "1"
        },
        {
          "Name": "business-agent",
          "Version": "1"
        }
      ]
      ...
    }
    ```

1. Open `src/MultiAgentWorkshop.AppHost/AppHost.cs`, find the comment `// Add resource for Microsoft Foundry` and add the code right underneath it. This adds the Microsoft Foundry project connection details.

    ```csharp
    // Add resource for Microsoft Foundry
    var foundry = builder.AddFoundry("foundry");
    ```

   Let's break down the code.

   - `builder.AddFoundry("foundry")`: This adds the Microsoft Foundry connection details through a custom resource, `FoundryResource`. If you want to know more about the Aspire custom resource, visit [Create custom hosting integrations](https://aspire.dev/integrations/custom-integrations/hosting-integrations/).

1. In the same file, find the comment `// Add resource for agents on Microsoft Foundry` and add the code right underneath it. This exposes the list of agent details to the referencing application.

    ```csharp
    // Add resource for agents on Microsoft Foundry
    var agents = builder.AddAgents("agents");
    ```

   Let's break down the code.

   - `builder.AddAgents("agents")`: This adds the list of agent details through a custom resource, `AgentResource`. If you want to know more about the Aspire custom resource, visit [Create custom hosting integrations](https://aspire.dev/integrations/custom-integrations/hosting-integrations/).

1. In the same file, find the comment `// Add backend agent service` and add the code right underneath it. This defines the backend agent service that references the `foundry` resource &ndash; all the Microsoft Foundry connection details are passed to the backend agent service app.

    ```csharp
    // Add backend agent service
    var agent = builder.AddProject<MultiAgentWorkshop_Agent>("agent")
                       .WithReference(foundry);
    ```

   Let's break down the code.

   - `builder.AddProject<MultiAgentWorkshop_Agent>("agent")`: This adds the backend agent service app as a .NET project.
   - `.WithReference(foundry)`: This references the foundry resource created above, which passes the Microsoft Foundry connection details to the backend agent service app.

1. In the same file, find the comment `// Add frontend web UI` and add the code right underneath it. This defines the frontend web UI that references both the `agents` and `agent` resources &ndash; the agent details and backend connection details are both passed to the frontend web UI app.

    ```csharp
    // Add frontend web UI
    var webUI = builder.AddProject<MultiAgentWorkshop_WebUI>("webui")
                       .WithExternalHttpEndpoints()
                       .WithReference(agents)
                       .WithReference(agent)
                       .WaitFor(agent);
    ```

   Let's break down the code.

   - `builder.AddProject<MultiAgentWorkshop_WebUI>("webui")`: This adds the frontend web UI app as a .NET project.
   - `.WithExternalHttpEndpoints()`: This exposes this frontend web UI app to the Internet, which is publicly accessible.
   - `.WithReference(agents)`: This references the agent resource created above, which passes the list of agents to the frontend web UI app.
   - `.WithReference(agent)`: This references the backend agent service app, which passes the connection details to the frontend web UI app.

## Implement group chat pattern on backend agent service

1. Make sure you're in the `workshop` directory.

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. Open `src/MultiAgentWorkshop.Agent/Program.cs`, find the comment `// Create AIProjectClient instance with EntraID authentication` and add the code right underneath it. This connects to the Microsoft Foundry project.

    ```csharp
    // Create AIProjectClient instance with EntraID authentication
    var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions() { TenantId = config["AZURE_TENANT_ID"] });
    var projectClient = new AIProjectClient(endpoint: new Uri(endpoint), tokenProvider: credential);
    ```

   Let's break down the code.

   - `new DefaultAzureCredential(...)`: This logs in to Azure without an API key. It uses your Azure CLI login or Azure Developer CLI login details on your local machine, and Managed Identity when the app is deployed to Azure.
   - `new AIProjectClient(endpoint, credential)`: This connects to the Microsoft Foundry project instance using the endpoint and login details.

1. In the same file, find the comment `// Register all agents passed from Aspire` and add the code right underneath it. This pulls the agent details from the Microsoft Foundry project and registers them in the IoC container as singleton services.

    ```csharp
    // Register all agents passed from Aspire
    foreach (var agentSettings in agents)
    {
        var agentReference = new AgentReference(agentSettings.Name, agentSettings.Version);

        var agent = projectClient.AsAIAgent(
            agentReference: agentReference,
            clientFactory: inner => new AgentRecordShimChatClient(inner)
        );

        builder.Services.AddKeyedSingleton<AIAgent>(agentSettings.Name, agent);
    }
    ```

   Let's break down the code.

   - We already know the list of agents but only know their names. Therefore, the code runs the `foreach` loop for each agent.
   - `new AgentReference(name, version)`: Using each agent's information, this creates a reference instance.
   - `projectClient.AsAIAgent(reference, factory)`: This connects to the actual agent using the reference details.
   - `builder.Services.AddKeyedSingleton<AIAgent>(name, agent)`: This registers the agent instance as a singleton service.

   > **NOTE**: You may notice the `AgentRecordShimChatClient` class. It's a temporary workaround for a version mismatch between the Microsoft Agent Framework and the Microsoft Foundry SDK, which will be removed soon.

1. In the same file, find the comment `// Build a group chat workflow pattern with the agents registered` and add the code right underneath it.

    ```csharp
    // Build a group chat workflow pattern with the agents registered
    builder.AddWorkflow("publisher", (sp, key) =>
    {
        var participants = agents.Select(a => sp.GetRequiredKeyedService<AIAgent>(a.Name));

        return AgentWorkflowBuilder.CreateGroupChatBuilderWith(agentList =>
                new RoundRobinGroupChatManager(agentList) { MaximumIterationCount = participants.Count() * 2 })
               .AddParticipants(participants)
               .WithName(key)
               .Build();
    }).AddAsAIAgent("publisher");
    ```

   Let's break down the code.

   - `var participants = agents.Select(a => sp.GetRequiredKeyedService<AIAgent>(a.Name));`: This identifies the list of participating agents in the group chat.
   - `builder.AddWorkflow("publisher", ...).AddAsAIAgent("publisher")`: This adds the multi-agent workflow as another agent instance named `publisher` and registers it as a singleton.
   - `AgentWorkflowBuilder.CreateGroupChatBuilderWith(...)`: This is the group chat workflow builder that uses the same name, `publisher`.
   - `new RoundRobinGroupChatManager(...)`: This sets the group chat strategy to round-robin. It also defines the maximum number of iterations. In this code, each agent responds twice during the round-robin cycle.

1. In the same file, find the comment `// Map AGUI to the publisher workflow agent` and add the code right underneath it. The workflow is exposed as an API endpoint at `ag-ui` so that the frontend web UI can communicate with this backend agent service app.

    ```csharp
    // Map AGUI to the publisher workflow agent
    app.MapAGUI(
        pattern: "ag-ui",
        aiAgent: app.Services.GetRequiredKeyedService<AIAgent>("publisher")
    );
    ```

## Implement group chat pattern on frontend web UI

1. Make sure you're in the `workshop` directory.

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. Open `src/MultiAgentWorkshop.WebUI/Program.cs`, find the comment `// Register all agents passed from Aspire` and add the code right underneath it. This registers all the agent details so that the web UI knows which agent is responding.

    ```csharp
    // Register all agents passed from Aspire
    builder.Services.AddSingleton(agents);
    ```

1. In the same file, find the comment `// Register the backend agent service as an HTTP client` and add the code right underneath it. Aspire already provides the frontend web UI app with the connection details for the backend agent service.

    ```csharp
    // Register the backend agent service as an HTTP client
    builder.Services.AddHttpClient("agent", client =>
    {
        client.BaseAddress = new Uri("https+http://agent");
    });
    ```

1. In the same file, find the comment `// Register AGUI client` and add the code right underneath it. Using this AGUI client, the frontend web UI app communicates with the backend agent service app via the `ag-ui` endpoint.

    ```csharp
    // Register AGUI client
    builder.Services.AddChatClient(sp => new AGUIChatClient(
        httpClient: sp.GetRequiredService<IHttpClientFactory>().CreateClient("agent"),
        endpoint: "ag-ui")
    );
    ```

## Run Aspire

1. Make sure you're in the `workshop` directory.

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. Make sure you've already logged in to Azure using both Azure CLI and Azure Developer CLI. If you're unsure, follow [this step](./00-setup.md#log-in-to-azure) again.

1. Run the following command to start all apps through Aspire.

    ```bash
    dotnet watch run --project ./src/MultiAgentWorkshop.AppHost
    ```

1. The Aspire dashboard opens automatically.

   ![Aspire Dashboard](./images/step-04-image-01.png)

   Click the backend agent service app.

1. When the Dev UI page opens, change the agent to `publisher` and click the "Configure & Run" button.

   ![Microsoft Agent Framework Dev UI - Group chat pattern](./images/step-04-image-02.png)

1. Send any request.

   ![Microsoft Agent Framework Dev UI - Send request](./images/step-04-image-03.png)

   See the result and how the workflow progresses on the left-hand side of the screen.

   ![Microsoft Agent Framework Dev UI - Workflow run](./images/step-04-image-04.png)

1. Go back to the Aspire dashboard and click the web UI app.

   ![Aspire Dashboard](./images/step-04-image-05.png)

1. Send any request.

   ![Microsoft Agent Framework Chat UI - Send request](./images/step-04-image-06.png)

   See the result.

1. Press `Ctrl`+`C` to terminate all running apps.

## Deploy to Azure

1. Make sure you're in the `workshop` directory.

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. Run the following command to provision and deploy both the frontend web UI and backend agent service apps to Azure.

    ```bash
    azd up
    ```

   While provisioning, you'll be asked to enter an environment name, Azure subscription, and location.

1. Once completed, you'll see the web UI application URL on the terminal screen. Open it in your web browser and send a request.

   ![Microsoft Agent Framework on Azure Container Apps - Send request](./images/step-04-image-07.png)

   See the result.

1. Once everything is done, remove all the apps and agents from Azure.

    ```bash
    # Delete both the web UI and agent service apps.
    azd down --purge --force

    # Delete all agents and the Microsoft Foundry resource.
    cd resources-foundry
    azd down --purge --force
    ```

---

Congratulations! 🎉 You've just completed the fourth multi-agent orchestration scenario &ndash; the group chat pattern. You're all done!

👈 [03: Handoff Pattern](./03-handoff-pattern.md) | [README](../README.md) 👉
