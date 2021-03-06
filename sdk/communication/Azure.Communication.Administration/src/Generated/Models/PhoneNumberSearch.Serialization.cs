// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using System.Text.Json;
using Azure.Core;

namespace Azure.Communication.Administration.Models
{
    public partial class PhoneNumberSearch
    {
        internal static PhoneNumberSearch DeserializePhoneNumberSearch(JsonElement element)
        {
            Optional<string> searchId = default;
            Optional<string> displayName = default;
            Optional<DateTimeOffset> createdAt = default;
            Optional<string> description = default;
            Optional<IReadOnlyList<string>> phonePlanIds = default;
            Optional<string> areaCode = default;
            Optional<int> quantity = default;
            Optional<IReadOnlyList<LocationOptionsDetails>> locationOptions = default;
            Optional<SearchStatus> status = default;
            Optional<IReadOnlyList<string>> phoneNumbers = default;
            Optional<DateTimeOffset> reservationExpiryDate = default;
            Optional<int> errorCode = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("searchId"))
                {
                    searchId = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("displayName"))
                {
                    displayName = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("createdAt"))
                {
                    createdAt = property.Value.GetDateTimeOffset("O");
                    continue;
                }
                if (property.NameEquals("description"))
                {
                    description = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("phonePlanIds"))
                {
                    List<string> array = new List<string>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(item.GetString());
                    }
                    phonePlanIds = array;
                    continue;
                }
                if (property.NameEquals("areaCode"))
                {
                    areaCode = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("quantity"))
                {
                    quantity = property.Value.GetInt32();
                    continue;
                }
                if (property.NameEquals("locationOptions"))
                {
                    List<LocationOptionsDetails> array = new List<LocationOptionsDetails>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(LocationOptionsDetails.DeserializeLocationOptionsDetails(item));
                    }
                    locationOptions = array;
                    continue;
                }
                if (property.NameEquals("status"))
                {
                    status = new SearchStatus(property.Value.GetString());
                    continue;
                }
                if (property.NameEquals("phoneNumbers"))
                {
                    List<string> array = new List<string>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(item.GetString());
                    }
                    phoneNumbers = array;
                    continue;
                }
                if (property.NameEquals("reservationExpiryDate"))
                {
                    reservationExpiryDate = property.Value.GetDateTimeOffset("O");
                    continue;
                }
                if (property.NameEquals("errorCode"))
                {
                    errorCode = property.Value.GetInt32();
                    continue;
                }
            }
            return new PhoneNumberSearch(searchId.Value, displayName.Value, Optional.ToNullable(createdAt), description.Value, Optional.ToList(phonePlanIds), areaCode.Value, Optional.ToNullable(quantity), Optional.ToList(locationOptions), Optional.ToNullable(status), Optional.ToList(phoneNumbers), Optional.ToNullable(reservationExpiryDate), Optional.ToNullable(errorCode));
        }
    }
}
