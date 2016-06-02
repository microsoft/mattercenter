using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
    public class MailAttachmentDetails
    {
        /// <summary>
        /// Gets or sets the full URL of mail attachment.
        /// </summary>
        /// <value>The full URL.</value>
        public string FullUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets overwrite option for mail attachment.
        /// </summary>
        /// <value>The overwrite.</value>
        public string AttachmentContent
        {
            get;
            set;
        }        

        /// <summary>
        /// Gets or sets value based on attachment call for mail attachment.
        /// </summary>
        /// <value>The Attachment Flag.</value>
        public bool IsAttachmentCall
        {
            get;
            set;
        }
    }
}
