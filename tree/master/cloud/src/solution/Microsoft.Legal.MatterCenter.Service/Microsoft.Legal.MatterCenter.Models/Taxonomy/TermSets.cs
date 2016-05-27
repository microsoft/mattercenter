// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Entity
// Author           : v-akdigh
// Created          : 03-06-2014
//
// ***********************************************************************
// <copyright file="TermSets.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines classes used by Taxonomy.</summary>
// ***********************************************************************
using System.Collections.Generic;


namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Provides the structure required for generic term sets. It includes the term set name and list of practice group under that term set.
    /// </summary>
    public class TermSets
    {
        /// <summary>
        /// Gets or sets the term set name. 
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of practice group terms.
        /// </summary>
        /// <value>The PG terms.</value>
        public IList<PracticeGroupTerm> PGTerms
        {
            get;
            set;
        }
    }
}
