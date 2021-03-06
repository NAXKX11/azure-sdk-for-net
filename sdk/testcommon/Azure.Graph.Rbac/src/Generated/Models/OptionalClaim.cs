// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

namespace Azure.Graph.Rbac.Models
{
    /// <summary> Specifying the claims to be included in a token. </summary>
    public partial class OptionalClaim
    {
        /// <summary> Initializes a new instance of OptionalClaim. </summary>
        public OptionalClaim()
        {
        }

        /// <summary> Initializes a new instance of OptionalClaim. </summary>
        /// <param name="name"> Claim name. </param>
        /// <param name="source"> Claim source. </param>
        /// <param name="essential"> Is this a required claim. </param>
        /// <param name="additionalProperties"> Any object. </param>
        internal OptionalClaim(string name, string source, bool? essential, object additionalProperties)
        {
            Name = name;
            Source = source;
            Essential = essential;
            AdditionalProperties = additionalProperties;
        }

        /// <summary> Claim name. </summary>
        public string Name { get; set; }
        /// <summary> Claim source. </summary>
        public string Source { get; set; }
        /// <summary> Is this a required claim. </summary>
        public bool? Essential { get; set; }
        /// <summary> Any object. </summary>
        public object AdditionalProperties { get; set; }
    }
}
