# 01 Sequential パターン

Sequential パターンでは、エージェントが定義されたパイプラインの中で順番に動作し、各エージェントの出力が次のエージェントに渡されます。このアプローチは、コンテンツ作成ワークフロー、段階的なデータ変換、ステップバイステップの分析など、自然な進行に従うタスクに適しています。

## シナリオ

エージェントを使用して技術ブログ記事を執筆します &ndash; リサーチ エージェント、アウトライン エージェント、ライター エージェント、エディター エージェントです。

<div>
  <img src="../../../docs/images/01-sequential-pattern-architecture.png" alt="アーキテクチャ - Sequential パターン" width="640" />
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
    bash $REPOSITORY_ROOT/scripts/setup.sh --session 01-sequential-pattern
    ```

    ```powershell
    # PowerShell
    & $REPOSITORY_ROOT/scripts/setup.ps1 -Session 01-sequential-pattern
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
    editor-agent
    writer-agent
    outliner-agent
    research-agent
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

1. 同じファイルで、コメント `// Add backend agent service` を見つけて、そのすぐ下にコードを追加します。これにより、`foundry` リソースを参照するバックエンド エージェント サービスが定義されます &ndash; すべての Microsoft Foundry 接続詳細がバックエンド エージェント サービス アプリに渡されます。

    ```csharp
    // バックエンド エージェント サービスを追加
    var agent = builder.AddProject<MultiAgentWorkshop_Agent>("agent")
                       .WithReference(foundry);
    ```

   コードの解説です。

   - `builder.AddProject<MultiAgentWorkshop_Agent>("agent")`: バックエンド エージェント サービス アプリを .NET プロジェクトとして追加します。
   - `.WithReference(foundry)`: 上で作成した foundry リソースを参照し、Microsoft Foundry の接続詳細をバックエンド エージェント サービス アプリに渡します。

1. 同じファイルで、コメント `// Add frontend web UI` を見つけて、そのすぐ下にコードを追加します。これにより、`agents` と `agent` の両方のリソースを参照するフロントエンド Web UI が定義されます &ndash; エージェントの詳細とバックエンドの接続詳細の両方がフロントエンド Web UI アプリに渡されます。

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

## バックエンド エージェント サービスへの Sequential パターンの実装

1. `workshop` ディレクトリにいることを確認してください。

    ```bash
    cd $REPOSITORY_ROOT/workshop
    ```

1. `src/MultiAgentWorkshop.Agent/Program.cs` を開き、コメント `// Create AIProjectClient instance with EntraID authentication` を見つけて、そのすぐ下にコードを追加します。これにより、Microsoft Foundry プロジェクトに接続します。

    ```csharp
    // EntraID 認証で AIProjectClient インスタンスを作成
    var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions() { TenantId = config["AZURE_TENANT_ID"] });
    var projectClient = new AIProjectClient(endpoint: new Uri(endpoint), tokenProvider: credential);
    ```

   コードの解説です。

   - `new DefaultAzureCredential(...)`: API キーなしで Azure にログインします。ローカル マシンでは Azure CLI または Azure Developer CLI のログイン情報を使用し、Azure にデプロイされたアプリではマネージド ID を使用します。
   - `new AIProjectClient(endpoint, credential)`: エンドポイントとログイン情報を使用して Microsoft Foundry プロジェクト インスタンスに接続します。

1. 同じファイルで、コメント `// Register all agents passed from Aspire` を見つけて、そのすぐ下にコードを追加します。これにより、Microsoft Foundry プロジェクトからエージェントの詳細を取得し、IoC コンテナーにシングルトン サービスとして登録します。

    ```csharp
    // Aspire から渡されたすべてのエージェントを登録
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

   コードの解説です。

   - エージェントのリストは既に分かっていますが、名前のみ分かっています。そのため、コードは各エージェントに対して `foreach` ループを実行します。
   - `new AgentReference(name, version)`: 各エージェントの情報を使用して参照インスタンスを作成します。
   - `projectClient.AsAIAgent(reference, factory)`: 参照の詳細を使用して実際のエージェントに接続します。
   - `builder.Services.AddKeyedSingleton<AIAgent>(name, agent)`: エージェント インスタンスをシングルトン サービスとして登録します。

   > **注意**: `AgentRecordShimChatClient` クラスにお気づきかもしれません。これは Microsoft Agent Framework と Microsoft Foundry SDK 間のバージョンの不一致に対する一時的な回避策であり、まもなく削除される予定です。

1. 同じファイルで、コメント `// Build a sequential workflow pattern with the agents registered` を見つけて、そのすぐ下にコードを追加します。

    ```csharp
    // 登録されたエージェントで Sequential ワークフロー パターンを構築
    builder.AddWorkflow("publisher", (sp, key) => AgentWorkflowBuilder.BuildSequential(
        workflowName: key,
        agents: [.. agents.Select(a => sp.GetRequiredKeyedService<AIAgent>(a.Name))]
    )).AddAsAIAgent("publisher");
    ```

   コードの解説です。

   - `builder.AddWorkflow("publisher", ...).AddAsAIAgent("publisher")`: マルチエージェント ワークフローを `publisher` という名前の別のエージェント インスタンスとして追加し、シングルトンとして登録します。
   - `AgentWorkflowBuilder.BuildSequential(...)`: 同じ名前 `publisher` を使用する Sequential ワークフロー ビルダーです。

     `agents` 配列で宣言された順序で、以前に登録されたサービスから複数のエージェントが追加されることに注意してください。

1. 同じファイルで、コメント `// Map AGUI to the publisher workflow agent` を見つけて、そのすぐ下にコードを追加します。ワークフローは `ag-ui` の API エンドポイントとして公開され、フロントエンド Web UI がこのバックエンド エージェント サービス アプリと通信できるようになります。

    ```csharp
    // AGUI を publisher ワークフロー エージェントにマッピング
    app.MapAGUI(
        pattern: "ag-ui",
        aiAgent: app.Services.GetRequiredKeyedService<AIAgent>("publisher")
    );
    ```

## フロントエンド Web UI への Sequential パターンの実装

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

   ![Aspire ダッシュボード](../../../docs/images/step-01-image-01.png)

   バックエンド エージェント サービス アプリをクリックします。

1. Dev UI ページが開いたら、エージェントを `publisher` に変更し、「Configure & Run」ボタンをクリックします。

   ![Microsoft Agent Framework Dev UI - Sequential パターン](../../../docs/images/step-01-image-02.png)

1. 任意のリクエストを送信します。

   ![Microsoft Agent Framework Dev UI - リクエストの送信](../../../docs/images/step-01-image-03.png)

   画面の左側で結果とワークフローの進行状況を確認します。

   ![Microsoft Agent Framework Dev UI - ワークフローの実行](../../../docs/images/step-01-image-04.png)

1. Aspire ダッシュボードに戻り、Web UI アプリをクリックします。

   ![Aspire ダッシュボード](../../../docs/images/step-01-image-05.png)

1. 任意のリクエストを送信します。

   ![Microsoft Agent Framework Chat UI - リクエストの送信](../../../docs/images/step-01-image-06.png)

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

   ![Azure Container Apps 上の Microsoft Agent Framework - リクエストの送信](../../../docs/images/step-01-image-07.png)

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

おめでとうございます！ 🎉 最初のマルチエージェント オーケストレーション シナリオ - Sequential パターンが完了しました。次に進みましょう！

👈 [00: Setup](./00-setup.md) | [02: Concurrent Pattern](./02-concurrent-pattern.md) 👉
