# 00 설정

이 세션에서는 로컬 머신 또는 GitHub Codespaces에서 개발 환경을 설정합니다.

## 사전 요구 사항

- 웹 브라우저: [Microsoft Edge](https://microsoft.com/edge) 또는 [Google Chrome](http://chrome.google.com)
- [Azure 구독](https://azure.microsoft.com/free)

## GitHub Codespaces에서 설정하기

워크숍을 위해 개발 환경 설정 시간을 절약하기 위해 GitHub Codespaces 사용을 강력히 권장합니다. 로컬 머신에서 개발 환경을 설정하려면 이 섹션을 건너뛰고 [VS Code에서 설정하기](#vs-code에서-설정하기) 섹션으로 이동하세요.

1. 아래 버튼을 클릭하여 새로운 GitHub Codespaces 인스턴스를 생성합니다.

   [![새 GitHub Codespaces 인스턴스 생성](https://github.com/codespaces/badge.svg)](https://codespaces.new/Azure-Samples/multi-agent-orchestration-workshop)

1. 생성이 완료되면 다음 명령어를 실행하여 필요한 도구가 올바르게 설치되었는지 확인합니다.

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

1. GitHub 로그인 상태를 확인합니다.

    ```bash
    gh auth status
    ```

1. 원격 저장소 정보를 확인합니다.

    ```bash
    git remote -v
    ```

   다음과 같이 표시되어야 합니다.

    ```text
    origin  https://github.com/Azure-Samples/multi-agent-orchestration-workshop.git (fetch)
    origin  https://github.com/Azure-Samples/multi-agent-orchestration-workshop.git (push)
    ```

   위 두 줄이 보이지 않으면 현재 GitHub Codespaces 인스턴스를 삭제하고 다시 생성하세요.

1. GitHub Codespaces 인스턴스는 본인 계정 소유가 아니므로, 다음 명령어를 사용하여 저장소를 포크하여 소유권을 이전합니다.

    ```bash
    git remote -v > remote.txt
    git add . && git commit -m "Add remote.txt for forking"
    ```

   아래와 같은 메시지가 표시될 수 있습니다:

    ```text
    You don't have write access to the Azure-Samples/multi-agent-orchestration-workshop repository, so you cannot push changes to it.
    To obtain write access we will point this codespace at your fork of Azure-Samples/multi-agent-orchestration-workshop, creating that fork if it doesn't exist.
    
    Would you like to proceed?
    ```

   `y`를 입력하여 계속합니다. 자동으로 본인 계정 아래에 저장소가 포크됩니다.

1. 원격 저장소 상태를 확인합니다.

    ```bash
    git remote -v
    ```

   이번에는 다음과 같이 네 줄이 표시되어야 합니다.

    ```text
    origin  https://github.com/{{YOUR_GITHUB_ID}}/multi-agent-orchestration-workshop.git (fetch)
    origin  https://github.com/{{YOUR_GITHUB_ID}}/multi-agent-orchestration-workshop.git (push)
    upstream        https://github.com/Azure-Samples/multi-agent-orchestration-workshop (fetch)
    upstream        https://github.com/Azure-Samples/multi-agent-orchestration-workshop (push)
    ```

   위 네 줄이 보이지 않으면 GitHub Codespaces 인스턴스를 삭제하고 다시 생성하세요.

## VS Code에서 설정하기

이미 GitHub Codespaces 인스턴스가 있다면 이 섹션을 건너뛰고 [Azure에 로그인하기](#azure에-로그인하기) 섹션으로 직접 이동하세요.

1. 다음 도구를 머신에 설치합니다.

   - [VS Code](https://code.visualstudio.com/download) + [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
   - [Docker Desktop](https://docs.docker.com/desktop/) 또는 동등한 도구
   - [GitHub CLI](https://cli.github.com)
   - [Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd)
   - [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli)
   - [Aspire CLI](https://aspire.dev/get-started/install-cli/)

   설치 후 다음 명령어를 실행하여 올바르게 설치되었는지 확인합니다.

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

### GitHub에 로그인하기

1. GitHub에 로그인합니다.

    ```bash
    gh auth login
    ```

   로그인 후 다음 명령어를 실행하여 로그인 상태를 확인합니다.

    ```bash
    gh auth status
    ```

### 프로젝트 복제하기

1. 이 저장소를 로컬 머신으로 포크합니다.

    ```bash
    gh repo fork Azure-Samples/multi-agent-orchestration-workshop --clone
    ```

1. 복제된 디렉터리로 이동합니다.

    ```bash
    cd multi-agent-orchestration-workshop
    ```

1. 원격 저장소 상태를 확인합니다.

    ```bash
    git remote -v
    ```

   다음과 같이 네 줄이 표시되어야 합니다.

    ```text
    origin  https://github.com/{{YOUR_GITHUB_ID}}/multi-agent-orchestration-workshop.git (fetch)
    origin  https://github.com/{{YOUR_GITHUB_ID}}/multi-agent-orchestration-workshop.git (push)
    upstream        https://github.com/Azure-Samples/multi-agent-orchestration-workshop (fetch)
    upstream        https://github.com/Azure-Samples/multi-agent-orchestration-workshop (push)
    ```

1. VS Code에서 저장소를 엽니다.

    ```bash
    code .
    ```

## Azure에 로그인하기

1. Azure에 로그인합니다.

    ```bash
    # Azure Developer CLI
    azd auth login

    # Azure CLI
    az login
    ```

   로그인 후 다음 명령어를 실행하여 로그인 상태를 확인합니다.

    ```bash
    # Azure Developer CLI
    azd auth login --check-status

    # Azure Login
    az account show
    ```

---

축하합니다! 🎉 개발 환경 설정을 완료했습니다. 다음 단계로 진행합시다!

👈 [README](../README.md) | [01: Sequential Pattern](./01-sequential-pattern.md) 👉
