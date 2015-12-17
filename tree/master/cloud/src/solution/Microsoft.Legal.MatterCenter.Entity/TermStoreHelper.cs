// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Entity
// Author           : v-rijadh
// Created          : 06-19-2014
//
// ***********************************************************************
// <copyright file="TermStoreHelper.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines classes used by Term Store object.</summary>
// ***********************************************************************

namespace Microsoft.Legal.MatterCenter.Entity
{
    #region using
    using System.Collections.Generic;
    #endregion

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
    }

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

    /// <summary>
    /// Provides the structure required for generic term groups. It includes the term group name and the list of term sets under the group.
    /// </summary>
    public class TermGroups
    {
        /// <summary>
        /// Gets or sets the term group name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of term sets under the term group.
        /// </summary>
        /// <value>The term sets.</value>
        public IList<TermSets> TermSets
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides the structure required for generic term stores. It includes the term store and the list of term group under the term store.
    /// </summary>
    public class TermStores
    {
        /// <summary>
        /// Gets or sets the term store name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of term groups under the term store.
        /// </summary>
        /// <value>The term groups.</value>
        public IList<TermGroups> TermGroups
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides the structure required for generic collection of term stores.
    /// </summary>
    public class TermStoreObject
    {
        /// <summary>
        /// Gets or sets the list of term stores.
        /// </summary>
        /// <value>The term stores.</value>
        public IList<TermStores> TermStores
        {
            get;
            set;
        }
    }
}