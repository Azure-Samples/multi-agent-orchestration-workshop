# 00 Setup

In this session, we'll set up a development environment either on your machine or in GitHub Codespaces.

## Prerequisites

- Web browser: either [Microsoft Edge](https://microsoft.com/edge) or [Google Chrome](http://chrome.google.com)
- [Azure Subscription](https://azure.microsoft.com/free)

## Setup in GitHub Codespaces

For the workshop, we strongly recommend using GitHub Codespaces to save time setting up your development environment. If you prefer to set up the development environment on your local machine, skip this section and move to the [Setup in VS Code](#setup-in-vs-code) section.

1. Click the button below to create a new GitHub Codespaces instance.

   [![Create a new GitHub Codespaces instance](https://github.com/codespaces/badge.svg)](https://codespaces.new/Azure-Samples/multi-agent-orchestration-workshop)

1. Once created, run the following commands to verify that the necessary tools are properly installed.

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

1. Check your GitHub login status.

    ```bash
    gh auth status
    ```

1. Get the remote repository details.

    ```bash
    git remote -v
    ```

   You should see the following.

    ```text
    origin  https://github.com/Azure-Samples/multi-agent-orchestration-workshop.git (fetch)
    origin  https://github.com/Azure-Samples/multi-agent-orchestration-workshop.git (push)
    ```

   If you don't see those two lines, delete the current GitHub Codespaces instance and create it again.

1. Since the GitHub Codespaces instance is NOT under your account, fork the repository to transfer ownership using the following commands.

    ```bash
    git remote -v > remote.txt
    git add . && git commit -m "Add remote.txt for forking"
    ```

   You might see a message like the one below:

    ```text
    You don't have write access to the Azure-Samples/maf-workshop-in-a-day-ko repository, so you cannot push changes to it.
    To obtain write access we will point this codespace at your fork of Azure-Samples/maf-workshop-in-a-day-ko, creating that fork if it doesn't exist.
    
    Would you like to proceed?
    ```

   Enter `y` to continue. It automatically forks the repository under your account.

1. Check the remote repository status.

    ```bash
    git remote -v
    ```

   This time, you should see the following four lines.

    ```text
    origin  https://github.com/{{YOUR_GITHUB_ID}}/maf-workshop-in-a-day-ko.git (fetch)
    origin  https://github.com/{{YOUR_GITHUB_ID}}/maf-workshop-in-a-day-ko.git (push)
    upstream        https://github.com/Azure-Samples/maf-workshop-in-a-day-ko (fetch)
    upstream        https://github.com/Azure-Samples/maf-workshop-in-a-day-ko (push)
    ```

   If you don't see those four lines, delete and recreate the GitHub Codespaces instance.

## Setup in VS Code

If you already have a GitHub Codespaces instance, skip this section and go directly to the [Log in to Azure](#log-in-to-azure) section.

1. Install the following tools on your machine.

   - [VS Code](https://code.visualstudio.com/download) + [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
   - [Docker Desktop](https://docs.docker.com/desktop/) or equivalent
   - [GitHub CLI](https://cli.github.com)
   - [Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd)
   - [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli)
   - [Aspire CLI](https://aspire.dev/get-started/install-cli/)

   Then, run the following commands to verify they are properly installed.

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

### Log in to GitHub

1. Log in to GitHub.

    ```bash
    gh auth login
    ```

   Once logged in, run the following command to verify the login status.

    ```bash
    gh auth status
    ```

### Clone project

1. Fork this repository to your local machine.

    ```bash
    gh repo fork Azure-Samples/multi-agent-orchestration-workshop --clone
    ```

1. Navigate to the cloned directory.

    ```bash
    cd multi-agent-orchestration-workshop
    ```

1. Check the remote repository status.

    ```bash
    git remote -v
    ```

   You should see the following four lines.

    ```text
    origin  https://github.com/{{YOUR_GITHUB_ID}}/maf-workshop-in-a-day-ko.git (fetch)
    origin  https://github.com/{{YOUR_GITHUB_ID}}/maf-workshop-in-a-day-ko.git (push)
    upstream        https://github.com/Azure-Samples/maf-workshop-in-a-day-ko (fetch)
    upstream        https://github.com/Azure-Samples/maf-workshop-in-a-day-ko (push)
    ```

1. Open the repository in VS Code.

    ```bash
    code .
    ```

## Log in to Azure

1. Log in to Azure.

    ```bash
    # Azure Developer CLI
    azd auth login

    # Azure CLI
    az login
    ```

   Once logged in, run the following command to verify the login status.

    ```bash
    # Azure Develper CLI
    azd auth login --check-status

    # Azure Login
    az account show
    ```

---

Congratulations! 🎉 You've just completed the development environment setup. Let's move on!

👈 [README](../README.md) | [01: Sequential Pattern](./01-sequential-pattern.md) 👉
