using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Provides the structure for mail attachment details. It includes MIME type, content type, attachment name, and size.
    /// </summary>
    public class AttachmentDetails
    {
        /// <summary>
        /// Gets or sets the type of the attachment.
        /// </summary>
        /// <value>The type of the attachment.</value>
        public string attachmentType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>The type of the content.</value>
        public string contentType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mail identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is in-line.
        /// </summary>
        /// <value><c>true</c> if this instance is in-line; otherwise, <c>false</c>.</value>
        public bool isInline
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the attachment name.
        /// </summary>
        /// <value>The name.</value>
        public string name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the attachment size.
        /// </summary>
        /// <value>The size.</value>
        public int size
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the original name of attachment
        /// </summary>
        public string originalName
        {
            get;
            set;
        }
    }
}
