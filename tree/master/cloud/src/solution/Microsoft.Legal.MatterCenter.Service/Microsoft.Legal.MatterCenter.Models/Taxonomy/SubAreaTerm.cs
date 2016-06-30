// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Entity
// Author           : v-akdigh
// Created          : 03-06-2014
//
// ***********************************************************************
// <copyright file="SubareaTerm.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines classes used by Taxonomy.</summary>
// ***********************************************************************
using System.Collections.Generic;

namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Provides the structure required for sub area of law term. It includes sub area of law, area of law, folders, folder structure flag, id , wssid and document template.
    /// </summary>
    public class SubareaTerm
    {
        /// <summary>
        /// Gets or sets the name of the term (sub area of law).
        /// </summary>
        /// <value>The name of the term.</value>
        public string TermName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the parent term (area of law).
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
        /// Gets or sets the is no folder structure present. If flag is not set, it will navigate to parent term to get the folder names.
        /// </summary>
        /// <value>The is no folder structure present.</value>
        public string IsNoFolderStructurePresent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document templates. This will be the default content type associated with the document library.
        /// </summary>
        /// <value>The document templates.</value>
        public string DocumentTemplates
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document template names. It includes all the content types that will be available for a document library.
        /// </summary>
        /// <value>The document template names.</value>
        public string DocumentTemplateNames
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the id of the term (sub area of law). This is a GUID value (string representation) and is used as the Id property of the Term.
        /// </summary>
        /// <value>The id of the term.</value>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the WssID of the term (sub area of law). This is an integer value being used as the Id of the Term in the TaxonomyHiddenList.
        /// </summary>
        /// <value>The WssID of the term.</value>
        public int WssId
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
    }
}
