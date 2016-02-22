// ***********************************************************************
// <copyright file="MatterProvisionObject.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provides meta data related information for matter provision.</summary>
// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.CreateSampleData
// Author           : v-akdigh
// Created          : 04-27-2015
//
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.CreateSampleData
{
    #region using
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    #endregion

    /// <summary>
    /// Class containing members which are used for data storage
    /// </summary>
    internal class DataStorage
    {
        /// <summary>
        /// Gets or sets the client name
        /// </summary>
        internal string ClientName { get; set; }

        /// <summary>
        /// Gets or sets the matter prefix
        /// </summary>
        internal string MatterPrefix { get; set; }

        /// <summary>
        /// Gets or sets the matter description 
        /// </summary>
        internal string MatterDescription { get; set; }

        /// <summary>
        /// Gets or sets the id for matter prefix
        /// </summary>
        internal string MatterIdPrefix { get; set; }

        /// <summary>
        /// Gets or sets the practice group
        /// </summary>
        internal string PracticeGroup { get; set; }

        /// <summary>
        /// Gets or sets the area of law
        /// </summary>
        internal string AreaOfLaw { get; set; }

        /// <summary>
        /// Gets or sets the sub area of law
        /// </summary>
        internal string SubAreaOfLaw { get; set; }

        /// <summary>
        /// Gets or sets the conflict check which is performed
        /// </summary>
        internal string ConflictCheckConductedBy { get; set; }

        /// <summary>
        /// Gets or sets the date of conflict 
        /// </summary>
        internal string ConflictDate { get; set; }

        /// <summary>
        /// Gets or sets the blocked user
        /// </summary>
        internal string BlockedUser { get; set; }

        /// <summary>
        /// Gets or sets the responsible attorneys
        /// </summary>
        internal string ResponsibleAttorneys { get; set; }

        /// <summary>
        /// Gets or sets the attorneys
        /// </summary>
        internal string Attorneys { get; set; }

        /// <summary>
        /// Gets or sets the blocked users while uploading
        /// </summary>
        internal string BlockedUploadUsers { get; set; }

        /// <summary>
        /// Gets or sets the permissions from parent
        /// </summary>
        internal string CopyPermissionsFromParent { get; set; }

        /// <summary>
        /// Gets or sets the conflict which are identified
        /// </summary>
        internal string ConflictIdentified { get; set; }

        /// <summary>
        /// Gets or sets the default contentType
        /// </summary>
        internal string DefaultContentType { get; set; }

        /// <summary>
        /// Gets or sets the document count
        /// </summary>
        internal string DocumentCount { get; set; }

        /// <summary>
        /// Gets or sets the matter guid
        /// </summary>
        internal string MatterGuid { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStorage" /> class.
        /// </summary>
        public DataStorage()
        {
            this.MatterGuid = Convert.ToString(Guid.NewGuid(), CultureInfo.InvariantCulture).Replace("-", string.Empty);
        }
    }

    /// <summary>
    /// This class contains members for conflict
    /// </summary>
    internal class Conflict
    {
        /// <summary>
        /// Gets or sets conflict check
        /// </summary>
        internal string ConflictCheckBy { get; set; }

        /// <summary>
        /// Gets or sets conflict check on 
        /// </summary>
        internal string ConflictCheckOn { get; set; }

        /// <summary>
        /// Gets or sets conflict which are identified 
        /// </summary>
        internal string ConflictIdentified { get; set; }

        /// <summary>
        /// Gets or sets secure matter
        /// </summary>
        internal string SecureMatter { get; set; }
    }

    /// <summary>
    /// This class contains members for matter type
    /// </summary>
    internal class MatterType
    {
        /// <summary>
        /// Gets or sets practice group
        /// </summary>
        internal string PracticeGroup { get; set; }

        /// <summary>
        /// Gets or sets area of law
        /// </summary>
        internal string AreaofLaw { get; set; }

        /// <summary>
        /// Gets or sets sub area of law
        /// </summary>
        internal string SubAreaofLaw { get; set; }
    }

    /// <summary>
    /// This class contains members for team info
    /// </summary>
    internal class TeamInfo
    {
        /// <summary>
        /// Gets or sets responsible attorneys
        /// </summary>
        internal string ResponsibleAttorneys { get; set; }

        /// <summary>
        /// Gets or sets attorneys
        /// </summary>
        internal string Attorneys { get; set; }

        /// <summary>
        /// Gets or sets blocked users
        /// </summary>
        internal string BlockedUsers { get; set; }

        /// <summary>
        /// Gets or sets blocked users while uploading
        /// </summary>
        internal string BlockedUploadUsers { get; set; }
    }

    /// <summary>
    /// Class to define Matter
    /// </summary>
    internal class Matter
    {
        /// <summary>
        /// Gets or sets Matter id
        /// </summary>
        internal string MatterId { get; set; }

        /// <summary>
        /// Gets or sets matter name
        /// </summary>
        internal string MatterName { get; set; }

        /// <summary>
        /// Gets or sets matter description
        /// </summary>
        internal string MatterDescription { get; set; }

        /// <summary>
        /// Gets or sets conflict
        /// </summary>
        internal Conflict Conflict { get; set; }

        /// <summary>
        /// Gets or sets assign user names
        /// </summary>
        internal IList<IList<string>> AssignUserNames { get; set; }

        /// <summary>
        /// Gets or sets block user names
        /// </summary>
        internal IList<string> BlockUserNames { get; set; }

        /// <summary>
        /// Gets or sets default content type
        /// </summary>
        internal string DefaultContentType { get; set; }

        /// <summary>
        /// Gets or sets matter type
        /// </summary>
        internal MatterType MatterType { get; set; }

        /// <summary>
        /// Gets or sets team information
        /// </summary>
        internal TeamInfo TeamInfo { get; set; }

        /// <summary>
        /// Gets or sets open date
        /// </summary>
        internal string OpenDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether permissions from parent or not
        /// </summary>
        internal bool CopyPermissionsFromParent { get; set; }

        /// <summary>
        /// Gets or sets document count
        /// </summary>
        internal string DocumentCount { get; set; }

        /// <summary>
        /// Gets or sets matter GUID
        /// </summary>
        internal string MatterGuid { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matter" /> class.
        /// </summary>
        /// <param name="matterData">Matter data</param>
        internal Matter(DataStorage matterData)
        {
            if (null != matterData)
            {
                this.MatterName = matterData.MatterPrefix;
                this.MatterId = matterData.MatterIdPrefix;
                this.MatterDescription = matterData.MatterDescription;
                this.MatterType = new MatterType()
                {
                    PracticeGroup = Utility.ProcessString(matterData.PracticeGroup),
                    AreaofLaw = Utility.ProcessString(matterData.AreaOfLaw),
                    SubAreaofLaw = Utility.ProcessString(matterData.SubAreaOfLaw)
                };
                this.Conflict = new Conflict()
                {
                    ConflictCheckBy = matterData.ConflictCheckConductedBy,
                    ConflictCheckOn = Utility.ValidateDateFormat(matterData.ConflictDate),
                    ConflictIdentified = matterData.ConflictIdentified,
                    SecureMatter = Utility.UpperFirst(Convert.ToString(!Convert.ToBoolean(matterData.CopyPermissionsFromParent, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture))
                };
                this.TeamInfo = new TeamInfo()
                {
                    ResponsibleAttorneys = Utility.ProcessString(matterData.ResponsibleAttorneys),
                    Attorneys = Utility.ProcessString(matterData.Attorneys),
                    BlockedUploadUsers = Utility.ProcessString(matterData.BlockedUploadUsers),
                    BlockedUsers = matterData.BlockedUser,
                };
                string dateFormat = System.Configuration.ConfigurationManager.AppSettings["DateFormat"];
                this.OpenDate = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture).ToString(dateFormat, CultureInfo.InvariantCulture);
                this.CopyPermissionsFromParent = Convert.ToBoolean(matterData.CopyPermissionsFromParent, CultureInfo.InvariantCulture);
                this.BlockUserNames = matterData.BlockedUser.Trim().Split(';').ToList();
                this.DefaultContentType = matterData.DefaultContentType;
                this.DocumentCount = matterData.DocumentCount;
                this.MatterGuid = matterData.MatterGuid;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matter" /> class.
        /// </summary>    

        internal Matter()
        { }
    }

    /// <summary>
    /// This class contains client members
    /// </summary>
    internal class Client
    {
        /// <summary>
        /// Gets or sets client id
        /// </summary>
        internal string ClientId { get; set; }

        /// <summary>
        /// Gets or sets client name
        /// </summary>
        internal string ClientName { get; set; }

        /// <summary>
        /// Gets or sets client url
        /// </summary>
        internal string ClientUrl { get; set; }
    }

    /// <summary>
    /// This class contains client term set members 
    /// </summary>
    internal class ClientTermSets
    {   
        /// <summary>
        /// Gets or sets the client terms in list object
        /// </summary>
        internal IList<Client> ClientTerms { get; set; }
    }

    /// <summary>
    /// Provides the structure required for sub area of law term. It includes sub area of law, area of law, folders, folder structure flag, id , wssid and document template.
    /// </summary>
    internal class SubareaTerm
    {
        /// <summary>
        /// Gets or sets the name of the term (sub area of law).
        /// </summary>
        /// <value>The name of the term.</value>
        internal string TermName { get; set; }

        /// <summary>
        /// Gets or sets the name of the parent term (area of law).
        /// </summary>
        /// <value>The name of the parent term.</value>
        internal string ParentTermName { get; set; }

        /// <summary>
        /// Gets or sets the document templates. This will be the default content type associated with the document library.
        /// </summary>
        /// <value>The document templates.</value>
        internal string DocumentTemplates { get; set; }

        /// <summary>
        /// Gets or sets the folder names. This is used for creation of folders when a matter is created.
        /// </summary>
        /// <value>The folder names.</value>
        internal string FolderNames { get; set; }

        /// <summary>
        /// Gets or sets the content type name. It includes all the content types that will be available for a document library.
        /// </summary>
        /// <value>The content type name.</value>
        internal string ContentTypeName { get; set; }

        /// <summary>
        /// Gets or sets the is no folder structure present. If flag is not set, it will navigate to parent term to get the folder names.
        /// </summary>
        /// <value>The is no folder structure present.</value>
        internal string IsNoFolderStructurePresent { get; set; }

        /// <summary>
        /// Gets or sets the id of the term (sub area of law). This is a GUID value (string representation) and is used as the Id property of the Term.
        /// </summary>
        /// <value>The id of the term.</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the WssID of the term (sub area of law). This is an integer value being used as the Id of the Term in the TaxonomyHiddenList.
        /// </summary>
        /// <value>The WssID of the term.</value>
        public int WssId { get; set; }
    }

    /// <summary>
    /// Provides the structure required for area of law term. It includes area of law, practice group, id , wssid and list of sub area of law.
    /// </summary>
    internal class AreaTerm
    {
        /// <summary>
        /// Gets or sets the name of the term (area of law).
        /// </summary>
        /// <value>The name of the term.</value>
        internal string TermName { get; set; }

        /// <summary>
        /// Gets or sets the name of the parent term (practice group).
        /// </summary>
        /// <value>The name of the parent term.</value>
        internal string ParentTermName { get; set; }

        /// <summary>
        /// Gets or sets the sub area terms. It includes the list of sub area of law falling under particular area of law.
        /// </summary>
        /// <value>The sub area terms.</value>
        internal IList<SubareaTerm> SubareaTerms { get; set; }

        /// <summary>
        /// Gets or sets the folder names. This is used for creation of folders when a matter is created.
        /// </summary>
        /// <value>The folder names.</value>
        internal string FolderNames { get; set; }

        /// <summary>
        /// Gets or sets the id of the term (area of law). This is a GUID value (string representation) and is used as the Id property of the Term.
        /// </summary>
        /// <value>The id of the term.</value>
        internal string Id { get; set; }

        /// <summary>
        /// Gets or sets the WssId of the term (area of law). This is an integer value being used as the Id of the Term in the TaxonomyHiddenList.
        /// </summary>
        /// <value>The WssId of the term.</value>
        internal int WssId { get; set; }
    }

    /// <summary>
    /// Provides the structure required for practice group term set. It includes the practice group, client term set, folders, id , wssid, and list of area team under practice group.
    /// </summary>
    internal class PracticeGroupTerm
    {
        /// <summary>
        /// Gets or sets the name of the term (practice group).
        /// </summary>
        /// <value>The name of the term.</value>
        internal string TermName { get; set; }

        /// <summary>
        /// Gets or sets the name of the parent term (client term set).
        /// </summary>
        /// <value>The name of the parent term.</value>
        internal string ParentTermName { get; set; }

        /// <summary>
        /// Gets or sets all the area of law under particular practice group.
        /// </summary>
        /// <value>The area terms.</value>
        internal IList<AreaTerm> AreaTerms { get; set; }

        /// <summary>
        /// Gets or sets the folder names. This is used for creation of folders when a matter is created.
        /// </summary>
        /// <value>The folder names.</value>
        internal string FolderNames { get; set; }

        /// <summary>
        /// Gets or sets the id of the term (practice group). This is a GUID value (string representation) and is used as the Id property of the Term.
        /// </summary>
        /// <value>The id of the term.</value>
        internal string Id { get; set; }

        /// <summary>
        /// Gets or sets the WssId of the term (practice group). This is an integer value being used as the Id of the Term in the TaxonomyHiddenList.
        /// </summary>
        /// <value>The WssId of the term.</value>
        internal int WssId { get; set; }
    }

    /// <summary>
    /// This class contains term set members
    /// </summary>
    internal class TermSets
    {
        /// <summary>
        /// Gets or sets name
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// Gets or sets practice group term in list object
        /// </summary>
        internal IList<PracticeGroupTerm> PGTerms { get; set; }
    }

    /// <summary>
    /// This class contains term store detail members
    /// </summary>
    internal class TermStoreDetails
    {
        /// <summary>
        /// Gets or sets the term group. Represents the name of parent node of term store.
        /// </summary>
        /// <value>The term group.</value>
        internal string TermGroup
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the term set. Represents the term sets under term group.
        /// </summary>
        /// <value>The name of the term set.</value>
        internal string TermSetName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the custom property. Represents custom properties associated with each term.
        /// </summary>
        /// <value>The name of the custom property.</value>
        internal string CustomPropertyName
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides the structure required to hold additional matter metadata for default values from term store. It includes term store information for matter, client, practice group, area of law and sub area of law.
    /// </summary>
    internal class MatterMetadata
    {
        /// <summary>
        /// Gets or sets the matter property
        /// </summary>
        internal Matter Matter { get; set; }

        /// <summary>
        /// Gets or sets the client property
        /// </summary>
        internal Client Client { get; set; }

        /// <summary>
        /// Gets or sets the practice group property
        /// </summary>
        internal PracticeGroupTerm PracticeGroupTerm { get; set; }

        /// <summary>
        /// Gets or sets the area of law property
        /// </summary>
        internal AreaTerm AreaTerm { get; set; }

        /// <summary>
        /// Gets or sets the subarea of law property
        /// </summary>
        internal SubareaTerm SubareaTerm { get; set; }

        /// <summary>
        /// Gets or sets the list of content types
        /// </summary>
        internal List<string> ContentTypes { get; set; }
    }
}
