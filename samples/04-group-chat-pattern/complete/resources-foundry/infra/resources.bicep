@description('Name of the environment that can be used as part of naming resource convention')
param environmentName string

@description('The location used for all deployed resources')
param location string = resourceGroup().location

@description('Tags that will be applied to all resources')
param tags object = {}

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

var azureAIUserRoleId = '53ca6127-db72-4b80-b1b0-d745d6d5456d'
var cognitiveServicesUserRoleId = 'a97b65f3-24c7-4388-baec-2e87135dc908'

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
output FOUNDRY_NAME string = foundry.name
output FOUNDRY_RESOURCE_ID string = foundry.id
output FOUNDRY_ENDPOINT string = foundry.properties.endpoints['AI Foundry API']
output FOUNDRY_OPENAI_ENDPOINT string = foundry.properties.endpoints['OpenAI Language Model Instance API']
output FOUNDRY_PROJECT_NAME string = foundryproject.name
output FOUNDRY_PROJECT_ENDPOINT string = foundryproject.properties.endpoints['AI Foundry API']
output FOUNDRY_MODEL_DEPLOYMENT_NAME string = deployGptModel ? foundrydeployment.name : ''
