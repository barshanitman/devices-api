param location string = resourceGroup().location

@description('Resourse name prefix')
param resourceNamePrefix string
var envResourceNamePrefix = toLower(resourceNamePrefix)

@description('Name of SQL Server')
param serverName string = uniqueString('sql', resourceGroup().id)

@description('Name of SQL Database')
param sqlDBName string = 'DevicesDB'

@description('Admin login for sql server')
param adminLogin string

@secure()
@description('AssetId api url ')
param assetIdApiUrl string = 'http://tech-assessment.vnext.com.au/api/devices/assetId/'

@secure()
@description('AssetId api x key ')
param assetIdApiXKey string = 'yeK7CM/Pj2vA3MFpuBxIFX7QIl1cKFOiviZaOjtVCrTq0VUzKeQjfw=='

@secure()
@description('Admin password for sql server')
param adminPassword string

resource azSqlServer 'Microsoft.Sql/servers@2022-05-01-preview' = {
  name: serverName
  location: location
  properties: {
    administratorLogin: adminLogin
    administratorLoginPassword: adminPassword
  }
}

resource azSqlDB 'Microsoft.Sql/servers/databases@2022-05-01-preview' = {
  parent: azSqlServer
  name: sqlDBName
  location: location
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
}

resource sqlServerFirewallRules 'Microsoft.Sql/servers/firewallRules@2021-02-01-preview' = {
  parent: azSqlServer
  name: '${envResourceNamePrefix} - IP rules'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '255.255.255.255'
  }
}

resource azStorageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  kind: 'StorageV2'
  location: location
  name: '${envResourceNamePrefix}storage'
  sku: {
    name: 'Standard_LRS'
  }
}

resource azAppInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: '${envResourceNamePrefix}-ai'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'

  }

}

resource azHostingPlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  location: location
  name: '${envResourceNamePrefix}-asp'
  kind: 'linux'
  sku: {
    name: 'S1'

  }
  properties: {
    reserved: true

  }
}

var dbConnectionString = 'Server=tcp:${azSqlServer.name}.database.windows.net,1433;Initial Catalog=${azSqlDB.name};Persist Security Info=False;User ID=${adminLogin};Password=${adminPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'

resource azFunctionApp 'Microsoft.Web/sites@2022-03-01' = {
  location: location
  name: '${envResourceNamePrefix}-app'
  kind: 'functionapp'
  properties: {
    serverFarmId: azHostingPlan.id
    siteConfig: {
      alwaysOn: true
      linuxFxVersion: 'DOTNETCORE|6.0'
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=barshanstorage;AccountKey=2a1qlSsOBFhQRwY4GaeM0tjj9BpfN63GuJ0awMhOAXzre/3SFPzZdjwB4ahee7ua/pRYeA44+KM1+AStnOvVUA==;EndpointSuffix=core.windows.net'
        }
        {
          name: 'CONNECTION_STRING'
          value: dbConnectionString

        }
        {
          name: 'API_URL'
          value: assetIdApiUrl

        }
        {
          name: 'X_KEY'
          value: assetIdApiXKey

        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }

      ]

    }

  }

}

output functionAppName string = azFunctionApp.name
