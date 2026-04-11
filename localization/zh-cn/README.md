# Multi-Agent Orchestration Workshop

这是一份使用 [Microsoft Agent Framework](https://aka.ms/agent-framework)、[Microsoft Foundry](https://aka.ms/microsoft-foundry) 和 [Aspire](https://aspire.dev) 构建基于 .NET 的多智能体应用的工作坊教材。

![MAF + Foundry 工作坊](../../images/hero.jpg)

## 这个工作坊是做什么的？

构建单智能体应用很简单。但在许多真实场景中，需要多个智能体协同工作，而构建多智能体应用远不如构建单智能体那么简单。[Microsoft Agent Framework](https://aka.ms/agent-framework) 提供了五种多智能体编排模式：

| 模式                                                                                                                              | 描述                                                |
|----------------------------------------------------------------------------------------------------------------------------------|------------------------------------------------------------|
| [顺序模式（Sequential）](https://learn.microsoft.com/agent-framework/workflows/orchestrations/sequential?pivots=programming-language-csharp) | 智能体按预定义的顺序依次执行        |
| [并发模式（Concurrent）](https://learn.microsoft.com/agent-framework/workflows/orchestrations/concurrent?pivots=programming-language-csharp) | 智能体并行执行                                 |
| [交接模式（Handoff）](https://learn.microsoft.com/agent-framework/workflows/orchestrations/handoff?pivots=programming-language-csharp)       | 智能体根据上下文将控制权传递给彼此     |
| [群聊模式（Group Chat）](https://learn.microsoft.com/agent-framework/workflows/orchestrations/group-chat?pivots=programming-language-csharp) | 智能体在共享对话中协作                |
| [Magentic](https://learn.microsoft.com/agent-framework/workflows/orchestrations/magentic?pivots=programming-language-python)     | 管理者智能体动态协调专业智能体 |

## 功能特性

在本工作坊中，我们将构建除 Magentic 模式以外的所有多智能体编排模式。完成每个模式后，您将获得以下架构：

- [Blazor](https://blazor.net) 前端聊天界面
- [ASP.NET](https://asp.net) 后端搭配 [Microsoft Agent Framework](https://aka.ms/agent-framework)
- [Microsoft Foundry Agent Service](https://aka.ms/microsoft-foundry/agent-service) 用于智能体托管
- [Aspire](https://aspire.dev) 用于云原生应用编排

> [!NOTE]
> Microsoft Agent Framework SDK 的 .NET 版本将在后续版本中支持 Magentic 模式。

## 前提条件

- [Azure 订阅（免费）](http://azure.microsoft.com/free)
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) 或更高版本
- [Visual Studio 2026](https://visualstudio.microsoft.com/downloads/) 或 [VS Code](https://code.visualstudio.com/download) + [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
- [Docker Desktop](https://docs.docker.com/desktop/) 或等效工具
- [GitHub CLI](https://cli.github.com)
- [Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd)
- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli)
- [Aspire CLI](https://aspire.dev/get-started/install-cli/)

## 工作坊课程

| 课程               | 文档                                                    | 代码示例                                              |
|-----------------------|-------------------------------------------------------------|----------------------------------------------------------|
| 00 环境搭建              | [00-setup.md](./docs/00-setup.md)                           |                                                          |
| 01 顺序模式 | [01-sequential-pattern.md](./docs/01-sequential-pattern.md) | [01-sequential-pattern](./samples/01-sequential-pattern) |
| 02 并发模式 | [02-concurrent-pattern.md](./docs/02-concurrent-pattern.md) | [02-concurrent-pattern](./samples/02-concurrent-pattern) |
| 03 交接模式    | [03-handoff-pattern.md](./docs/03-handoff-pattern.md)       | [03-handoff-pattern](./samples/03-handoff-pattern)       |
| 04 群聊模式 | [04-group-chat-pattern.md](./docs/04-group-chat-pattern.md) | [04-group-chat-pattern](./samples/04-group-chat-pattern) |

## 选择您的首选语言！

本工作坊教材提供以下语言版本。

[English](../../README.md) | [Español](../es-es/README.md) | [日本語](../ja-jp/README.md) | [한국어](../ko-kr/README.md) | [Português](../pt-br/README.md) | [中文(简体)](./README.md)

## 资源

- [Microsoft Agent Framework](https://aka.ms/agent-framework)
- [Microsoft Agent Framework - 工作流编排](https://learn.microsoft.com/agent-framework/workflows/orchestrations)
- [Microsoft Foundry](https://aka.ms/microsoft-foundry)
- [Microsoft Foundry Agent Service](https://aka.ms/microsoft-foundry/agent-service)
- [Model Context Protocol (MCP)](https://modelcontextprotocol.io)
- [Aspire](https://aspire.dev)
