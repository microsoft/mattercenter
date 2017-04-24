
namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Mail Request object that is used to get mails information from user
    /// user mail box
    /// </summary>
    public class MailRequest: ServiceRequest
    {        
        /// <summary>
        /// Mail folder from which we need to retrieve mail information
        /// </summary>
        public string MailFolder
        {
            get; set;
        }
        /// <summary>
        /// From which page onwards we need to retrieve email information
        /// </summary>
        public int UpToPageNumbersToSkip
        {
            get; set;
        }
        /// <summary>
        /// string messages to filter emails
        /// </summary>
        public string MessageToSearch
        {
            get; set;
        }

        /// <summary>
        /// whether to include attachemnts as part of email message response
        /// </summary>
        public bool IncludeAttachments
        {
            get; set; 
        }
    }
}
