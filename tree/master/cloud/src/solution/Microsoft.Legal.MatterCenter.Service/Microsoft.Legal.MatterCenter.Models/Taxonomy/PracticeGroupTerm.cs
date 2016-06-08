
// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Entity
// Author           : v-akdigh
// Created          : 03-06-2014
//
// ***********************************************************************
// <copyright file="PracticeGroupTerm.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines classes used by Taxonomy.</summary>
// ***********************************************************************
using System.Collections.Generic;
namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Provides the structure required for practice group term set. It includes the practice group, client term set, folders, id , wssid, and list of area team under practice group.
    /// </summary>
    public class PracticeGroupTerm
    {
        /// <summary>
        /// Gets or sets the name of the term (practice group).
        /// </summary>
        /// <value>The name of the term.</value>
        public string TermName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the parent term (client term set).
        /// </summary>
        /// <value>The name of the parent term.</value>
        public string ParentTermName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the folder names. This is used for creation of folders when a matter is created.
        /// </summary>
        /// <value>The folder names.</value>
        public string FolderNames
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets all the area of law under particular practice group.
        /// </summary>
        /// <value>The area terms.</value>
        public IList<AreaTerm> AreaTerms
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the id of the term (practice group). This is a GUID value (string representation) and is used as the Id property of the Term.
        /// </summary>
        /// <value>The id of the term.</value>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the WssId of the term (practice group). This is an integer value being used as the Id of the Term in the TaxonomyHiddenList.
        /// </summary>
        /// <value>The WssId of the term.</value>
        public int WssId
        {
            get;
            set;
        }
    }
}
