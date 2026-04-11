# Multi-Agent Orchestration Workshop

Este é o material de workshop para construir aplicações multi-agente baseadas em .NET usando [Microsoft Agent Framework](https://aka.ms/agent-framework), [Microsoft Foundry](https://aka.ms/microsoft-foundry) e [Aspire](https://aspire.dev).

![Workshop MAF + Foundry](../../images/hero.jpg)

## Para que serve este workshop?

Construir uma aplicação com um único agente é fácil. Mas existem muitos casos de uso reais que exigem múltiplos agentes trabalhando juntos, e construir uma aplicação multi-agente não é tão simples quanto construir uma com um único agente. O [Microsoft Agent Framework](https://aka.ms/agent-framework) oferece cinco padrões de orquestração multi-agente:

| Padrão                                                                                                                           | Descrição                                                          |
|----------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------|
| [Sequential](https://learn.microsoft.com/agent-framework/workflows/orchestrations/sequential?pivots=programming-language-csharp) | Os agentes executam um após o outro em uma ordem definida          |
| [Concurrent](https://learn.microsoft.com/agent-framework/workflows/orchestrations/concurrent?pivots=programming-language-csharp) | Os agentes executam em paralelo                                    |
| [Handoff](https://learn.microsoft.com/agent-framework/workflows/orchestrations/handoff?pivots=programming-language-csharp)       | Os agentes transferem o controle entre si com base no contexto     |
| [Group Chat](https://learn.microsoft.com/agent-framework/workflows/orchestrations/group-chat?pivots=programming-language-csharp) | Os agentes colaboram em uma conversa compartilhada                 |
| [Magentic](https://learn.microsoft.com/agent-framework/workflows/orchestrations/magentic?pivots=programming-language-python)     | Um agente gerente coordena dinamicamente agentes especializados    |

## Funcionalidades

Neste workshop, construímos todos os padrões de orquestração multi-agente, exceto o padrão Magentic. Ao concluir cada padrão, você terá a seguinte arquitetura:

- Frontend [Blazor](https://blazor.net) para a interface de chat
- Backend [ASP.NET](https://asp.net) com [Microsoft Agent Framework](https://aka.ms/agent-framework)
- [Microsoft Foundry Agent Service](https://aka.ms/microsoft-foundry/agent-service) para hospedagem de agentes
- [Aspire](https://aspire.dev) para orquestração de aplicações cloud-native

> [!NOTE]
> A versão .NET do SDK do Microsoft Agent Framework suportará o padrão Magentic em uma versão futura.

## Pré-requisitos

- [Assinatura Azure (gratuita)](http://azure.microsoft.com/free)
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) ou superior
- [Visual Studio 2026](https://visualstudio.microsoft.com/downloads/) ou [VS Code](https://code.visualstudio.com/download) + [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
- [Docker Desktop](https://docs.docker.com/desktop/) ou equivalente
- [GitHub CLI](https://cli.github.com)
- [Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd)
- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli)
- [Aspire CLI](https://aspire.dev/get-started/install-cli/)

## Sessões do workshop

| Sessão                      | Documento                                                   | Exemplo de Código                                        |
|-----------------------------|-------------------------------------------------------------|----------------------------------------------------------|
| 00 Configuração             | [00-setup.md](./docs/00-setup.md)                           |                                                          |
| 01 Padrão Sequential        | [01-sequential-pattern.md](./docs/01-sequential-pattern.md) | [01-sequential-pattern](./samples/01-sequential-pattern) |
| 02 Padrão Concurrent        | [02-concurrent-pattern.md](./docs/02-concurrent-pattern.md) | [02-concurrent-pattern](./samples/02-concurrent-pattern) |
| 03 Padrão Handoff           | [03-handoff-pattern.md](./docs/03-handoff-pattern.md)       | [03-handoff-pattern](./samples/03-handoff-pattern)       |
| 04 Padrão Group Chat        | [04-group-chat-pattern.md](./docs/04-group-chat-pattern.md) | [04-group-chat-pattern](./samples/04-group-chat-pattern) |

## Use seu idioma preferido!

Este material de workshop está disponível nos seguintes idiomas.

[English](../../README.md) | [Español](../es-es/README.md) | [日本語](../ja-jp/README.md) | [한국어](../ko-kr/README.md) | [Português](./README.md) | [中文(简体)](../zh-cn/README.md)

## Recursos

- [Microsoft Agent Framework](https://aka.ms/agent-framework)
- [Microsoft Agent Framework - Workflow Orchestrations](https://learn.microsoft.com/agent-framework/workflows/orchestrations)
- [Microsoft Foundry](https://aka.ms/microsoft-foundry)
- [Microsoft Foundry Agent Service](https://aka.ms/microsoft-foundry/agent-service)
- [Model Context Protocol (MCP)](https://modelcontextprotocol.io)
- [Aspire](https://aspire.dev)
