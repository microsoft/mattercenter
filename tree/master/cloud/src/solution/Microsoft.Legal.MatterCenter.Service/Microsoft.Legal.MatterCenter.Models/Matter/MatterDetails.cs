using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Provides the structure required to hold additional matter meta-data. It includes matter practice group, area of law, sub area of law, responsible attorney, team members, and blocked users.
    /// </summary>
    public class MatterDetails
    {
        ///// <summary>
        ///// Gets or sets the practice group associated with the matter.
        ///// </summary>
        ///// <value>The practice group.</value>

        //public string PracticeGroup
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// Gets or sets the area of law associated with the matter.
        ///// </summary>
        ///// <value>The area of law.</value>

        //public string AreaOfLaw
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// Gets or sets the sub area of law associated with the matter.
        ///// </summary>
        ///// <value>The sub area of law.</value>

        //public string SubareaOfLaw
        //{
        //    get;
        //    set;
        //}

        public IDictionary<string, ManagedColumn> ManagedColumnTerms
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
}
