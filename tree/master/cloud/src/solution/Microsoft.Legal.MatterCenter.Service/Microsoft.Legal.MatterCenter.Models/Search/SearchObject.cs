
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
    /// Provides the structure required for searching matters/documents. It includes page number, search keyword, filter conditions, sort conditions, and numbers of items to be shown on page.
    /// </summary>
    public class SearchObject
    {
        /// <summary>
        /// Gets or sets the page number. Represents the current page number that is displayed on the page.
        /// </summary>
        /// <value>The page number.</value>

        public int PageNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the items per page. Represents number of items to be shown on the page.
        /// </summary>
        /// <value>The items per page.</value>

        public int ItemsPerPage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the search term. Represents the search keyword/term to retrieve the results.
        /// </summary>
        /// <value>The search term.</value>

        public string SearchTerm
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the filters. Represents the filtering condition.
        /// </summary>
        /// <value>The filters.</value>

        public FilterObject Filters
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the sort. Represents the searching condition.
        /// </summary>
        /// <value>The sort condition.</value>

        public SortObject Sort
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the search term. Represents the isunique or not for filter results.
        /// </summary>
        /// <value>The search term.</value>

        public bool IsUnique
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the search term. Represents the column name for filter to get unique result.
        /// </summary>
        /// <value>The search term.</value>

        public string UniqueColumnName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the search term. Represents the filter value for filter to get unique result.
        /// </summary>
        /// <value>The search term.</value>

        public string FilterValue
        {
            get;
            set;
        }
    }
}
