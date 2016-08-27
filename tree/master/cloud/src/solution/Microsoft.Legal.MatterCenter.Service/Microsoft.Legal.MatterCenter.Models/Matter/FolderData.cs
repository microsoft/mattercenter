using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Provides the structure for folder hierarchy within matter.
    /// </summary>
    public class FolderData
    {
        /// <summary>
        /// Gets or sets the name. Represents the folder under the matter library.
        /// </summary>
        /// <value>The name.</value>        
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the URL. Represents the folder URL under the matter library.
        /// </summary>
        /// <value>The URL.</value>       
        public string URL
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parent URL. Represents the parent URL.
        /// </summary>
        /// <value>The parent URL.</value>        
        public string ParentURL
        {
            get;
            set;
        }
    }
}
