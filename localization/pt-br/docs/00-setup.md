# 00 Configuração

Nesta sessão, vamos configurar o ambiente de desenvolvimento na sua máquina ou no GitHub Codespaces.

## Pré-requisitos

- Navegador web: [Microsoft Edge](https://microsoft.com/edge) ou [Google Chrome](http://chrome.google.com)
- [Assinatura Azure](https://azure.microsoft.com/free)

## Configuração no GitHub Codespaces

Para o workshop, recomendamos fortemente o uso do GitHub Codespaces para economizar tempo na configuração do ambiente de desenvolvimento. Se você preferir configurar o ambiente na sua máquina local, pule esta seção e vá para a seção [Configuração no VS Code](#configuração-no-vs-code).

1. Clique no botão abaixo para criar uma nova instância do GitHub Codespaces.

   [![Criar uma nova instância do GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://codespaces.new/Azure-Samples/multi-agent-orchestration-workshop)

1. Após a criação, execute os seguintes comandos para verificar se as ferramentas necessárias estão instaladas corretamente.

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

1. Verifique o status do seu login no GitHub.

    ```bash
    gh auth status
    ```

1. Obtenha os detalhes do repositório remoto.

    ```bash
    git remote -v
    ```

   Você deverá ver o seguinte.

    ```text
    origin  https://github.com/Azure-Samples/multi-agent-orchestration-workshop.git (fetch)
    origin  https://github.com/Azure-Samples/multi-agent-orchestration-workshop.git (push)
    ```

   Se você não vir essas duas linhas, exclua a instância atual do GitHub Codespaces e crie-a novamente.

1. Como a instância do GitHub Codespaces NÃO está sob a sua conta, faça um fork do repositório para transferir a propriedade usando os seguintes comandos.

    ```bash
    git remote -v > remote.txt
    git add . && git commit -m "Add remote.txt for forking"
    ```

   Você pode ver uma mensagem como a seguinte:

    ```text
    You don't have write access to the Azure-Samples/multi-agent-orchestration-workshop repository, so you cannot push changes to it.
    To obtain write access we will point this codespace at your fork of Azure-Samples/multi-agent-orchestration-workshop, creating that fork if it doesn't exist.
    
    Would you like to proceed?
    ```

   Digite `y` para continuar. Isso fará automaticamente o fork do repositório na sua conta.

1. Verifique o status do repositório remoto.

    ```bash
    git remote -v
    ```

   Desta vez, você deverá ver as seguintes quatro linhas.

    ```text
    origin  https://github.com/{{YOUR_GITHUB_ID}}/multi-agent-orchestration-workshop.git (fetch)
    origin  https://github.com/{{YOUR_GITHUB_ID}}/multi-agent-orchestration-workshop.git (push)
    upstream        https://github.com/Azure-Samples/multi-agent-orchestration-workshop (fetch)
    upstream        https://github.com/Azure-Samples/multi-agent-orchestration-workshop (push)
    ```

   Se você não vir essas quatro linhas, exclua e recrie a instância do GitHub Codespaces.

## Configuração no VS Code

Se você já tem uma instância do GitHub Codespaces, pule esta seção e vá diretamente para a seção [Fazer login no Azure](#fazer-login-no-azure).

1. Instale as seguintes ferramentas na sua máquina.

   - [VS Code](https://code.visualstudio.com/download) + [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
   - [Docker Desktop](https://docs.docker.com/desktop/) ou equivalente
   - [GitHub CLI](https://cli.github.com)
   - [Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd)
   - [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli)
   - [Aspire CLI](https://aspire.dev/get-started/install-cli/)

   Em seguida, execute os seguintes comandos para verificar se estão instalados corretamente.

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

### Fazer login no GitHub

1. Faça login no GitHub.

    ```bash
    gh auth login
    ```

   Após fazer login, execute o seguinte comando para verificar o status do login.

    ```bash
    gh auth status
    ```

### Clonar o projeto

1. Faça fork deste repositório para a sua máquina local.

    ```bash
    gh repo fork Azure-Samples/multi-agent-orchestration-workshop --clone
    ```

1. Navegue até o diretório clonado.

    ```bash
    cd multi-agent-orchestration-workshop
    ```

1. Verifique o status do repositório remoto.

    ```bash
    git remote -v
    ```

   Você deverá ver as seguintes quatro linhas.

    ```text
    origin  https://github.com/{{YOUR_GITHUB_ID}}/multi-agent-orchestration-workshop.git (fetch)
    origin  https://github.com/{{YOUR_GITHUB_ID}}/multi-agent-orchestration-workshop.git (push)
    upstream        https://github.com/Azure-Samples/multi-agent-orchestration-workshop (fetch)
    upstream        https://github.com/Azure-Samples/multi-agent-orchestration-workshop (push)
    ```

1. Abra o repositório no VS Code.

    ```bash
    code .
    ```

## Fazer login no Azure

1. Faça login no Azure.

    ```bash
    # Azure Developer CLI
    azd auth login

    # Azure CLI
    az login
    ```

   Após fazer login, execute o seguinte comando para verificar o status do login.

    ```bash
    # Azure Developer CLI
    azd auth login --check-status

    # Azure Login
    az account show
    ```

---

Parabéns! 🎉 Você acabou de concluir a configuração do ambiente de desenvolvimento. Vamos prosseguir!

👈 [README](../README.md) | [01: Sequential Pattern](./01-sequential-pattern.md) 👉
