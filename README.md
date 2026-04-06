# Multi-Agent Orchestration Workshop

This is a workshop material to build .NET-based multi-agent apps using [Microsoft Agent Framework](https://aka.ms/agent-framework), [Microsoft Foundry](https://aka.ms/microsoft-foundry) with [Aspire](https://aspire.dev).

## What is this workshop for?

Building a single-agent app is easy. But there are many real-world use cases that requires multiple agents working together, and building a multi-agent app is not as simple as building a single-agent one. [Microsoft Agent Framework](https://aka.ms/agent-framework) offers five multi-agent orchestration patterns:

| Pattern                                                                                                                          | Description                                                |
|----------------------------------------------------------------------------------------------------------------------------------|------------------------------------------------------------|
| [Sequential](https://learn.microsoft.com/agent-framework/workflows/orchestrations/sequential?pivots=programming-language-csharp) | Agents execute one after another in a defined order        |
| [Concurrent](https://learn.microsoft.com/agent-framework/workflows/orchestrations/concurrent?pivots=programming-language-csharp) | Agents execute in parallel                                 |
| [Handoff](https://learn.microsoft.com/agent-framework/workflows/orchestrations/handoff?pivots=programming-language-csharp)       | Agents transfer control to each other based on context     |
| [Group Chat](https://learn.microsoft.com/agent-framework/workflows/orchestrations/group-chat?pivots=programming-language-csharp) | Agents collaborate in a shared conversation                |
| [Magentic](https://learn.microsoft.com/agent-framework/workflows/orchestrations/magentic?pivots=programming-language-python)     | A manager agent dynamically coordinates specialized agents |

## Features

In this workshop, we're building all multi-agent orchestration patterns except the Magentic pattern. Once you complete each pattern, you'll have the following architecture:

- [Blazor](https://blazor.net) frontend for chat UI
- [ASP.NET](https://asp.net) backend with [Microsoft Agent Framework](https://aka.ms/agent-framework)
- [Microsoft Foundry Agent Service](https://aka.ms/microsoft-foundry/agent-service) for agent hosting
- [Aspire](https://aspire.dev) for cloud-native app orchestration

## Prerequisites

- [Azure subscription (free)](http://azure.microsoft.com/free)
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or higher
- [Visual Studio 2026](https://visualstudio.microsoft.com/downloads/) or [VS Code](https://code.visualstudio.com/download) + [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
- [Docker Desktop](https://docs.docker.com/desktop/) or equivalent
- [Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd)
- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli)
- [Aspire CLI](https://aspire.dev/get-started/install-cli/)

## Workshop sessions

| Session               | Document                                                         | Code Sample                                                               |
|-----------------------|------------------------------------------------------------------|---------------------------------------------------------------------------|
| 00 Setup              | [docs/00-setup.md](./docs/00-setup.md)                           |                                                                           |
| 01 Sequential Pattern | [docs/01-sequential-pattern.md](./docs/01-sequential-pattern.md) | [samples/01-sequential-pattern](./samples/01-sequential-pattern/complete) |
| 02 Concurrent Pattern | [docs/02-concurrent-pattern.md](./docs/02-concurrent-pattern.md) | [samples/02-concurrent-pattern](./samples/02-concurrent-pattern/complete) |
| 03 Handoff Pattern    | [docs/03-handoff-pattern.md](./docs/03-handoff-pattern.md)       | [samples/03-handoff-pattern](./samples/03-handoff-pattern/complete)       |
| 04 Group Chat Pattern | [docs/04-group-chat-pattern.md](./docs/04-group-chat-pattern.md) | [samples/04-group-chat-pattern](./samples/04-group-chat-pattern/complete) |

## Use your local language!

This workshop materials are provided with your local languages.

[English](./README.md) | [中文(简体)](./localization/zh-cn/README.md) | [Español](./localization/es/README.md) | [日本語](./localization/ja/README.md) | [한국어](./localization/ko/README.md)

## Resources

- [Microsoft Agent Framework](https://aka.ms/agent-framework)
- [Microsoft Agent Framework - Workflow Orchestrations](https://learn.microsoft.com/agent-framework/workflows/orchestrations)
- [Microsoft Foundry](https://aka.ms/microsoft-foundry)
- [Microsoft Foundry Agent Service](https://aka.ms/microsoft-foundry/agent-service)
- [Model Context Protocol (MCP)](https://modelcontextprotocol.io)
- [Aspire](https://aspire.dev)
