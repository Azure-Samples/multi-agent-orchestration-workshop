@description('Name of the environment that can be used as part of naming resource convention')
param environmentName string

@description('The location used for all deployed resources')
param location string = resourceGroup().location

@description('Tags that will be applied to all resources')
param tags object = {}

param mcpTodoExists bool

@description('Id of the user or app to assign application roles')
param principalId string

@description('Principal type of user or app')
param principalType string

@description('The SKU for the Azure OpenAI resource')
@allowed(['S0'])
param sku string = 'S0'

@description('Disallow key-based authentication for the Azure OpenAI resource. Should be disabled in production environments in favor of managed identities')
param disableLocalAuth bool = true

@description('Deploy GPT model automatically')
param deployGptModel bool = true

@description('GPT model to deploy')
param gptModelName string = 'gpt-5-mini'

@description('GPT model version')
param gptModelVersion string = '2025-08-07'

@description('GPT deployment capacity')
param gptCapacity int = 10

var abbrs = loadJsonContent('./abbreviations.json')
var resourceToken = uniqueString(subscription().id, resourceGroup().id, location)

var acrPullRoleId = '7f951dda-4ed3-4680-a7ca-43fe172d538d'
var azureAIUserRoleId = '53ca6127-db72-4b80-b1b0-d745d6d5456d'
var cognitiveServicesUserRoleId = 'a97b65f3-24c7-4388-baec-2e87135dc908'

// Monitor application with Azure Monitor
module monitoring 'br/public:avm/ptn/azd/monitoring:0.1.0' = {
  name: 'monitoring'
  params: {
    logAnalyticsName: '${abbrs.operationalInsightsWorkspaces}${resourceToken}'
    applicationInsightsName: '${abbrs.insightsComponents}${resourceToken}'
    applicationInsightsDashboardName: '${abbrs.portalDashboards}${resourceToken}'
    location: location
    tags: tags
  }
}

// Container registry
module containerRegistry 'br/public:avm/res/container-registry/registry:0.1.1' = {
  name: 'registry'
  params: {
    name: '${abbrs.containerRegistryRegistries}${resourceToken}'
    location: location
    tags: tags
    publicNetworkAccess: 'Enabled'
    roleAssignments: [
      {
        principalId: mcpTodoIdentity.outputs.principalId
        principalType: 'ServicePrincipal'
        roleDefinitionIdOrName: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', acrPullRoleId)
      }
    ]
  }
}

// Container apps environment
module containerAppsEnvironment 'br/public:avm/res/app/managed-environment:0.4.5' = {
  name: 'container-apps-environment'
  params: {
    logAnalyticsWorkspaceResourceId: monitoring.outputs.logAnalyticsWorkspaceResourceId
    name: '${abbrs.appManagedEnvironments}${resourceToken}'
    location: location
    zoneRedundant: false
  }
}

// User assigned identity
module mcpTodoIdentity 'br/public:avm/res/managed-identity/user-assigned-identity:0.2.1' = {
  name: 'mcp-todo-identity'
  params: {
    name: '${abbrs.managedIdentityUserAssignedIdentities}mcp-todo-${resourceToken}'
    location: location
  }
}

// Azure Container Apps
module mcpTodoFetchLatestImage './modules/fetch-container-image.bicep' = {
  name: 'mcp-todo-fetch-image'
  params: {
    exists: mcpTodoExists
    name: 'mcp-todo'
  }
}

module mcpTodo 'br/public:avm/res/app/container-app:0.8.0' = {
  name: 'mcp-todo'
  params: {
    name: 'mcp-todo'
    ingressTargetPort: 8080
    scaleMinReplicas: 1
    scaleMaxReplicas: 10
    secrets: {
      secureList: []
    }
    containers: [
      {
        image: mcpTodoFetchLatestImage.outputs.?containers[?0].?image ?? 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'
        name: 'main'
        resources: {
          cpu: json('0.5')
          memory: '1.0Gi'
        }
        env: [
          {
            name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
            value: monitoring.outputs.applicationInsightsConnectionString
          }
          {
            name: 'AZURE_CLIENT_ID'
            value: mcpTodoIdentity.outputs.clientId
          }
          {
            name: 'PORT'
            value: '8080'
          }
        ]
      }
    ]
    managedIdentities: {
      systemAssigned: false
      userAssignedResourceIds: [
        mcpTodoIdentity.outputs.resourceId
      ]
    }
    registries: [
      {
        server: containerRegistry.outputs.loginServer
        identity: mcpTodoIdentity.outputs.resourceId
      }
    ]
    environmentResourceId: containerAppsEnvironment.outputs.resourceId
    location: location
    tags: union(tags, { 'azd-service-name': 'mcp-todo' })
  }
}

// Deploy Microsoft Foundry resources
resource foundry 'Microsoft.CognitiveServices/accounts@2025-10-01-preview' = {
  name: 'foundry-${resourceToken}'
  location: location
  tags: tags
  kind: 'AIServices'
  identity: {
    type: 'SystemAssigned'
  }
  sku: {
    name: sku
  }
  properties: {
    customSubDomainName: 'foundry-${resourceToken}'
    networkAcls: {
      defaultAction: 'Allow'
      bypass: 'AzureServices'
    }
    publicNetworkAccess: 'Enabled'
    restrictOutboundNetworkAccess: false
    disableLocalAuth: disableLocalAuth
    allowProjectManagement: true
  }
}

resource foundryproject 'Microsoft.CognitiveServices/accounts/projects@2025-10-01-preview' = {
  parent: foundry
  name: 'proj-${resourceToken}'
  location: location
  tags: tags
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    displayName: 'proj-${environmentName}'
    description: 'Foundry project for managing AI resources in the ${environmentName} environment'
  }
}

resource foundrydeployment 'Microsoft.CognitiveServices/accounts/deployments@2025-10-01-preview' = {
  parent: foundry
  name: gptModelName
  tags: tags
  sku: {
    name: 'GlobalStandard'
    capacity: gptCapacity
  }
  properties: {
    model: {
      format: 'OpenAI'
      name: gptModelName
      version: gptModelVersion
    }
  }
}

resource azureAIUserRole 'Microsoft.Authorization/roleDefinitions@2022-05-01-preview' existing = {
  name: azureAIUserRoleId
  scope: resourceGroup()
}

resource localAzureAIUserRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: foundry
  name: guid(foundry.id, foundryproject.id, azureAIUserRole.id, principalId)
  properties: {
    principalId: principalId
    roleDefinitionId: azureAIUserRole.id
    principalType: principalType
  }
}

resource cloudAzureAIUserRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: foundry
  name: guid(foundry.id, foundryproject.id, azureAIUserRole.id, foundryproject.name)
  properties: {
    principalId: foundryproject.identity.principalId
    roleDefinitionId: azureAIUserRole.id
    principalType: 'ServicePrincipal'
  }
}

resource cognitiveServicesUserRole 'Microsoft.Authorization/roleDefinitions@2022-05-01-preview' existing = {
  name: cognitiveServicesUserRoleId
  scope: resourceGroup()
}

resource localCognitiveServicesUserRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: foundry
  name: guid(foundry.id, foundryproject.id, cognitiveServicesUserRole.id, principalId)
  properties: {
    principalId: principalId
    roleDefinitionId: cognitiveServicesUserRole.id
    principalType: principalType
  }
}

resource cloudCognitiveServicesUserRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: foundry
  name: guid(foundry.id, foundryproject.id, cognitiveServicesUserRole.id, foundryproject.name)
  properties: {
    principalId: foundryproject.identity.principalId
    roleDefinitionId: cognitiveServicesUserRole.id
    principalType: 'ServicePrincipal'
  }
}

// Outputs
output AZURE_CONTAINER_REGISTRY_ENDPOINT string = containerRegistry.outputs.loginServer
output AZURE_RESOURCE_MCP_TODO_ID string = mcpTodo.outputs.resourceId
output AZURE_RESOURCE_MCP_TODO_NAME string = mcpTodo.outputs.name
output AZURE_RESOURCE_MCP_TODO_FQDN string = mcpTodo.outputs.fqdn

output FOUNDRY_NAME string = foundry.name
output FOUNDRY_RESOURCE_ID string = foundry.id
output FOUNDRY_ENDPOINT string = foundry.properties.endpoints['AI Foundry API']
output FOUNDRY_OPENAI_ENDPOINT string = foundry.properties.endpoints['OpenAI Language Model Instance API']
output FOUNDRY_PROJECT_NAME string = foundryproject.name
output FOUNDRY_PROJECT_ENDPOINT string = foundryproject.properties.endpoints['AI Foundry API']
output FOUNDRY_MODEL_DEPLOYMENT_NAME string = deployGptModel ? foundrydeployment.name : ''
