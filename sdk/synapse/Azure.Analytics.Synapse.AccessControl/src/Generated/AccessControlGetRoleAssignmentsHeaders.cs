// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using Azure;
using Azure.Core;

namespace Azure.Analytics.Synapse.AccessControl
{
    internal class AccessControlGetRoleAssignmentsHeaders
    {
        private readonly Response _response;
        public AccessControlGetRoleAssignmentsHeaders(Response response)
        {
            _response = response;
        }
        /// <summary> If the number of role assignments to be listed exceeds the maxResults limit, a continuation token is returned in this response header.  When a continuation token is returned in the response, it must be specified in a subsequent invocation of the list operation to continue listing the role assignments. </summary>
        public string XMsContinuation => _response.Headers.TryGetValue("x-ms-continuation", out string value) ? value : null;
    }
}
