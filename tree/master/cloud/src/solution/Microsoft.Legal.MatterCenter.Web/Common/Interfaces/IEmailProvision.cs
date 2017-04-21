using Microsoft.Legal.MatterCenter.Models;


namespace Microsoft.Legal.MatterCenter.Web.Common
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEmailProvision
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attachmentRequestVM"></param>
        /// <returns></returns>
        GenericResponseVM UploadAttachments(AttachmentRequestVM attachmentRequestVM);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attachmentRequest"></param>
        /// <returns></returns>
        GenericResponseVM UploadEmails(AttachmentRequestVM attachmentRequest);
    }
}
