// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Net;
using System.Threading.Tasks;

using Azure.Core.Pipeline;
using Azure.Core.TestFramework;
using Azure.Management.Resources;
using Azure.Management.Resources.Tests;

using NUnit.Framework;

namespace Azure.Management.Resource.Tests
{
    [AsyncOnly]
#pragma warning disable SA1649 // File name should match first type name
    public class InMemoryResourceGroupTests : ClientTestBase
#pragma warning restore SA1649 // File name should match first type name
    {
        public InMemoryResourceGroupTests(bool isAsync): base(isAsync)
        {
        }

        public ResourcesManagementClient GetResourceManagementClient(HttpPipelineTransport transport)
        {
            ResourcesManagementClientOptions options = new ResourcesManagementClientOptions();
            options.Transport = transport;

            return InstrumentClient(new ResourcesManagementClient(
                "ddd5f1d5-499c-45ee-a5ff-024d36ade650",
                new TestCredential(), options));
        }

        [Test]
        public async Task ResourceGroupExistsReturnsTrue()
        {
            var mockResponse = new MockResponse((int)HttpStatusCode.NoContent);
            //mockResponse.SetContent("IF HAVE")

            var mockTransport = new MockTransport(mockResponse);
            var client = GetResourceManagementClient(mockTransport);

            var result = await client.GetResourceGroupsClient().CheckExistenceAsync("foo");


            // Validate response
            Assert.AreEqual(result.Status, (int)HttpStatusCode.NoContent);
        }
    }
}
