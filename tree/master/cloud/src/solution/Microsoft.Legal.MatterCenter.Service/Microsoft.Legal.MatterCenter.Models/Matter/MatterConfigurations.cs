using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
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

        /// <summary>
        /// The client name/url to which the settings needs to be retrieved or needs to be updated
        /// </summary>
        public string ClientUrl
        {
            get;
            set;
        }

        /// <summary>
        /// The date time when the matter configurations are updated
        /// </summary>
        public string CachedItemModifiedDate
        {
            get;
            set;
        }

        /// <summary>
        /// The user who has modified the matter configurations
        /// </summary>
        public IList<string> UserId
        {
            get;
            set;
        }
    }
}
