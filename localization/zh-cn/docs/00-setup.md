# 00 环境搭建

在本课程中，我们将在您的本地机器或 GitHub Codespaces 中搭建开发环境。

## 前提条件

- Web 浏览器：[Microsoft Edge](https://microsoft.com/edge) 或 [Google Chrome](http://chrome.google.com)
- [Azure 订阅](https://azure.microsoft.com/free)

## 在 GitHub Codespaces 中搭建

对于本工作坊，我们强烈建议使用 GitHub Codespaces 以节省搭建开发环境的时间。如果您更倾向于在本地机器上搭建开发环境，请跳过本节，直接前往[在 VS Code 中搭建](#在-vs-code-中搭建)部分。

1. 点击下方按钮创建一个新的 GitHub Codespaces 实例。

   [![创建新的 GitHub Codespaces 实例](https://github.com/codespaces/badge.svg)](https://codespaces.new/Azure-Samples/multi-agent-orchestration-workshop)

1. 创建完成后，运行以下命令验证必要工具是否已正确安装。

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

1. 检查您的 GitHub 登录状态。

    ```bash
    gh auth status
    ```

1. 获取远程仓库详情。

    ```bash
    git remote -v
    ```

   您应该看到以下内容。

    ```text
    origin  https://github.com/Azure-Samples/multi-agent-orchestration-workshop.git (fetch)
    origin  https://github.com/Azure-Samples/multi-agent-orchestration-workshop.git (push)
    ```

   如果没有看到这两行，请删除当前的 GitHub Codespaces 实例并重新创建。

1. 由于 GitHub Codespaces 实例不在您的账户下，请使用以下命令 fork 仓库以转移所有权。

    ```bash
    git remote -v > remote.txt
    git add . && git commit -m "Add remote.txt for forking"
    ```

   您可能会看到类似以下的提示信息：

    ```text
    You don't have write access to the Azure-Samples/multi-agent-orchestration-workshop repository, so you cannot push changes to it.
    To obtain write access we will point this codespace at your fork of Azure-Samples/multi-agent-orchestration-workshop, creating that fork if it doesn't exist.
    
    Would you like to proceed?
    ```

   输入 `y` 继续。它将自动在您的账户下 fork 该仓库。

1. 检查远程仓库状态。

    ```bash
    git remote -v
    ```

   这次，您应该看到以下四行。

    ```text
    origin  https://github.com/{{YOUR_GITHUB_ID}}/multi-agent-orchestration-workshop.git (fetch)
    origin  https://github.com/{{YOUR_GITHUB_ID}}/multi-agent-orchestration-workshop.git (push)
    upstream        https://github.com/Azure-Samples/multi-agent-orchestration-workshop (fetch)
    upstream        https://github.com/Azure-Samples/multi-agent-orchestration-workshop (push)
    ```

   如果没有看到这四行，请删除并重新创建 GitHub Codespaces 实例。

## 在 VS Code 中搭建

如果您已经创建了 GitHub Codespaces 实例，请跳过本节，直接前往[登录 Azure](#登录-azure)部分。

1. 在您的机器上安装以下工具。

   - [VS Code](https://code.visualstudio.com/download) + [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
   - [Docker Desktop](https://docs.docker.com/desktop/) 或等效工具
   - [GitHub CLI](https://cli.github.com)
   - [Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd)
   - [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli)
   - [Aspire CLI](https://aspire.dev/get-started/install-cli/)

   然后，运行以下命令验证它们是否已正确安装。

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

### 登录 GitHub

1. 登录 GitHub。

    ```bash
    gh auth login
    ```

   登录成功后，运行以下命令验证登录状态。

    ```bash
    gh auth status
    ```

### 克隆项目

1. 将此仓库 fork 到您的本地机器。

    ```bash
    gh repo fork Azure-Samples/multi-agent-orchestration-workshop --clone
    ```

1. 导航到克隆的目录。

    ```bash
    cd multi-agent-orchestration-workshop
    ```

1. 检查远程仓库状态。

    ```bash
    git remote -v
    ```

   您应该看到以下四行。

    ```text
    origin  https://github.com/{{YOUR_GITHUB_ID}}/multi-agent-orchestration-workshop.git (fetch)
    origin  https://github.com/{{YOUR_GITHUB_ID}}/multi-agent-orchestration-workshop.git (push)
    upstream        https://github.com/Azure-Samples/multi-agent-orchestration-workshop (fetch)
    upstream        https://github.com/Azure-Samples/multi-agent-orchestration-workshop (push)
    ```

1. 在 VS Code 中打开仓库。

    ```bash
    code .
    ```

## 登录 Azure

1. 登录 Azure。

    ```bash
    # Azure Developer CLI
    azd auth login

    # Azure CLI
    az login
    ```

   登录成功后，运行以下命令验证登录状态。

    ```bash
    # Azure Developer CLI
    azd auth login --check-status

    # Azure CLI
    az account show
    ```

---

恭喜！🎉 您已完成开发环境搭建。让我们继续吧！

👈 [README](../README.md) | [01: 顺序模式](./01-sequential-pattern.md) 👉
