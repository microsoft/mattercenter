
// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Entity
// Author           : v-akdigh
// Created          : 03-06-2014
//
// ***********************************************************************
// <copyright file="DateFilterObject.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines classes used by Search object.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Provides the structure required for storing date filter.
    /// </summary>
    public class DateFilterObject
    {
        /// <summary>
        /// Gets or sets the 'From' modified date. Represents 'From' modified date selected for filtering.
        /// </summary>
        public string ModifiedFromDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the 'To' modified date. Represents 'To' modified date selected for filtering.
        /// </summary>
        public string ModifiedToDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the 'From' created date. Represents 'From' created date selected for filtering.
        /// </summary>
        public string CreatedFromDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the 'To' created date. Represents 'To' created date selected for filtering.
        /// </summary>
        public string CreatedToDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the 'From' open date. Represents 'From' open date selected for filtering.
        /// </summary>
        public string OpenDateFrom
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the 'To' open date. Represents 'To' open date selected for filtering.
        /// </summary>
        public string OpenDateTo
        {
            get;
            set;
        }
    }
}
