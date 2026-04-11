# 03 交接模式

在交接模式中，智能体根据对话上下文动态地将控制权传递给另一个智能体。分诊智能体接收初始请求，并将其路由到最适合处理该请求的专家智能体。专家智能体之间也可以在问题跨领域时相互重定向。这非常适合 IT 支持、客户服务等场景，或任何在不同阶段需要不同专业知识的工作流。

<div>
  <img src="../../../../docs/images/03-handoff-pattern-architecture.png" alt="架构 - 交接模式" width="640" />
</div>

## 操作说明

按照 [03-handoff-pattern.md](../../docs/03-handoff-pattern.md) 中的说明，使用 [start](../../../../samples/03-handoff-pattern/start) 项目进行操作。

完成后，将您的成果与 [complete](../../../../samples/03-handoff-pattern/complete) 项目进行对比。
