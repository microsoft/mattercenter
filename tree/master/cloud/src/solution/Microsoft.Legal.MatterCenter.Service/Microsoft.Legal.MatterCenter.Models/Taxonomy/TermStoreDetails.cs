// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Entity
// Author           : v-nikhid
// Created          : 06-19-2014
//
// ***********************************************************************
// <copyright file="TermStoreDetails.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines classes used by Taxonomy.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Provides the structure required to hold term store meta-data. It includes term group, term set, and additional custom property.
    /// </summary>
    public class TermStoreDetails
    {
        /// <summary>
        /// Gets or sets the term group. Represents the name of parent node of term store.
        /// </summary>
        /// <value>The term group.</value>

        public string TermGroup
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the term set. Represents the term sets under term group.
        /// </summary>
        /// <value>The name of the term set.</value>

        public string TermSetName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the custom property. Represents custom properties associated with each term.
        /// </summary>
        /// <value>The name of the custom property.</value>

        public string CustomPropertyName
        {
            get;
            set;
        }
    }
}
