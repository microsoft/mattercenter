using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
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
        /// Gets or sets the entity type (user or security group).
        /// </summary>
        public string EntityType { get; set; }

        /// <summary>
        /// Gets or sets the entity data returned from people picker web service
        /// </summary>
        public EntityData EntityData { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderName { get; set; }
        public bool IsResolved { get; set; }
    }
}
