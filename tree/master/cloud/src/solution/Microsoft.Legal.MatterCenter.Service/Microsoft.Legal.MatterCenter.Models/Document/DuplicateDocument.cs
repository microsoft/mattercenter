using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Provides the structure required for checking if document already exists.
    /// </summary>
    public class DuplicateDocument
    {
        /// <summary>
        /// Gets or sets a value indicating whether a document with the same name exists or not.
        /// </summary>
        /// <value><c>true</c> if document with the same name found; otherwise, <c>false</c></value>
        public bool DocumentExists { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a potential duplicate exists, i.e. along with name, other properties also match
        /// </summary>
        /// <value><c>true</c> if potential duplicate found; otherwise, <c>false</c></value>
        public bool HasPotentialDuplicate { get; set; }

        /// <summary>
        /// Constructor to initialize the DuplicateDocument object
        /// </summary>
        /// <param name="documentExists">Default value for DocumentExists property</param>
        /// <param name="hasPotentialDuplicate">Default value for HasPotentialDuplicate property</param>
        public DuplicateDocument(bool documentExists, bool hasPotentialDuplicate)
        {
            this.DocumentExists = documentExists;
            this.HasPotentialDuplicate = hasPotentialDuplicate;
        }
    }
}
