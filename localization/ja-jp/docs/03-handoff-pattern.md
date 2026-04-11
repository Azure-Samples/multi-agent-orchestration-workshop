# 03 Handoff パターン

Handoff パターンでは、エージェントが会話のコンテキストに基づいて動的に制御を相互に渡します。トリアージ エージェントが最初のリクエストを受け取り、それを処理するのに最も適した専門エージェントにルーティングします。専門エージェントは、問題が複数の領域にまたがる場合、互いにリダイレクトすることもできます。これは、IT サポート、カスタマー サービス、または異なる段階で異なる専門知識が必要なワークフローに適しています。

## シナリオ

IT サポートチームで働いており、エージェントを使用します &ndash; 一般サポート エージェント、ネットワーク スペシャリスト エージェント、保証エージェント、およびトリアージ エージェントです。

<div>
  <img src="../../../docs/images/03-handoff-pattern-architecture.png" alt="アーキテクチャ - Handoff パターン" width="640" />
</div>

## リポジトリ ルートの取得

1. まず `$REPOSITORY_ROOT` 変数を取得します。

    ```bash
    # zsh/bash
    REPOSITORY_ROOT=$(git rev-parse --show-toplevel)
    ```

    ```powershell
    # PowerShell
    $REPOSITORY_ROOT = git rev-parse --show-toplevel
    ```

## スタート プロジェクトのコピー

1. すでに `workshop` ディレクトリがある場合は、先にリネームまたは削除してください。

1. セットアップ スクリプトを実行して、スタート プロジェクトを `workshop` ディレクトリにコピーします。

    ```bash
    # zsh/bash
    bash $REPOSITORY_ROOT/scripts/setup.sh --session 03-handoff-pattern
    ```

    ```powershell
    # PowerShell
    & $REPOSITORY_ROOT/scripts/setup.ps1 -Session 03-handoff-pattern
    ```

## エージェントのデプロイ

1. `workshop` ディレクトリにいることを確認してください。

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. `src/MultiAgentWorkshop.PromptAgent/appsettings.json` を開き、コメント行 `// Add agents` を見つけて、その下に `Agents` プロパティを追加します。

    ```jsonc
    {
      ...
      // エージェントを追加
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

1. `resources-foundry` ディレクトリに移動します。

    ```bash
    pushd resources-foundry
    ```

1. 以下のコマンドを実行して、上記で定義したエージェントを Microsoft Foundry にプロビジョニングおよびデプロイします。

    ```bash
    azd up
    ```

   プロビジョニング中に、環境名、Azure サブスクリプション、およびロケーションの入力を求められます。

1. プロビジョニングとデプロイが完了したら、以下のコマンドを実行して、エージェントが正常にデプロイされたことを確認します。

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

   4つのエージェント名が表示されるはずです。

    ```text
    warranty-agent
    network-specialist-agent
    general-support-agent
    triage-agent 
    ```

1. workshop ディレクトリに戻ります。

    ```bash
    popd
    ```

## Aspire オーケストレーションの構成

1. `workshop` ディレクトリにいることを確認してください。

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. 必要なエージェント情報がすべて記録されていることを確認します。

    ```bash
    dotnet user-secrets --project ./src/MultiAgentWorkshop.AppHost list
    ```

   `AZURE_TENANT_ID`、`FOUNDRY_NAME`、`FOUNDRY_PROJECT_NAME`、`FOUNDRY_RESOURCE_GROUP`、および `Foundry:Project:Endpoint` の値が表示されるはずです。

1. `src/MultiAgentWorkshop.AppHost/appsettings.json` を開き、コメント行 `// Add agents` を見つけて、その下に `Agents` プロパティを追加します。

    ```jsonc
    {
      ...
      // エージェントを追加
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

1. `src/MultiAgentWorkshop.AppHost/AppHost.cs` を開き、コメント `// Add resource for Microsoft Foundry` を見つけて、そのすぐ下にコードを追加します。これにより、Microsoft Foundry プロジェクトの接続詳細が追加されます。

    ```csharp
    // Microsoft Foundry のリソースを追加
    var foundry = builder.AddFoundry("foundry");
    ```

   コードの解説です。

   - `builder.AddFoundry("foundry")`: カスタム リソース `FoundryResource` を通じて Microsoft Foundry の接続詳細を追加します。Aspire のカスタム リソースについて詳しく知りたい場合は、[カスタム ホスティング統合の作成](https://aspire.dev/integrations/custom-integrations/hosting-integrations/) をご覧ください。

1. 同じファイルで、コメント `// Add resource for agents on Microsoft Foundry` を見つけて、そのすぐ下にコードを追加します。これにより、エージェントの詳細リストが参照アプリケーションに公開されます。

    ```csharp
    // Microsoft Foundry 上のエージェントのリソースを追加
    var agents = builder.AddAgents("agents");
    ```

   コードの解説です。

   - `builder.AddAgents("agents")`: カスタム リソース `AgentResource` を通じてエージェントの詳細リストを追加します。Aspire のカスタム リソースについて詳しく知りたい場合は、[カスタム ホスティング統合の作成](https://aspire.dev/integrations/custom-integrations/hosting-integrations/) をご覧ください。

1. 同じファイルで、コメント `// Add backend agent service` を見つけて、そのすぐ下にコードを追加します。これにより、`foundry` リソースを参照するバックエンド エージェント サービスが定義されます — すべての Microsoft Foundry 接続詳細がバックエンド エージェント サービス アプリに渡されます。

    ```csharp
    // バックエンド エージェント サービスを追加
    var agent = builder.AddProject<MultiAgentWorkshop_Agent>("agent")
                       .WithReference(foundry);
    ```

   コードの解説です。

   - `builder.AddProject<MultiAgentWorkshop_Agent>("agent")`: バックエンド エージェント サービス アプリを .NET プロジェクトとして追加します。
   - `.WithReference(foundry)`: 上で作成した foundry リソースを参照し、Microsoft Foundry の接続詳細をバックエンド エージェント サービス アプリに渡します。

1. 同じファイルで、コメント `// Add frontend web UI` を見つけて、そのすぐ下にコードを追加します。これにより、`agents` と `agent` の両方のリソースを参照するフロントエンド Web UI が定義されます — エージェントの詳細とバックエンドの接続詳細の両方がフロントエンド Web UI アプリに渡されます。

    ```csharp
    // フロントエンド Web UI を追加
    var webUI = builder.AddProject<MultiAgentWorkshop_WebUI>("webui")
                       .WithExternalHttpEndpoints()
                       .WithReference(agents)
                       .WithReference(agent)
                       .WaitFor(agent);
    ```

   コードの解説です。

   - `builder.AddProject<MultiAgentWorkshop_WebUI>("webui")`: フロントエンド Web UI アプリを .NET プロジェクトとして追加します。
   - `.WithExternalHttpEndpoints()`: このフロントエンド Web UI アプリをインターネットに公開し、パブリック アクセス可能にします。
   - `.WithReference(agents)`: 上で作成した agent リソースを参照し、エージェントのリストをフロントエンド Web UI アプリに渡します。
   - `.WithReference(agent)`: バックエンド エージェント サービス アプリを参照し、接続詳細をフロントエンド Web UI アプリに渡します。

## バックエンド エージェント サービスへの Handoff パターンの実装

1. `workshop` ディレクトリにいることを確認してください。

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. `src/MultiAgentWorkshop.Agent/Program.cs` を開き、コメント `// Create AzureOpenAIClient instance with EntraID authentication` を見つけて、そのすぐ下にコードを追加します。これにより、Microsoft Foundry プロジェクトに接続します。

    ```csharp
    // EntraID 認証で AzureOpenAIClient インスタンスを作成
    var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions() { TenantId = config["AZURE_TENANT_ID"] });

    var url = new Uri(endpoint.GetAzureOpenAIResponsesEndpoint());
    var chatClient = new AzureOpenAIClient(url, credential)
                        .GetResponsesClient()
                        .AsIChatClient(model);
    ```

   コードの解説です。

   - `new DefaultAzureCredential(...)`: API キーなしで Azure にログインします。ローカル マシンでは Azure CLI または Azure Developer CLI のログイン情報を使用し、Azure にデプロイされたアプリではマネージド ID を使用します。
   - `endpoint.GetAzureOpenAIResponsesEndpoint()`: Microsoft Foundry プロジェクト エンドポイントを Microsoft Foundry OpenAI エンドポイントに変換します。これは、Microsoft Foundry SDK が現在 Handoff パターンをサポートしていないためです。
   - `new AzureOpenAIClient(url, credential)`: エンドポイントとログイン情報を使用して Azure OpenAI インスタンスに接続し、`IChatClient` インスタンスに変換します。

1. 同じファイルで、コメント `// Register all agents passed from Aspire` を見つけて、そのすぐ下にコードを追加します。これにより、Microsoft Foundry プロジェクトからエージェントの詳細を取得し、IoC コンテナーにシングルトン サービスとして登録します。

    ```csharp
    // Aspire から渡されたすべてのエージェントを登録
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

   コードの解説です。

   - エージェントのリストは既に分かっていますが、名前のみ分かっています。そのため、コードは各エージェントに対して `foreach` ループを実行します。
   - `await File.ReadAllTextAsync(...)`: エージェントの指示ファイルをインポートします。
   - `new ChatClientAgent(chatClient, instructions, name)`: 各エージェントの情報、指示、および `IChatClient` インスタンスを使用して、エージェント インスタンスを作成します。
   - `builder.Services.AddKeyedSingleton<AIAgent>(name, agent)`: エージェント インスタンスをシングルトン サービスとして登録します。

1. 同じファイルで、コメント `// Build a handoff workflow pattern with the agents registered` を見つけて、そのすぐ下にコードを追加します。

    ```csharp
    // 登録されたエージェントで Handoff ワークフロー パターンを構築
    builder.AddWorkflow("publisher", (sp, key) =>
    {
        var triage = sp.GetRequiredKeyedService<AIAgent>("triage-agent");
        var generalSupport = sp.GetRequiredKeyedService<AIAgent>("general-support-agent");
        var networkSpecialist = sp.GetRequiredKeyedService<AIAgent>("network-specialist-agent");
        var warranty = sp.GetRequiredKeyedService<AIAgent>("warranty-agent");

        var specialists = new[] { generalSupport, networkSpecialist, warranty };

        var workflow = AgentWorkflowBuilder.CreateHandoffBuilderWith(triage)
            // トリアージはどの専門エージェントにもハンドオフ可能
            .WithHandoffs(triage, specialists)
            // 各専門エージェントは他の専門エージェントにハンドオフ可能
            .WithHandoffs(generalSupport, [networkSpecialist, warranty])
            .WithHandoffs(networkSpecialist, [generalSupport, warranty])
            .WithHandoffs(warranty, [generalSupport, networkSpecialist])
            // すべての専門エージェントはトリアージに戻す
            .WithHandoffs(specialists, triage, "Hand back to triage when the issue is resolved or needs further routing")
            .Build();

        // HandoffWorkflowBuilder.Build() はワークフロー名を設定しません。
        // AddWorkflow の名前検証を通過するためにリフレクションで設定します。
        typeof(Workflow).GetProperty("Name")!.SetValue(workflow, key);

        return workflow;
    }).AddAsAIAgent("publisher");
    ```

   コードの解説です。

   - `var specialists = new[] { generalSupport, networkSpecialist, warranty };`: グループ チャットに参加する専門エージェントのリストを定義します。トリアージ エージェントは、ユーザーのリクエストを専門エージェントの1つにルーティングする出発点です。
   - `builder.AddWorkflow("publisher", ...).AddAsAIAgent("publisher")`: マルチエージェント ワークフローを `publisher` という名前の別のエージェント インスタンスとして追加し、シングルトンとして登録します。
   - `AgentWorkflowBuilder.CreateHandoffBuilderWith(triage)`: トリアージ エージェントを使用した Handoff ワークフロー ビルダーです。
   - `.WithHandoffs(triage, specialists)`: トリアージ エージェントから専門エージェントへのハンドオフを定義します。
   - `.WithHandoffs(generalSupport, [networkSpecialist, warranty])`: 一般サポート エージェントから他の専門エージェントへのハンドオフを定義します。
   - `.WithHandoffs(networkSpecialist, [generalSupport, warranty])`: ネットワーク スペシャリスト エージェントから他の専門エージェントへのハンドオフを定義します。
   - `.WithHandoffs(warranty, [generalSupport, networkSpecialist])`: 保証エージェントから他の専門エージェントへのハンドオフを定義します。
   - `.WithHandoffs(specialists, triage, "Hand back to triage when the issue is resolved or needs further routing")`: 問題が解決したか、さらにルーティングが必要な場合に、すべての専門エージェントからトリアージ エージェントへのハンドオフを定義します。
   - `typeof(Workflow).GetProperty("Name")!.SetValue(workflow, key);`: ワークフロー名を注入します。これは一時的な回避策です。

1. 同じファイルで、コメント `// Map AGUI to the publisher workflow agent` を見つけて、そのすぐ下にコードを追加します。ワークフローは `ag-ui` の API エンドポイントとして公開され、フロントエンド Web UI がこのバックエンド エージェント サービス アプリと通信できるようになります。

    ```csharp
    // AGUI を publisher ワークフロー エージェントにマッピング
    app.MapAGUI(
        pattern: "ag-ui",
        aiAgent: app.Services.GetRequiredKeyedService<AIAgent>("publisher")
                             .CreateFixedAgent()
    );
    ```

   `.CreateFixedAgent()` は、出力ストリームが適切に処理されるまでの一時的な回避策であることに注意してください。

## フロントエンド Web UI への Handoff パターンの実装

1. `workshop` ディレクトリにいることを確認してください。

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. `src/MultiAgentWorkshop.WebUI/Program.cs` を開き、コメント `// Register all agents passed from Aspire` を見つけて、そのすぐ下にコードを追加します。これにより、すべてのエージェントの詳細が登録され、Web UI がどのエージェントが応答しているかを把握できるようになります。

    ```csharp
    // Aspire から渡されたすべてのエージェントを登録
    builder.Services.AddSingleton(agents);
    ```

1. 同じファイルで、コメント `// Register the backend agent service as an HTTP client` を見つけて、そのすぐ下にコードを追加します。Aspire は既にフロントエンド Web UI アプリにバックエンド エージェント サービスの接続詳細を提供しています。

    ```csharp
    // バックエンド エージェント サービスを HTTP クライアントとして登録
    builder.Services.AddHttpClient("agent", client =>
    {
        client.BaseAddress = new Uri("https+http://agent");
    });
    ```

1. 同じファイルで、コメント `// Register AGUI client` を見つけて、そのすぐ下にコードを追加します。この AGUI クライアントを使用して、フロントエンド Web UI アプリは `ag-ui` エンドポイント経由でバックエンド エージェント サービス アプリと通信します。

    ```csharp
    // AGUI クライアントを登録
    builder.Services.AddChatClient(sp => new AGUIChatClient(
        httpClient: sp.GetRequiredService<IHttpClientFactory>().CreateClient("agent"),
        endpoint: "ag-ui")
    );
    ```

## Aspire の実行

1. `workshop` ディレクトリにいることを確認してください。

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. Azure CLI と Azure Developer CLI の両方で Azure に既にログインしていることを確認してください。不明な場合は、[こちらの手順](./00-setup.md#azure-へのログイン) を再度実行してください。

1. 以下のコマンドを実行して、Aspire を通じてすべてのアプリを起動します。

    ```bash
    dotnet watch run --project ./src/MultiAgentWorkshop.AppHost
    ```

1. Aspire ダッシュボードが自動的に開きます。

   ![Aspire ダッシュボード](../../../docs/images/step-03-image-01.png)

   バックエンド エージェント サービス アプリをクリックします。

1. Dev UI ページが開いたら、エージェントを `publisher` に変更して、トリアージ エージェントがリクエストを他のエージェントに配分する様子を確認します。

   ![Microsoft Agent Framework Dev UI - Handoff パターン](../../../docs/images/step-03-image-02.png)

1. 任意のリクエストを送信します。

   ![Microsoft Agent Framework Dev UI - リクエストの送信](../../../docs/images/step-03-image-03.png)

   画面の左側で結果とワークフローの進行状況を確認します。

   ![Microsoft Agent Framework Dev UI - ワークフローの実行](../../../docs/images/step-03-image-04.png)

1. Aspire ダッシュボードに戻り、Web UI アプリをクリックします。

   ![Aspire ダッシュボード](../../../docs/images/step-03-image-05.png)

1. 任意のリクエストを送信します。

   ![Microsoft Agent Framework Chat UI - リクエストの送信](../../../docs/images/step-03-image-06.png)

   結果を確認します。

1. `Ctrl`+`C` を押してすべての実行中のアプリを終了します。

## Azure へのデプロイ

1. `workshop` ディレクトリにいることを確認してください。

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. 以下のコマンドを実行して、フロントエンド Web UI とバックエンド エージェント サービス アプリの両方を Azure にプロビジョニングおよびデプロイします。

    ```bash
    azd up
    ```

   プロビジョニング中に、環境名、Azure サブスクリプション、およびロケーションの入力を求められます。

1. 完了すると、ターミナル画面に Web UI アプリケーションの URL が表示されます。Web ブラウザーで開き、リクエストを送信します。

   ![Azure Container Apps 上の Microsoft Agent Framework - リクエストの送信](../../../docs/images/step-03-image-07.png)

   結果を確認します。

1. すべて完了したら、Azure からすべてのアプリとエージェントを削除します。

    ```bash
    # Web UI とエージェント サービス アプリの両方を削除します。
    azd down --purge --force

    # すべてのエージェントと Microsoft Foundry リソースを削除します。
    cd resources-foundry
    azd down --purge --force
    ```

---

おめでとうございます！ 🎉 3番目のマルチエージェント オーケストレーション シナリオ &ndash; Handoff パターンが完了しました。次に進みましょう！

👈 [02: Concurrent Pattern](./02-concurrent-pattern.md) | [04: Group Chat Pattern](./04-group-chat-pattern.md) 👉
