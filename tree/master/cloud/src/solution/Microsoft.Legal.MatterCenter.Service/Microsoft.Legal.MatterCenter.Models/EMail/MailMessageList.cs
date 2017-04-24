using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graph;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace Microsoft.Legal.MatterCenter.Models
{
    public class MailMessageList
    {
        /// <summary>
        /// The list of messages
        /// </summary>
        [JsonProperty("value")]
        public List<MailMessage> Messages { get; set; }
        public int MailCount { get; set; }
    }

    /// <summary>
    /// Defines an email message
    /// </summary>
    public class MailMessage
    {
        public MailMessage()
        {
            this.Attachments = new List<AttachmentDetails>();
        }
        /// <summary>
        /// The importance of the email message
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ItemImportance Importance { get; set; }
       
        /// <summary>
        /// The subject of the email message
        /// </summary>
        public String Subject { get; set; }
        /// <summary>
        /// The body of the email message
        /// </summary>
        [JsonProperty("body")]
        public ItemBody Body { get; set; }
        /// <summary>
        /// The UTC sent date and time of the email message
        /// </summary>
        public Nullable<DateTime> SentDateTime { get; set; }
        /// <summary>
        /// The UTC received date and time of the email message
        /// </summary>
        public Nullable<DateTime> ReceivedDateTime { get; set; }
        /// <summary>
        /// Defines whether the email message is read on unread
        /// </summary>
        public Boolean IsRead { get; set; }
        /// <summary>
        /// Defines whether the email message is a draft
        /// </summary>
        public Boolean IsDraft { get; set; }
        /// <summary>
        /// Defines whether the email has attachments
        /// </summary>
        public Boolean HasAttachments { get; set; }
        /// <summary>
        /// The list of email message attachments, if any
        /// </summary>
        public List<AttachmentDetails> Attachments { get; set; }
        /// <summary>
        /// get or sets id of email message
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// get or sets current document is email or not
        /// </summary>
        public string IsEmail { get; set; }
        /// <summary>
        /// get or sets current document size
        /// </summary>
        [JsonProperty("contentBytes")]
        public string ContentBytes { get; set; }
    }

    

    /// <summary>
    /// Defines the importance of an email message
    /// </summary>
    public enum ItemImportance
    {
        /// <summary>
        /// Low importance
        /// </summary>
        Low,
        /// <summary>
        /// Normal importance, default value
        /// </summary>
        Normal,
        /// <summary>
        /// High importance
        /// </summary>
        High,
    }

    

    /// <summary>
    /// Defines a user info
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// The email address
        /// </summary>
        public String Address { get; set; }
        /// <summary>
        /// The description of the email address
        /// </summary>
        public String Name { get; set; }
    }
}
