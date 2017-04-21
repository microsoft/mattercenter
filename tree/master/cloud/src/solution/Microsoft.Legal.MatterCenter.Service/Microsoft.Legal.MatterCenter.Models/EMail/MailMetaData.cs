using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Provides the structure to hold mail meta data
    /// </summary>
    public struct MailMetaData
    {
        /// <summary>
        /// Gets or sets the user who receives the mail.
        /// </summary>
        /// <value>Name of the user who receives the mail</value>
        public string mailReceiver { get; set; }

        /// <summary>
        /// Gets or sets the user who sends the mail.
        /// </summary>
        /// <value>Name of the user who sends the mail</value>
        public string mailSender { get; set; }

        /// <summary>
        /// Gets or sets the mail received date.
        /// </summary>
        /// <value>Mail received date</value>
        public string receivedDate { get; set; }

        /// <summary>
        /// Gets or sets the user who are in CC.
        /// </summary>
        /// <value>Name of the user who are in CC</value>
        public string cc { get; set; }

        /// <summary>
        /// Gets or sets the attachment of the mail.
        /// </summary>
        /// <value>Attachment of the mail</value>
        public string attachment { get; set; }

        /// <summary>
        /// Gets or sets the importance of the mail.
        /// </summary>
        /// <value>Importance of the mail</value>
        public string mailImportance { get; set; }

        /// <summary>
        /// Gets or sets the subject of the mail.
        /// </summary>
        /// <value>Subject of the mail</value>
        public string mailSubject { get; set; }

        /// <summary>
        /// Gets or sets the categories of the mail
        /// </summary>
        /// <value>Category of the mail</value>
        public string categories { get; set; }

        /// <summary>
        /// Gets or sets the sensitivity of the mail
        /// </summary>
        /// <value>Sensitivity of the mail</value>
        public string sensitivity { get; set; }

        /// <summary>
        /// Gets or sets the conversation id of the mail
        /// </summary>
        /// <value>Conversation id of the mail</value>
        public string conversationId { get; set; }

        /// <summary>
        /// Gets or sets the conversation topic  of the mail
        /// </summary>
        /// <value>Conversation topic of the mail</value>
        public string conversationTopic { get; set; }

        /// <summary>
        /// Gets or sets the sent date of the mail
        /// </summary>
        /// <value>Sent date of the mail</value>
        public string sentDate { get; set; }

        /// <summary>
        /// Gets or sets the 'Has attachments' of the mail
        /// </summary>
        /// <value>'Has attachments' of the mail</value>
        public string hasAttachments { get; set; }

        /// <summary>
        /// Gets or sets the original name of the mail
        /// </summary>
        /// <value>Original name of the mail</value>
        public string originalName { get; set; }

        /// <summary>
        /// Get or sets the attachements associated with the current mail message
        /// </summary>
        public List<AttachmentDetails> AttachmentDetails { get; set; }

        /// <summary>
        /// Get or sets the message id
        /// </summary>
        public string id { get; set; }
    }
}
