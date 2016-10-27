
// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Entity
// Author           : v-akdigh
// Created          : 03-06-2014
//
// ***********************************************************************
// <copyright file="SortObject.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines classes used by Search object.</summary>
// ***********************************************************************

namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Provides the structure required for sorting matters/documents. It includes sortable property and sorting direction.
    /// </summary>
    public class SortObject
    {
        /// <summary>
        /// Gets or sets the by property. Represents the property selected for sorting.
        /// </summary>
        /// <value>The by property.</value>

        public string ByProperty
        {
            get;
            set;
        }

        public string ByColumn
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the direction. Represents the order for sorting.
        /// </summary>
        /// <value>The direction.</value>

        public int Direction
        {
            get;
            set;
        }

        /// <summary>
        /// This property will determine whether pinned documents or matters needs to filtered or sorted
        /// </summary>
        public bool SortAndFilterPinnedData
        {
            get;set;
        }
    }
}
