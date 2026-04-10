$REPOSITORY_ROOT = git rev-parse --show-toplevel

$secrets = @{}
dotnet user-secrets `
    --project "$REPOSITORY_ROOT/samples/04-group-chat-pattern/start/src/MultiAgentWorkshop.AppHost/MultiAgentWorkshop.AppHost.csproj" list | `
    ForEach-Object {
        $parts = $_ -split ' = ', 2
        $secrets[$parts[0]] = $parts[1]
    }

$foundryName = $secrets["FOUNDRY_NAME"]
$foundryProjectName = $secrets["FOUNDRY_PROJECT_NAME"]
$foundryResourceGroup = $secrets["FOUNDRY_RESOURCE_GROUP"]

$userAssignedIdentityName = azd env get-value MANAGED_IDENTITY_NAME
$resourceGroup = "rg-$(azd env get-value AZURE_ENV_NAME)"

$azureAIUserRoleId = "53ca6127-db72-4b80-b1b0-d745d6d5456d"
$cognitiveServicesUserRoleId = "a97b65f3-24c7-4388-baec-2e87135dc908"

$principalId = az identity show `
    --name $userAssignedIdentityName `
    --resource-group $resourceGroup `
    --query principalId -o tsv

$foundryResourceId = az cognitiveservices account show `
    --name $foundryName `
    --resource-group $foundryResourceGroup `
    --query id -o tsv

$azureAIUserRole = az role assignment create `
    --assignee $principalId `
    --role $azureAIUserRoleId `
    --scope $foundryResourceId

$cognitiveServicesUserRole = az role assignment create `
    --assignee $principalId `
    --role $cognitiveServicesUserRoleId `
    --scope $foundryResourceId
