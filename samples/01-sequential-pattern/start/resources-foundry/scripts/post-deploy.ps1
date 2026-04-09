$REPOSITORY_ROOT = git rev-parse --show-toplevel

pushd "$REPOSITORY_ROOT/samples/01-sequential-pattern/start/resources-foundry"

$tenantId = azd env get-value AZURE_TENANT_ID

$foundryName = azd env get-value FOUNDRY_NAME
$foundryProjectName = azd env get-value FOUNDRY_PROJECT_NAME
$foundryResourceGroup = "rg-$(azd env get-value AZURE_ENV_NAME)"

$projectEndpoint = azd env get-value FOUNDRY_PROJECT_ENDPOINT

dotnet user-secrets --file "$REPOSITORY_ROOT/samples/01-sequential-pattern/start/src/MultiAgentWorkshop.PromptAgent/create-agents.cs" set "AZURE_TENANT_ID" $tenantId

dotnet user-secrets --file "$REPOSITORY_ROOT/samples/01-sequential-pattern/start/src/MultiAgentWorkshop.PromptAgent/create-agents.cs" set "FOUNDRY_NAME" $foundryName
dotnet user-secrets --file "$REPOSITORY_ROOT/samples/01-sequential-pattern/start/src/MultiAgentWorkshop.PromptAgent/create-agents.cs" set "FOUNDRY_PROJECT_NAME" $foundryProjectName
dotnet user-secrets --file "$REPOSITORY_ROOT/samples/01-sequential-pattern/start/src/MultiAgentWorkshop.PromptAgent/create-agents.cs" set "FOUNDRY_RESOURCE_GROUP" $foundryResourceGroup

dotnet user-secrets --file "$REPOSITORY_ROOT/samples/01-sequential-pattern/start/src/MultiAgentWorkshop.PromptAgent/create-agents.cs" set "Foundry:Project:Endpoint" $projectEndpoint

dotnet run --file "$REPOSITORY_ROOT/samples/01-sequential-pattern/start/src/MultiAgentWorkshop.PromptAgent/create-agents.cs"

popd
