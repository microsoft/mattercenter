using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
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
