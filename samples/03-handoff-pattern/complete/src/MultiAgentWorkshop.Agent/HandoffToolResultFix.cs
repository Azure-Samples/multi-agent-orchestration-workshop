// ============================================================================
// TEMPORARY WORKAROUND — Remove when upstream fix is released.
//
// Upstream issues:
//   https://github.com/microsoft/agent-framework/issues/2775
//   https://github.com/microsoft/agent-framework/issues/3962
//
// Problem:
//   Handoff tools return plain string content (e.g. "Transferred.") which
//   causes AGUIChatClient to throw JsonException in
//   DeserializeResultIfAvailable.
//
// Fix:
//   Wraps the workflow AIAgent with a streaming middleware that converts any
//   plain-string FunctionResultContent.Result values to JsonElement before
//   the AGUI serialization pipeline processes them.
// ============================================================================

using System.Runtime.CompilerServices;
using System.Text.Json;

using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

/// <summary>
/// Temporary workaround for microsoft/agent-framework#2775.
/// Wraps a handoff workflow <see cref="AIAgent"/> so that plain-string tool
/// results are serialized to <see cref="JsonElement"/> before the AG-UI
/// pipeline processes them. Safe to remove once the upstream fix ships.
/// </summary>
internal static class HandoffToolResultFix
{
    public static AIAgent CreateFixedAgent(this AIAgent innerAgent)
    {
        return new AIAgentBuilder(innerAgent)
            .Use(inner => new HandoffToolResultFixAgent(inner))
            .Build();
    }
}

internal sealed class HandoffToolResultFixAgent(AIAgent inner) : DelegatingAIAgent(inner)
{
    protected override async IAsyncEnumerable<AgentResponseUpdate> RunCoreStreamingAsync(
        IEnumerable<ChatMessage> messages,
        AgentSession? session = null,
        AgentRunOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var update in base.RunCoreStreamingAsync(messages, session, options, cancellationToken))
        {
            foreach (var content in update.Contents)
            {
                if (content is FunctionResultContent frc && frc.Result is string s)
                {
                    frc.Result = JsonSerializer.SerializeToElement(s);
                }
            }

            yield return update;
        }
    }
}
