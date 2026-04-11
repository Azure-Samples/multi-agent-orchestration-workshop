# 멀티 에이전트 오케스트레이션 워크숍

이 워크숍은 [Microsoft Agent Framework](https://aka.ms/agent-framework), [Microsoft Foundry](https://aka.ms/microsoft-foundry), [Aspire](https://aspire.dev)를 활용하여 .NET 기반 멀티 에이전트 앱을 구축하는 실습 자료입니다.

![MAF + Foundry 워크숍](../../images/hero.jpg)

## 이 워크숍은 무엇을 위한 것인가요?

단일 에이전트 앱을 만드는 것은 쉽습니다. 하지만 실제로는 여러 에이전트가 함께 협력해야 하는 사례가 많으며, 멀티 에이전트 앱을 구축하는 것은 단일 에이전트 앱만큼 간단하지 않습니다. [Microsoft Agent Framework](https://aka.ms/agent-framework)는 다섯 가지 멀티 에이전트 오케스트레이션 패턴을 제공합니다:

| 패턴                                                                                                                              | 설명                                                    |
|----------------------------------------------------------------------------------------------------------------------------------|------------------------------------------------------------|
| [Sequential](https://learn.microsoft.com/agent-framework/workflows/orchestrations/sequential?pivots=programming-language-csharp) | 에이전트가 정의된 순서대로 하나씩 차례로 실행됩니다        |
| [Concurrent](https://learn.microsoft.com/agent-framework/workflows/orchestrations/concurrent?pivots=programming-language-csharp) | 에이전트가 병렬로 실행됩니다                                 |
| [Handoff](https://learn.microsoft.com/agent-framework/workflows/orchestrations/handoff?pivots=programming-language-csharp)       | 에이전트가 컨텍스트에 따라 서로 제어권을 넘깁니다     |
| [Group Chat](https://learn.microsoft.com/agent-framework/workflows/orchestrations/group-chat?pivots=programming-language-csharp) | 에이전트가 공유 대화에서 협업합니다                |
| [Magentic](https://learn.microsoft.com/agent-framework/workflows/orchestrations/magentic?pivots=programming-language-python)     | 관리자 에이전트가 전문 에이전트를 동적으로 조율합니다 |

## 특징

이 워크숍에서는 Magentic 패턴을 제외한 모든 멀티 에이전트 오케스트레이션 패턴을 구축합니다. 각 패턴을 완료하면 다음과 같은 아키텍처를 갖게 됩니다:

- [Blazor](https://blazor.net) 채팅 UI용 프론트엔드
- [ASP.NET](https://asp.net) 백엔드 + [Microsoft Agent Framework](https://aka.ms/agent-framework)
- [Microsoft Foundry Agent Service](https://aka.ms/microsoft-foundry/agent-service) 에이전트 호스팅
- [Aspire](https://aspire.dev) 클라우드 네이티브 앱 오케스트레이션

> [!NOTE]
> Microsoft Agent Framework SDK의 .NET 버전은 향후 릴리스에서 Magentic 패턴을 지원할 예정입니다.

## 사전 요구 사항

- [Azure 구독 (무료)](http://azure.microsoft.com/free)
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) 이상
- [Visual Studio 2026](https://visualstudio.microsoft.com/downloads/) 또는 [VS Code](https://code.visualstudio.com/download) + [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
- [Docker Desktop](https://docs.docker.com/desktop/) 또는 동등한 도구
- [GitHub CLI](https://cli.github.com)
- [Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd)
- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli)
- [Aspire CLI](https://aspire.dev/get-started/install-cli/)

## 워크숍 세션

| 세션                  | 문서                                                        | 코드 샘플                                                |
|-----------------------|-------------------------------------------------------------|----------------------------------------------------------|
| 00 설정               | [00-setup.md](./docs/00-setup.md)                           |                                                          |
| 01 Sequential 패턴    | [01-sequential-pattern.md](./docs/01-sequential-pattern.md) | [01-sequential-pattern](./samples/01-sequential-pattern) |
| 02 Concurrent 패턴    | [02-concurrent-pattern.md](./docs/02-concurrent-pattern.md) | [02-concurrent-pattern](./samples/02-concurrent-pattern) |
| 03 Handoff 패턴       | [03-handoff-pattern.md](./docs/03-handoff-pattern.md)       | [03-handoff-pattern](./samples/03-handoff-pattern)       |
| 04 Group Chat 패턴    | [04-group-chat-pattern.md](./docs/04-group-chat-pattern.md) | [04-group-chat-pattern](./samples/04-group-chat-pattern) |

## 원하는 언어를 선택하세요!

이 워크숍 자료는 다음 언어로 제공됩니다.

[English](../../README.md) | [Español](../es-es/README.md) | [日本語](../ja-jp/README.md) | [한국어](./README.md) | [Português](../pt-br/README.md) | [中文(简体)](../zh-cn/README.md)

## 리소스

- [Microsoft Agent Framework](https://aka.ms/agent-framework)
- [Microsoft Agent Framework - Workflow Orchestrations](https://learn.microsoft.com/agent-framework/workflows/orchestrations)
- [Microsoft Foundry](https://aka.ms/microsoft-foundry)
- [Microsoft Foundry Agent Service](https://aka.ms/microsoft-foundry/agent-service)
- [Model Context Protocol (MCP)](https://modelcontextprotocol.io)
- [Aspire](https://aspire.dev)
