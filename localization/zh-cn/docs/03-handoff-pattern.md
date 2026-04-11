# 03 交接模式

在交接模式中，智能体根据对话上下文动态地将控制权传递给彼此。分诊智能体接收初始请求，并将其路由到最适合处理该请求的专家。专家之间也可以在问题跨领域时互相重定向。这非常适合 IT 支持、客户服务或任何在不同阶段需要不同专业知识的工作流场景。

## 场景

您在一个 IT 支持团队中工作，使用智能体——包括通用支持智能体、网络专家智能体、保修智能体和分诊智能体。

<div>
  <img src="../../../docs/images/03-handoff-pattern-architecture.png" alt="架构图 - 交接模式" width="640" />
</div>

## 获取仓库根目录

1. 首先获取 `$REPOSITORY_ROOT` 变量。

    ```bash
    # zsh/bash
    REPOSITORY_ROOT=$(git rev-parse --show-toplevel)
    ```

    ```powershell
    # PowerShell
    $REPOSITORY_ROOT = git rev-parse --show-toplevel
    ```

## 复制起始项目

1. 如果您已经有 `workshop` 目录，请先重命名或删除它。

1. 运行安装脚本将起始项目复制到 `workshop` 目录。

    ```bash
    # zsh/bash
    bash $REPOSITORY_ROOT/scripts/setup.sh --session 03-handoff-pattern
    ```

    ```powershell
    # PowerShell
    & $REPOSITORY_ROOT/scripts/setup.ps1 -Session 03-handoff-pattern
    ```

## 部署智能体

1. 确保您在 `workshop` 目录中。

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. 打开 `src/MultiAgentWorkshop.PromptAgent/appsettings.json`，找到注释行 `// Add agents`，在其下方添加 `Agents` 属性。

    ```jsonc
    {
      ...
      // 添加智能体
      "Agents": [
        {
          "Name": "triage-agent",
          "Version": "1"
        },
        {
          "Name": "general-support-agent",
          "Version": "1"
        },
        {
          "Name": "network-specialist-agent",
          "Version": "1"
        },
        {
          "Name": "warranty-agent",
          "Version": "1"
        }
      ]
      ...
    }
    ```

1. 导航到 `resources-foundry` 目录。

    ```bash
    pushd resources-foundry
    ```

1. 运行以下命令将上面定义的智能体预配并部署到 Microsoft Foundry。

    ```bash
    azd up
    ```

   在预配过程中，系统会要求您输入环境名称、Azure 订阅和位置。

1. 预配和部署完成后，运行以下命令确认智能体已成功部署。

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

   您应该看到四个智能体名称。

    ```text
    warranty-agent
    network-specialist-agent
    general-support-agent
    triage-agent 
    ```

1. 导航回 workshop 目录。

    ```bash
    popd
    ```

## 配置 Aspire 编排

1. 确保您在 `workshop` 目录中。

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. 验证所有必要的智能体信息已被记录。

    ```bash
    dotnet user-secrets --project ./src/MultiAgentWorkshop.AppHost list
    ```

   您应该看到 `AZURE_TENANT_ID`、`FOUNDRY_NAME`、`FOUNDRY_PROJECT_NAME`、`FOUNDRY_RESOURCE_GROUP` 和 `Foundry:Project:Endpoint` 的值。

1. 打开 `src/MultiAgentWorkshop.AppHost/appsettings.json`，找到注释行 `// Add agents`，在其下方添加 `Agents` 属性。

    ```jsonc
    {
      ...
      // 添加智能体
      "Agents": [
        {
          "Name": "triage-agent",
          "Version": "1"
        },
        {
          "Name": "general-support-agent",
          "Version": "1"
        },
        {
          "Name": "network-specialist-agent",
          "Version": "1"
        },
        {
          "Name": "warranty-agent",
          "Version": "1"
        }
      ]
      ...
    }
    ```

1. 打开 `src/MultiAgentWorkshop.AppHost/AppHost.cs`，找到注释 `// Add resource for Microsoft Foundry`，在其正下方添加代码。这将添加 Microsoft Foundry 项目的连接详情。

    ```csharp
    // 添加 Microsoft Foundry 资源
    var foundry = builder.AddFoundry("foundry");
    ```

   让我们分解这段代码。

   - `builder.AddFoundry("foundry")`：这通过自定义资源 `FoundryResource` 添加 Microsoft Foundry 连接详情。如果您想了解更多关于 Aspire 自定义资源的信息，请访问 [Create custom hosting integrations](https://aspire.dev/integrations/custom-integrations/hosting-integrations/)。

1. 在同一文件中，找到注释 `// Add resource for agents on Microsoft Foundry`，在其正下方添加代码。这将向引用的应用公开智能体详情列表。

    ```csharp
    // 添加 Microsoft Foundry 上的智能体资源
    var agents = builder.AddAgents("agents");
    ```

   让我们分解这段代码。

   - `builder.AddAgents("agents")`：这通过自定义资源 `AgentResource` 添加智能体详情列表。如果您想了解更多关于 Aspire 自定义资源的信息，请访问 [Create custom hosting integrations](https://aspire.dev/integrations/custom-integrations/hosting-integrations/)。

1. 在同一文件中，找到注释 `// Add backend agent service`，在其正下方添加代码。这将定义引用 `foundry` 资源的后端智能体服务——所有 Microsoft Foundry 连接详情将传递给后端智能体服务应用。

    ```csharp
    // 添加后端智能体服务
    var agent = builder.AddProject<MultiAgentWorkshop_Agent>("agent")
                       .WithReference(foundry);
    ```

   让我们分解这段代码。

   - `builder.AddProject<MultiAgentWorkshop_Agent>("agent")`：这将后端智能体服务应用作为 .NET 项目添加。
   - `.WithReference(foundry)`：这引用了上面创建的 foundry 资源，将 Microsoft Foundry 连接详情传递给后端智能体服务应用。

1. 在同一文件中，找到注释 `// Add frontend web UI`，在其正下方添加代码。这将定义引用 `agents` 和 `agent` 两个资源的前端 Web UI——智能体详情和后端连接详情都将传递给前端 Web UI 应用。

    ```csharp
    // 添加前端 Web UI
    var webUI = builder.AddProject<MultiAgentWorkshop_WebUI>("webui")
                       .WithExternalHttpEndpoints()
                       .WithReference(agents)
                       .WithReference(agent)
                       .WaitFor(agent);
    ```

   让我们分解这段代码。

   - `builder.AddProject<MultiAgentWorkshop_WebUI>("webui")`：这将前端 Web UI 应用作为 .NET 项目添加。
   - `.WithExternalHttpEndpoints()`：这将前端 Web UI 应用暴露到互联网，使其可公开访问。
   - `.WithReference(agents)`：这引用了上面创建的 agents 资源，将智能体列表传递给前端 Web UI 应用。
   - `.WithReference(agent)`：这引用了后端智能体服务应用，将连接详情传递给前端 Web UI 应用。

## 在后端智能体服务中实现交接模式

1. 确保您在 `workshop` 目录中。

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. 打开 `src/MultiAgentWorkshop.Agent/Program.cs`，找到注释 `// Create AzureOpenAIClient instance with EntraID authentication`，在其正下方添加代码。这将连接到 Microsoft Foundry 项目。

    ```csharp
    // 使用 EntraID 身份验证创建 AzureOpenAIClient 实例
    var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions() { TenantId = config["AZURE_TENANT_ID"] });

    var chatClient = new AzureOpenAIClient(new Uri(endpoint), credential)
                        .GetResponsesClient()
                        .AsIChatClient(model);
    ```

   让我们分解这段代码。

   - `new DefaultAzureCredential(...)`：这无需 API 密钥即可登录 Azure。在本地机器上使用您的 Azure CLI 登录或 Azure Developer CLI 登录详情，在部署到 Azure 时使用托管标识。
   - `new AzureOpenAIClient(new Uri(endpoint), credential)`：这使用端点和登录详情连接到 Azure OpenAI 实例，并将其转换为 `IChatClient` 实例。

1. 在同一文件中，找到注释 `// Register all agents passed from Aspire`，在其正下方添加代码。这将从 Microsoft Foundry 项目中拉取智能体详情，并将它们作为单例服务注册到 IoC 容器中。

    ```csharp
    // 注册所有从 Aspire 传递的智能体
    foreach (var agentSettings in agents)
    {
        var instruction = await File.ReadAllTextAsync(
            Path.Combine(AppContext.BaseDirectory, "Prompts", $"{agentSettings.Name}.txt"));

        var agent = new ChatClientAgent(
            chatClient,
            instructions: instruction,
            name: agentSettings.Name);

        builder.Services.AddKeyedSingleton<AIAgent>(agentSettings.Name, agent);
    }
    ```

   让我们分解这段代码。

   - 我们已经知道智能体列表，但只知道它们的名称。因此，代码对每个智能体运行 `foreach` 循环。
   - `await File.ReadAllTextAsync(...)`：这导入智能体指令文件。
   - `new ChatClientAgent(chatClient, instructions, name)`：使用每个智能体的信息、指令和 `IChatClient` 实例创建智能体实例。
   - `builder.Services.AddKeyedSingleton<AIAgent>(name, agent)`：将智能体实例注册为单例服务。

1. 在同一文件中，找到注释 `// Build a handoff workflow pattern with the agents registered`，在其正下方添加代码。

    ```csharp
    // 使用已注册的智能体构建交接工作流模式
    builder.AddWorkflow("publisher", (sp, key) =>
    {
        var triage = sp.GetRequiredKeyedService<AIAgent>("triage-agent");
        var generalSupport = sp.GetRequiredKeyedService<AIAgent>("general-support-agent");
        var networkSpecialist = sp.GetRequiredKeyedService<AIAgent>("network-specialist-agent");
        var warranty = sp.GetRequiredKeyedService<AIAgent>("warranty-agent");

        var specialists = new[] { generalSupport, networkSpecialist, warranty };

        var workflow = AgentWorkflowBuilder.CreateHandoffBuilderWith(triage)
            // 分诊智能体可以交接给任何专家
            .WithHandoffs(triage, specialists)
            // 每个专家可以交接给其他专家
            .WithHandoffs(generalSupport, [networkSpecialist, warranty])
            .WithHandoffs(networkSpecialist, [generalSupport, warranty])
            .WithHandoffs(warranty, [generalSupport, networkSpecialist])
            // 所有专家交接回分诊智能体
            .WithHandoffs(specialists, triage, "Hand back to triage when the issue is resolved or needs further routing")
            .Build();

        // HandoffWorkflowBuilder.Build() 不会设置工作流名称。
        // 通过反射设置名称以通过 AddWorkflow 的名称验证。
        typeof(Workflow).GetProperty("Name")!.SetValue(workflow, key);

        return workflow;
    }).AddAsAIAgent("publisher");
    ```

   让我们分解这段代码。

   - `var specialists = new[] { generalSupport, networkSpecialist, warranty };`：这定义了专家智能体列表。分诊智能体是起始点，将用户请求重新路由到其中一个专家智能体。
   - `builder.AddWorkflow("publisher", ...).AddAsAIAgent("publisher")`：这将多智能体工作流作为另一个名为 `publisher` 的智能体实例添加，并将其注册为单例。
   - `AgentWorkflowBuilder.CreateHandoffBuilderWith(triage)`：这是带有分诊智能体的交接工作流构建器。
   - `.WithHandoffs(triage, specialists)`：这定义了从分诊智能体到专家智能体的交接。
   - `.WithHandoffs(generalSupport, [networkSpecialist, warranty])`：这定义了从通用支持智能体到其他专家智能体的交接。
   - `.WithHandoffs(networkSpecialist, [generalSupport, warranty])`：这定义了从网络专家智能体到其他专家智能体的交接。
   - `.WithHandoffs(warranty, [generalSupport, networkSpecialist])`：这定义了从保修智能体到其他专家智能体的交接。
   - `.WithHandoffs(specialists, triage, "Hand back to triage when the issue is resolved or needs further routing")`：这定义了当问题解决或需要进一步路由时，所有专家智能体交接回分诊智能体。
   - `typeof(Workflow).GetProperty("Name")!.SetValue(workflow, key);`：这注入工作流名称，是一个临时的解决方案。

1. 在同一文件中，找到注释 `// Map AGUI to the publisher workflow agent`，在其正下方添加代码。工作流通过 `ag-ui` 端点作为 API 暴露，以便前端 Web UI 可以与此后端智能体服务应用通信。

    ```csharp
    // 将 AGUI 映射到 publisher 工作流智能体
    app.MapAGUI(
        pattern: "ag-ui",
        aiAgent: app.Services.GetRequiredKeyedService<AIAgent>("publisher")
                             .CreateFixedAgent()
    );
    ```

   请注意，`.CreateFixedAgent()` 是一个临时的解决方案，直到输出流被正确处理。

## 在前端 Web UI 中实现交接模式

1. 确保您在 `workshop` 目录中。

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. 打开 `src/MultiAgentWorkshop.WebUI/Program.cs`，找到注释 `// Register all agents passed from Aspire`，在其正下方添加代码。这将注册所有智能体详情，以便 Web UI 知道哪个智能体在响应。

    ```csharp
    // 注册所有从 Aspire 传递的智能体
    builder.Services.AddSingleton(agents);
    ```

1. 在同一文件中，找到注释 `// Register the backend agent service as an HTTP client`，在其正下方添加代码。Aspire 已经为前端 Web UI 应用提供了后端智能体服务的连接详情。

    ```csharp
    // 将后端智能体服务注册为 HTTP 客户端
    builder.Services.AddHttpClient("agent", client =>
    {
        client.BaseAddress = new Uri("https+http://agent");
    });
    ```

1. 在同一文件中，找到注释 `// Register AGUI client`，在其正下方添加代码。使用此 AGUI 客户端，前端 Web UI 应用通过 `ag-ui` 端点与后端智能体服务应用通信。

    ```csharp
    // 注册 AGUI 客户端
    builder.Services.AddChatClient(sp => new AGUIChatClient(
        httpClient: sp.GetRequiredService<IHttpClientFactory>().CreateClient("agent"),
        endpoint: "ag-ui")
    );
    ```

## 运行 Aspire

1. 确保您在 `workshop` 目录中。

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. 确保您已经使用 Azure CLI 和 Azure Developer CLI 登录了 Azure。如果不确定，请再次按照[此步骤](./00-setup.md#登录-azure)操作。

1. 运行以下命令通过 Aspire 启动所有应用。

    ```bash
    dotnet watch run --project ./src/MultiAgentWorkshop.AppHost
    ```

1. Aspire 仪表板会自动打开。

   ![Aspire 仪表板](../../../docs/images/step-03-image-01.png)

   点击后端智能体服务应用。

1. 当 Dev UI 页面打开后，将智能体切换为 `publisher`，查看分诊智能体如何将请求分发给其他智能体。

   ![Microsoft Agent Framework Dev UI - 交接模式](../../../docs/images/step-03-image-02.png)

1. 发送任意请求。

   ![Microsoft Agent Framework Dev UI - 发送请求](../../../docs/images/step-03-image-03.png)

   查看结果以及屏幕左侧的工作流进度。

   ![Microsoft Agent Framework Dev UI - 工作流运行](../../../docs/images/step-03-image-04.png)

1. 返回 Aspire 仪表板，点击 Web UI 应用。

   ![Aspire 仪表板](../../../docs/images/step-03-image-05.png)

1. 发送任意请求。

   ![Microsoft Agent Framework Chat UI - 发送请求](../../../docs/images/step-03-image-06.png)

   查看结果。

1. 按 `Ctrl`+`C` 终止所有运行中的应用。

## 部署到 Azure

1. 确保您在 `workshop` 目录中。

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. 运行以下命令将前端 Web UI 和后端智能体服务应用预配并部署到 Azure。

    ```bash
    azd up
    ```

   在预配过程中，系统会要求您输入环境名称、Azure 订阅和位置。

1. 完成后，您将在终端屏幕上看到 Web UI 应用的 URL。在浏览器中打开它并发送请求。

   ![Microsoft Agent Framework on Azure Container Apps - 发送请求](../../../docs/images/step-03-image-07.png)

   查看结果。

1. 一切完成后，从 Azure 中删除所有应用和智能体。

    ```bash
    # 删除 Web UI 和智能体服务应用。
    azd down --purge --force

    # 删除所有智能体和 Microsoft Foundry 资源。
    cd resources-foundry
    azd down --purge --force
    ```

---

恭喜！🎉 您已完成第三个多智能体编排场景——交接模式。让我们继续吧！

👈 [02: 并发模式](./02-concurrent-pattern.md) | [04: 群聊模式](./04-group-chat-pattern.md) 👉
