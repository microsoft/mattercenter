using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
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
}
