// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//
// Code generated by Microsoft (R) AutoRest Code Generator 1.0.1.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace Microsoft.Azure.Management.CosmosDB.Fluent.Models
{
    using Microsoft.Azure;
    using Microsoft.Azure.Management;
    using Microsoft.Azure.Management.CosmosDB;
    using Microsoft.Azure.Management.CosmosDB.Fluent;
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// The failover policy for a given region of a database account.
    /// </summary>
    public partial class FailoverPolicyInner
    {
        /// <summary>
        /// Initializes a new instance of the FailoverPolicyInner class.
        /// </summary>
        public FailoverPolicyInner()
        {
          CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the FailoverPolicyInner class.
        /// </summary>
        /// <param name="id">The unique identifier of the region in which the
        /// database account replicates to. Example:
        /// &amp;lt;accountName&amp;gt;-&amp;lt;locationName&amp;gt;.</param>
        /// <param name="locationName">The name of the region in which the
        /// database account exists.</param>
        /// <param name="failoverPriority">The failover priority of the region.
        /// A failover priority of 0 indicates a write region. The maximum
        /// value for a failover priority = (total number of regions - 1).
        /// Failover priority values must be unique for each of the regions in
        /// which the database account exists.</param>
        public FailoverPolicyInner(string id = default(string), string locationName = default(string), int? failoverPriority = default(int?))
        {
            Id = id;
            LocationName = locationName;
            FailoverPriority = failoverPriority;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets the unique identifier of the region in which the database
        /// account replicates to. Example:
        /// &amp;amp;lt;accountName&amp;amp;gt;-&amp;amp;lt;locationName&amp;amp;gt;.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; private set; }

        /// <summary>
        /// Gets or sets the name of the region in which the database account
        /// exists.
        /// </summary>
        [JsonProperty(PropertyName = "locationName")]
        public string LocationName { get; set; }

        /// <summary>
        /// Gets or sets the failover priority of the region. A failover
        /// priority of 0 indicates a write region. The maximum value for a
        /// failover priority = (total number of regions - 1). Failover
        /// priority values must be unique for each of the regions in which the
        /// database account exists.
        /// </summary>
        [JsonProperty(PropertyName = "failoverPriority")]
        public int? FailoverPriority { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (FailoverPriority < 0)
            {
                throw new ValidationException(ValidationRules.InclusiveMinimum, "FailoverPriority", 0);
            }
        }
    }
}