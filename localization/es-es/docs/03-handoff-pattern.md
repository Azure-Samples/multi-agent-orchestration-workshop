# 03 Patrón Handoff

En un patrón handoff, los agentes pasan dinámicamente el control entre sí según el contexto de la conversación. Un agente de triaje recibe la solicitud inicial y la dirige al especialista más adecuado para manejarla. Los especialistas también pueden redirigir a otros cuando el problema cruza dominios. Esto funciona bien para escenarios como soporte de TI, servicio al cliente o cualquier flujo de trabajo donde se necesita diferente experiencia en diferentes etapas.

## Escenario

Usted trabaja para un equipo de soporte de TI con agentes: agente de soporte general, agente especialista en redes, agente de garantía y agente de triaje.

<div>
  <img src="../../../docs/images/03-handoff-pattern-architecture.png" alt="Arquitectura - Patrón Handoff" width="640" />
</div>

## Obtener la raíz del repositorio

1. Obtenga la variable `$REPOSITORY_ROOT` primero.

    ```bash
    # zsh/bash
    REPOSITORY_ROOT=$(git rev-parse --show-toplevel)
    ```

    ```powershell
    # PowerShell
    $REPOSITORY_ROOT = git rev-parse --show-toplevel
    ```

## Copiar el proyecto inicial

1. Si ya tiene el directorio `workshop`, renómbrelo o elimínelo primero.

1. Ejecute el script de configuración para copiar el proyecto inicial al directorio `workshop`.

    ```bash
    # zsh/bash
    bash $REPOSITORY_ROOT/scripts/setup.sh --session 03-handoff-pattern
    ```

    ```powershell
    # PowerShell
    & $REPOSITORY_ROOT/scripts/setup.ps1 -Session 03-handoff-pattern
    ```

## Desplegar agentes

1. Asegúrese de estar en el directorio `workshop`.

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. Abra `src/MultiAgentWorkshop.PromptAgent/appsettings.json`, busque la línea de comentario `// Add agents` y agregue la propiedad `Agents` debajo de ella.

    ```jsonc
    {
      ...
      // Add agents
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

1. Navegue al directorio `resources-foundry`.

    ```bash
    pushd resources-foundry
    ```

1. Ejecute el siguiente comando para aprovisionar y desplegar los agentes definidos anteriormente en Microsoft Foundry.

    ```bash
    azd up
    ```

   Durante el aprovisionamiento, se le pedirá que ingrese un nombre de entorno, una suscripción de Azure y una ubicación.

1. Una vez que el aprovisionamiento y el despliegue hayan finalizado, ejecute el siguiente comando para confirmar que los agentes se han desplegado correctamente.

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

   Debería ver los cuatro nombres de agentes.

    ```text
    warranty-agent
    network-specialist-agent
    general-support-agent
    triage-agent
    ```

1. Navegue de vuelta al directorio del taller.

    ```bash
    popd
    ```

## Configurar la orquestación de Aspire

1. Asegúrese de estar en el directorio `workshop`.

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. Verifique que toda la información necesaria de los agentes haya sido registrada.

    ```bash
    dotnet user-secrets --project ./src/MultiAgentWorkshop.AppHost list
    ```

   Debería ver los valores de `AZURE_TENANT_ID`, `FOUNDRY_NAME`, `FOUNDRY_PROJECT_NAME`, `FOUNDRY_RESOURCE_GROUP` y `Foundry:Project:Endpoint`.

1. Abra `src/MultiAgentWorkshop.AppHost/appsettings.json`, busque la línea de comentario `// Add agents` y agregue la propiedad `Agents` debajo de ella.

    ```jsonc
    {
      ...
      // Add agents
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

1. Abra `src/MultiAgentWorkshop.AppHost/AppHost.cs`, busque el comentario `// Add resource for Microsoft Foundry` y agregue el código justo debajo. Esto agrega los detalles de conexión del proyecto de Microsoft Foundry.

    ```csharp
    // Add resource for Microsoft Foundry
    var foundry = builder.AddFoundry("foundry");
    ```

   Analicemos el código.

   - `builder.AddFoundry("foundry")`: Esto agrega los detalles de conexión de Microsoft Foundry a través de un recurso personalizado, `FoundryResource`. Si desea saber más sobre el recurso personalizado de Aspire, visite [Create custom hosting integrations](https://aspire.dev/integrations/custom-integrations/hosting-integrations/).

1. En el mismo archivo, busque el comentario `// Add resource for agents on Microsoft Foundry` y agregue el código justo debajo. Esto expone la lista de detalles de los agentes a la aplicación que los referencia.

    ```csharp
    // Add resource for agents on Microsoft Foundry
    var agents = builder.AddAgents("agents");
    ```

   Analicemos el código.

   - `builder.AddAgents("agents")`: Esto agrega la lista de detalles de los agentes a través de un recurso personalizado, `AgentResource`. Si desea saber más sobre el recurso personalizado de Aspire, visite [Create custom hosting integrations](https://aspire.dev/integrations/custom-integrations/hosting-integrations/).

1. En el mismo archivo, busque el comentario `// Add backend agent service` y agregue el código justo debajo. Esto define el servicio de agente del backend que referencia el recurso `foundry` — todos los detalles de conexión de Microsoft Foundry se pasan a la aplicación del servicio de agente del backend.

    ```csharp
    // Add backend agent service
    var agent = builder.AddProject<MultiAgentWorkshop_Agent>("agent")
                       .WithReference(foundry);
    ```

   Analicemos el código.

   - `builder.AddProject<MultiAgentWorkshop_Agent>("agent")`: Esto agrega la aplicación del servicio de agente del backend como un proyecto .NET.
   - `.WithReference(foundry)`: Esto referencia el recurso foundry creado anteriormente, que pasa los detalles de conexión de Microsoft Foundry a la aplicación del servicio de agente del backend.

1. En el mismo archivo, busque el comentario `// Add frontend web UI` y agregue el código justo debajo. Esto define la interfaz web del frontend que referencia tanto los recursos `agents` como `agent` — los detalles de los agentes y los detalles de conexión del backend se pasan a la aplicación de la interfaz web del frontend.

    ```csharp
    // Add frontend web UI
    var webUI = builder.AddProject<MultiAgentWorkshop_WebUI>("webui")
                       .WithExternalHttpEndpoints()
                       .WithReference(agents)
                       .WithReference(agent)
                       .WaitFor(agent);
    ```

   Analicemos el código.

   - `builder.AddProject<MultiAgentWorkshop_WebUI>("webui")`: Esto agrega la aplicación de la interfaz web del frontend como un proyecto .NET.
   - `.WithExternalHttpEndpoints()`: Esto expone esta aplicación de interfaz web del frontend a Internet, haciéndola accesible públicamente.
   - `.WithReference(agents)`: Esto referencia el recurso de agentes creado anteriormente, que pasa la lista de agentes a la aplicación de la interfaz web del frontend.
   - `.WithReference(agent)`: Esto referencia la aplicación del servicio de agente del backend, que pasa los detalles de conexión a la aplicación de la interfaz web del frontend.

## Implementar el patrón handoff en el servicio de agente del backend

1. Asegúrese de estar en el directorio `workshop`.

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. Abra `src/MultiAgentWorkshop.Agent/Program.cs`, busque el comentario `// Create AzureOpenAIClient instance with EntraID authentication` y agregue el código justo debajo. Esto se conecta al proyecto de Microsoft Foundry.

    ```csharp
    // Create AzureOpenAIClient instance with EntraID authentication
    var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions() { TenantId = config["AZURE_TENANT_ID"] });

    var url = new Uri(endpoint.GetAzureOpenAIResponsesEndpoint());
    var chatClient = new AzureOpenAIClient(url, credential)
                        .GetResponsesClient()
                        .AsIChatClient(model);
    ```

   Analicemos el código.

   - `new DefaultAzureCredential(...)`: Esto inicia sesión en Azure sin una clave API. Utiliza los datos de inicio de sesión de Azure CLI o Azure Developer CLI en su máquina local, y Managed Identity cuando la aplicación se despliega en Azure.
   - `endpoint.GetAzureOpenAIResponsesEndpoint()`: Esto convierte el endpoint del proyecto de Microsoft Foundry al endpoint de Microsoft Foundry OpenAI porque el SDK de Microsoft Foundry actualmente no admite el patrón handoff.
   - `new AzureOpenAIClient(url, credential)`: Esto se conecta a la instancia de Azure OpenAI usando el endpoint y los datos de inicio de sesión y lo convierte a una instancia de `IChatClient`.

1. En el mismo archivo, busque el comentario `// Register all agents passed from Aspire` y agregue el código justo debajo. Esto obtiene los detalles de los agentes del proyecto de Microsoft Foundry y los registra en el contenedor IoC como servicios singleton.

    ```csharp
    // Register all agents passed from Aspire
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

   Analicemos el código.

   - Ya conocemos la lista de agentes pero solo sabemos sus nombres. Por lo tanto, el código ejecuta el bucle `foreach` para cada agente.
   - `await File.ReadAllTextAsync(...)`: Esto importa el archivo de instrucciones del agente.
   - `new ChatClientAgent(chatClient, instructions, name)`: Usando la información de cada agente, las instrucciones y la instancia de `IChatClient`, esto crea una instancia del agente.
   - `builder.Services.AddKeyedSingleton<AIAgent>(name, agent)`: Esto registra la instancia del agente como un servicio singleton.

1. En el mismo archivo, busque el comentario `// Build a handoff workflow pattern with the agents registered` y agregue el código justo debajo.

    ```csharp
    // Build a handoff workflow pattern with the agents registered
    builder.AddWorkflow("publisher", (sp, key) =>
    {
        var triage = sp.GetRequiredKeyedService<AIAgent>("triage-agent");
        var generalSupport = sp.GetRequiredKeyedService<AIAgent>("general-support-agent");
        var networkSpecialist = sp.GetRequiredKeyedService<AIAgent>("network-specialist-agent");
        var warranty = sp.GetRequiredKeyedService<AIAgent>("warranty-agent");

        var specialists = new[] { generalSupport, networkSpecialist, warranty };

        var workflow = AgentWorkflowBuilder.CreateHandoffBuilderWith(triage)
            // El triaje puede transferir a cualquier especialista
            .WithHandoffs(triage, specialists)
            // Cada especialista puede transferir a otros especialistas
            .WithHandoffs(generalSupport, [networkSpecialist, warranty])
            .WithHandoffs(networkSpecialist, [generalSupport, warranty])
            .WithHandoffs(warranty, [generalSupport, networkSpecialist])
            // Todos los especialistas transfieren de vuelta al triaje
            .WithHandoffs(specialists, triage, "Hand back to triage when the issue is resolved or needs further routing")
            .Build();

        // HandoffWorkflowBuilder.Build() doesn't set the workflow name.
        // Set it via reflection so AddWorkflow's name validation passes.
        typeof(Workflow).GetProperty("Name")!.SetValue(workflow, key);

        return workflow;
    }).AddAsAIAgent("publisher");
    ```

   Analicemos el código.

   - `var specialists = new[] { generalSupport, networkSpecialist, warranty };`: Esto define la lista de agentes especialistas. El agente de triaje es el punto de partida que redirige la solicitud del usuario a uno de los agentes especialistas.
   - `builder.AddWorkflow("publisher", ...).AddAsAIAgent("publisher")`: Esto agrega el flujo de trabajo multi-agente como otra instancia de agente llamada `publisher` y lo registra como singleton.
   - `AgentWorkflowBuilder.CreateHandoffBuilderWith(triage)`: Este es el constructor del flujo de trabajo handoff con el agente de triaje.
   - `.WithHandoffs(triage, specialists)`: Esto define la transferencia del agente de triaje a los agentes especialistas.
   - `.WithHandoffs(generalSupport, [networkSpecialist, warranty])`: Esto define la transferencia del agente de soporte general a los otros agentes especialistas.
   - `.WithHandoffs(networkSpecialist, [generalSupport, warranty])`: Esto define la transferencia del agente especialista en redes a los otros agentes especialistas.
   - `.WithHandoffs(warranty, [generalSupport, networkSpecialist])`: Esto define la transferencia del agente de garantía a los otros agentes especialistas.
   - `.WithHandoffs(specialists, triage, "Hand back to triage when the issue is resolved or needs further routing")`: Esto define la transferencia de todos los agentes especialistas de vuelta al agente de triaje cuando el problema se resuelve o necesita redirección adicional.
   - `typeof(Workflow).GetProperty("Name")!.SetValue(workflow, key);`: Esto inyecta el nombre del flujo de trabajo, que es una solución temporal.

1. En el mismo archivo, busque el comentario `// Map AGUI to the publisher workflow agent` y agregue el código justo debajo. El flujo de trabajo se expone como un endpoint API en `ag-ui` para que la interfaz web del frontend pueda comunicarse con esta aplicación del servicio de agente del backend.

    ```csharp
    // Map AGUI to the publisher workflow agent
    app.MapAGUI(
        pattern: "ag-ui",
        aiAgent: app.Services.GetRequiredKeyedService<AIAgent>("publisher")
                             .CreateFixedAgent()
    );
    ```

   Note que `.CreateFixedAgent()` es una solución temporal hasta que el flujo de salida se maneje correctamente.

## Implementar el patrón handoff en la interfaz web del frontend

1. Asegúrese de estar en el directorio `workshop`.

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. Abra `src/MultiAgentWorkshop.WebUI/Program.cs`, busque el comentario `// Register all agents passed from Aspire` y agregue el código justo debajo. Esto registra todos los detalles de los agentes para que la interfaz web sepa qué agente está respondiendo.

    ```csharp
    // Register all agents passed from Aspire
    builder.Services.AddSingleton(agents);
    ```

1. En el mismo archivo, busque el comentario `// Register the backend agent service as an HTTP client` y agregue el código justo debajo. Aspire ya proporciona a la aplicación de la interfaz web del frontend los detalles de conexión del servicio de agente del backend.

    ```csharp
    // Register the backend agent service as an HTTP client
    builder.Services.AddHttpClient("agent", client =>
    {
        client.BaseAddress = new Uri("https+http://agent");
    });
    ```

1. En el mismo archivo, busque el comentario `// Register AGUI client` y agregue el código justo debajo. Usando este cliente AGUI, la aplicación de la interfaz web del frontend se comunica con la aplicación del servicio de agente del backend a través del endpoint `ag-ui`.

    ```csharp
    // Register AGUI client
    builder.Services.AddChatClient(sp => new AGUIChatClient(
        httpClient: sp.GetRequiredService<IHttpClientFactory>().CreateClient("agent"),
        endpoint: "ag-ui")
    );
    ```

## Ejecutar Aspire

1. Asegúrese de estar en el directorio `workshop`.

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. Asegúrese de haber iniciado sesión en Azure usando tanto Azure CLI como Azure Developer CLI. Si no está seguro, siga [este paso](./00-setup.md#iniciar-sesión-en-azure) nuevamente.

1. Ejecute el siguiente comando para iniciar todas las aplicaciones a través de Aspire.

    ```bash
    dotnet watch run --project ./src/MultiAgentWorkshop.AppHost
    ```

1. El panel de Aspire se abre automáticamente.

   ![Panel de Aspire](../../../docs/images/step-03-image-01.png)

   Haga clic en la aplicación del servicio de agente del backend.

1. Cuando se abra la página Dev UI, cambie el agente a `publisher` y observe que el agente de triaje distribuye la solicitud a los otros agentes.

   ![Microsoft Agent Framework Dev UI - Patrón handoff](../../../docs/images/step-03-image-02.png)

1. Envíe cualquier solicitud.

   ![Microsoft Agent Framework Dev UI - Enviar solicitud](../../../docs/images/step-03-image-03.png)

   Observe el resultado y cómo el flujo de trabajo progresa en el lado izquierdo de la pantalla.

   ![Microsoft Agent Framework Dev UI - Ejecución del flujo de trabajo](../../../docs/images/step-03-image-04.png)

1. Regrese al panel de Aspire y haga clic en la aplicación de la interfaz web.

   ![Panel de Aspire](../../../docs/images/step-03-image-05.png)

1. Envíe cualquier solicitud.

   ![Microsoft Agent Framework Chat UI - Enviar solicitud](../../../docs/images/step-03-image-06.png)

   Observe el resultado.

1. Presione `Ctrl`+`C` para terminar todas las aplicaciones en ejecución.

## Desplegar en Azure

1. Asegúrese de estar en el directorio `workshop`.

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. Ejecute el siguiente comando para aprovisionar y desplegar tanto la interfaz web del frontend como las aplicaciones del servicio de agente del backend en Azure.

    ```bash
    azd up
    ```

   Durante el aprovisionamiento, se le pedirá que ingrese un nombre de entorno, una suscripción de Azure y una ubicación.

1. Una vez completado, verá la URL de la aplicación de la interfaz web en la pantalla del terminal. Ábrala en su navegador web y envíe una solicitud.

   ![Microsoft Agent Framework en Azure Container Apps - Enviar solicitud](../../../docs/images/step-03-image-07.png)

   Observe el resultado.

1. Una vez que todo esté listo, elimine todas las aplicaciones y agentes de Azure.

    ```bash
    # Eliminar tanto la interfaz web como las aplicaciones del servicio de agente.
    azd down --purge --force

    # Eliminar todos los agentes y el recurso de Microsoft Foundry.
    cd resources-foundry
    azd down --purge --force
    ```

---

¡Felicitaciones! 🎉 Acaba de completar el tercer escenario de orquestación multi-agente: el patrón handoff. ¡Continuemos!

👈 [02: Patrón Concurrente](./02-concurrent-pattern.md) | [04: Patrón Group Chat](./04-group-chat-pattern.md) 👉
