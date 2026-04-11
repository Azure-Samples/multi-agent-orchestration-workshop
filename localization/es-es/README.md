# Taller de Orquestación Multi-Agente

Este es material de taller para construir aplicaciones multi-agente basadas en .NET utilizando [Microsoft Agent Framework](https://aka.ms/agent-framework), [Microsoft Foundry](https://aka.ms/microsoft-foundry), y [Aspire](https://aspire.dev).

![Taller MAF + Foundry](../../images/hero.jpg)

## ¿Para qué es este taller?

Construir una aplicación de un solo agente es fácil. Sin embargo, hay muchos casos de uso del mundo real que requieren múltiples agentes trabajando juntos, y construir una aplicación multi-agente no es tan simple como construir una de un solo agente. [Microsoft Agent Framework](https://aka.ms/agent-framework) ofrece cinco patrones de orquestación multi-agente:

| Patrón                                                                                                                           | Descripción                                                          |
|----------------------------------------------------------------------------------------------------------------------------------|----------------------------------------------------------------------|
| [Secuencial](https://learn.microsoft.com/agent-framework/workflows/orchestrations/sequential?pivots=programming-language-csharp) | Los agentes se ejecutan uno tras otro en un orden definido           |
| [Concurrente](https://learn.microsoft.com/agent-framework/workflows/orchestrations/concurrent?pivots=programming-language-csharp) | Los agentes se ejecutan en paralelo                                  |
| [Handoff](https://learn.microsoft.com/agent-framework/workflows/orchestrations/handoff?pivots=programming-language-csharp)       | Los agentes transfieren el control entre sí según el contexto        |
| [Group Chat](https://learn.microsoft.com/agent-framework/workflows/orchestrations/group-chat?pivots=programming-language-csharp) | Los agentes colaboran en una conversación compartida                 |
| [Magentic](https://learn.microsoft.com/agent-framework/workflows/orchestrations/magentic?pivots=programming-language-python)     | Un agente administrador coordina dinámicamente agentes especializados |

## Características

En este taller, construimos todos los patrones de orquestación multi-agente excepto el patrón Magentic. Una vez que complete cada patrón, tendrá la siguiente arquitectura:

- Frontend [Blazor](https://blazor.net) para la interfaz de chat
- Backend [ASP.NET](https://asp.net) con [Microsoft Agent Framework](https://aka.ms/agent-framework)
- [Microsoft Foundry Agent Service](https://aka.ms/microsoft-foundry/agent-service) para el alojamiento de agentes
- [Aspire](https://aspire.dev) para la orquestación de aplicaciones nativas en la nube

> [!NOTE]
> La versión .NET del SDK de Microsoft Agent Framework admitirá el patrón Magentic en una próxima versión.

## Requisitos previos

- [Suscripción de Azure (gratuita)](http://azure.microsoft.com/free)
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) o superior
- [Visual Studio 2026](https://visualstudio.microsoft.com/downloads/) o [VS Code](https://code.visualstudio.com/download) + [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
- [Docker Desktop](https://docs.docker.com/desktop/) o equivalente
- [GitHub CLI](https://cli.github.com)
- [Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd)
- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli)
- [Aspire CLI](https://aspire.dev/get-started/install-cli/)

## Sesiones del taller

| Sesión                    | Documento                                                   | Ejemplo de código                                        |
|---------------------------|-------------------------------------------------------------|----------------------------------------------------------|
| 00 Configuración          | [00-setup.md](./docs/00-setup.md)                           |                                                          |
| 01 Patrón Secuencial      | [01-sequential-pattern.md](./docs/01-sequential-pattern.md) | [01-sequential-pattern](./samples/01-sequential-pattern) |
| 02 Patrón Concurrente     | [02-concurrent-pattern.md](./docs/02-concurrent-pattern.md) | [02-concurrent-pattern](./samples/02-concurrent-pattern) |
| 03 Patrón Handoff         | [03-handoff-pattern.md](./docs/03-handoff-pattern.md)       | [03-handoff-pattern](./samples/03-handoff-pattern)       |
| 04 Patrón Group Chat      | [04-group-chat-pattern.md](./docs/04-group-chat-pattern.md) | [04-group-chat-pattern](./samples/04-group-chat-pattern) |

## ¡Use su idioma preferido!

Este material de taller está disponible en los siguientes idiomas.

[English](../../README.md) | [Español](./README.md) | [日本語](../ja-jp/README.md) | [한국어](../ko-kr/README.md) | [Português](../pt-br/README.md) | [中文(简体)](../zh-cn/README.md)

## Recursos

- [Microsoft Agent Framework](https://aka.ms/agent-framework)
- [Microsoft Agent Framework - Workflow Orchestrations](https://learn.microsoft.com/agent-framework/workflows/orchestrations)
- [Microsoft Foundry](https://aka.ms/microsoft-foundry)
- [Microsoft Foundry Agent Service](https://aka.ms/microsoft-foundry/agent-service)
- [Model Context Protocol (MCP)](https://modelcontextprotocol.io)
- [Aspire](https://aspire.dev)
