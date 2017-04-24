using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Provides the structure required for mail XML response
    /// </summary>
    public struct MailXPath
    {
        /// <summary>
        /// Gets or sets the user who receives the mail.
        /// </summary>
        /// <value>Name of the user who receives the mail</value>
        public string mailReceiver { get; set; }

        /// <summary>
        /// Gets or sets the user who are in CC.
        /// </summary>
        /// <value>Name of the user who are in CC</value>
        public string mailCC { get; set; }

        /// <summary>
        /// Gets or sets the mail received date.
        /// </summary>
        /// <value>Mail received date</value>
        public string mailRecieved { get; set; }

        /// <summary>
        /// Gets or sets the user who sends the mail.
        /// </summary>
        /// <value>Name of the user who sends the mail</value>
        public string mailFromName { get; set; }

        /// <summary>
        /// Gets or sets the address of the mail.
        /// </summary>
        /// <value>Address of the mail</value>
        public string mailFromAddress { get; set; }

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
        /// Gets or sets the conversation id of the mail.
        /// </summary>
        /// <value>Conversation id of the mail</value>
        public string mailConversationId { get; set; }

        /// <summary>
        /// Gets or sets the sensitivity of the mail.
        /// </summary>
        /// <value>Sensitivity of the mail</value>
        public string mailSensitivity { get; set; }

        /// <summary>
        /// Gets or sets the conversation topic of the mail.
        /// </summary>
        /// <value>Conversation topic of the mail</value>
        public string mailConversationTopic { get; set; }

        /// <summary>
        /// Gets or sets the sent date of the mail.
        /// </summary>
        /// <value>Sent date of the mail</value>
        public string mailSentDate { get; set; }

        /// <summary>
        /// Gets or sets the 'Has attachments' of the mail.
        /// </summary>
        /// <value>'Has attachments' value of the mail</value>
        public string mailHasAttachments { get; set; }

        /// <summary>
        /// Gets or sets the categories of the mail.
        /// </summary>
        /// <value>Categories of the mail</value>
        public string mailCategories { get; set; }
    }
}
