
// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Entity
// Author           : v-akdigh
// Created          : 03-06-2014
//
// ***********************************************************************
// <copyright file="FilterObject.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines classes used by Search object.</summary>
// ***********************************************************************
using System.Collections.Generic;


namespace Microsoft.Legal.MatterCenter.Models
{

    /// <summary>
    /// Provides the structure required for filtering matters/documents. It includes list of clients, practice group, area of law, sub area of law, from date, to date, and document author.
    /// </summary>
    public class FilterObject
    {
        /// <summary>
        /// Gets or sets the clients list. Represents the clients selected for filtering.
        /// </summary>
        /// <value>The clients list.</value>

        public IList<string> ClientsList
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of practice group. Represents the practice groups selected for filtering.
        /// </summary>
        /// <value>The PGList.</value>

        public IList<string> PGList
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of area of law. Represents the area of law selected for filtering.
        /// </summary>
        /// <value>The AOL list.</value>

        public IList<string> AOLList
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets FromDate. Represents the start date for filtering.
        /// </summary>
        /// <value>From date.</value>

        public string FromDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets ToDate. Represents the end date for filtering.
        /// </summary>
        /// <value>To date.</value>

        public string ToDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the FilterByMe flag. Represents filtering on items created by logged-in user.
        /// </summary>
        /// <value>The filter by me.</value>

        public int FilterByMe
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document author. Represents authors selected for filtering.
        /// </summary>
        /// <value>The document author.</value>

        public string DocumentAuthor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document/ matter name. Represents document/ matter name selected for filtering.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document's/ matter's client name. Represents document's/ matter's client name selected for filtering.
        /// </summary>
        public string ClientName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document's checked out user name. Represents the list of check out user names selected for filtering.
        /// </summary>
        public string DocumentCheckoutUsers
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Date filters. Represents collection of date filters selected for filtering.
        /// </summary>
        public DateFilterObject DateFilters
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Responsible attorney. Represents the list of Responsible attorneys selected for filtering.
        /// </summary>
        public string ResponsibleAttorneys
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Sub area of Law. Represents Sub area of Law selected for filtering.
        /// </summary>
        public string SubareaOfLaw
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Sub area of Law. Represents Sub area of Law selected for filtering.
        /// </summary>
        public string PracticeGroup
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Project Name. Represents Project Name selected for filtering.
        /// </summary>
        public string ProjectName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Sub area of Law. Represents Sub area of Law selected for filtering.
        /// </summary>
        public string AreaOfLaw
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the Sub area of Law. Represents Sub area of Law selected for filtering.
        /// </summary>
        public string ProjectID
        {
            get;
            set;
        }

    }

}
