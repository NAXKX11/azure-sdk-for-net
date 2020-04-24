// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Management.KeyVault.Models;

namespace Azure.Management.KeyVault
{
    public partial class OperationsClient
    {
        private readonly ClientDiagnostics _clientDiagnostics;
        private readonly HttpPipeline _pipeline;
        internal OperationsRestClient RestClient { get; }
        /// <summary> Initializes a new instance of OperationsClient for mocking. </summary>
        protected OperationsClient()
        {
        }

        /// <summary> Initializes a new instance of OperationsClient. </summary>
        public OperationsClient(TokenCredential tokenCredential, KeyVaultManagementClientOptions options = null)
        {
            options = new KeyVaultManagementClientOptions();
            _clientDiagnostics = new ClientDiagnostics(options);
            _pipeline = ManagementPipelineBuilder.Build(tokenCredential, options);
            RestClient = new OperationsRestClient(_clientDiagnostics, _pipeline, options.Version);
        }

        /// <summary> Lists all of the available Key Vault Rest API operations. </summary>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual AsyncPageable<Operation> ListAsync(CancellationToken cancellationToken = default)
        {
            async Task<Page<Operation>> FirstPageFunc(int? pageSizeHint)
            {
                var response = await RestClient.ListAsync(cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            async Task<Page<Operation>> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = await RestClient.ListNextPageAsync(nextLink, cancellationToken).ConfigureAwait(false);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateAsyncEnumerable(FirstPageFunc, NextPageFunc);
        }

        /// <summary> Lists all of the available Key Vault Rest API operations. </summary>
        /// <param name="cancellationToken"> The cancellation token to use. </param>
        public virtual Pageable<Operation> List(CancellationToken cancellationToken = default)
        {
            Page<Operation> FirstPageFunc(int? pageSizeHint)
            {
                var response = RestClient.List(cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            Page<Operation> NextPageFunc(string nextLink, int? pageSizeHint)
            {
                var response = RestClient.ListNextPage(nextLink, cancellationToken);
                return Page.FromValues(response.Value.Value, response.Value.NextLink, response.GetRawResponse());
            }
            return PageableHelpers.CreateEnumerable(FirstPageFunc, NextPageFunc);
        }
    }
}