# 01 Patrón Secuencial

En un patrón secuencial, los agentes trabajan uno tras otro en un pipeline definido, donde la salida de cada agente alimenta al siguiente. Este enfoque funciona bien para tareas que siguen una progresión natural, como flujos de trabajo de creación de contenido, transformaciones de datos por etapas o análisis paso a paso.

## Escenario

Está escribiendo una publicación técnica de blog con agentes: agente de investigación, agente de esquema, agente de redacción y agente de edición.

<div>
  <img src="../../../docs/images/01-sequential-pattern-architecture.png" alt="Arquitectura - Patrón Secuencial" width="640" />
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
    bash $REPOSITORY_ROOT/scripts/setup.sh --session 01-sequential-pattern
    ```

    ```powershell
    # PowerShell
    & $REPOSITORY_ROOT/scripts/setup.ps1 -Session 01-sequential-pattern
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
          "Name": "research-agent",
          "Version": "1"
        },
        {
          "Name": "outliner-agent",
          "Version": "1"
        },
        {
          "Name": "writer-agent",
          "Version": "1"
        },
        {
          "Name": "editor-agent",
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
    editor-agent
    writer-agent
    outliner-agent
    research-agent
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
          "Name": "research-agent",
          "Version": "1"
        },
        {
          "Name": "outliner-agent",
          "Version": "1"
        },
        {
          "Name": "writer-agent",
          "Version": "1"
        },
        {
          "Name": "editor-agent",
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

## Implementar el patrón secuencial en el servicio de agente del backend

1. Asegúrese de estar en el directorio `workshop`.

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. Abra `src/MultiAgentWorkshop.Agent/Program.cs`, busque el comentario `// Create AIProjectClient instance with EntraID authentication` y agregue el código justo debajo. Esto se conecta al proyecto de Microsoft Foundry.

    ```csharp
    // Create AIProjectClient instance with EntraID authentication
    var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions() { TenantId = config["AZURE_TENANT_ID"] });
    var projectClient = new AIProjectClient(endpoint: new Uri(endpoint), tokenProvider: credential);
    ```

   Analicemos el código.

   - `new DefaultAzureCredential(...)`: Esto inicia sesión en Azure sin una clave API. Utiliza los datos de inicio de sesión de Azure CLI o Azure Developer CLI en su máquina local, y Managed Identity cuando la aplicación se despliega en Azure.
   - `new AIProjectClient(endpoint, credential)`: Esto se conecta a la instancia del proyecto de Microsoft Foundry usando el endpoint y los datos de inicio de sesión.

1. En el mismo archivo, busque el comentario `// Register all agents passed from Aspire` y agregue el código justo debajo. Esto obtiene los detalles de los agentes del proyecto de Microsoft Foundry y los registra en el contenedor IoC como servicios singleton.

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

   Analicemos el código.

   - Ya conocemos la lista de agentes pero solo sabemos sus nombres. Por lo tanto, el código ejecuta el bucle `foreach` para cada agente.
   - `new AgentReference(name, version)`: Usando la información de cada agente, esto crea una instancia de referencia.
   - `projectClient.AsAIAgent(reference, factory)`: Esto se conecta al agente real usando los detalles de referencia.
   - `builder.Services.AddKeyedSingleton<AIAgent>(name, agent)`: Esto registra la instancia del agente como un servicio singleton.

   > **NOTA**: Puede notar la clase `AgentRecordShimChatClient`. Es una solución temporal para una incompatibilidad de versiones entre Microsoft Agent Framework y el SDK de Microsoft Foundry, que será eliminada pronto.

1. En el mismo archivo, busque el comentario `// Build a sequential workflow pattern with the agents registered` y agregue el código justo debajo.

    ```csharp
    // Build a sequential workflow pattern with the agents registered
    builder.AddWorkflow("publisher", (sp, key) => AgentWorkflowBuilder.BuildSequential(
        workflowName: key,
        agents: [.. agents.Select(a => sp.GetRequiredKeyedService<AIAgent>(a.Name))]
    )).AddAsAIAgent("publisher");
    ```

   Analicemos el código.

   - `builder.AddWorkflow("publisher", ...).AddAsAIAgent("publisher")`: Esto agrega el flujo de trabajo multi-agente como otra instancia de agente llamada `publisher` y lo registra como singleton.
   - `AgentWorkflowBuilder.BuildSequential(...)`: Este es el constructor del flujo de trabajo secuencial que usa el mismo nombre, `publisher`.

     Note que agrega múltiples agentes de los servicios previamente registrados en el orden declarado por el arreglo `agents`.

1. En el mismo archivo, busque el comentario `// Map AGUI to the publisher workflow agent` y agregue el código justo debajo. El flujo de trabajo se expone como un endpoint API en `ag-ui` para que la interfaz web del frontend pueda comunicarse con esta aplicación del servicio de agente del backend.

    ```csharp
    // Map AGUI to the publisher workflow agent
    app.MapAGUI(
        pattern: "ag-ui",
        aiAgent: app.Services.GetRequiredKeyedService<AIAgent>("publisher")
    );
    ```

## Implementar el patrón secuencial en la interfaz web del frontend

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

   ![Panel de Aspire](../../../docs/images/step-01-image-01.png)

   Haga clic en la aplicación del servicio de agente del backend.

1. Cuando se abra la página Dev UI, cambie el agente a `publisher` y haga clic en el botón "Configure & Run".

   ![Microsoft Agent Framework Dev UI - Patrón secuencial](../../../docs/images/step-01-image-02.png)

1. Envíe cualquier solicitud.

   ![Microsoft Agent Framework Dev UI - Enviar solicitud](../../../docs/images/step-01-image-03.png)

   Observe el resultado y cómo el flujo de trabajo progresa en el lado izquierdo de la pantalla.

   ![Microsoft Agent Framework Dev UI - Ejecución del flujo de trabajo](../../../docs/images/step-01-image-04.png)

1. Regrese al panel de Aspire y haga clic en la aplicación de la interfaz web.

   ![Panel de Aspire](../../../docs/images/step-01-image-05.png)

1. Envíe cualquier solicitud.

   ![Microsoft Agent Framework Chat UI - Enviar solicitud](../../../docs/images/step-01-image-06.png)

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

   ![Microsoft Agent Framework en Azure Container Apps - Enviar solicitud](../../../docs/images/step-01-image-07.png)

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

¡Felicitaciones! 🎉 Acaba de completar el primer escenario de orquestación multi-agente: el patrón secuencial. ¡Continuemos!

👈 [00: Configuración](./00-setup.md) | [02: Patrón Concurrente](./02-concurrent-pattern.md) 👉
