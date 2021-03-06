// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Text.Json;
using Azure.Core;

namespace Azure.ResourceManager.Network.Models
{
    public partial class ExpressRouteConnection : IUtf8JsonSerializable
    {
        void IUtf8JsonSerializable.Write(Utf8JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("name");
            writer.WriteStringValue(Name);
            if (Optional.IsDefined(Id))
            {
                writer.WritePropertyName("id");
                writer.WriteStringValue(Id);
            }
            writer.WritePropertyName("properties");
            writer.WriteStartObject();
            if (Optional.IsDefined(ExpressRouteCircuitPeering))
            {
                writer.WritePropertyName("expressRouteCircuitPeering");
                writer.WriteObjectValue(ExpressRouteCircuitPeering);
            }
            if (Optional.IsDefined(AuthorizationKey))
            {
                writer.WritePropertyName("authorizationKey");
                writer.WriteStringValue(AuthorizationKey);
            }
            if (Optional.IsDefined(RoutingWeight))
            {
                writer.WritePropertyName("routingWeight");
                writer.WriteNumberValue(RoutingWeight.Value);
            }
            if (Optional.IsDefined(EnableInternetSecurity))
            {
                writer.WritePropertyName("enableInternetSecurity");
                writer.WriteBooleanValue(EnableInternetSecurity.Value);
            }
            if (Optional.IsDefined(RoutingConfiguration))
            {
                writer.WritePropertyName("routingConfiguration");
                writer.WriteObjectValue(RoutingConfiguration);
            }
            writer.WriteEndObject();
            writer.WriteEndObject();
        }

        internal static ExpressRouteConnection DeserializeExpressRouteConnection(JsonElement element)
        {
            string name = default;
            Optional<string> id = default;
            Optional<ProvisioningState> provisioningState = default;
            Optional<ExpressRouteCircuitPeeringId> expressRouteCircuitPeering = default;
            Optional<string> authorizationKey = default;
            Optional<int> routingWeight = default;
            Optional<bool> enableInternetSecurity = default;
            Optional<RoutingConfiguration> routingConfiguration = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("name"))
                {
                    name = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("id"))
                {
                    id = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("properties"))
                {
                    foreach (var property0 in property.Value.EnumerateObject())
                    {
                        if (property0.NameEquals("provisioningState"))
                        {
                            provisioningState = new ProvisioningState(property0.Value.GetString());
                            continue;
                        }
                        if (property0.NameEquals("expressRouteCircuitPeering"))
                        {
                            expressRouteCircuitPeering = ExpressRouteCircuitPeeringId.DeserializeExpressRouteCircuitPeeringId(property0.Value);
                            continue;
                        }
                        if (property0.NameEquals("authorizationKey"))
                        {
                            authorizationKey = property0.Value.GetString();
                            continue;
                        }
                        if (property0.NameEquals("routingWeight"))
                        {
                            routingWeight = property0.Value.GetInt32();
                            continue;
                        }
                        if (property0.NameEquals("enableInternetSecurity"))
                        {
                            enableInternetSecurity = property0.Value.GetBoolean();
                            continue;
                        }
                        if (property0.NameEquals("routingConfiguration"))
                        {
                            routingConfiguration = RoutingConfiguration.DeserializeRoutingConfiguration(property0.Value);
                            continue;
                        }
                    }
                    continue;
                }
            }
            return new ExpressRouteConnection(id.Value, name, Optional.ToNullable(provisioningState), expressRouteCircuitPeering.Value, authorizationKey.Value, Optional.ToNullable(routingWeight), Optional.ToNullable(enableInternetSecurity), routingConfiguration.Value);
        }
    }
}
