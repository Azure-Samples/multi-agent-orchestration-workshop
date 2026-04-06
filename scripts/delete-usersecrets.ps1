$REPOSITORY_ROOT = git rev-parse --show-toplevel

dotnet user-secrets `
    --project "$REPOSITORY_ROOT/src/MultiAgentWorkshop.AppHost/MultiAgentWorkshop.AppHost.csproj" list | `
    ForEach-Object {
        $parts = $_ -split ' = ', 2

        dotnet user-secrets `
            --project "$REPOSITORY_ROOT/src/MultiAgentWorkshop.AppHost/MultiAgentWorkshop.AppHost.csproj" `
            remove $parts[0]
    }
