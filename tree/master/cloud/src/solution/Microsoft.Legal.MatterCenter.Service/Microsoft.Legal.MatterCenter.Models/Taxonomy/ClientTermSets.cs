
// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Entity
// Author           : v-akdigh
// Created          : 03-06-2014
//
// ***********************************************************************
// <copyright file="ClientTermSets.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines classes used by Taxonomy.</summary>
// ***********************************************************************

using System.Collections.Generic;

namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Provides the structure required for client term set. It includes term set name and list of terms under the term set.
    /// </summary>
    public class ClientTermSets
    {
        /// <summary>
        /// Gets or sets the term name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of client terms present under the clients term set.
        /// </summary>
        /// <value>The client terms.</value>
        public IList<Client> ClientTerms
        {
            get;
            set;
        }
    }
}
