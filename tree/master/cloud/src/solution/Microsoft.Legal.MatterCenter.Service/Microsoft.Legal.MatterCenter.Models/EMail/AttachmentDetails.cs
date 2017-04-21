using Newtonsoft.Json;

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
        [JsonProperty("attachmentType")]
        public string attachmentType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>The type of the content.</value>
        [JsonProperty("contentType")]
        public string contentType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mail identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [JsonProperty("id")]
        public string id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is in-line.
        /// </summary>
        /// <value><c>true</c> if this instance is in-line; otherwise, <c>false</c>.</value>
        [JsonProperty("isInline")]
        public bool isInline
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the attachment name.
        /// </summary>
        /// <value>The name.</value>
        [JsonProperty("name")]
        public string name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the attachment size.
        /// </summary>
        /// <value>The size.</value>
        [JsonProperty("size")]
        public int size
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the original name of attachment
        /// </summary>
        [JsonProperty("originalName")]
        public string originalName
        {
            get;
            set;
        }

        /// <summary>
        /// Get or sets the message id associated to this attachment
        /// </summary>
        public string ParentMessageId { get; set; }

        /// <summary>
        /// Get or sets the  current document is email or not
        /// </summary>
        public string IsEmail { get; set; }

        /// <summary>
        /// get or sets the current email or document size
        /// </summary>
        [JsonProperty("contentBytes")]
        public string ContentBytes { get; set; }
    }
}
