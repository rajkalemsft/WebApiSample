# Azure EventHub OAuth samples
[![Open in Remote - Containers](https://img.shields.io/static/v1?label=Remote%20-%20Containers&message=Open&color=blue&logo=visualstudiocode)](https://vscode.dev/redirect?url=vscode://event-hub-ouath-samples.remote-containers/cloneInVolume?https://github.com/rajkalemsft/event-hub-ouath-samples)

Python code samples to demonstrate ability to connect to Azure EventHub with Kafka protocol using Azure AD OAUTH. Samples use Azure AD ClientCredential auth flow with both secret and certificate creds.

**Sample demonstrates,**

  1. Different authentication mechanisms for EventHub using AzureAD.
  2. Code uses VSCode devcontainer feature to run locally in an isolated environment.
  3. Has docker image definition to build the image

**Sample relies on below packages,**

    **Azure.Identity** -> For Azure AD AUTH. Please refer defaultazurecredential

    **Confluent-Kafka** -> To connect to EventHub using Kafka protocol

**Repo Contents**

    **/eventhub** --> Includes producer and consumer sample with azure sdk for EventHub

    **/eventhub-kafka** --> Includes producer and consumer sample with confluent-kafka and Azure.Identity package

**Setup**

Edit the .env file for below values before you open the repo in .devcontainer. Refer API doc [here](https://azuresdkdocs.blob.core.windows.net/$web/python/azure-identity/latest/azure.identity.html#azure.identity.EnvironmentCredential)

    AZURE_AUTHORITY_HOST=login.microsoftonline.com

    AZURE_CLIENT_ID=<<AppClientId>>

    AZURE_CLIENT_SECRET=<<AppSecret>>

    AZURE_TENANT_ID=<<TenantID>>

    AZURE_CLIENT_CERTIFICATE_PATH=<<AzureAdClientSecretCertPath>>
    
    AZURE_CLIENT_SEND_CERTIFICATE_CHAIN=<<TrueToValidateISsuesAndSubjectName>>
  
    EVENT_HUB_HOSTNAME=<<EventHubNameSpace>>

    EVENT_HUB_NAME=<<EvemtHubName>>

    CONSUMER_GROUP=<<ConsumerGroupName>>

    VAULT_URL= <<KeyVaultURL To Retrieve Certificate - Only for Cert Cred flow>>

    AZURE_KV_CLIENT_ID=<<ClientId-KeyVaultAccess>>

    AZURE_KV_CLIENT_SECRET=<<ClientSecret-KeyVaultAccess>>
  
** Docker Container**

 Build the container and publish to Azure Container Registry (ACR) 
    
    docker build -t <>.azurecr.io/<>:<> .

    az login az acr login <>

    docker push <>.azurecr.io/<>:<>
  
  ** For ManagedIdentity Auth**, please refer to [azurefunc-eventhub-managedidentity-auth](https://github.com/rajkalemsft/azurefunc-eventhub-managedidentity-auth)
  
  
