# 03 Padrão Handoff

No padrão handoff, os agentes passam dinamicamente o controle uns aos outros com base no contexto da conversa. Um agente de triagem recebe a solicitação inicial e a encaminha para o especialista mais adequado para tratá-la. Os especialistas também podem redirecionar entre si quando o problema abrange diferentes domínios. Isso funciona bem para cenários como suporte de TI, atendimento ao cliente ou qualquer fluxo de trabalho onde diferentes especialidades são necessárias em diferentes estágios.

## Cenário

Você trabalha em uma equipe de suporte de TI com agentes &ndash; agente de suporte geral, agente especialista em rede, agente de garantia e agente de triagem.

<div>
  <img src="../../../docs/images/03-handoff-pattern-architecture.png" alt="Arquitetura - Padrão Handoff" width="640" />
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
    bash $REPOSITORY_ROOT/scripts/setup.sh --session 03-handoff-pattern
    ```

    ```powershell
    # PowerShell
    & $REPOSITORY_ROOT/scripts/setup.ps1 -Session 03-handoff-pattern
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
    warranty-agent
    network-specialist-agent
    general-support-agent
    triage-agent 
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

## Implementar o padrão handoff no serviço de agente backend

1. Certifique-se de que você está no diretório `workshop`.

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. Abra `src/MultiAgentWorkshop.Agent/Program.cs`, encontre o comentário `// Create AzureOpenAIClient instance with EntraID authentication` e adicione o código logo abaixo dele. Isso conecta ao projeto Microsoft Foundry.

    ```csharp
    // Criar instância do AzureOpenAIClient com autenticação EntraID
    var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions() { TenantId = config["AZURE_TENANT_ID"] });

    var url = new Uri(endpoint.GetAzureOpenAIResponsesEndpoint());
    var chatClient = new AzureOpenAIClient(url, credential)
                        .GetResponsesClient()
                        .AsIChatClient(model);
    ```

   Vamos analisar o código.

   - `new DefaultAzureCredential(...)`: Isso faz login no Azure sem uma chave de API. Usa os detalhes de login do Azure CLI ou Azure Developer CLI na sua máquina local, e Managed Identity quando o aplicativo é implantado no Azure.
   - `endpoint.GetAzureOpenAIResponsesEndpoint()`: Isso converte o endpoint do projeto Microsoft Foundry para o endpoint OpenAI do Microsoft Foundry porque o SDK do Microsoft Foundry atualmente não suporta o padrão handoff.
   - `new AzureOpenAIClient(url, credential)`: Isso conecta à instância do Azure OpenAI usando o endpoint e os detalhes de login e o converte em uma instância `IChatClient`.

1. No mesmo arquivo, encontre o comentário `// Register all agents passed from Aspire` e adicione o código logo abaixo dele. Isso obtém os detalhes dos agentes do projeto Microsoft Foundry e os registra no contêiner IoC como serviços singleton.

    ```csharp
    // Registrar todos os agentes passados pelo Aspire
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

   Vamos analisar o código.

   - Já conhecemos a lista de agentes, mas sabemos apenas seus nomes. Portanto, o código executa o loop `foreach` para cada agente.
   - `await File.ReadAllTextAsync(...)`: Isso importa o arquivo de instrução do agente.
   - `new ChatClientAgent(chatClient, instructions, name)`: Usando as informações de cada agente, a instrução e a instância `IChatClient`, isso cria uma instância de agente.
   - `builder.Services.AddKeyedSingleton<AIAgent>(name, agent)`: Isso registra a instância do agente como um serviço singleton.

1. No mesmo arquivo, encontre o comentário `// Build a handoff workflow pattern with the agents registered` e adicione o código logo abaixo dele.

    ```csharp
    // Construir um padrão de fluxo de trabalho handoff com os agentes registrados
    builder.AddWorkflow("publisher", (sp, key) =>
    {
        var triage = sp.GetRequiredKeyedService<AIAgent>("triage-agent");
        var generalSupport = sp.GetRequiredKeyedService<AIAgent>("general-support-agent");
        var networkSpecialist = sp.GetRequiredKeyedService<AIAgent>("network-specialist-agent");
        var warranty = sp.GetRequiredKeyedService<AIAgent>("warranty-agent");

        var specialists = new[] { generalSupport, networkSpecialist, warranty };

        var workflow = AgentWorkflowBuilder.CreateHandoffBuilderWith(triage)
            // A triagem pode transferir para qualquer especialista
            .WithHandoffs(triage, specialists)
            // Cada especialista pode transferir para outros especialistas
            .WithHandoffs(generalSupport, [networkSpecialist, warranty])
            .WithHandoffs(networkSpecialist, [generalSupport, warranty])
            .WithHandoffs(warranty, [generalSupport, networkSpecialist])
            // Todos os especialistas retornam para a triagem
            .WithHandoffs(specialists, triage, "Hand back to triage when the issue is resolved or needs further routing")
            .Build();

        // HandoffWorkflowBuilder.Build() não define o nome do fluxo de trabalho.
        // Define via reflexão para que a validação de nome do AddWorkflow funcione.
        typeof(Workflow).GetProperty("Name")!.SetValue(workflow, key);

        return workflow;
    }).AddAsAIAgent("publisher");
    ```

   Vamos analisar o código.

   - `var specialists = new[] { generalSupport, networkSpecialist, warranty };`: Isso define a lista de agentes especialistas. O agente de triagem é o ponto de partida que redireciona a solicitação do usuário para um dos agentes especialistas.
   - `builder.AddWorkflow("publisher", ...).AddAsAIAgent("publisher")`: Isso adiciona o fluxo de trabalho multi-agente como outra instância de agente chamada `publisher` e o registra como singleton.
   - `AgentWorkflowBuilder.CreateHandoffBuilderWith(triage)`: Este é o construtor de fluxo de trabalho handoff com o agente de triagem.
   - `.WithHandoffs(triage, specialists)`: Isso define a transferência do agente de triagem para os agentes especialistas.
   - `.WithHandoffs(generalSupport, [networkSpecialist, warranty])`: Isso define a transferência do agente de suporte geral para os outros agentes especialistas.
   - `.WithHandoffs(networkSpecialist, [generalSupport, warranty])`: Isso define a transferência do agente especialista em rede para os outros agentes especialistas.
   - `.WithHandoffs(warranty, [generalSupport, networkSpecialist])`: Isso define a transferência do agente de garantia para os outros agentes especialistas.
   - `.WithHandoffs(specialists, triage, "Hand back to triage when the issue is resolved or needs further routing")`: Isso define a transferência de todos os agentes especialistas de volta para o agente de triagem quando o problema é resolvido ou precisa de encaminhamento adicional.
   - `typeof(Workflow).GetProperty("Name")!.SetValue(workflow, key);`: Isso injeta o nome do fluxo de trabalho, que é uma solução temporária.

1. No mesmo arquivo, encontre o comentário `// Map AGUI to the publisher workflow agent` e adicione o código logo abaixo dele. O fluxo de trabalho é exposto como um endpoint de API em `ag-ui` para que a interface web frontend possa se comunicar com este aplicativo de serviço de agente backend.

    ```csharp
    // Mapear AGUI para o agente de fluxo de trabalho publisher
    app.MapAGUI(
        pattern: "ag-ui",
        aiAgent: app.Services.GetRequiredKeyedService<AIAgent>("publisher")
                             .CreateFixedAgent()
    );
    ```

   Note que `.CreateFixedAgent()` é uma solução temporária até que o fluxo de saída seja tratado corretamente.

## Implementar o padrão handoff na interface web frontend

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

   ![Painel do Aspire](../../../docs/images/step-03-image-01.png)

   Clique no aplicativo de serviço de agente backend.

1. Quando a página Dev UI abrir, altere o agente para `publisher` e veja que o agente de triagem distribui a solicitação para os outros agentes.

   ![Microsoft Agent Framework Dev UI - Padrão handoff](../../../docs/images/step-03-image-02.png)

1. Envie qualquer solicitação.

   ![Microsoft Agent Framework Dev UI - Enviar solicitação](../../../docs/images/step-03-image-03.png)

   Veja o resultado e como o fluxo de trabalho progride no lado esquerdo da tela.

   ![Microsoft Agent Framework Dev UI - Execução do fluxo de trabalho](../../../docs/images/step-03-image-04.png)

1. Volte ao painel do Aspire e clique no aplicativo de interface web.

   ![Painel do Aspire](../../../docs/images/step-03-image-05.png)

1. Envie qualquer solicitação.

   ![Microsoft Agent Framework Chat UI - Enviar solicitação](../../../docs/images/step-03-image-06.png)

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

   ![Microsoft Agent Framework no Azure Container Apps - Enviar solicitação](../../../docs/images/step-03-image-07.png)

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

Parabéns! 🎉 Você acabou de concluir o terceiro cenário de orquestração multi-agente &ndash; o padrão handoff. Vamos prosseguir!

👈 [02: Concurrent Pattern](./02-concurrent-pattern.md) | [04: Group Chat Pattern](./04-group-chat-pattern.md) 👉
