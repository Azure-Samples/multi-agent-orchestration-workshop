# 00 Configuración

En esta sesión, configuraremos un entorno de desarrollo ya sea en su máquina o en GitHub Codespaces.

## Requisitos previos

- Navegador web: [Microsoft Edge](https://microsoft.com/edge) o [Google Chrome](http://chrome.google.com)
- [Suscripción de Azure](https://azure.microsoft.com/free)

## Configuración en GitHub Codespaces

Para el taller, recomendamos encarecidamente usar GitHub Codespaces para ahorrar tiempo configurando su entorno de desarrollo. Si prefiere configurar el entorno de desarrollo en su máquina local, omita esta sección y vaya a la sección [Configuración en VS Code](#configuración-en-vs-code).

1. Haga clic en el botón de abajo para crear una nueva instancia de GitHub Codespaces.

   [![Crear una nueva instancia de GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://codespaces.new/Azure-Samples/multi-agent-orchestration-workshop)

1. Una vez creada, ejecute los siguientes comandos para verificar que las herramientas necesarias estén correctamente instaladas.

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

1. Verifique su estado de inicio de sesión en GitHub.

    ```bash
    gh auth status
    ```

1. Obtenga los detalles del repositorio remoto.

    ```bash
    git remote -v
    ```

   Debería ver lo siguiente.

    ```text
    origin  https://github.com/Azure-Samples/multi-agent-orchestration-workshop.git (fetch)
    origin  https://github.com/Azure-Samples/multi-agent-orchestration-workshop.git (push)
    ```

   Si no ve esas dos líneas, elimine la instancia actual de GitHub Codespaces y créela nuevamente.

1. Dado que la instancia de GitHub Codespaces NO está bajo su cuenta, haga un fork del repositorio para transferir la propiedad usando los siguientes comandos.

    ```bash
    git remote -v > remote.txt
    git add . && git commit -m "Add remote.txt for forking"
    ```

   Podría ver un mensaje como el siguiente:

    ```text
    You don't have write access to the Azure-Samples/multi-agent-orchestration-workshop repository, so you cannot push changes to it.
    To obtain write access we will point this codespace at your fork of Azure-Samples/multi-agent-orchestration-workshop, creating that fork if it doesn't exist.
    
    Would you like to proceed?
    ```

   Ingrese `y` para continuar. Automáticamente se hará un fork del repositorio bajo su cuenta.

1. Verifique el estado del repositorio remoto.

    ```bash
    git remote -v
    ```

   Esta vez, debería ver las siguientes cuatro líneas.

    ```text
    origin  https://github.com/{{YOUR_GITHUB_ID}}/multi-agent-orchestration-workshop.git (fetch)
    origin  https://github.com/{{YOUR_GITHUB_ID}}/multi-agent-orchestration-workshop.git (push)
    upstream        https://github.com/Azure-Samples/multi-agent-orchestration-workshop (fetch)
    upstream        https://github.com/Azure-Samples/multi-agent-orchestration-workshop (push)
    ```

   Si no ve esas cuatro líneas, elimine y vuelva a crear la instancia de GitHub Codespaces.

## Configuración en VS Code

Si ya tiene una instancia de GitHub Codespaces, omita esta sección y vaya directamente a la sección [Iniciar sesión en Azure](#iniciar-sesión-en-azure).

1. Instale las siguientes herramientas en su máquina.

   - [VS Code](https://code.visualstudio.com/download) + [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
   - [Docker Desktop](https://docs.docker.com/desktop/) o equivalente
   - [GitHub CLI](https://cli.github.com)
   - [Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd)
   - [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli)
   - [Aspire CLI](https://aspire.dev/get-started/install-cli/)

   Luego, ejecute los siguientes comandos para verificar que estén correctamente instalados.

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

### Iniciar sesión en GitHub

1. Inicie sesión en GitHub.

    ```bash
    gh auth login
    ```

   Una vez que haya iniciado sesión, ejecute el siguiente comando para verificar el estado de inicio de sesión.

    ```bash
    gh auth status
    ```

### Clonar el proyecto

1. Haga un fork de este repositorio en su máquina local.

    ```bash
    gh repo fork Azure-Samples/multi-agent-orchestration-workshop --clone
    ```

1. Navegue al directorio clonado.

    ```bash
    cd multi-agent-orchestration-workshop
    ```

1. Verifique el estado del repositorio remoto.

    ```bash
    git remote -v
    ```

   Debería ver las siguientes cuatro líneas.

    ```text
    origin  https://github.com/{{YOUR_GITHUB_ID}}/multi-agent-orchestration-workshop.git (fetch)
    origin  https://github.com/{{YOUR_GITHUB_ID}}/multi-agent-orchestration-workshop.git (push)
    upstream        https://github.com/Azure-Samples/multi-agent-orchestration-workshop (fetch)
    upstream        https://github.com/Azure-Samples/multi-agent-orchestration-workshop (push)
    ```

1. Abra el repositorio en VS Code.

    ```bash
    code .
    ```

## Iniciar sesión en Azure

1. Inicie sesión en Azure.

    ```bash
    # Azure Developer CLI
    azd auth login

    # Azure CLI
    az login
    ```

   Una vez que haya iniciado sesión, ejecute el siguiente comando para verificar el estado de inicio de sesión.

    ```bash
    # Azure Develper CLI
    azd auth login --check-status

    # Azure Login
    az account show
    ```

---

¡Felicitaciones! 🎉 Acaba de completar la configuración del entorno de desarrollo. ¡Continuemos!

👈 [README](../README.md) | [01: Patrón Secuencial](./01-sequential-pattern.md) 👉
