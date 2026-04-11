# 02 Padrão Concurrent

No padrão concurrent, múltiplos agentes analisam a mesma entrada simultaneamente, cada um trazendo sua própria expertise. Quando todos os agentes terminam, suas saídas são combinadas em um resultado unificado. Isso é ideal para tarefas que se beneficiam de múltiplos pontos de vista trabalhando ao mesmo tempo, como análise com múltiplas perspectivas, avaliação em conjunto ou tomada de decisão colaborativa.

## Cenário

Você trabalha para uma empresa de capital de risco e está analisando uma proposta de startup com agentes &ndash; agente de análise de mercado, agente de viabilidade tecnológica, agente de modelo financeiro, agente de avaliação de riscos e agente agregador.

<div>
  <img src="../../../docs/images/02-concurrent-pattern-architecture.png" alt="Arquitetura - Padrão Concurrent" width="640" />
</div>

## Obter a raiz do repositório

1. Obtenha a variável `$REPOSITORY_ROOT` primeiro.

    ```bash
    # zsh/bash
    REPOSITORY_ROOT=$(git rev-parse --show-toplevel)
    ```

    ```powershell
    # PowerShell
    $REPOSITORY_ROOT = git rev-parse --show-toplevel
    ```

## Copiar o projeto inicial

1. Se você já tem o diretório `workshop`, renomeie ou remova-o primeiro.

1. Execute o script de configuração para copiar o projeto inicial para o diretório `workshop`.

    ```bash
    # zsh/bash
    bash $REPOSITORY_ROOT/scripts/setup.sh --session 02-concurrent-pattern
    ```

    ```powershell
    # PowerShell
    & $REPOSITORY_ROOT/scripts/setup.ps1 -Session 02-concurrent-pattern
    ```

## Implantar os agentes

1. Certifique-se de que você está no diretório `workshop`.

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. Abra `src/MultiAgentWorkshop.PromptAgent/appsettings.json`, encontre a linha de comentário `// Add agents` e adicione a propriedade `Agents` logo abaixo dela.

    ```jsonc
    {
      ...
      // Adicionar agentes
      "Agents": [
        {
          "Name": "market-analysis-agent",
          "Version": "1"
        },
        {
          "Name": "technology-feasibility-agent",
          "Version": "1"
        },
        {
          "Name": "financial-model-agent",
          "Version": "1"
        },
        {
          "Name": "risk-assessment-agent",
          "Version": "1"
        },
        {
          "Name": "aggregator-agent",
          "Version": "1"
        }
      ]
      ...
    }
    ```

1. Navegue até o diretório `resources-foundry`.

    ```bash
    pushd resources-foundry
    ```

1. Execute o seguinte comando para provisionar e implantar os agentes definidos acima no Microsoft Foundry.

    ```bash
    azd up
    ```

   Durante o provisionamento, será solicitado que você informe um nome de ambiente, assinatura Azure e localização.

1. Após o provisionamento e a implantação serem concluídos, execute o seguinte comando para confirmar que os agentes foram implantados com sucesso.

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

   Você deverá ver os cinco nomes dos agentes.

    ```text
    aggregator-agent
    risk-assessment-agent
    financial-model-agent
    technology-feasibility-agent
    market-analysis-agent
    ```

1. Navegue de volta ao diretório do workshop.

    ```bash
    popd
    ```

## Configurar a orquestração do Aspire

1. Certifique-se de que você está no diretório `workshop`.

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. Verifique se todas as informações necessárias dos agentes foram registradas.

    ```bash
    dotnet user-secrets --project ./src/MultiAgentWorkshop.AppHost list
    ```

   Você deverá ver os valores de `AZURE_TENANT_ID`, `FOUNDRY_NAME`, `FOUNDRY_PROJECT_NAME`, `FOUNDRY_RESOURCE_GROUP` e `Foundry:Project:Endpoint`.

1. Abra `src/MultiAgentWorkshop.AppHost/appsettings.json`, encontre a linha de comentário `// Add agents` e adicione a propriedade `Agents` logo abaixo dela.

    ```jsonc
    {
      ...
      // Adicionar agentes
      "Agents": [
        {
          "Name": "market-analysis-agent",
          "Version": "1"
        },
        {
          "Name": "technology-feasibility-agent",
          "Version": "1"
        },
        {
          "Name": "financial-model-agent",
          "Version": "1"
        },
        {
          "Name": "risk-assessment-agent",
          "Version": "1"
        },
        {
          "Name": "aggregator-agent",
          "Version": "1"
        }
      ]
      ...
    }
    ```

1. Abra `src/MultiAgentWorkshop.AppHost/AppHost.cs`, encontre o comentário `// Add resource for Microsoft Foundry` e adicione o código logo abaixo dele. Isso adiciona os detalhes de conexão do projeto Microsoft Foundry.

    ```csharp
    // Adicionar recurso para Microsoft Foundry
    var foundry = builder.AddFoundry("foundry");
    ```

   Vamos analisar o código.

   - `builder.AddFoundry("foundry")`: Isso adiciona os detalhes de conexão do Microsoft Foundry por meio de um recurso personalizado, `FoundryResource`. Se você quiser saber mais sobre o recurso personalizado do Aspire, visite [Create custom hosting integrations](https://aspire.dev/integrations/custom-integrations/hosting-integrations/).

1. No mesmo arquivo, encontre o comentário `// Add resource for agents on Microsoft Foundry` e adicione o código logo abaixo dele. Isso expõe a lista de detalhes dos agentes para a aplicação que o referencia.

    ```csharp
    // Adicionar recurso para agentes no Microsoft Foundry
    var agents = builder.AddAgents("agents");
    ```

   Vamos analisar o código.

   - `builder.AddAgents("agents")`: Isso adiciona a lista de detalhes dos agentes por meio de um recurso personalizado, `AgentResource`. Se você quiser saber mais sobre o recurso personalizado do Aspire, visite [Create custom hosting integrations](https://aspire.dev/integrations/custom-integrations/hosting-integrations/).

1. No mesmo arquivo, encontre o comentário `// Add backend agent service` e adicione o código logo abaixo dele. Isso define o serviço de agente backend que referencia o recurso `foundry` — todos os detalhes de conexão do Microsoft Foundry são passados para o aplicativo de serviço de agente backend.

    ```csharp
    // Adicionar serviço de agente backend
    var agent = builder.AddProject<MultiAgentWorkshop_Agent>("agent")
                       .WithReference(foundry);
    ```

   Vamos analisar o código.

   - `builder.AddProject<MultiAgentWorkshop_Agent>("agent")`: Isso adiciona o aplicativo de serviço de agente backend como um projeto .NET.
   - `.WithReference(foundry)`: Isso referencia o recurso foundry criado acima, que passa os detalhes de conexão do Microsoft Foundry para o aplicativo de serviço de agente backend.

1. No mesmo arquivo, encontre o comentário `// Add frontend web UI` e adicione o código logo abaixo dele. Isso define a interface web frontend que referencia os recursos `agents` e `agent` — os detalhes dos agentes e os detalhes de conexão do backend são passados para o aplicativo de interface web frontend.

    ```csharp
    // Adicionar interface web frontend
    var webUI = builder.AddProject<MultiAgentWorkshop_WebUI>("webui")
                       .WithExternalHttpEndpoints()
                       .WithReference(agents)
                       .WithReference(agent)
                       .WaitFor(agent);
    ```

   Vamos analisar o código.

   - `builder.AddProject<MultiAgentWorkshop_WebUI>("webui")`: Isso adiciona o aplicativo de interface web frontend como um projeto .NET.
   - `.WithExternalHttpEndpoints()`: Isso expõe o aplicativo de interface web frontend para a Internet, tornando-o acessível publicamente.
   - `.WithReference(agents)`: Isso referencia o recurso de agentes criado acima, que passa a lista de agentes para o aplicativo de interface web frontend.
   - `.WithReference(agent)`: Isso referencia o aplicativo de serviço de agente backend, que passa os detalhes de conexão para o aplicativo de interface web frontend.

## Implementar o padrão concurrent no serviço de agente backend

1. Certifique-se de que você está no diretório `workshop`.

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. Abra `src/MultiAgentWorkshop.Agent/Program.cs`, encontre o comentário `// Create AIProjectClient instance with EntraID authentication` e adicione o código logo abaixo dele. Isso conecta ao projeto Microsoft Foundry.

    ```csharp
    // Criar instância do AIProjectClient com autenticação EntraID
    var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions() { TenantId = config["AZURE_TENANT_ID"] });
    var projectClient = new AIProjectClient(endpoint: new Uri(endpoint), tokenProvider: credential);
    ```

   Vamos analisar o código.

   - `new DefaultAzureCredential(...)`: Isso faz login no Azure sem uma chave de API. Usa os detalhes de login do Azure CLI ou Azure Developer CLI na sua máquina local, e Managed Identity quando o aplicativo é implantado no Azure.
   - `new AIProjectClient(endpoint, credential)`: Isso conecta à instância do projeto Microsoft Foundry usando o endpoint e os detalhes de login.

1. No mesmo arquivo, encontre o comentário `// Register all agents passed from Aspire` e adicione o código logo abaixo dele. Isso obtém os detalhes dos agentes do projeto Microsoft Foundry e os registra no contêiner IoC como serviços singleton.

    ```csharp
    // Registrar todos os agentes passados pelo Aspire
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

   Vamos analisar o código.

   - Já conhecemos a lista de agentes, mas sabemos apenas seus nomes. Portanto, o código executa o loop `foreach` para cada agente.
   - `new AgentReference(name, version)`: Usando as informações de cada agente, isso cria uma instância de referência.
   - `projectClient.AsAIAgent(reference, factory)`: Isso conecta ao agente real usando os detalhes de referência.
   - `builder.Services.AddKeyedSingleton<AIAgent>(name, agent)`: Isso registra a instância do agente como um serviço singleton.

   > **NOTA**: Você pode notar a classe `AgentRecordShimChatClient`. É uma solução temporária para uma incompatibilidade de versão entre o Microsoft Agent Framework e o SDK do Microsoft Foundry, que será removida em breve.

1. No mesmo arquivo, encontre o comentário `// Build a concurrent workflow pattern with the agents registered` e adicione o código logo abaixo dele.

    ```csharp
    // Construir um padrão de fluxo de trabalho concurrent com os agentes registrados
    var concurrentAgents = agents.Where(a => a.Name != "aggregator-agent");
    var aggregatorAgent = agents.SingleOrDefault(a => a.Name == "aggregator-agent");

    builder.AddWorkflow("concurrent-analysis", (sp, key) => AgentWorkflowBuilder.BuildConcurrent(
        workflowName: key,
        agents: [.. concurrentAgents.Select(a => sp.GetRequiredKeyedService<AIAgent>(a.Name))],
        aggregator: null
    )).AddAsAIAgent("concurrent-analysis");

    builder.AddWorkflow("publisher", (sp, key) => AgentWorkflowBuilder.BuildSequential(
        workflowName: key,
        agents: [
            sp.GetRequiredKeyedService<AIAgent>("concurrent-analysis"),
            sp.GetRequiredKeyedService<AIAgent>(aggregatorAgent!.Name)
        ]
    )).AddAsAIAgent("publisher");
    ```

   Vamos analisar o código.

   - `builder.AddWorkflow("concurrent-analysis", ...).AddAsAIAgent("concurrent-analysis")`: Isso adiciona o fluxo de trabalho multi-agente como outra instância de agente chamada `concurrent-analysis` e o registra como singleton.
   - `AgentWorkflowBuilder.BuildConcurrent(...)`: Este é o construtor de fluxo de trabalho concurrent que usa o mesmo nome, `concurrent-analysis`.

     Note que ele adiciona múltiplos agentes dos serviços registrados anteriormente declarados pelo array `agents`.

     Note também que ele passa `null` para o agregador para que possamos usar o `aggregator-agent` fornecido pelo Microsoft Foundry.
   - `builder.AddWorkflow("publisher, ...).AddAsAIAgent("publisher")`: Isso adiciona o fluxo de trabalho multi-agente como outra instância de agente chamada `publisher` e o registra como singleton.
   - `AgentWorkflowBuilder.BuildSequential(...)`: Este é o construtor de fluxo de trabalho sequential que usa o mesmo nome, `publisher`.

     Note que ele adiciona tanto o fluxo de trabalho `concurrent-analysis` quanto o `aggregator-agent` para que o agente agregador resuma o que cada agente respondeu no fluxo de trabalho concurrent.

1. No mesmo arquivo, encontre o comentário `// Map AGUI to the publisher workflow agent` e adicione o código logo abaixo dele. O fluxo de trabalho é exposto como um endpoint de API em `ag-ui` para que a interface web frontend possa se comunicar com este aplicativo de serviço de agente backend.

    ```csharp
    // Mapear AGUI para o agente de fluxo de trabalho publisher
    app.MapAGUI(
        pattern: "ag-ui",
        aiAgent: app.Services.GetRequiredKeyedService<AIAgent>("publisher")
    );
    ```

## Implementar o padrão concurrent na interface web frontend

1. Certifique-se de que você está no diretório `workshop`.

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. Abra `src/MultiAgentWorkshop.WebUI/Program.cs`, encontre o comentário `// Register all agents passed from Aspire` e adicione o código logo abaixo dele. Isso registra todos os detalhes dos agentes para que a interface web saiba qual agente está respondendo.

    ```csharp
    // Registrar todos os agentes passados pelo Aspire
    builder.Services.AddSingleton(agents);
    ```

1. No mesmo arquivo, encontre o comentário `// Register the backend agent service as an HTTP client` e adicione o código logo abaixo dele. O Aspire já fornece ao aplicativo de interface web frontend os detalhes de conexão para o serviço de agente backend.

    ```csharp
    // Registrar o serviço de agente backend como um cliente HTTP
    builder.Services.AddHttpClient("agent", client =>
    {
        client.BaseAddress = new Uri("https+http://agent");
    });
    ```

1. No mesmo arquivo, encontre o comentário `// Register AGUI client` e adicione o código logo abaixo dele. Usando este cliente AGUI, o aplicativo de interface web frontend se comunica com o aplicativo de serviço de agente backend via o endpoint `ag-ui`.

    ```csharp
    // Registrar cliente AGUI
    builder.Services.AddChatClient(sp => new AGUIChatClient(
        httpClient: sp.GetRequiredService<IHttpClientFactory>().CreateClient("agent"),
        endpoint: "ag-ui")
    );
    ```

## Executar o Aspire

1. Certifique-se de que você está no diretório `workshop`.

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. Certifique-se de que você já fez login no Azure usando tanto o Azure CLI quanto o Azure Developer CLI. Se não tiver certeza, siga [este passo](./00-setup.md#fazer-login-no-azure) novamente.

1. Execute o seguinte comando para iniciar todos os aplicativos através do Aspire.

    ```bash
    dotnet watch run --project ./src/MultiAgentWorkshop.AppHost
    ```

1. O painel do Aspire abre automaticamente.

   ![Painel do Aspire](../../../docs/images/step-02-image-01.png)

   Clique no aplicativo de serviço de agente backend.

1. Quando a página Dev UI abrir, altere o agente para `concurrent-analysis` e veja que todos os agentes executam concorrentemente.

   ![Microsoft Agent Framework Dev UI - Padrão concurrent](../../../docs/images/step-02-image-02.png)

   Em seguida, altere o agente para `publisher` e veja como o padrão sequential combina o `concurrent-analysis` com o `aggregator-agent`.

   ![Microsoft Agent Framework Dev UI - Padrão sequential](../../../docs/images/step-02-image-03.png)

1. Envie qualquer solicitação.

   ![Microsoft Agent Framework Dev UI - Enviar solicitação](../../../docs/images/step-02-image-04.png)

   Veja o resultado e como o fluxo de trabalho progride no lado esquerdo da tela.

   ![Microsoft Agent Framework Dev UI - Execução do fluxo de trabalho](../../../docs/images/step-02-image-05.png)

1. Volte ao painel do Aspire e clique no aplicativo de interface web.

   ![Painel do Aspire](../../../docs/images/step-02-image-06.png)

1. Envie qualquer solicitação.

   ![Microsoft Agent Framework Chat UI - Enviar solicitação](../../../docs/images/step-02-image-07.png)

   Veja o resultado.

1. Pressione `Ctrl`+`C` para encerrar todos os aplicativos em execução.

## Implantar no Azure

1. Certifique-se de que você está no diretório `workshop`.

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. Execute o seguinte comando para provisionar e implantar tanto a interface web frontend quanto o serviço de agente backend no Azure.

    ```bash
    azd up
    ```

   Durante o provisionamento, será solicitado que você informe um nome de ambiente, assinatura Azure e localização.

1. Após concluído, você verá a URL do aplicativo de interface web na tela do terminal. Abra-a no seu navegador e envie uma solicitação.

   ![Microsoft Agent Framework no Azure Container Apps - Enviar solicitação](../../../docs/images/step-02-image-08.png)

   Veja o resultado.

1. Quando tudo estiver pronto, remova todos os aplicativos e agentes do Azure.

    ```bash
    # Excluir tanto o aplicativo de interface web quanto o de serviço de agente.
    azd down --purge --force

    # Excluir todos os agentes e o recurso Microsoft Foundry.
    cd resources-foundry
    azd down --purge --force
    ```

---

Parabéns! 🎉 Você acabou de concluir o segundo cenário de orquestração multi-agente - o padrão concurrent. Vamos prosseguir!

👈 [01: Sequential Pattern](./01-sequential-pattern.md) | [03: Handoff Pattern](./03-handoff-pattern.md) 👉
