// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core.Testing;
using Azure.Graph.Rbac;
using Azure.Graph.Rbac.Models;
using Azure.Identity;
using Azure.Management.KeyVault.Models;
using Azure.Management.Resource;
using NUnit.Framework;

namespace Azure.Management.KeyVault.Tests
{
    [ClientTestFixture(KeyVaultManagementClientOptions.ServiceVersion.V2019_09_01)]
    [NonParallelizable]
    public abstract class VaultOperationsTestsBase : RecordedTestBase<KeyVaultManagementTestEnvironment>
    {
        private readonly KeyVaultManagementClientOptions.ServiceVersion _serviceVersion;

        private const string ObjectIdKey = "ObjectId";
        private const string ApplicationIdKey = "ApplicationId";

        public string tenantId { get; set; }
        public string objectId { get; set; }
        public string applicationId { get; set; }
        public string location { get; set; }
        public string subscriptionId { get; set; }

        public AccessPolicyEntry accPol { get; internal set; }
        public string objectIdGuid { get; internal set; }
        public string rgName { get; internal set; }
        public Dictionary<string, string> tags { get; internal set; }
        public Guid tenantIdGuid { get; internal set; }
        public string vaultName { get; internal set; }
        public VaultProperties vaultProperties { get; internal set; }


        public VaultsClient VaultsClient { get; set; }
        public ResourcesClient ResourcesClient { get; set; }
        public UsersClient GraphUsersClient { get; set; }
        public ProvidersClient ResourceProvidersClient { get; set; }


        protected VaultOperationsTestsBase(bool isAsync, KeyVaultManagementClientOptions.ServiceVersion serviceVersion)
            : base(isAsync)
        {
            _serviceVersion = serviceVersion;
        }

        protected async Task Initialize()
        {
            if (Mode == RecordedTestMode.Playback && Recording.IsTrack1SessionRecord())
            {
                this.tenantId = TestEnvironment.TenantIdTrack1;
                this.subscriptionId = TestEnvironment.SubscriptionIdTrack1;
            }
            else
            {
                this.tenantId = TestEnvironment.TenantId;
                this.subscriptionId = TestEnvironment.SubscriptionId;
            }
            this.applicationId = Recording.GetVariable(ApplicationIdKey, Guid.NewGuid().ToString()); ;

            VaultsClient = GetVaultsClient();
            ResourcesClient = GetResourcesClient();
            GraphUsersClient = GetGraphUsersClient();
            ResourceProvidersClient = GetResourceProvidersClient();

            if (Mode == RecordedTestMode.Playback)
            {
                this.objectId = Recording.GetVariable(ObjectIdKey, string.Empty);
            }
            else if (Mode == RecordedTestMode.Record)
            {
                //TODO: verify in record mode; seems graph request is not in records, why?
                var userName = TestEnvironment.UserName;
                this.objectId = (await GraphUsersClient.GetAsync(userName)).Value.ObjectId;

                ServicePrincipalsClient client;
                var = client.ListAsync($"appId eq {this.ClientId}");
                var odataQuery = new ODataQuery<ServicePrincipal>(sp => sp.AppId == acsServicePrincipal.SpId);
                var servicePrincipal = GraphClient.ServicePrincipals.List(odataQuery).First();
            }

            rgName = Recording.GenerateAssetName("sdktestrg");
            vaultName = Recording.GenerateAssetName("sdktestvault");

            tenantIdGuid = new Guid(tenantId);
            objectIdGuid = objectId;
            tags = new Dictionary<string, string> { { "tag1", "value1" }, { "tag2", "value2" }, { "tag3", "value3" } };

            var permissions = new Permissions
            {
                Keys = new KeyPermissions[] { new KeyPermissions("all") },
                Secrets = new SecretPermissions[] { new SecretPermissions("all") },
                Certificates = new CertificatePermissions[] { new CertificatePermissions("all") },
                Storage = new StoragePermissions[] { new StoragePermissions("all") },
            };
            accPol = new AccessPolicyEntry(tenantIdGuid, objectId, permissions);

            IList<IPRule> ipRules = new List<IPRule>();
            ipRules.Add(new IPRule("1.2.3.4/32"));
            ipRules.Add(new IPRule("1.0.0.0/25"));

            vaultProperties = new VaultProperties(tenantIdGuid, new Sku(SkuName.Standard));


            vaultProperties.EnabledForDeployment = true;
            vaultProperties.EnabledForDiskEncryption = true;
            vaultProperties.EnabledForTemplateDeployment = true;
            vaultProperties.EnableSoftDelete = false;//
            vaultProperties.VaultUri = "";
            vaultProperties.NetworkAcls = new NetworkRuleSet() { Bypass = "AzureServices", DefaultAction = "Allow", IpRules = ipRules, VirtualNetworkRules = null };
            vaultProperties.AccessPolicies = new[] { accPol };

            var provider = (await ResourceProvidersClient.GetAsync("Microsoft.KeyVault")).Value;
            this.location = provider.ResourceTypes.Where(
                (resType) =>
                {
                    if (resType.ResourceType == "vaults")
                        return true;
                    else
                        return false;
                }
                ).First().Locations.FirstOrDefault();
        }

        internal VaultsClient GetVaultsClient(TestRecording recording = null)
        {
            recording = recording ?? Recording;

            return InstrumentClient(new VaultsClient(this.subscriptionId,
                TestEnvironment.Credential,
                recording.InstrumentClientOptions(new KeyVaultManagementClientOptions(_serviceVersion))));
        }

        internal ResourcesClient GetResourcesClient(TestRecording recording = null)
        {
            recording = recording ?? Recording;

            return InstrumentClient(new ResourcesClient(this.subscriptionId,
                TestEnvironment.Credential,
                recording.InstrumentClientOptions(new ResourceManagementClientOptions())));
        }

        internal UsersClient GetGraphUsersClient(TestRecording recording = null)
        {
            recording = recording ?? Recording;

            return InstrumentClient(new UsersClient(this.tenantId,
                TestEnvironment.Credential,
                recording.InstrumentClientOptions(new RbacManagementClientOptions())));
        }

        internal ProvidersClient GetResourceProvidersClient(TestRecording recording = null)
        {
            recording = recording ?? Recording;

            return InstrumentClient(new ProvidersClient(this.subscriptionId,
                TestEnvironment.Credential,
                recording.InstrumentClientOptions(new ResourceManagementClientOptions())));
        }

        public override void StartTestRecording()
        {
            base.StartTestRecording();
        }
    }
}
