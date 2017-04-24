using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Provides the structure required for performing content check.
    /// </summary>
    public class ContentCheckDetails
    {
        /// <summary>
        /// Gets or sets the name of the file for which content check needs to be performed
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the Subject field of the file for which content check needs to be performed. Applicable only for email
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the size of the file for which content check needs to be performed
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Gets or sets the From field of the file for which content check needs to be performed. Applicable only for email
        /// </summary>
        public string FromField { get; set; }

        /// <summary>
        /// Gets or sets the Sent date field of the file for which content check needs to be performed. Applicable only for email
        /// </summary>
        public string SentDate { get; set; }

        /// <summary>
        /// Two-parameters constructor to initialize object for checking if document exists with the same file name and size
        /// </summary>
        /// <param name="fileName">Name of the file being uploaded</param>
        /// <param name="fileSize">Size of the file being uploaded</param>
        public ContentCheckDetails(string fileName, long fileSize)
        {
            this.FileName = fileName;
            this.Subject = string.Empty;
            this.FileSize = fileSize;
            this.FromField = string.Empty;
            this.SentDate = string.Empty;
        }

        /// <summary>
        /// Four-parameters constructor to initialize object for checking if mail exists with the same file name, size, from field and sent date
        /// </summary>
        /// <param name="fileName">Name of the mail being uploaded</param>
        /// <param name="fileSize">Size of the mail being uploaded</param>
        /// <param name="fromField">Value in the From field of the mail being uploaded</param>
        /// <param name="sentDate">Sent date of the mail being uploaded</param>
        public ContentCheckDetails(string fileName, string subject, long fileSize, string fromField, string sentDate)
        {
            this.FileName = fileName;
            this.Subject = subject;
            this.FileSize = fileSize;
            this.FromField = fromField;
            this.SentDate = sentDate;
        }
    }
}
