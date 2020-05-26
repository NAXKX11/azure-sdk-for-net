// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Azure.Core;
using Azure.Core.TestFramework;
using Azure.Management.Resources;
using Azure.Management.Resources.Models;
using Azure.Management.Resources.Tests;

using NUnit.Framework;

namespace ResourceGroups.Tests
{
#pragma warning disable SA1649 // File name should match first type name
    public class LiveDeploymentTests : ResourceOperationsTestsBase
#pragma warning restore SA1649 // File name should match first type name
    {
        public LiveDeploymentTests(bool isAsync)
            : base(isAsync)
        {
        }

        private const string DummyTemplateUri = "https://testtemplates.blob.core.windows.net/templates/dummytemplate.js";
        private const string GoodWebsiteTemplateUri = "https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/201-web-app-github-deploy/azuredeploy.json";
        private const string BadTemplateUri = "https://testtemplates.blob.core.windows.net/templates/bad-website-1.js";

        private const string LocationWestEurope = "West Europe";
        private const string LocationSouthCentralUS = "South Central US";

        [SetUp]
        public void ClearChallengeCacheforRecord()
        {
            // in record mode we reset the challenge cache before each test so that the challenge call
            // is always made.  This allows tests to be replayed independently and in any order
            if (Mode == RecordedTestMode.Record || Mode == RecordedTestMode.Playback)
            {
                Initialize();
            }
        }

        [Test]
        public async Task CreateDeploymentWithStringTemplateAndParameters()
        {
            var templateString = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ScenarioTests", "simple-storage-account.json"));
            var parameterString = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ScenarioTests", "simple-storage-account-parameters.json"));

            JsonElement jsonTemplate = JsonSerializer.Deserialize<JsonElement>(templateString);
            JsonElement jsonParameter = JsonSerializer.Deserialize<JsonElement>(parameterString);

            object template = jsonTemplate.GetObject();

            var parameterDictionary = jsonParameter.GetObject() as IDictionary<string, object>;
            object parameter = parameterDictionary;
            if (parameterDictionary.ContainsKey("parameters"))
            {
                parameter = parameterDictionary["parameters"];
            }

            var parameters = new Deployment
            (
                new DeploymentProperties(DeploymentMode.Incremental)
                {
                    Template = template,
                    Parameters = parameter
                }
            );

            string groupName = Recording.GenerateAssetName("csmrg");
            string deploymentName = Recording.GenerateAssetName("csmd");
            await ResourceGroupsClient.CreateOrUpdateAsync(groupName, new ResourceGroup(LiveDeploymentTests.LocationWestEurope));
            var rawResult = await DeploymentsClient.StartCreateOrUpdateAsync(groupName, deploymentName, parameters);
            await rawResult.WaitForCompletionAsync();

            var deployment = (await DeploymentsClient.GetAsync(groupName, deploymentName)).Value;
            Assert.AreEqual("Succeeded", deployment.Properties.ProvisioningState);
        }

        [Test]
        public async Task CreateDeploymentAndValidateProperties()
        {
            string resourceName = Recording.GenerateAssetName("csmr");

            var parameters = new Deployment
            (
                new DeploymentProperties(DeploymentMode.Incremental)
                {
                    TemplateLink = new TemplateLink(GoodWebsiteTemplateUri),
                    Parameters = new Dictionary<string, object>
                                {
                                    {
                                        "repoURL", new Dictionary<string, object>
                                        {
                                            { "value", "https://github.com/devigned/az-roadshow-oss.git" }
                                        }
                                    },
                                    {
                                        "siteName", new Dictionary<string, object>
                                        {
                                            { "value", resourceName }
                                        }
                                    },
                                    {
                                        "location", new Dictionary<string, object>
                                        {
                                            { "value", "westus" }
                                        }
                                    },
                                    {
                                        "sku", new Dictionary<string, object>
                                        {
                                            { "value", "F1" }
                                        }
                                    }
                                }
                }
            )
            {
                Tags = new Dictionary<string, string> { { "tagKey1", "tagValue1" } }
            };
            string groupName = Recording.GenerateAssetName("csmrg");
            string deploymentName = Recording.GenerateAssetName("csmd");
            await ResourceGroupsClient.CreateOrUpdateAsync(groupName, new ResourceGroup(LiveDeploymentTests.LocationWestEurope));
            var rawresult = await DeploymentsClient.StartCreateOrUpdateAsync(groupName, deploymentName, parameters);
            var deploymentCreateResult = (await rawresult.WaitForCompletionAsync()).Value;

            Assert.NotNull(deploymentCreateResult.Id);
            Assert.AreEqual(deploymentName, deploymentCreateResult.Name);

            if (Mode == RecordedTestMode.Record)
                Thread.Sleep(1000);

            var deploymentListResult = await DeploymentsClient.ListByResourceGroupAsync(groupName, null).ToEnumerableAsync();
            var deploymentGetResult = (await DeploymentsClient.GetAsync(groupName, deploymentName)).Value;

            Assert.IsNotEmpty(deploymentListResult);
            Assert.AreEqual(deploymentName, deploymentGetResult.Name);
            Assert.AreEqual(deploymentName, deploymentListResult.First().Name);
            Assert.AreEqual(GoodWebsiteTemplateUri, deploymentGetResult.Properties.TemplateLink.Uri);
            Assert.AreEqual(GoodWebsiteTemplateUri, deploymentListResult.First().Properties.TemplateLink.Uri);
            Assert.NotNull(deploymentGetResult.Properties.ProvisioningState);
            Assert.NotNull(deploymentListResult.First().Properties.ProvisioningState);
            Assert.NotNull(deploymentGetResult.Properties.CorrelationId);
            Assert.NotNull(deploymentListResult.First().Properties.CorrelationId);
            Assert.NotNull(deploymentListResult.First().Tags);
            Assert.True(deploymentListResult.First().Tags.ContainsKey("tagKey1"));
        }

        [Test]
        public async Task ValidateGoodDeployment()
        {
            string groupName = Recording.GenerateAssetName("csmrg");
            string deploymentName = Recording.GenerateAssetName("csmd");
            string resourceName = Recording.GenerateAssetName("csres");

            var parameters = new Deployment
            (
                new DeploymentProperties(DeploymentMode.Incremental)
                {
                    TemplateLink = new TemplateLink(GoodWebsiteTemplateUri),
                    Parameters = new Dictionary<string, object>
                                {
                                    {
                                        "repoURL", new Dictionary<string, object>
                                        {
                                            { "value", "https://github.com/devigned/az-roadshow-oss.git" }
                                        }
                                    },
                                    {
                                        "siteName", new Dictionary<string, object>
                                        {
                                            { "value", resourceName }
                                        }
                                    },
                                    {
                                        "location", new Dictionary<string, object>
                                        {
                                            { "value", "westus" }
                                        }
                                    },
                                    {
                                        "sku", new Dictionary<string, object>
                                        {
                                            { "value", "F1" }
                                        }
                                    }
                                }
                }
            );

            await ResourceGroupsClient.CreateOrUpdateAsync(groupName, new ResourceGroup(LiveDeploymentTests.LocationWestEurope));

            //Action
            var rawvalidationResult = await DeploymentsClient.StartValidateAsync(groupName, deploymentName, parameters);
            var validationResult = (await rawvalidationResult.WaitForCompletionAsync()).Value;

            //Assert
            Assert.Null(validationResult.Error);
            Assert.NotNull(validationResult.Properties);
            Assert.NotNull(validationResult.Properties.Providers);
            Assert.AreEqual(1, validationResult.Properties.Providers.Count);
            Assert.AreEqual("Microsoft.Web", validationResult.Properties.Providers[0].Namespace);
        }

        [Test]
        public async Task ValidateBadDeployment()
        {
            string groupName = Recording.GenerateAssetName("csmrg");
            string deploymentName = Recording.GenerateAssetName("csmd");
            var parameters = new Deployment
            (
                new DeploymentProperties(DeploymentMode.Incremental)
                {
                    TemplateLink = new TemplateLink(BadTemplateUri),
                    Parameters = new Dictionary<string, object>
                                {
                                    {
                                        "siteName", new Dictionary<string, object>
                                        {
                                            { "value", "mctest0101" }
                                        }
                                    },
                                    {
                                        "hostingPlanName", new Dictionary<string, object>
                                        {
                                            { "value", "mctest0101" }
                                        }
                                    },
                                    {
                                        "siteMode", new Dictionary<string, object>
                                        {
                                            { "value", "Limited" }
                                        }
                                    },
                                    {
                                        "computeMode", new Dictionary<string, object>
                                        {
                                            { "value", "Shared" }
                                        }
                                    },
                                    {
                                        "siteLocation", new Dictionary<string, object>
                                        {
                                            { "value", "North Europe" }
                                        }
                                    },
                                    {
                                        "sku", new Dictionary<string, object>
                                        {
                                            { "value", "Free" }
                                        }
                                    },
                                    {
                                        "workerSize", new Dictionary<string, object>
                                        {
                                            { "value", "0" }
                                        }
                                    }
                                }
                }
            );

            // TODO
            await ResourceGroupsClient.CreateOrUpdateAsync(groupName, new ResourceGroup(LiveDeploymentTests.LocationWestEurope));
            try
            {
                var rawresult = await DeploymentsClient.StartValidateAsync(groupName, deploymentName, parameters);
                var result = (await rawresult.WaitForCompletionAsync()).Value;
                Assert.NotNull(result);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("InvalidTemplate"));
            }

        }

        [Test]
        public async Task CreateLargeWebDeploymentTemplateWorks()
        {
            string resourceName = Recording.GenerateAssetName("csmr");
            string groupName = Recording.GenerateAssetName("csmrg");
            string deploymentName = Recording.GenerateAssetName("csmd");

            var parameters = new Deployment
            (
                new DeploymentProperties(DeploymentMode.Incremental)
                {
                    TemplateLink = new TemplateLink(GoodWebsiteTemplateUri),
                    Parameters = new Dictionary<string, object>
                                {
                                    {
                                        "repoURL", new Dictionary<string, object>
                                        {
                                            { "value", "https://github.com/devigned/az-roadshow-oss.git" }
                                        }
                                    },
                                    {
                                        "siteName", new Dictionary<string, object>
                                        {
                                            { "value", resourceName }
                                        }
                                    },
                                    {
                                        "location", new Dictionary<string, object>
                                        {
                                            { "value", "westus" }
                                        }
                                    },
                                    {
                                        "sku", new Dictionary<string, object>
                                        {
                                            { "value", "F1" }
                                        }
                                    }
                                }
                }
            );

            await ResourceGroupsClient.CreateOrUpdateAsync(groupName, new ResourceGroup(LiveDeploymentTests.LocationSouthCentralUS));
            await DeploymentsClient.StartCreateOrUpdateAsync(groupName, deploymentName, parameters);

            // Wait until deployment completes
            if (Mode == RecordedTestMode.Record)
                Thread.Sleep(30000);
            var operations = await DeploymentClient.ListAsync(groupName, deploymentName, null).ToEnumerableAsync();

            Assert.True(operations.Any());
        }

        [Test]
        public async Task SubscriptionLevelDeployment()
        {
            string groupName = "SDK-test";
            string deploymentName = Recording.GenerateAssetName("csmd");
            string resourceName = Recording.GenerateAssetName("csmr");
            Dictionary<string, object> template =
JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ScenarioTests", "subscription_level_template.json")));

            var parameters = new Deployment
            (
                new DeploymentProperties(DeploymentMode.Incremental)
                {
                    Template = template,
                    Parameters = new Dictionary<string, object>
                                {
                                    {
                                        "storageAccountName", new Dictionary<string, object>
                                        {
                                            { "value", "armbuilddemo1803" }
                                        }
                                    }
                                }
                }
            )
            {
                Location = "WestUS",
                Tags = new Dictionary<string, string> { { "tagKey1", "tagValue1" } }
            };

            await ResourceGroupsClient.CreateOrUpdateAsync(groupName, new ResourceGroup("WestUS"));

            //Validate
            var rawValidationResult = await DeploymentsClient.StartValidateAtSubscriptionScopeAsync(deploymentName, parameters);
            var validationResult = (await rawValidationResult.WaitForCompletionAsync()).Value;

            //Assert
            Assert.Null(validationResult.Error);
            Assert.NotNull(validationResult.Properties);
            Assert.NotNull(validationResult.Properties.Providers);

            //Put deployment
            var rawDeploymentResult = await DeploymentsClient.StartCreateOrUpdateAtSubscriptionScopeAsync(deploymentName, parameters);
            await rawDeploymentResult.WaitForCompletionAsync();

            var deployment = (await DeploymentsClient.GetAtSubscriptionScopeAsync(deploymentName)).Value;
            Assert.AreEqual("Succeeded", deployment.Properties.ProvisioningState);
            Assert.NotNull(deployment.Tags);
            Assert.True(deployment.Tags.ContainsKey("tagKey1"));
        }

        [Test]
        public async Task ManagementGroupLevelDeployment()
        {
            string groupId = "tag-mg1";
            string deploymentName = Recording.GenerateAssetName("csharpsdktest");
            Dictionary<string, object> template = JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ScenarioTests", "management_group_level_template.json")));

            var parameters = new ScopedDeployment
            (
                "East US",
                new DeploymentProperties(DeploymentMode.Incremental)
                {
                    Template = template,
                    Parameters = new Dictionary<string, object>
                                {
                                    {
                                        "storageAccountName", new Dictionary<string, object>
                                        {
                                            { "value", "tagsa021920" }
                                        }
                                    }
                                }
                }
            )
            {
                Tags = new Dictionary<string, string> { { "tagKey1", "tagValue1" } }
            };

            //Validate
            var rawValidationResult = await DeploymentsClient.StartValidateAtManagementGroupScopeAsync(groupId, deploymentName, parameters);
            var validationResult = (await rawValidationResult.WaitForCompletionAsync()).Value;

            //Assert
            Assert.Null(validationResult.Error);
            Assert.NotNull(validationResult.Properties);
            Assert.NotNull(validationResult.Properties.Providers);

            //Put deployment
            var deploymentResult = await DeploymentsClient.StartCreateOrUpdateAtManagementGroupScopeAsync(groupId, deploymentName, parameters);
            await deploymentResult.WaitForCompletionAsync();

            var deployment = (await DeploymentsClient.GetAtManagementGroupScopeAsync(groupId, deploymentName)).Value;
            Assert.AreEqual("Succeeded", deployment.Properties.ProvisioningState);
            Assert.NotNull(deployment.Tags);
            Assert.True(deployment.Tags.ContainsKey("tagKey1"));
        }

        [Test]
        public async Task TenantLevelDeployment()
        {
            string deploymentName = Recording.GenerateAssetName("csharpsdktest");
            Dictionary<string, object> template =
JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ScenarioTests", "tenant_level_template.json")));

            var parameters = new ScopedDeployment
            (
                "East US 2",
                new DeploymentProperties(DeploymentMode.Incremental)
                {
                    Template = template,
                    Parameters = new Dictionary<string, object>
                                {
                                    {
                                        "managementGroupId", new Dictionary<string, object>
                                        {
                                            { "value", "tiano-mgtest01" }
                                        }
                                    }
                                }
                }
            )
            {
                Tags = new Dictionary<string, string> { { "tagKey1", "tagValue1" } }
            };

            //Validate
            var rawValidationResult = await DeploymentsClient.StartValidateAtTenantScopeAsync(deploymentName, parameters);
            var validationResult = (await rawValidationResult.WaitForCompletionAsync()).Value;

            //Assert
            Assert.Null(validationResult.Error);
            Assert.NotNull(validationResult.Properties);
            Assert.NotNull(validationResult.Properties.Providers);

            //Put deployment
            var deploymentResult = await DeploymentsClient.StartCreateOrUpdateAtTenantScopeAsync(deploymentName, parameters);
            await deploymentResult.WaitForCompletionAsync();

            var deployment = (await DeploymentsClient.GetAtTenantScopeAsync(deploymentName)).Value;
            Assert.AreEqual("Succeeded", deployment.Properties.ProvisioningState);
            Assert.NotNull(deployment.Tags);
            Assert.True(deployment.Tags.ContainsKey("tagKey1"));

            var deploymentOperations = await DeploymentClient.ListAtTenantScopeAsync(deploymentName).ToEnumerableAsync();
            Assert.AreEqual(4, deploymentOperations.Count());
        }

        [Test]
        public async Task DeploymentWithScope_AtTenant()
        {
            string deploymentName = Recording.GenerateAssetName("csharpsdktest");
            Dictionary<string, object> template =
JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ScenarioTests", "tenant_level_template.json")));

            var parameters = new Deployment
            (
                new DeploymentProperties(DeploymentMode.Incremental)
                {
                    Template = template,
                    Parameters = new Dictionary<string, object>
                                {
                                    {
                                        "storageAccountName", new Dictionary<string, object>
                                        {
                                            { "value", "tiano-mgtest01" }
                                        }
                                    }
                                }
                }
            )
            {
                Location = "East US 2",
                Tags = new Dictionary<string, string> { { "tagKey1", "tagValue1" } }
            };

            //Validate
            var rawValidationResult = await DeploymentsClient.StartValidateAtScopeAsync(scope: "", deploymentName: deploymentName, parameters: parameters);
            var validationResult = (await rawValidationResult.WaitForCompletionAsync()).Value;

            //Assert
            Assert.Null(validationResult.Error);
            Assert.NotNull(validationResult.Properties);
            Assert.NotNull(validationResult.Properties.Providers);

            //Put deployment
            var deploymentResult = await DeploymentsClient.StartCreateOrUpdateAtScopeAsync(scope: "", deploymentName: deploymentName, parameters: parameters);
            await deploymentResult.WaitForCompletionAsync();

            var deployment = (await DeploymentsClient.GetAtScopeAsync(scope: "", deploymentName: deploymentName)).Value;
            Assert.AreEqual("Succeeded", deployment.Properties.ProvisioningState);
            Assert.NotNull(deployment.Tags);
            Assert.True(deployment.Tags.ContainsKey("tagKey1"));

            var deploymentOperations = await DeploymentClient.ListAtScopeAsync(scope: "", deploymentName: deploymentName).ToEnumerableAsync();
            Assert.AreEqual(4, deploymentOperations.Count());
        }

        [Test]
        public async Task DeploymentWithScope_AtManagementGroup()
        {
            string groupId = "tag-mg1";
            string deploymentName = Recording.GenerateAssetName("csharpsdktest");
            Dictionary<string, object> template =
JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ScenarioTests", "management_group_level_template.json")));

            var parameters = new Deployment
            (
                new DeploymentProperties(DeploymentMode.Incremental)
                {
                    Template = template,
                    Parameters = new Dictionary<string, object>
                                {
                                    {
                                        "storageAccountName", new Dictionary<string, object>
                                        {
                                            { "value", "tagsa021920" }
                                        }
                                    }
                                }
                }
            )
            {
                Location = "East US",
                Tags = new Dictionary<string, string> { { "tagKey1", "tagValue1" } }
            };

            var managementGroupScope = $"//providers/Microsoft.Management/managementGroups/{groupId}";

            //Validate
            var rawValidationResult = await DeploymentsClient.StartValidateAtScopeAsync(scope: managementGroupScope, deploymentName: deploymentName, parameters: parameters);
            var validationResult = (await rawValidationResult.WaitForCompletionAsync()).Value;

            //Assert
            Assert.Null(validationResult.Error);
            Assert.NotNull(validationResult.Properties);
            Assert.NotNull(validationResult.Properties.Providers);

            //Put deployment
            var deploymentResult = await DeploymentsClient.StartCreateOrUpdateAtScopeAsync(scope: managementGroupScope, deploymentName: deploymentName, parameters: parameters);
            await deploymentResult.WaitForCompletionAsync();

            var deployment = (await DeploymentsClient.GetAtScopeAsync(scope: managementGroupScope, deploymentName: deploymentName)).Value;
            Assert.AreEqual("Succeeded", deployment.Properties.ProvisioningState);
            Assert.NotNull(deployment.Tags);
            Assert.True(deployment.Tags.ContainsKey("tagKey1"));

            var deploymentOperations = await DeploymentClient.ListAtScopeAsync(scope: managementGroupScope, deploymentName: deploymentName).ToEnumerableAsync();
            Assert.AreEqual(4, deploymentOperations.Count());
        }

        [Test]
        public async Task DeploymentWithScope_AtSubscription()
        {
            string groupName = "SDK-test";
            string deploymentName = Recording.GenerateAssetName("csmd");
            Dictionary<string, object> template =
JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ScenarioTests", "subscription_level_template.json")));

            var parameters = new Deployment
            (
                new DeploymentProperties(DeploymentMode.Incremental)
                {
                    Template = template,
                    Parameters = new Dictionary<string, object>
                                {
                                    {
                                        "storageAccountName", new Dictionary<string, object>
                                        {
                                            { "value", "armbuilddemo1803" }
                                        }
                                    }
                                }
                }
            )
            {
                Location = "WestUS",
                Tags = new Dictionary<string, string> { { "tagKey1", "tagValue1" } }
            };

            await ResourceGroupsClient.CreateOrUpdateAsync(groupName, new ResourceGroup("WestUS"));

            var subscriptionScope = $"//subscriptions/{subscriptionId}";

            //Validate
            var rawValidationResult = await DeploymentsClient.StartValidateAtScopeAsync(scope: subscriptionScope, deploymentName: deploymentName, parameters: parameters);
            var validationResult = (await rawValidationResult.WaitForCompletionAsync()).Value;

            //Assert
            Assert.Null(validationResult.Error);
            Assert.NotNull(validationResult.Properties);
            Assert.NotNull(validationResult.Properties.Providers);

            //Put deployment
            var deploymentResult = await DeploymentsClient.StartCreateOrUpdateAtScopeAsync(scope: subscriptionScope, deploymentName: deploymentName, parameters: parameters);
            await deploymentResult.WaitForCompletionAsync();

            var deployment = (await DeploymentsClient.GetAtScopeAsync(scope: subscriptionScope, deploymentName: deploymentName)).Value;
            Assert.AreEqual("Succeeded", deployment.Properties.ProvisioningState);
            Assert.NotNull(deployment.Tags);
            Assert.True(deployment.Tags.ContainsKey("tagKey1"));

            var deploymentOperations = await DeploymentClient.ListAtScopeAsync(scope: subscriptionScope, deploymentName: deploymentName).ToEnumerableAsync();
            Assert.AreEqual(4, deploymentOperations.Count());
        }

        [Test]
        public async Task DeploymentWithScope_AtResourceGroup()
        {
            string groupName = "SDK-test-01";
            string deploymentName = Recording.GenerateAssetName("csmd");
            Dictionary<string, object> template = JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ScenarioTests", "simple-storage-account.json")));

            var parameters = new Deployment
            (
                new DeploymentProperties(DeploymentMode.Incremental)
                {
                    Template = template,
                    Parameters = new Dictionary<string, object>
                                {
                                    {
                                        "storageAccountName", new Dictionary<string, object>
                                        {
                                            { "value", "tianotest105" }
                                        }
                                    }
                                }
                }
            )
            {
                Tags = new Dictionary<string, string> { { "tagKey1", "tagValue1" } }
            };

            await ResourceGroupsClient.CreateOrUpdateAsync(groupName, new ResourceGroup("WestUS"));

            var resourceGroupScope = $"//subscriptions/{subscriptionId}/resourceGroups/{groupName}";

            //Validate
            var rawValidationResult = await DeploymentsClient.StartValidateAtScopeAsync(scope: resourceGroupScope, deploymentName: deploymentName, parameters: parameters);
            var validationResult = (await rawValidationResult.WaitForCompletionAsync()).Value;

            //Assert
            Assert.Null(validationResult.Error);
            Assert.NotNull(validationResult.Properties);
            Assert.NotNull(validationResult.Properties.Providers);

            //Put deployment
            var deploymentResult = await DeploymentsClient.StartCreateOrUpdateAtScopeAsync(scope: resourceGroupScope, deploymentName: deploymentName, parameters: parameters);
            await deploymentResult.WaitForCompletionAsync();

            var deployment = (await DeploymentsClient.GetAtScopeAsync(scope: resourceGroupScope, deploymentName: deploymentName)).Value;
            Assert.AreEqual("Succeeded", deployment.Properties.ProvisioningState);
            Assert.NotNull(deployment.Tags);
            Assert.True(deployment.Tags.ContainsKey("tagKey1"));

            var deploymentOperations = await DeploymentClient.ListAtScopeAsync(scope: resourceGroupScope, deploymentName: deploymentName).ToEnumerableAsync();
            Assert.AreEqual(2, deploymentOperations.Count());
        }
    }
}
