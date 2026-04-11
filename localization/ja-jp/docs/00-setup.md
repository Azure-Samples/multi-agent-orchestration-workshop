# 00 セットアップ

このセッションでは、お使いのマシンまたは GitHub Codespaces で開発環境をセットアップします。

## 前提条件

- Web ブラウザー: [Microsoft Edge](https://microsoft.com/edge) または [Google Chrome](http://chrome.google.com)
- [Azure サブスクリプション](https://azure.microsoft.com/free)

## GitHub Codespaces でのセットアップ

ワークショップでは、開発環境のセットアップ時間を節約するために GitHub Codespaces の使用を強くお勧めします。ローカル マシンで開発環境をセットアップする場合は、このセクションをスキップして [VS Code でのセットアップ](#vs-code-でのセットアップ) セクションに進んでください。

1. 下のボタンをクリックして、新しい GitHub Codespaces インスタンスを作成します。

   [![新しい GitHub Codespaces インスタンスを作成する](https://github.com/codespaces/badge.svg)](https://codespaces.new/Azure-Samples/multi-agent-orchestration-workshop)

1. 作成が完了したら、以下のコマンドを実行して、必要なツールが正しくインストールされていることを確認します。

    ```bash
    # .NET SDK
    dotnet --list-sdks

    # node.js
    node --version
    npm --version

    # PowerShell
    pwsh --version

    # Docker
    docker info

    # GitHub CLI
    gh --version

    # azd CLI
    azd version

    # az CLI
    az --version
    az bicep version

    # Aspire CLI
    aspire --version
    ```

1. GitHub のログイン状態を確認します。

    ```bash
    gh auth status
    ```

1. リモート リポジトリの詳細を取得します。

    ```bash
    git remote -v
    ```

   以下のように表示されるはずです。

    ```text
    origin  https://github.com/Azure-Samples/multi-agent-orchestration-workshop.git (fetch)
    origin  https://github.com/Azure-Samples/multi-agent-orchestration-workshop.git (push)
    ```

   これらの2行が表示されない場合は、現在の GitHub Codespaces インスタンスを削除して、再度作成してください。

1. GitHub Codespaces インスタンスはあなたのアカウント配下にないため、以下のコマンドを使用してリポジトリをフォークし、所有権を移転します。

    ```bash
    git remote -v > remote.txt
    git add . && git commit -m "Add remote.txt for forking"
    ```

   以下のようなメッセージが表示される場合があります：

    ```text
    You don't have write access to the Azure-Samples/multi-agent-orchestration-workshop repository, so you cannot push changes to it.
    To obtain write access we will point this codespace at your fork of Azure-Samples/multi-agent-orchestration-workshop, creating that fork if it doesn't exist.
    
    Would you like to proceed?
    ```

   `y` を入力して続行します。自動的にあなたのアカウント配下にリポジトリがフォークされます。

1. リモート リポジトリの状態を確認します。

    ```bash
    git remote -v
    ```

   今度は以下の4行が表示されるはずです。

    ```text
    origin  https://github.com/{{YOUR_GITHUB_ID}}/multi-agent-orchestration-workshop.git (fetch)
    origin  https://github.com/{{YOUR_GITHUB_ID}}/multi-agent-orchestration-workshop.git (push)
    upstream        https://github.com/Azure-Samples/multi-agent-orchestration-workshop (fetch)
    upstream        https://github.com/Azure-Samples/multi-agent-orchestration-workshop (push)
    ```

   これらの4行が表示されない場合は、GitHub Codespaces インスタンスを削除して再作成してください。

## VS Code でのセットアップ

GitHub Codespaces インスタンスを既にお持ちの場合は、このセクションをスキップして [Azure へのログイン](#azure-へのログイン) セクションに直接進んでください。

1. お使いのマシンに以下のツールをインストールします。

   - [VS Code](https://code.visualstudio.com/download) + [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
   - [Docker Desktop](https://docs.docker.com/desktop/) または同等のツール
   - [GitHub CLI](https://cli.github.com)
   - [Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd)
   - [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli)
   - [Aspire CLI](https://aspire.dev/get-started/install-cli/)

   次に、以下のコマンドを実行して、正しくインストールされていることを確認します。

    ```bash
    # .NET SDK
    dotnet --list-sdks

    # Docker
    docker info

    # GitHub CLI
    gh --version

    # azd CLI
    azd version

    # az CLI
    az --version
    az bicep version

    # Aspire CLI
    aspire --version
    ```

### GitHub へのログイン

1. GitHub にログインします。

    ```bash
    gh auth login
    ```

   ログインしたら、以下のコマンドを実行してログイン状態を確認します。

    ```bash
    gh auth status
    ```

### プロジェクトのクローン

1. このリポジトリをローカル マシンにフォークします。

    ```bash
    gh repo fork Azure-Samples/multi-agent-orchestration-workshop --clone
    ```

1. クローンしたディレクトリに移動します。

    ```bash
    cd multi-agent-orchestration-workshop
    ```

1. リモート リポジトリの状態を確認します。

    ```bash
    git remote -v
    ```

   以下の4行が表示されるはずです。

    ```text
    origin  https://github.com/{{YOUR_GITHUB_ID}}/multi-agent-orchestration-workshop.git (fetch)
    origin  https://github.com/{{YOUR_GITHUB_ID}}/multi-agent-orchestration-workshop.git (push)
    upstream        https://github.com/Azure-Samples/multi-agent-orchestration-workshop (fetch)
    upstream        https://github.com/Azure-Samples/multi-agent-orchestration-workshop (push)
    ```

1. VS Code でリポジトリを開きます。

    ```bash
    code .
    ```

## Azure へのログイン

1. Azure にログインします。

    ```bash
    # Azure Developer CLI
    azd auth login

    # Azure CLI
    az login
    ```

   ログインしたら、以下のコマンドを実行してログイン状態を確認します。

    ```bash
    # Azure Developer CLI
    azd auth login --check-status

    # Azure CLI
    az account show
    ```

---

おめでとうございます！ 🎉 開発環境のセットアップが完了しました。次に進みましょう！

👈 [README](../README.md) | [01: Sequential Pattern](./01-sequential-pattern.md) 👉
