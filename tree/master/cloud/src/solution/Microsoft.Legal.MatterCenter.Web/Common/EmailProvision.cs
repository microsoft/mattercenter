using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
/// <summary>
/// this class deals with email/attachments upload methods
/// </summary>
namespace Microsoft.Legal.MatterCenter.Web.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class EmailProvision:IEmailProvision
    {
        private IUploadHelperFunctions uploadHelperFunctions;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uploadHelperFunctions"></param>
        public EmailProvision(IUploadHelperFunctions uploadHelperFunctions)
        {
            this.uploadHelperFunctions = uploadHelperFunctions;
        }

        /// <summary>
        /// this method uploads dropped attachments to the selected matter library
        /// </summary>
        /// <param name="attachmentRequestVM"></param>
        /// <returns></returns>
        public GenericResponseVM UploadAttachments(AttachmentRequestVM attachmentRequestVM)
        {
            int attachmentCount = 0;
            string message = string.Empty;
            var client = attachmentRequestVM.Client;
            var serviceRequest = attachmentRequestVM.ServiceRequest;
            GenericResponseVM genericResponse = null;
            foreach (AttachmentDetails attachment in serviceRequest.Attachments)
            {
                genericResponse = uploadHelperFunctions.UploadAttachmentOfEmail(client, 
                    serviceRequest,
                    attachment,
                    serviceRequest.FolderPath[attachmentCount], ref message);
                if (genericResponse != null && genericResponse.IsError == true)
                {
                    //result = false;
                    break;
                }
                attachmentCount++;
            }
            return genericResponse;
        }
        /// <summary>
        /// this method upload dropped emails to the selected matter
        /// </summary>
        /// <param name="attachmentRequestVM"></param>
        /// <returns></returns>
        public GenericResponseVM UploadEmails(AttachmentRequestVM attachmentRequestVM)
        {
            string message = string.Empty;
            var client = attachmentRequestVM.Client;
            var serviceRequest = attachmentRequestVM.ServiceRequest;
            GenericResponseVM genericResponse = uploadHelperFunctions.Upload(client, serviceRequest, ref message);
            return null;
        }
    }
}
