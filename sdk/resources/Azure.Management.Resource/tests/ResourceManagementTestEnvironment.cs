// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Azure.Core.Testing;

namespace Azure.Management.KeyVault.Tests
{
    public class ResourceManagementTestEnvironment : TestEnvironment
    {
        private const string TenantIdKey = "TenantId";
        private const string SubIdKey = "SubId";
        private const string ApplicationIdKey = "ApplicationId";

        public ResourceManagementTestEnvironment() : base("resourcemgmt")
        {
        }

        public string TenantIdTrack1 => GetRecordedVariable(TenantIdKey);

        public string SubscriptionIdTrack1 => GetRecordedVariable(SubIdKey);

        public string ApplicationIdTrack1 => GetRecordedVariable(ApplicationIdKey);
    }
}
