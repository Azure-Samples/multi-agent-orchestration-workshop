#!/bin/bash

REPOSITORY_ROOT=$(git rev-parse --show-toplevel)

secrets="$REPOSITORY_ROOT/samples/02-concurrent-pattern/complete/src/MultiAgentWorkshop.AppHost/MultiAgentWorkshop.AppHost.csproj"

foundryName=$(dotnet user-secrets --project "$secrets" list \
    | grep '^FOUNDRY_NAME ' | sed 's/^FOUNDRY_NAME = //')
foundryProjectName=$(dotnet user-secrets --project "$secrets" list \
    | grep '^FOUNDRY_PROJECT_NAME ' | sed 's/^FOUNDRY_PROJECT_NAME = //')
foundryResourceGroup=$(dotnet user-secrets --project "$secrets" list \
    | grep '^FOUNDRY_RESOURCE_GROUP ' | sed 's/^FOUNDRY_RESOURCE_GROUP = //')

resourceGroup="rg-$(azd env get-value AZURE_ENV_NAME)"
userAssignedIdentityName=$(azd env get-value MANAGED_IDENTITY_NAME)
containerAppsEnvironmentName=$(azd env get-value AZURE_CONTAINER_APPS_ENVIRONMENT_NAME)

azureAIUserRoleId="53ca6127-db72-4b80-b1b0-d745d6d5456d"
cognitiveServicesUserRoleId="a97b65f3-24c7-4388-baec-2e87135dc908"
contributorRoleId="b24988ac-6180-42a0-ab88-20f7382dd24c"

currentUserPrincipalId=$(az ad signed-in-user show --query id -o tsv)

principalId=$(az identity show \
    --name "$userAssignedIdentityName" \
    --resource-group "$resourceGroup" \
    --query principalId -o tsv)

foundryResourceId=$(az cognitiveservices account show \
    --name "$foundryName" \
    --resource-group "$foundryResourceGroup" \
    --query id -o tsv)

containerAppsEnvironmentId=$(az containerapp env show \
    --name "$containerAppsEnvironmentName" \
    --resource-group "$resourceGroup" \
    --query id -o tsv)

az role assignment create \
    --assignee "$principalId" \
    --role "$azureAIUserRoleId" \
    --scope "$foundryResourceId" > /dev/null

az role assignment create \
    --assignee "$principalId" \
    --role "$cognitiveServicesUserRoleId" \
    --scope "$foundryResourceId" > /dev/null

az role assignment create \
    --assignee "$currentUserPrincipalId" \
    --role "$contributorRoleId" \
    --scope "$containerAppsEnvironmentId" > /dev/null
