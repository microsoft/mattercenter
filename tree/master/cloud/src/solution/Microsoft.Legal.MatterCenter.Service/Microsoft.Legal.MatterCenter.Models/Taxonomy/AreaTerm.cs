
// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Entity
// Author           : v-akdigh
// Created          : 03-06-2014
//
// ***********************************************************************
// <copyright file="AreaTerm.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines classes used by Taxonomy.</summary>
// ***********************************************************************


using System.Collections.Generic;
namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Provides the structure required for area of law term. It includes area of law, practice group, id , wssid and list of sub area of law.
    /// </summary>
    public class AreaTerm
    {
        /// <summary>
        /// Gets or sets the name of the term (area of law).
        /// </summary>
        /// <value>The name of the term.</value>
        public string TermName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the parent term (practice group).
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
        /// Gets or sets the sub area terms. It includes the list of sub area of law falling under particular area of law.
        /// </summary>
        /// <value>The sub area terms.</value>
        public IList<SubareaTerm> SubareaTerms
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the id of the term (area of law). This is a GUID value (string representation) and is used as the Id property of the Term.
        /// </summary>
        /// <value>The id of the term.</value>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the WssId of the term (area of law). This is an integer value being used as the Id of the Term in the TaxonomyHiddenList.
        /// </summary>
        /// <value>The WssId of the term.</value>
        public int WssId
        {
            get;
            set;
        }
    }
}
