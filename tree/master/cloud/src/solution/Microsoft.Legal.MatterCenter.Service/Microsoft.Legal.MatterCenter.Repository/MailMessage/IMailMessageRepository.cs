using Microsoft.Legal.MatterCenter.Models;
namespace Microsoft.Legal.MatterCenter.Repository
{
    public interface IMailMessageRepository
    {
        /// <summary>
        /// this method gets all the user inbox mails
        /// </summary>
        /// <param name="mailRequest"></param>
        /// <returns></returns>
        MailMessageList GetUserInboxEmails(MailRequest mailRequest);
        /// <summary>
        /// this method gets attachment information
        /// </summary>
        /// <param name="mailRequest"></param>
        /// <returns></returns>
        byte[] GetAttachments(string mailId, string attachmentId);
       
        /// <summary>
        /// this method gets mail information
        /// </summary>
        /// <param name="mailRequest"></param>
        /// <returns></returns>
        byte[] GetEmailContent(string mailId);
    }
}
