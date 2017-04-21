using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Provides the structure of mail/attachment mainly EWS URL, folder location, document library, and flag to overwrite.
    /// </summary>
    public class ServiceRequest
    {
        /// <summary>
        /// Gets or sets the attachment token.
        /// </summary>
        /// <value>The attachment token.</value>
        public string AttachmentToken
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Exchange Web Service URL.
        /// </summary>
        /// <value>The EWS URL.</value>
        public Uri EwsUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the attachments. Represents the objects selected.
        /// </summary>
        /// <value>The attachments.</value>
        public IList<AttachmentDetails> Attachments
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mail identifier.
        /// </summary>
        /// <value>The mail identifier.</value>
        public string MailId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the folder path. Represents the folder path where item needs to be uploaded.
        /// </summary>
        /// <value>The folder path.</value>
        public IList<string> FolderPath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mail subject.
        /// </summary>
        /// <value>The subject.</value>
        public string Subject
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ServiceRequest"/> has overwrite option.
        /// </summary>
        /// <value><c>true</c> if overwrite; otherwise, <c>false</c>.</value>
        public bool Overwrite
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets document library name.
        /// </summary>
        /// <value>Name of document Library</value>
        public string DocumentLibraryName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to perform content check or not.
        /// </summary>
        /// <value>Name of document Library</value>
        public bool PerformContentCheck
        {
            get;
            set;
        }

        /// <summary>
        /// Gets content check flag whether allowed or not
        /// </summary>
        /// <value>Content check enabled or not</value>
        public bool AllowContentCheck
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets all the additional matter properties
        /// </summary>
        public MatterExtraProperties DocumentExtraProperties { get; set; }

    }
}
