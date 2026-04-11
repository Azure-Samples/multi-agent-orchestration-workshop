# 04 Padrão Group Chat

No padrão group chat, múltiplos agentes participam de uma conversa compartilhada, alternando para contribuir com suas especialidades. Um orquestrador gerencia o fluxo da discussão, decidindo qual agente fala a seguir e quando a conversa deve terminar. Isso é adequado para planejamento multifuncional, brainstorming criativo ou qualquer tarefa onde perspectivas diversas precisam interagir e se complementar iterativamente.

## Cenário

Você faz parte de uma equipe multifuncional de produto planejando o lançamento de um novo produto com agentes &ndash; agente de estratégia de produto, agente de experiência do usuário, agente técnico e agente de negócios.

<div>
  <img src="../../../docs/images/04-group-chat-pattern-architecture.png" alt="Arquitetura - Padrão Group Chat" width="640" />
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
    bash $REPOSITORY_ROOT/scripts/setup.sh --session 04-group-chat-pattern
    ```

    ```powershell
    # PowerShell
    & $REPOSITORY_ROOT/scripts/setup.ps1 -Session 04-group-chat-pattern
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

   Você deverá ver os quatro nomes dos agentes.

    ```text
    business-agent
    technical-agent
    user-experience-agent
    product-strategy-agent
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

1. No mesmo arquivo, encontre o comentário `// Add backend agent service` e adicione o código logo abaixo dele. Isso define o serviço de agente backend que referencia o recurso `foundry` &ndash; todos os detalhes de conexão do Microsoft Foundry são passados para o aplicativo de serviço de agente backend.

    ```csharp
    // Adicionar serviço de agente backend
    var agent = builder.AddProject<MultiAgentWorkshop_Agent>("agent")
                       .WithReference(foundry);
    ```

   Vamos analisar o código.

   - `builder.AddProject<MultiAgentWorkshop_Agent>("agent")`: Isso adiciona o aplicativo de serviço de agente backend como um projeto .NET.
   - `.WithReference(foundry)`: Isso referencia o recurso foundry criado acima, que passa os detalhes de conexão do Microsoft Foundry para o aplicativo de serviço de agente backend.

1. No mesmo arquivo, encontre o comentário `// Add frontend web UI` e adicione o código logo abaixo dele. Isso define a interface web frontend que referencia os recursos `agents` e `agent` &ndash; os detalhes dos agentes e os detalhes de conexão do backend são passados para o aplicativo de interface web frontend.

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

## Implementar o padrão group chat no serviço de agente backend

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

1. No mesmo arquivo, encontre o comentário `// Build a group chat workflow pattern with the agents registered` e adicione o código logo abaixo dele.

    ```csharp
    // Construir um padrão de fluxo de trabalho group chat com os agentes registrados
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

   Vamos analisar o código.

   - `var participants = agents.Select(a => sp.GetRequiredKeyedService<AIAgent>(a.Name));`: Isso identifica a lista de agentes participantes no group chat.
   - `builder.AddWorkflow("publisher", ...).AddAsAIAgent("publisher")`: Isso adiciona o fluxo de trabalho multi-agente como outra instância de agente chamada `publisher` e o registra como singleton.
   - `AgentWorkflowBuilder.CreateGroupChatBuilderWith(...)`: Este é o construtor de fluxo de trabalho group chat que usa o mesmo nome, `publisher`.
   - `new RoundRobinGroupChatManager(...)`: Isso define a estratégia do group chat como round-robin. Também define o número máximo de iterações. Neste código, cada agente responde duas vezes durante o ciclo round-robin.

1. No mesmo arquivo, encontre o comentário `// Map AGUI to the publisher workflow agent` e adicione o código logo abaixo dele. O fluxo de trabalho é exposto como um endpoint de API em `ag-ui` para que a interface web frontend possa se comunicar com este aplicativo de serviço de agente backend.

    ```csharp
    // Mapear AGUI para o agente de fluxo de trabalho publisher
    app.MapAGUI(
        pattern: "ag-ui",
        aiAgent: app.Services.GetRequiredKeyedService<AIAgent>("publisher")
    );
    ```

## Implementar o padrão group chat na interface web frontend

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

   ![Painel do Aspire](../../../docs/images/step-04-image-01.png)

   Clique no aplicativo de serviço de agente backend.

1. Quando a página Dev UI abrir, altere o agente para `publisher` e clique no botão "Configure & Run".

   ![Microsoft Agent Framework Dev UI - Padrão group chat](../../../docs/images/step-04-image-02.png)

1. Envie qualquer solicitação.

   ![Microsoft Agent Framework Dev UI - Enviar solicitação](../../../docs/images/step-04-image-03.png)

   Veja o resultado e como o fluxo de trabalho progride no lado esquerdo da tela.

   ![Microsoft Agent Framework Dev UI - Execução do fluxo de trabalho](../../../docs/images/step-04-image-04.png)

1. Volte ao painel do Aspire e clique no aplicativo de interface web.

   ![Painel do Aspire](../../../docs/images/step-04-image-05.png)

1. Envie qualquer solicitação.

   ![Microsoft Agent Framework Chat UI - Enviar solicitação](../../../docs/images/step-04-image-06.png)

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

   ![Microsoft Agent Framework no Azure Container Apps - Enviar solicitação](../../../docs/images/step-04-image-07.png)

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

Parabéns! 🎉 Você acabou de concluir o quarto cenário de orquestração multi-agente &ndash; o padrão group chat. Você concluiu tudo!

👈 [03: Handoff Pattern](./03-handoff-pattern.md) | [README](../README.md) 👉
