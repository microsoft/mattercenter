using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Provides the structure required for pin/unpin operation performed on matter/document. Meta-data includes the list name, list column, URL, and matter/document details.
    /// </summary>
    public class PinUnpinDetails
    {
        /// <summary>
        /// Gets or sets the name of the list for operation.
        /// </summary>
        /// <value>The name of the list.</value>
        public string ListName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the pinned list column details.
        /// </summary>
        /// <value>The pinned list column details.</value>
        public string PinnedListColumnDetails
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the URL of matter/document.
        /// </summary>
        /// <value>The URL.</value>
        public string URL
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user pinned matter data.
        /// </summary>
        /// <value>The user pinned matter data.</value>
        public MatterData UserPinnedMatterData
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user pinned document data.
        /// </summary>
        /// <value>The user pinned document data.</value>
        public DocumentData UserPinnedDocumentData
        {
            get;
            set;
        }
    }
}
