// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Azure.Core.Testing;
using Azure.Identity;
using Azure.Management.KeyVault.Models;
using NUnit.Framework;

namespace Azure.Management.KeyVault.Tests
{
    public class VaultOperationsTests : VaultOperationsTestsBase
    {
        public VaultOperationsTests(bool isAsync, KeyVaultManagementClientOptions.ServiceVersion serviceVersion)
            : base(isAsync, serviceVersion)
        {
        }

        [SetUp]
        public void ClearChallengeCacheforRecord()
        {
            // in record mode we reset the challenge cache before each test so that the challenge call
            // is always made.  This allows tests to be replayed independently and in any order
            if (Mode == RecordedTestMode.Record || Mode == RecordedTestMode.Playback)
            {
                Initialize().ConfigureAwait(false).GetAwaiter().GetResult();
                //ChallengeBasedAuthenticationPolicy.AuthenticationChallenge.ClearCache();
            }
        }

        [Test]
        public async Task CreateKeyVaultDisableSoftDelete()
        {
            this.accPol.ApplicationId = Guid.Parse(this.applicationId);
            this.vaultProperties.EnableSoftDelete = false;

            var parameters = new VaultCreateOrUpdateParameters(location, vaultProperties);
            parameters.Tags = this.tags;
            var vault = await VaultsClient.StartCreateOrUpdateAsync(
                resourceGroupName: this.rgName,
                vaultName: this.vaultName,
                parameters: parameters
                );
            var vaultValue = await vault.WaitForCompletionAsync();

            Assert.False(vaultValue.Value.Properties.EnableSoftDelete);
        }

    }
}
