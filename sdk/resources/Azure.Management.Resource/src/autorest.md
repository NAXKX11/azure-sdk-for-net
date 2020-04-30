# Azure.Search Code Generation

Run ` dotnet msbuild /t:GenerateCode` to generate code.

## AutoRest Configuration
> see https://aka.ms/autorest


```yaml
payload-flattening-threshold: 2
azure-arm: true
input-file:
    - C:\AME\azure-rest-api-specs\specification\resources\resource-manager\Microsoft.Resources/stable/2019-10-01/resources.json
    - C:\AME\azure-rest-api-specs\specification\resources\resource-manager/Microsoft.Resources/preview/2019-10-01-preview/deploymentScripts.json
```

# input-file:
    - C:\AME\azure-rest-api-specs\specification\resources\resource-manager\Microsoft.Resources/stable/2019-10-01/resources.json
    - C:\AME\azure-rest-api-specs\specification\resources\resource-manager/Microsoft.Resources/stable/2019-11-01/subscriptions.json
    - C:\AME\azure-rest-api-specs\specification\resources\resource-manager/Microsoft.Authorization/stable/2019-09-01/policyAssignments.json
    - C:\AME\azure-rest-api-specs\specification\resources\resource-manager/Microsoft.Authorization/stable/2019-09-01/policyDefinitions.json
    - C:\AME\azure-rest-api-specs\specification\resources\resource-manager/Microsoft.Authorization/stable/2019-09-01/policySetDefinitions.json
    - C:\AME\azure-rest-api-specs\specification\resources\resource-manager/Microsoft.Resources/preview/2019-10-01-preview/deploymentScripts.json

# require: C:\AME\azure-rest-api-specs\specification\resources\resource-manager\readme.md

# tag: package-resources-2019-10
# tag: package-subscriptions-2019-11
# tag: package-policy-2019-09
# package-deploymentscripts-2019-10-preview
