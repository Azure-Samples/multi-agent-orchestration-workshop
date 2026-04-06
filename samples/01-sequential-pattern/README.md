# 01 Sequential Pattern

In a sequential pattern, agents work one after another in a defined pipeline, where each agent's output feeds into the next. This approach works well for tasks that follow a natural progression, like content creation workflows, staged data transformations, or step-by-step analysis.

```mermaid
graph TB
    User([User]) -->|Chat message| WebUI

    subgraph Aspire["Aspire AppHost"]
        WebUI["WebUI<br/>(Blazor Server)"]
        Agent["Agent Service<br/>(ASP.NET Core)"]

        WebUI -->|"AG-UI Protocol"| Agent
    end

    subgraph Foundry["Microsoft Foundry"]
        A1["research-agent"] -->|research brief| A2["outliner-agent"]
        A2 -->|outline| A3["writer-agent"]
        A3 -->|draft post| A4["editor-agent"]
    end

    Agent -->|"Sequential workflow"| A1
    A4 -->|final result| Agent
    WebUI -->|Rendered output| User
```

## Instruction

Follow the instruction, [01-sequential-pattern.md](../../docs/01-sequential-pattern.md) with the [start](./start) project.

Once you complete, compare yours to the [complete](./complete) project.

