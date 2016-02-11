// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Entity
// Author           : v-nikhid
// Created          : 06-19-2014
//
// ***********************************************************************
// <copyright file="MatterProvisionObjects.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines classes used under current project.</summary>
// ***********************************************************************
//// Keeping using System over here because of usage of CLSComplaint attribute for namespace
using System;
[assembly: CLSCompliant(true)]
namespace Microsoft.Legal.MatterCenter.Entity
{
    #region using
    using System;
    using System.Collections.Generic;
    #endregion

    /// <summary>
    /// Holds the SP app token to authenticate current request.
    /// </summary>   
    [CLSCompliant(true)]
    public class RequestObject
    {
        /// <summary>
        /// Gets or sets the SP application token. Represents the token for authenticated user.
        /// </summary>
        /// <value>The SP application token.</value>

        public string SPAppToken
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the refresh token. Represents the token used for single sign-on.
        /// </summary>
        /// <value>The refresh token.</value>

        public string RefreshToken
        {
            get;
            set;
        }
    }

    /// <summary> 
    /// Provides the structure required to hold flag values
    /// </summary>   
    public class MatterProvisionFlags
    {
        /// <summary>
        /// Gets or sets value for matter landing flag
        /// </summary>
        /// <value>Matter landing</value>

        public string MatterLandingFlag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets boolean value for send email flag
        /// </summary>
        /// <value>Send email</value>

        public bool SendEmailFlag
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides the structure required to hold client meta-data. It includes client unique ID, client name, and client URL.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Gets or sets the client identifier. Represents the unique identifier for client.
        /// </summary>
        /// <value>The client identifier.</value>

        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the client. Represents the unique name of client.
        /// </summary>
        /// <value>The name of the client.</value>

        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the client URL.
        /// </summary>
        /// <value>The client URL.</value>

        public string Url
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides the structure required to hold matter meta-data. It includes matter ID, name, description, conflict details, allowed teams, blocked users, permissions, and content type.
    /// </summary>
    public class Matter
    {
        /// <summary>
        /// Gets or sets the matter identifier. Represents the matter ID under a client.
        /// </summary>
        /// <value>The matter identifier.</value>

        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the matter. Represents the matter name under a client.
        /// </summary>
        /// <value>The name of the matter.</value>

        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the matter description. Represents the description of matter library.
        /// </summary>
        /// <value>The matter description.</value>

        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the matter conflict meta-data. Represents conflict checked by user, conflict check date, and security of matter from external users.
        /// </summary>
        /// <value>The conflict.</value>

        public Conflict Conflict
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user names. Represents the user names associated with the matter.
        /// </summary>
        /// <value>The user names.</value>

        public IList<string> UserNames
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the content types. Represents all the content types associated with the matter library.
        /// </summary>
        /// <value>The content types.</value>

        public IList<string> ContentTypes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the assigned user names. Represents the team members and responsible attorneys associated with the matter.
        /// </summary>
        /// <value>The assign user names.</value>

        public IList<IList<string>> AssignUserNames
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the assigned user email addresses. Represents the team members associated with the project.
        /// </summary>
        /// <value>The list of assign user email addresses.</value>
        public IList<IList<string>> AssignUserEmails
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the blocked user names. Represents the users who are blocked from viewing the matter details.
        /// </summary>
        /// <value>The block user names.</value>

        public IList<string> BlockUserNames
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the permissions. Represents the permission levels associated with the matter.
        /// </summary>
        /// <value>The permissions.</value>

        public IList<string> Permissions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the roles. Represents the role of users associated with the matter.
        /// </summary>
        /// <value>The roles.</value>

        public IList<string> Roles
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the folder names. Represents the list of folders that will be created with the matter.
        /// </summary>
        /// <value>The folder names.</value>

        public IList<string> FolderNames
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the default content type.
        /// </summary>
        /// <value>The default content type.</value>

        public string DefaultContentType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Document template count. Represents the document templates count of matter.
        /// </summary>
        /// <value>The Document template count.</value>

        public IList<string> DocumentTemplateCount
        {
            get;
            set;
        }

        ///// <summary>
        ///// Gets or sets the Matter GUID.
        ///// </summary>
        ///// <value>The Matter GUID.</value>

        public string MatterGuid
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides the structure required to hold matter conflict meta-data. It includes user who performed the conflict check, the conflict check date, and security of matter from external users.
    /// </summary>
    public class Conflict
    {
        /// <summary>
        /// Gets or sets the user who performed the conflict check for matter.
        /// </summary>
        /// <value>The conflict check by.</value>

        public string CheckBy
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the conflict check date for matter.
        /// </summary>
        /// <value>The conflict check on.</value>

        public string CheckOn
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the conflict identified option (Yes/No) for matter. Represents if the conflict was identified or not.
        /// </summary>
        /// <value>The conflict identified.</value>

        public string Identified
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the flag to mark the matter as secure or not.
        /// </summary>
        /// <value>The secure matter.</value>

        public string SecureMatter
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides the structure required to hold additional matter meta-data. It includes matter practice group, area of law, sub area of law, responsible attorney, team members, and blocked users.
    /// </summary>
    public class MatterDetails
    {
        /// <summary>
        /// Gets or sets the practice group associated with the matter.
        /// </summary>
        /// <value>The practice group.</value>

        public string PracticeGroup
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the area of law associated with the matter.
        /// </summary>
        /// <value>The area of law.</value>

        public string AreaOfLaw
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the sub area of law associated with the matter.
        /// </summary>
        /// <value>The sub area of law.</value>

        public string SubareaOfLaw
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the responsible attorney associated with the matter.
        /// </summary>
        /// <value>The responsible attorney.</value>

        public string ResponsibleAttorney
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the responsible attorney email associated with the matter.
        /// </summary>
        /// <value>The list of responsible attorney email addresses.</value>

        public string ResponsibleAttorneyEmail
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the team members associated with the matter.
        /// </summary>
        /// <value>The team members.</value>

        public string TeamMembers
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the users who can only view the matter.
        /// </summary>
        /// <value>The upload blocked users.</value>

        public IList<string> UploadBlockedUsers
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the role information associated with the matter.
        /// </summary>
        /// <value>The team members.</value>

        public string RoleInformation
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides the structure required to hold role meta-data. It includes role ID, name, and flag indicating whether role is mandatory or not.
    /// </summary>
    public class Role
    {
        /// <summary>
        /// Gets or sets the role identifier. Represents the role identifier.
        /// </summary>
        /// <value>The role identifier.</value>

        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the role. Represents the role name.
        /// </summary>
        /// <value>The name of the role.</value>

        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Role"/> is mandatory.
        /// </summary>
        /// <value><c>true</c> if mandatory; otherwise, <c>false</c>.</value>

        public bool Mandatory
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides the structure required to hold user meta-data. It includes user ID, name/alias, logged-in user name, and email.
    /// </summary>
    public class Users
    {
        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        /// <value>The name of the user.</value>

        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the logged-in user name.
        /// </summary>
        /// <value>The name of the user log on.</value>

        public string LogOnName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user email.
        /// </summary>
        /// <value>The user email.</value>

        public string Email
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the entity type (user or security group).
        /// </summary>
        public string EntityType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Provider name indicating tenant or extranet user
        /// </summary>
        public string ProviderName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the entity data returned from people picker web service
        /// </summary>
        public EntityData EntityData { get; set; }
    }

    /// <summary>
    /// Provides the structure required to hold term store meta-data. It includes term group, term set, and additional custom property.
    /// </summary>
    public class TermStoreDetails
    {
        /// <summary>
        /// Gets or sets the term group. Represents the name of parent node of term store.
        /// </summary>
        /// <value>The term group.</value>

        public string TermGroup
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the term set. Represents the term sets under term group.
        /// </summary>
        /// <value>The name of the term set.</value>

        public string TermSetName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the custom property. Represents custom properties associated with each term.
        /// </summary>
        /// <value>The name of the custom property.</value>

        public string CustomPropertyName
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides the structure required to hold user meta-data retrieved from client people picker web service.
    /// </summary>
    public class PeoplePickerUser
    {
        /// <summary>
        /// Gets or sets the user logon name.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the user email.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the user display name.
        /// </summary>
        public string DisplayText { get; set; }

        /// <summary>
        /// Gets or sets the Provider name indicating tenant or extranet user
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// Gets or sets the entity type (user or security group).
        /// </summary>
        public string EntityType { get; set; }

        /// <summary>
        /// Gets or sets the entity data returned from people picker web service
        /// </summary>
        public EntityData EntityData { get; set; }
    }

    /// <summary>
    /// Holds the structure of entity data returned from people picker web service.
    /// </summary>
    public class EntityData
    {
        /// <summary>
        /// Gets or sets the title of user or security group
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the department of user or security group
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Gets or sets the email of user or security group
        /// </summary>
        public string Email { get; set; }
    }

    /// <summary>
    /// Provides the structure required to hold matter property bag information (stamped properties).
    /// </summary>
    public class MatterStampedDetails
    {
        /// <summary>
        /// Indicates if the matter is old or new
        /// </summary>
        public string IsNewMatter { get; set; }

        /// <summary>
        /// Provides the structure required to hold matter meta-data. It includes matter ID, name, description, conflict details, allowed teams, blocked users, permissions, and content type.
        /// </summary>
        public Matter MatterObject { get; set; }

        /// <summary>
        /// Provides the structure required to hold additional matter meta-data. It includes matter practice group, area of law, sub area of law, responsible attorney, team members, and blocked users.
        /// </summary>
        public MatterDetails MatterDetailsObject { get; set; }

        /// <summary>
        /// Provides the structure required to hold client meta-data. It includes client unique ID, client name, and client URL.
        /// </summary>
        public Client ClientObject { get; set; }
    }

    /// <summary>
    /// Provides the structure needed to revert the matter permission update operation.
    /// </summary>
    [CLSCompliant(false)]
    public class MatterRevertList
    {
        /// <summary>
        /// Gets or sets a matter library instance 
        /// </summary>
        public string MatterLibrary { get; set; }

        /// <summary>
        /// Gets or sets a matter OneNote library instance
        /// </summary>
        public string MatterOneNoteLibrary { get; set; }

        /// <summary>
        /// Gets or sets a matter calendar instance
        /// </summary>
        public string MatterCalendar { get; set; }

        /// <summary>
        /// Gets or sets a matter site pages instance
        /// </summary>
        public string MatterSitePages { get; set; }

        /// <summary>
        /// Gets or sets a matter task instance
        /// </summary>
        public string MatterTask { get; set; }
    }

    /// <summary>
    /// Represents configuration for the matter. Holds value for various entities on the Matter Provision page
    /// </summary>   
    public class MatterConfigurations
    {
        /// <summary>
        /// Gets or sets the name of the matter. Represents the name of matter library.
        /// </summary>
        /// <value>The default name of the matter.</value>
        public string DefaultMatterName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the default matter id
        /// </summary>
        /// <value>The default matter id</value>
        public string DefaultMatterId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the default matter type
        /// </summary>
        /// <value>The default matter type</value>
        public string DefaultMatterType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the matter types
        /// </summary>
        /// <value>The matter types</value>
        public string MatterTypes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the users of the matters
        /// </summary>
        /// <value>The matter users</value>
        public string MatterUsers
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the emails for users of the projects
        /// </summary>
        /// <value>The list of matter center user emails</value>
        public string MatterUserEmails
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the matter roles for the users
        /// </summary>
        /// <value>The matter roles for the users</value>
        public string MatterRoles
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the matter permissions
        /// </summary>
        /// <value>The matter permissions</value>
        public string MatterPermissions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the flag for calendar selection on settings page
        /// </summary>
        /// <value>Calendar selected flag</value>
        public bool IsCalendarSelected
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the email option selection flag
        /// </summary>
        /// <value>The email option selection flag</value>
        public bool IsEmailOptionSelected
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the RSS selection flag
        /// </summary>
        /// <value>The RSS selection flag</value>
        public bool IsRSSSelected
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the matter id.
        /// </summary>
        /// <value>The matter id.</value>
        public bool IsRestrictedAccessSelected
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the conflict check flag
        /// </summary>
        /// <value>The conflict check visibility flag</value>
        public bool IsConflictCheck
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the matter description mandatory flag
        /// </summary>
        /// <value>The matter description mandatory flag</value>
        public bool IsMatterDescriptionMandatory
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the matter practice group
        /// </summary>
        /// <value>The matter group</value>
        public string MatterPracticeGroup
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the matter area of law
        /// </summary>
        /// <value>The matter area of law</value>
        public string MatterAreaofLaw
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the content check property
        /// </summary>
        /// <value>The content check property</value>
        public bool IsContentCheck
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tasks selected property
        /// </summary>
        /// <value>The tasks selected property</value>
        public bool IsTaskSelected
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides the structure required to hold additional matter meta-data for default values from term store. It includes term store information for matter, client, practice group, area of law and sub area of law.
    /// </summary>
    public class MatterMetadata
    {
        /// <summary>
        /// Gets or sets the matter property
        /// </summary>
        public Matter Matter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the client property
        /// </summary>
        public Client Client
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the practice group property
        /// </summary>
        public PracticeGroupTerm PracticeGroupTerm
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the area of law property
        /// </summary>
        public AreaTerm AreaTerm
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Subarea of law property
        /// </summary>
        public SubareaTerm SubareaTerm
        {
            get;
            set;
        }
    }
}
