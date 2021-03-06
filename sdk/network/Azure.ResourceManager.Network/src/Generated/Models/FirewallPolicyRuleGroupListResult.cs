// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;
using Azure.Core;

namespace Azure.ResourceManager.Network.Models
{
    /// <summary> Response for ListFirewallPolicyRuleGroups API service call. </summary>
    public partial class FirewallPolicyRuleGroupListResult
    {
        /// <summary> Initializes a new instance of FirewallPolicyRuleGroupListResult. </summary>
        internal FirewallPolicyRuleGroupListResult()
        {
            Value = new ChangeTrackingList<FirewallPolicyRuleGroup>();
        }

        /// <summary> Initializes a new instance of FirewallPolicyRuleGroupListResult. </summary>
        /// <param name="value"> List of FirewallPolicyRuleGroups in a FirewallPolicy. </param>
        /// <param name="nextLink"> URL to get the next set of results. </param>
        internal FirewallPolicyRuleGroupListResult(IReadOnlyList<FirewallPolicyRuleGroup> value, string nextLink)
        {
            Value = value;
            NextLink = nextLink;
        }

        /// <summary> List of FirewallPolicyRuleGroups in a FirewallPolicy. </summary>
        public IReadOnlyList<FirewallPolicyRuleGroup> Value { get; }
        /// <summary> URL to get the next set of results. </summary>
        public string NextLink { get; }
    }
}
