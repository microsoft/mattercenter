// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Entity
// Author           : v-nikhid
// Created          : 06-19-2014
//
// ***********************************************************************
// <copyright file="MatterProvisionObjects.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This class contains all the properties related to a particular client selected by the user</summary>
// ***********************************************************************

namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Provides the structure required to hold client meta-data. It includes client unique ID, client name, and client URL.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Gets or sets the client identifier. Represents the unique identifier for client.
        /// </summary>
        /// <value>The client identifier.</value>

        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the client. Represents the unique name of client.
        /// </summary>
        /// <value>The name of the client.</value>

        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the client URL.
        /// </summary>
        /// <value>The client URL.</value>

        public virtual string Url
        {
            get;
            set;
        }
    }
}
