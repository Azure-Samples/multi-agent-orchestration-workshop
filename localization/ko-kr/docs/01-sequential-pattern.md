# 01 Sequential 패턴

Sequential 패턴에서는 에이전트가 정의된 파이프라인에서 하나씩 차례로 작업하며, 각 에이전트의 출력이 다음 에이전트의 입력으로 전달됩니다. 이 접근 방식은 콘텐츠 생성 워크플로, 단계별 데이터 변환 또는 단계적 분석과 같이 자연스러운 흐름을 따르는 작업에 적합합니다.

## 시나리오

에이전트를 사용하여 기술 블로그 게시물을 작성합니다 &ndash; 리서치 에이전트, 아웃라이너 에이전트, 작성 에이전트, 편집 에이전트.

<div>
  <img src="../../../docs/images/01-sequential-pattern-architecture.png" alt="아키텍처 - Sequential 패턴" width="640" />
</div>

## 저장소 루트 가져오기

1. 먼저 `$REPOSITORY_ROOT` 변수를 가져옵니다.

    ```bash
    # zsh/bash
    REPOSITORY_ROOT=$(git rev-parse --show-toplevel)
    ```

    ```powershell
    # PowerShell
    $REPOSITORY_ROOT = git rev-parse --show-toplevel
    ```

## 시작 프로젝트 복사하기

1. 이미 `workshop` 디렉터리가 있다면 먼저 이름을 변경하거나 삭제합니다.

1. 설정 스크립트를 실행하여 시작 프로젝트를 `workshop` 디렉터리에 복사합니다.

    ```bash
    # zsh/bash
    bash $REPOSITORY_ROOT/scripts/setup.sh --session 01-sequential-pattern
    ```

    ```powershell
    # PowerShell
    & $REPOSITORY_ROOT/scripts/setup.ps1 -Session 01-sequential-pattern
    ```

## 에이전트 배포하기

1. `workshop` 디렉터리에 있는지 확인합니다.

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. `src/MultiAgentWorkshop.PromptAgent/appsettings.json`을 열고, `// Add agents` 주석 줄을 찾아 그 아래에 `Agents` 속성을 추가합니다.

    ```jsonc
    {
      ...
      // 에이전트 추가
      "Agents": [
        "research-agent",
        "outliner-agent",
        "writer-agent",
        "editor-agent"
      ]
      ...
    }
    ```

1. `resources-foundry` 디렉터리로 이동합니다.

    ```bash
    pushd resources-foundry
    ```

1. 다음 명령어를 실행하여 위에서 정의한 에이전트를 Microsoft Foundry에 프로비저닝하고 배포합니다.

    ```bash
    azd up
    ```

   프로비저닝 중에 환경 이름, Azure 구독 및 위치를 입력하라는 메시지가 표시됩니다.

1. 프로비저닝과 배포가 완료되면 다음 명령어를 실행하여 에이전트가 성공적으로 배포되었는지 확인합니다.

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

   네 개의 에이전트 이름이 표시되어야 합니다.

    ```text
    editor-agent
    writer-agent
    outliner-agent
    research-agent
    ```

1. workshop 디렉터리로 돌아갑니다.

    ```bash
    popd
    ```

## Aspire 오케스트레이션 구성하기

1. `workshop` 디렉터리에 있는지 확인합니다.

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. 필요한 모든 에이전트 정보가 기록되었는지 확인합니다.

    ```bash
    dotnet user-secrets --project ./src/MultiAgentWorkshop.AppHost list
    ```

   `AZURE_TENANT_ID`, `FOUNDRY_NAME`, `FOUNDRY_PROJECT_NAME`, `FOUNDRY_RESOURCE_GROUP`, `Foundry:Project:Endpoint` 값이 표시되어야 합니다.

1. `src/MultiAgentWorkshop.AppHost/appsettings.json`을 열고, `// Add agents` 주석 줄을 찾아 그 아래에 `Agents` 속성을 추가합니다.

    ```jsonc
    {
      ...
      // 에이전트 추가
      "Agents": [
        "research-agent",
        "outliner-agent",
        "writer-agent",
        "editor-agent"
      ]
      ...
    }
    ```

1. `src/MultiAgentWorkshop.AppHost/AppHost.cs`를 열고, `// Add resource for Microsoft Foundry` 주석을 찾아 바로 아래에 코드를 추가합니다. 이 코드는 Microsoft Foundry 프로젝트 연결 정보를 추가합니다.

    ```csharp
    // Microsoft Foundry 리소스 추가
    var foundry = builder.AddFoundryConnectionString("foundry");
    ```

   코드를 분석해 봅시다.

   - `builder.AddFoundryConnectionString("foundry")`: 확장 메서드 `AddFoundryConnectionString()`을 통해 Microsoft Foundry 연결 문자열을 추가합니다.

1. 같은 파일에서 `// Add resource for agents on Microsoft Foundry` 주석을 찾아 바로 아래에 코드를 추가합니다. 이 코드는 에이전트 세부 정보 목록을 참조하는 애플리케이션에 노출합니다.

    ```csharp
    // Microsoft Foundry의 에이전트 리소스 추가
    var agents = builder.AddFoundryAgentsConnectionString("agents");
    ```

   코드를 분석해 봅시다.

   - `builder.AddFoundryAgentsConnectionString("agents")`: 확장 메서드 `AddFoundryAgentsConnectionString()`을 통해 에이전트 세부 정보 목록을 추가합니다.

1. 같은 파일에서 `// Add backend agent service` 주석을 찾아 바로 아래에 코드를 추가합니다. 이 코드는 `foundry` 리소스를 참조하는 백엔드 에이전트 서비스를 정의합니다 &ndash; 모든 Microsoft Foundry 연결 정보가 백엔드 에이전트 서비스 앱으로 전달됩니다.

    ```csharp
    // 백엔드 에이전트 서비스 추가
    var agent = builder.AddProject<Projects.MultiAgentWorkshop_Agent>("agent")
                       .WithReference(foundry)
                       .WaitFor(foundry);
    ```

   코드를 분석해 봅시다.

   - `builder.AddProject<Projects.MultiAgentWorkshop_Agent>("agent")`: 백엔드 에이전트 서비스 앱을 .NET 프로젝트로 추가합니다.
   - `.WithReference(foundry)`: 위에서 생성한 foundry 연결 문자열 리소스를 참조하여 Microsoft Foundry 연결 정보를 백엔드 에이전트 서비스 앱으로 전달합니다.
   - `.WaitFor(foundry)`: 종속성 활성화 순서를 유지하여 `foundry` 연결 리소스가 실행 중일 때까지 이 `agent` 프로젝트 리소스가 활성화되지 않도록 합니다.

1. 같은 파일에서 `// Add frontend web UI` 주석을 찾아 바로 아래에 코드를 추가합니다. 이 코드는 `agents`와 `agent` 리소스를 모두 참조하는 프론트엔드 웹 UI를 정의합니다 &ndash; 에이전트 세부 정보와 백엔드 연결 정보가 모두 프론트엔드 웹 UI 앱으로 전달됩니다.

    ```csharp
    // 프론트엔드 웹 UI 추가
    var webUI = builder.AddProject<Projects.MultiAgentWorkshop_WebUI>("webui")
                       .WithExternalHttpEndpoints()
                       .WithReference(agents)
                       .WithReference(agent)
                       .WaitFor(agents)
                       .WaitFor(agent);
    ```

   코드를 분석해 봅시다.

   - `builder.AddProject<Projects.MultiAgentWorkshop_WebUI>("webui")`: 프론트엔드 웹 UI 앱을 .NET 프로젝트로 추가합니다.
   - `.WithExternalHttpEndpoints()`: 이 프론트엔드 웹 UI 앱을 인터넷에 노출하여 공개적으로 접근할 수 있게 합니다.
   - `.WithReference(agents)`: 위에서 생성한 에이전트 연결 문자열 리소스를 참조하여 에이전트 목록을 프론트엔드 웹 UI 앱으로 전달합니다.
   - `.WithReference(agent)`: 백엔드 에이전트 서비스 앱을 참조하여 연결 정보를 프론트엔드 웹 UI 앱으로 전달합니다.
   - `.WaitFor(agents)`: 종속성 활성화 순서를 유지하여 `agents` 연결 리소스가 실행 중일 때까지 이 `webui` 프로젝트 리소스가 활성화되지 않도록 합니다.
   - `.WaitFor(agent)`: 종속성 활성화 순서를 유지하여 `agent` 프로젝트 리소스가 실행 중일 때까지 이 `webui` 프로젝트 리소스가 활성화되지 않도록 합니다.

## 백엔드 에이전트 서비스에서 Sequential 패턴 구현하기

1. `workshop` 디렉터리에 있는지 확인합니다.

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. `src/MultiAgentWorkshop.Agent/Program.cs`를 열고, `// Create AIProjectClient instance with EntraID authentication` 주석을 찾아 바로 아래에 코드를 추가합니다. 이 코드는 Microsoft Foundry 프로젝트에 연결합니다.

    ```csharp
    // EntraID 인증으로 AIProjectClient 인스턴스 생성
    var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions() { TenantId = config["AZURE_TENANT_ID"] });
    var projectClient = new AIProjectClient(endpoint: new Uri(endpoint!), tokenProvider: credential);
    ```

   코드를 분석해 봅시다.

   - `new DefaultAzureCredential(...)`: API 키 없이 Azure에 로그인합니다. 로컬 머신에서는 Azure CLI 또는 Azure Developer CLI 로그인 정보를 사용하고, Azure에 앱이 배포되면 Managed Identity를 사용합니다.
   - `new AIProjectClient(endpoint, credential)`: 엔드포인트와 로그인 정보를 사용하여 Microsoft Foundry 프로젝트 인스턴스에 연결합니다.

1. 같은 파일에서 `// Register all agents passed from Aspire` 주석을 찾아 바로 아래에 코드를 추가합니다. 이 코드는 Microsoft Foundry 프로젝트에서 에이전트 세부 정보를 가져와 IoC 컨테이너에 싱글톤 서비스로 등록합니다.

    ```csharp
    // Aspire에서 전달된 모든 에이전트 등록
    foreach (var agentName in agentNames!)
    {
        var agentRecord = await projectClient.AgentAdministrationClient
                                             .GetAgentAsync(agentName);
        var agent = projectClient.AsAIAgent(agentRecord);

        builder.Services.AddKeyedSingleton<AIAgent>(agentName, agent);
    }
    ```

   코드를 분석해 봅시다.

   - 에이전트 목록은 이미 알고 있지만 이름만 알고 있으므로, 코드는 각 에이전트에 대해 `foreach` 루프를 실행합니다.
   - `projectClient.AgentAdministrationClient.GetAgentAsync(agentName)`: 각 에이전트의 정보를 사용하여 `ProjectsAgentRecord` 인스턴스를 생성합니다.
   - `projectClient.AsAIAgent(agentRecord)`: 참조 정보를 사용하여 실제 에이전트에 연결합니다.
   - `builder.Services.AddKeyedSingleton<AIAgent>(agentName, agent)`: 에이전트 인스턴스를 싱글톤 서비스로 등록합니다.

1. 같은 파일에서 `// Build a sequential workflow pattern with the agents registered` 주석을 찾아 바로 아래에 코드를 추가합니다.

    ```csharp
    // 등록된 에이전트로 Sequential 워크플로 패턴 구축
    builder.AddWorkflow("publisher", (sp, key) => AgentWorkflowBuilder.BuildSequential(
        workflowName: key,
        agents: [.. agentNames!.Select(name => sp.GetRequiredKeyedService<AIAgent>(name))]
    )).AddAsAIAgent("publisher");
    ```

   코드를 분석해 봅시다.

   - `builder.AddWorkflow("publisher", ...).AddAsAIAgent("publisher")`: 멀티 에이전트 워크플로를 `publisher`라는 또 다른 에이전트 인스턴스로 추가하고 싱글톤으로 등록합니다.
   - `AgentWorkflowBuilder.BuildSequential(...)`: 동일한 이름 `publisher`를 사용하는 Sequential 워크플로 빌더입니다.

     이전에 등록된 서비스에서 `agentNames` 배열에 선언된 순서대로 여러 에이전트를 추가하는 점에 주목하세요.

1. 같은 파일에서 `// Map AGUI to the publisher workflow agent` 주석을 찾아 바로 아래에 코드를 추가합니다. 워크플로는 `ag-ui` API 엔드포인트로 노출되어 프론트엔드 웹 UI가 이 백엔드 에이전트 서비스 앱과 통신할 수 있습니다.

    ```csharp
    // publisher 워크플로 에이전트에 AGUI 매핑
    app.MapAGUI(
        pattern: "ag-ui",
        aiAgent: app.Services.GetRequiredKeyedService<AIAgent>("publisher")
    );
    ```

## 프론트엔드 웹 UI에서 Sequential 패턴 구현하기

1. `workshop` 디렉터리에 있는지 확인합니다.

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. `src/MultiAgentWorkshop.WebUI/Program.cs`를 열고, `// Register all agents passed from Aspire` 주석을 찾아 바로 아래에 코드를 추가합니다. 이 코드는 웹 UI가 어떤 에이전트가 응답하는지 알 수 있도록 모든 에이전트 세부 정보를 등록합니다.

    ```csharp
    // Aspire에서 전달된 모든 에이전트 등록
    builder.Services.AddSingleton(agentNames!);
    ```

1. 같은 파일에서 `// Register the backend agent service as an HTTP client` 주석을 찾아 바로 아래에 코드를 추가합니다. Aspire가 이미 프론트엔드 웹 UI 앱에 백엔드 에이전트 서비스의 연결 정보를 제공합니다.

    ```csharp
    // 백엔드 에이전트 서비스를 HTTP 클라이언트로 등록
    builder.Services.AddHttpClient("agent", client =>
    {
        client.BaseAddress = new Uri("https+http://agent");
    });
    ```

1. 같은 파일에서 `// Register AGUI client` 주석을 찾아 바로 아래에 코드를 추가합니다. 이 AGUI 클라이언트를 사용하여 프론트엔드 웹 UI 앱이 `ag-ui` 엔드포인트를 통해 백엔드 에이전트 서비스 앱과 통신합니다.

    ```csharp
    // AGUI 클라이언트 등록
    builder.Services.AddChatClient(sp => new AGUIChatClient(
        httpClient: sp.GetRequiredService<IHttpClientFactory>().CreateClient("agent"),
        endpoint: "ag-ui")
    );
    ```

## Aspire 실행하기

1. `workshop` 디렉터리에 있는지 확인합니다.

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. Azure CLI와 Azure Developer CLI를 모두 사용하여 이미 Azure에 로그인했는지 확인합니다. 확실하지 않다면 [이 단계](./00-setup.md#azure에-로그인하기)를 다시 따라하세요.

1. 다음 명령어를 실행하여 Aspire를 통해 모든 앱을 시작합니다.

    ```bash
    dotnet watch run --project ./src/MultiAgentWorkshop.AppHost
    ```

1. Aspire 대시보드가 자동으로 열립니다.

   ![Aspire 대시보드](../../../docs/images/step-01-image-01.png)

   백엔드 에이전트 서비스 앱을 클릭합니다.

1. Dev UI 페이지가 열리면 에이전트를 `publisher`로 변경하고 "Configure & Run" 버튼을 클릭합니다.

   ![Microsoft Agent Framework Dev UI - Sequential 패턴](../../../docs/images/step-01-image-02.png)

1. 아무 요청이나 보내보세요.

   ![Microsoft Agent Framework Dev UI - 요청 보내기](../../../docs/images/step-01-image-03.png)

   결과를 확인하고 화면 왼쪽에서 워크플로가 어떻게 진행되는지 살펴보세요.

   ![Microsoft Agent Framework Dev UI - 워크플로 실행](../../../docs/images/step-01-image-04.png)

1. Aspire 대시보드로 돌아가서 웹 UI 앱을 클릭합니다.

   ![Aspire 대시보드](../../../docs/images/step-01-image-05.png)

1. 아무 요청이나 보내보세요.

   ![Microsoft Agent Framework Chat UI - 요청 보내기](../../../docs/images/step-01-image-06.png)

   결과를 확인합니다.

1. `Ctrl`+`C`를 눌러 실행 중인 모든 앱을 종료합니다.

## Azure에 배포하기

1. `workshop` 디렉터리에 있는지 확인합니다.

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. 다음 명령어를 실행하여 프론트엔드 웹 UI와 백엔드 에이전트 서비스 앱을 모두 Azure에 프로비저닝하고 배포합니다.

    ```bash
    azd up
    ```

   프로비저닝 중에 환경 이름, Azure 구독 및 위치를 입력하라는 메시지가 표시됩니다.

1. 완료되면 터미널 화면에 웹 UI 애플리케이션 URL이 표시됩니다. 웹 브라우저에서 열고 요청을 보내보세요.

   ![Azure Container Apps의 Microsoft Agent Framework - 요청 보내기](../../../docs/images/step-01-image-07.png)

   결과를 확인합니다.

1. 모든 작업이 완료되면 Azure에서 모든 앱과 에이전트를 삭제합니다.

    ```bash
    # 웹 UI와 에이전트 서비스 앱을 모두 삭제합니다.
    azd down --purge --force

    # 모든 에이전트와 Microsoft Foundry 리소스를 삭제합니다.
    cd resources-foundry
    azd down --purge --force
    ```

---

축하합니다! 🎉 첫 번째 멀티 에이전트 오케스트레이션 시나리오인 Sequential 패턴을 완료했습니다. 다음 단계로 진행합시다!

👈 [00: Setup](./00-setup.md) | [02: Concurrent Pattern](./02-concurrent-pattern.md) 👉
